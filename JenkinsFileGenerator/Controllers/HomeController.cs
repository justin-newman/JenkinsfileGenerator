using JenkinsFileGenerator.Configuration;
using JenkinsFileGenerator.Models;
using SharpBucket.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace JenkinsFileGenerator.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var model = new IndexViewModel {
                xunitVersion = "2.4.1",
                agent = "any",
                deployToLocation = "Azure - Test",
                jenkinsfileName = "Jenkinsfile.jenkins"
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = StringCreatorFactory.Create((RepositoryType)model.RepoType);

                var filetext = new StringBuilder();

                filetext.Append($"{repo.BaseStart(model.agent)}");
                filetext.Append($"{repo.Checkout()}");
                filetext.Append($"{repo.NuGet(model.solutionName, model.project)}");
                if (!model.skipTests)
                    filetext.Append($"{repo.TestsAndCoverage(model.project, model.xunitVersion, model.FrameworkVersion, model.OctopusProjectName)}");
                filetext.Append($"{repo.DevAudit(model.project)}");
                filetext.Append($"{repo.Deploy(model.solutionName, model.pipelineName, model.project, model.deployToLocation, model.OctopusProjectName, model.FrameworkVersion)}");
                if (model.automatedTests)
                    filetext.Append($"{repo.WebdriverIO()}");
                filetext.Append($"{repo.BaseEnd()}");
                filetext.Append($"{repo.PostBuildStart()}");
                if (model.automatedTests)
                    filetext.Append($"{repo.PostBuildAlways(model.OctopusProjectName)}");
                filetext.Append($"{repo.PostBuildSuccess()}");
                filetext.Append($"{repo.PostBuildFailure()}");
                filetext.Append($"{repo.PostBuildEnd()}");

                byte[] toBytes = Encoding.ASCII.GetBytes(filetext.ToString());

                return File(toBytes, "text/plain", model.jenkinsfileName);
            }
            return View();
        }
    }
}