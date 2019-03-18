using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public class StringCreatorNetCoreRepository : StringCreatorBaseRepository
    {
        //I'm concatenating the strings to make it more readable, if this is slow we can not do that

        public override string Build(string solutionName)
        {
            return $"           stage ('Build') {{\r\n" +
                   $"               steps {{\r\n" +
                   $"                   timeout(time: 5, unit: 'MINUTES') {{\r\n" +
                   $"                       echo 'Restoring NuGet Packages'\r\n" +
                   $"                       bat 'dotnet restore --configfile C:\\\\tools\\\\NuGet.config'\r\n" +
                   $"\r\n" +
                   $"                       echo 'Running MSBuild'\r\n" +
                   $"                       bat 'dotnet build --configuration Release'\r\n" +
                   $"                   }}\r\n" +
                   $"               }}\r\n" +
                   $"           }}\r\n" +
                   $"\r\n";
        }

        public override string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion)
        {
            StringBuilder sb = new StringBuilder();

                sb.Append($"            stage ('{projectName} Tests and Coverage') {{\r\n" +
                          $"                steps {{\r\n" +
                          $"                    script{{\r\n" +
                          $"                        try {{\r\n" +
                          $"                            echo 'Generating dotCover Report'\r\n" +
                          $"                            bat '\"C:\\\\Tools\\\\JetBrains\\\\Installations\\\\dotCover08\\\\dotCover.exe\" a /TargetExecutable=\"C:\\\\Tools\\\\xUnit\\\\{xunitVersion}\\\\xunit.console.exe\" /TargetArguments=\".\\\\tests\\\\{projectName}.Tests\\\\bin\\\\Release\\\\{FrameworkVersion}\\\\{projectName}.Tests.dll -nunit .\\\\{projectName}TestResults.xml\" /Output=\"Reports\\\\{projectName}DotCoverCoverageReport.html\" /ReportType=\"HTML\" /Filters=-:*.Tests /Attributefilters=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;'" +
                          $"\r\n" +
                          $"                            echo 'Running Report Generator'\r\n" +
                          $"			                bat '\"C:\\\\Tools\\\\reportgenerator\\\\ReportGenerator.exe\" -reports:\"{projectName}CodeCoverage.coveragexml\" -targetDir:Reports\\\\{projectName}CodeCoverageHTML -assemblyfilters:\"-[*.tests]* -[*.Tests]*\"'\r\n" +
                          $"\r\n" +
                          $"                            echo 'Generating {projectName} xUnit Test Result Report'\r\n" +
                          $"                            step([\r\n" +
                          $"                                $class: 'XUnitPublisher',\r\n" +
                          $"                                testTimeMargin: '3000',\r\n" +
                          $"                                thresholdMode: 1,\r\n" +
                          $"                                tools: [\r\n" +
                          $"                                    [$class: 'NUnitJunitHudsonTestType',\r\n" +
                          $"                                        deleteOutputFiles: true,\r\n" +
                          $"						                failIfNotNew: true,\r\n" +
                          $"							            pattern: '**/*TestResults.xml',\r\n" +
                          $"							            skipNoTestFiles: false,\r\n" +
                          $"							            stopProcessingIfError: true\r\n" +
                          $"                                    ]\r\n" +
                          $"                                ]\r\n" +
                          $"                            ])\r\n" +
                          $"                            echo 'Done Generating {projectName} Unit Test Report'\r\n" +
                          $"\r\n" +
                          $"                            publishHTML([\r\n" +
                          $"					            reportTitles: '{projectName} DotCover Coverage Report',\r\n" +
                          $"					            allowMissing: false,\r\n" +
                          $"					            alwaysLinkToLastBuild: false,\r\n" +
                          $"					            keepAll: false,\r\n" +
                          $"					            reportDir: 'Reports',\r\n" +
                          $"					            reportFiles: '{projectName}DotCoverCoverageReport.html',\r\n" +
                          $"					            reportName: '{projectName} DotCover Code Coverage'\r\n" +
                          $"                            ])\r\n" +
                          $"                        }} catch(err) {{\r\n" +
                          $"                            currentBuild.result = 'SUCCESS'\r\n" +
                          $"                        }}\r\n" +
                          $"                    }}\r\n" +
                          $"                }}\r\n" +
                          $"            }}\r\n" +
                          $"\r\n"
                );
            return sb.ToString();
        }

        public override string DeployTo(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion)
        {
            var encodedPipelineName = HttpUtility.UrlEncode(pipelineName);

            StringBuilder sb = new StringBuilder();


                sb.Append($"		    stage ('Deploy {OctopusProjectName} to {deployToLocation}') {{\r\n" +
                          $"			    steps {{\r\n" +
                          $"				    timeout(time: 10, unit: 'MINUTES') {{\r\n" +
                          $"					    echo 'Publishing {OctopusProjectName} to Octopus Deploy'\r\n" +
                          $"					    bat 'dotnet publish \"{solutionName}.sln\" --configuration Debug --output \"{solutionName}/Debug\"'\r\n" +
                          $"					    bat 'dotnet publish \"{solutionName}.sln\" --configuration Release --output \"{solutionName}/Release\"'\r\n" +
                          $"                    bat '\"C:\\\\Tools\\\\Octopus\\\\Octo.exe\" pack --id=\"{OctopusProjectName}.Debug\" --version=1.0.%BUILD_NUMBER%+%BRANCH_NAME% --basePath src/{projectName}/{solutionName}/Debug'\r\n" +
                          $"                    bat '\"C:\\\\Tools\\\\Octopus\\\\Octo.exe\" pack --id=\"{OctopusProjectName}.Release\" --version=1.0.%BUILD_NUMBER%+%BRANCH_NAME% --basePath src/{projectName}/{solutionName}/Release'\r\n" +
                          $"                    bat '\"C:\\\\Tools\\\\Octopus\\\\Octo.exe\" push --package {OctopusProjectName}.Debug.1.0.%BUILD_NUMBER%+%BRANCH_NAME%.nupkg --package {OctopusProjectName}.Release.1.0.%BUILD_NUMBER%+%BRANCH_NAME%.nupkg --server %OctopusServer% --apiKey %OctopusApiKey% --replace-existing'\r\n" +
                          $"                    bat '\"C:\\\\Tools\\\\Octopus\\\\Octo.exe\" delete-releases --project \"{OctopusProjectName}\" --minversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --maxversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --server %OctopusServer%/ --apiKey %OctopusApiKey%'\r\n" +
                          $"					    bat '\"C:\\\\Tools\\\\Octopus\\\\Octo.exe\" create-release --project \"{OctopusProjectName}\" --version 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --packageversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --server %OctopusServer%/ --apiKey %OctopusApiKey% --releaseNotes \"Jenkins Build[%BUILD_NUMBER%] (%JenkinsServer%/blue/organizations/jenkins/{encodedPipelineName}/detail/%BRANCH_NAME%/%BUILD_NUMBER%/changes)\" --deployto=\"{deployToLocation}\" --progress'\r\n" +
                          $"                   }}\r\n" +
                          $"               }}\r\n" +
                          $"           }}\r\n"
                );
            return sb.ToString();
        }
        //TODO: public override string WebdriverIO(){}
    }
}