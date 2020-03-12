//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;

//namespace JenkinsFileGenerator.Models
//{
//    public class StringCreatorFullFrameworkRepository : StringCreatorBaseRepository
//    {
//        //I'm concatenating the strings to make it more readable, if this is slow we can not do that
//        public override string NuGet(string solutionName, string projectName)
//        {
//            return $"            stage ('Checkout') {{\r\n" +
//                   $"                steps {{\r\n" +
//                   $"                    timeout(time: 3, unit: 'MINUTES') {{\r\n" +
//                   $"                        retry(3) {{\r\n" +
//                   $"                            checkout scm\r\n" +
//                   $"                        }}\r\n" +
//                   $"                    }}\r\n" +
//                   $"                }}\r\n" +
//                   $"            }}" +
//                   $"\r\n";
//        }

//        public override string Deploy(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion)
//        {
//            StringBuilder sb = new StringBuilder();

//            sb.Append($"            stage ('Deploy') {{\r\n" +
//                      $"                steps {{\r\n" +
//                      $"                    timeout(time: 20, unit: 'MINUTES') {{\r\n" +
//                      $"                        powershell label: \"Packing and Deploying {OctopusProjectName} to Octopus\", script: \"Deploy-App -Environment '${{env.DEPLOY_TO}}' -OctoProject '{OctopusProjectName}' -ProjectName '{projectName}' -BuildNumber '${{env.BUILD_NUMBER}}' -BranchName '${{env.BRANCH_NAME}}' -SkipUpdates\"\r\n" +
//                      $"                    }}\r\n" +
//                      $"                }}\r\n" +
//                      $"            }}\r\n"
//            );
//            return sb.ToString();
//        }

//        public override string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion, string OctopusProjectName)
//        {
//            StringBuilder sb = new StringBuilder();

//            sb.Append($"            stage ('Unit Tests and Test Coverage') {{\r\n" +
//                      $"                steps {{\r\n" +
//                      $"                    script{{\r\n" +
//                      $"                        powershell label: \"Installing LocalDeploy\", script: \"Install-Module LocalDeploy -Force -Scope CurrentUser\"\r\n" +
//                      $"                        powershell label: \"Updating App Settings\", script: \"Update-Config -Environment Test -ProjectName '{projectName}' -SkipUpdates\"\r\n" +
//                      $"                        try {{\r\n" +
//                      $"                            powershell label: \"Running Unit Tests\", script: \"Invoke-Tests -ProjectName '{projectName}' -DotCover ${{env.dotCover}} -SkipUpdates\"\r\n" +
//                      $"                        }} catch(err) {{\r\n" +
//                      $"                            currentBuild.result = 'SUCCESS'\r\n" +
//                      $"                        }}\r\n" +
//                      $"                        junit allowEmptyResults: true, testResults: 'TestResults/*.xml'\r\n" +
//                      $"                    }}\r\n" +
//                      $"                }}\r\n" +
//                      $"            }}\r\n" +
//                      $"\r\n"
//            );
//            return sb.ToString();
//        }
//    }
//}