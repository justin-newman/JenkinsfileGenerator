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
                   $"    agent {agent}\r\n" +
                   $"\r\n" +
                   $"        parameters {{\r\n" +
                   $"            choice(name: 'DEPLOY_TO',\r\n" +
                   $"                choices: ['Test', 'Dev', 'Staging'],\r\n" +
                   $"                description: 'Please Select an Environment.')\r\n" +
                   $"        }}\r\n" +
                   $"        stages {{\r\n";
        }

        public virtual string Checkout()
        {
            return $"            stage ('Checkout') {{\r\n" +
                   $"                steps {{\r\n" +
                   $"                    timeout(time: 3, unit: 'MINUTES') {{\r\n" +
                   $"                        retry(3) {{\r\n" +
                   $"                            checkout scm\r\n" +
                   $"                        }}\r\n" +
                   $"                    }}\r\n" +
                   $"                }}\r\n" +
                   $"            }}" +
                   $"\r\n";
        }

        public abstract string NuGet(string solutionName, string projectName);

        public virtual string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion, string OctopusProjectName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"            stage ('Unit Tests and Test Coverage') {{\r\n" +
                      $"                steps {{\r\n" +
                      $"                    script{{\r\n" +
                      $"                        powershell label: \"Installing LocalDeploy\", script: \"Install-Module LocalDeploy -Force -Scope CurrentUser\"\r\n" +
                      $"                        powershell label: \"Updating App Settings\", script: \"Update-Config -Environment Test -ProjectName '{projectName}' -SkipUpdates\"\r\n" +
                      $"                        try {{\r\n" +
                      $"                            powershell label: \"Running Unit Tests\", script: \"Invoke-Tests -ProjectName '{projectName}' -DotCover ${{env.dotCover}} -SkipUpdates\"\r\n" +
                      $"                        }} catch(err) {{\r\n" +
                      $"                            currentBuild.result = 'SUCCESS'\r\n" +
                      $"                        }}\r\n" +
                      $"                        junit allowEmptyResults: true, testResults: 'TestResults/*.xml'\r\n" +
                      $"                    }}\r\n" +
                      $"                }}\r\n" +
                      $"            }}\r\n" +
                      $"\r\n"
            );
            return sb.ToString();
        }

        public virtual string DevAudit(string projectName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"            stage ('DevAudit') {{\r\n" +
                      $"                steps {{\r\n" +
                      $"                    powershell label: \"Auditing Packages\", script: \"Invoke-DevAudit -ProjectName '{projectName}' -SkipUpdates\"\r\n" +
                      $"                }}\r\n" +
                      $"            }}\r\n" +
                      $"\r\n"
            );

            return sb.ToString();
        }
        public abstract string Deploy(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion);

        public virtual string WebdriverIO()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"            stage ('WebdriverIO Tests') {{\r\n" +
                      $"                steps {{\r\n" +
                      $"                    timeout(time: 60, unit: 'MINUTES') {{\r\n" +
                      $"                        dir ('AutomatedTests') {{\r\n" +
                      $"                            bat 'npm install'\r\n" +
                      $"\r\n" +
                      $"                            sauce('c077eb41-7ba4-42ae-af67-c4ef32e00907') {{\r\n" +
                      $"                                sauceconnect() {{\r\n" +
                      $"                                    echo 'Run WebdriverIO tests'\r\n" +
                      $"                                    bat 'npm run test-sauce'\r\n" +
                      $"                                }}\r\n" +
                      $"                            }}\r\n" +
                      $"\r\n" +
                      $"                        }}\r\n" +
                      $"                    }}\r\n" +
                      $"                }}\r\n" +
                      $"            }}\r\n" +
                      $"\r\n"
            );
            return sb.ToString();
        }

        public virtual string BaseEnd()
        {
            return $"        }}\r\n";
        }

        public virtual string PostBuildStart()
        {
            return $"    post {{\r\n";
        }

        public virtual string PostBuildAlways(string OctopusProjectName)
        {
            return $"        always {{\r\n" +
                   $"            publishHTML([allowMissing: true, alwaysLinkToLastBuild: true, includes: '**/**/*', keepAll: true, reportDir: 'Reports', reportFiles: '*.html', reportName: 'HTML Report', reportTitles: '{OctopusProjectName} DotCover Coverage Report'])" +
                   $"            allure results: [[path: 'AutomatedTests/allure-results']]\r\n" +
                   $"        }}\r\n";
        }

        public virtual string PostBuildSuccess()
        {
            return $"        success {{\r\n" +
                   $"            emailext (  to: \"jnewman@cmnhospitals.org\", \r\n" +
                   $"                subject: \"SUCCESS: ${{env.JOB_NAME}} - ${{env.BRANCH_NAME}} Branch - Build # ${{env.BUILD_NUMBER}} - SUCCESS!\", \r\n" +
                   $"                body: \"\"\"<br /><h1>SUCCESS: </h1><h3>${{env.JOB_NAME}} - Build # ${{env.BUILD_NUMBER}} - SUCCESS:</h3><br /><p>Check console output at ${{env.RUN_DISPLAY_URL}} to view the results.</p>\"\"\" \r\n" +
                   $"            )\r\n" +
                   $"\r\n" +
                   $"            slackSend color: \"good\", message: \"SUCCESS: Job - '${{env.JOB_NAME}}' [Build # ${{env.BUILD_NUMBER}}] (<${{env.RUN_DISPLAY_URL}}|View Build>)\" \r\n" +
                   $"        }}\r\n";
        }

        public virtual string PostBuildFailure()
        {
            return $"        failure {{\r\n" +
                   $"            emailext (  to: \"jnewman@cmnhospitals.org\", \r\n" +
                   $"                subject: \"FAILURE: ${{env.JOB_NAME}} - ${{env.BRANCH_NAME}} Branch - Build # ${{env.BUILD_NUMBER}} - FAILURE!\", \r\n" +
                   $"                body: \"\"\"<br /><h1>FAILURE: </h1><h3>${{env.JOB_NAME}} - Build # ${{env.BUILD_NUMBER}} - FAILURE:</h3><br /><p>Check console output at ${{env.RUN_DISPLAY_URL}} to view the results.</p>\"\"\" \r\n" +
                   $"            )\r\n" +
                   $"\r\n" +
                   $"            slackSend color: \"danger\", message: \"FAILURE: Job - '${{env.JOB_NAME}}' [Build # ${{env.BUILD_NUMBER}}] (<${{env.RUN_DISPLAY_URL}}|View Build>)\" \r\n" +
                   $"        }}\r\n";
        }

        public virtual string PostBuildEnd()
        {
            return $"    }}\r\n" +
                   $"}}\r\n";
        }
    }
}