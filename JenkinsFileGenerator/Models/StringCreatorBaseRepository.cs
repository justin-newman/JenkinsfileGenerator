using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public abstract class StringCreatorBaseRepository : IStringCreatorRepository
    {

        public virtual string BaseStart(string agent)
        {
            return $"pipeline {{\r\n" +
                   $"   agent {agent}\r\n" +
                   $"\r\n" +
                   $"       stages {{\r\n";
        }

        public virtual string Checkout()
        {
            return $"           stage ('Checkout') {{\r\n" +
                   $"               steps {{\r\n" +
                   $"                   timeout(time: 3, unit: 'MINUTES') {{\r\n" +
                   $"                       retry(3) {{\r\n" +
                   $"                           checkout scm\r\n" +
                   $"                       }}\r\n" +
                   $"                   }}\r\n" +
                   $"               }}\r\n" +
                   $"           }}" +
                   $"\r\n";
        }

        public virtual string NodeInstallAndBuild()
        {
            return $"           stage ('NodeInstallAndBuild') {{\r\n" +
                   $"               steps {{\r\n" +
                   $"                   echo 'Check Versions and Install NPM Packages'\r\n" +
                   $"                   bat 'node --version'\r\n" +
                   $"                   bat 'npm --version'\r\n" +
                   $"                   bat 'npm install'\r\n" +
                   $"\r\n" +
                   $"                   echo 'Run Gulp Tasks'\r\n" +
                   $"                   bat 'gulp build'\r\n" +
                   $"               }}\r\n" +
                   $"           }}" +
                   $"\r\n";
        }

        public abstract string Build(string solutionName);

        public virtual string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion)
        {
            return $"";
        }

    public virtual string DevAudit(string projectName)
        {
            return $"";
        }
        public abstract string DeployTo(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion);

        public virtual string BaseEnd()
        {
            return $"       }}\r\n";
        }

        public virtual string PostBuildStart()
        {
            return $"   post {{\r\n";
        }

        public virtual string PostBuildSuccess()
        {
            return $"       success {{\r\n" +
                   $"           emailext (  to: \"jnewman@cmnhospitals.org\", \r\n" +
                   $"               subject: \"SUCCESS: ${{env.JOB_NAME}} - ${{env.BRANCH_NAME}} Branch - Build # ${{env.BUILD_NUMBER}} - SUCCESS!\", \r\n" +
                   $"               body: \"\"\"<br /><h1>SUCCESS: </h1><h3>${{env.JOB_NAME}} - Build # ${{env.BUILD_NUMBER}} - SUCCESS:</h3><br /><p>Check console output at ${{env.RUN_DISPLAY_URL}} to view the results.</p>\"\"\" \r\n" +
                   $"           )\r\n" +
                   $"\r\n" +
                   $"           slackSend color: \"good\", message: \"SUCCESS: Job - '${{env.JOB_NAME}}' [Build # ${{env.BUILD_NUMBER}}] (<${{env.RUN_DISPLAY_URL}}|View Build>)\" \r\n" +
                   $"       }}\r\n";
        }

        public virtual string PostBuildFailure()
        {
            return $"       failure {{\r\n" +
                   $"           emailext (  to: \"jnewman@cmnhospitals.org\", \r\n" +
                   $"               subject: \"FAILURE: ${{env.JOB_NAME}} - ${{env.BRANCH_NAME}} Branch - Build # ${{env.BUILD_NUMBER}} - FAILURE!\", \r\n" +
                   $"               body: \"\"\"<br /><h1>FAILURE: </h1><h3>${{env.JOB_NAME}} - Build # ${{env.BUILD_NUMBER}} - FAILURE:</h3><br /><p>Check console output at ${{env.RUN_DISPLAY_URL}} to view the results.</p>\"\"\" \r\n" +
                   $"           )\r\n" +
                   $"\r\n" +
                   $"           slackSend color: \"danger\", message: \"FAILURE: Job - '${{env.JOB_NAME}}' [Build # ${{env.BUILD_NUMBER}}] (<${{env.RUN_DISPLAY_URL}}|View Build>)\" \r\n" +
                   $"       }}\r\n";
        }

        public virtual string PostBuildEnd()
        {
            return $"   }}\r\n" +
                   $"}}\r\n";
        }
    }
}