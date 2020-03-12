using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsFileGenerator.Models
{
    public interface IStringCreatorRepository
    {
        string BaseStart(string agent);

        string Checkout();

        string NuGet(string solutionName, string projectName);

        string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion, string OctopusProjectName);

        string DevAudit(string projectName);

        string Deploy(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion);

        string WebdriverIO();

        string BaseEnd();

        string PostBuildStart();

        string PostBuildAlways(string OctopusProjectName);

        string PostBuildSuccess();

        string PostBuildFailure();

        string PostBuildEnd();

    }
}
