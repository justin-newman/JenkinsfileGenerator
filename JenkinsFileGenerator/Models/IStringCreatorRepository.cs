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

        string NodeInstallAndBuild();

        string Build(string solutionName);

        string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion);

        string DevAudit(string projectName);

        string DeployTo(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion);

        //TODO: string WebriverIO();

        string BaseEnd();

        string PostBuildStart();

        string PostBuildSuccess();

        string PostBuildFailure();

        string PostBuildEnd();

    }
}
