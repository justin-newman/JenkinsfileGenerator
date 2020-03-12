using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public class StringCreatorDatabaseRepository : StringCreatorBaseRepository
    {
        //I'm concatenating the strings to make it more readable, if this is slow we can not do that
        public string msBuildlocation = $"MSBuild";

        public override string NuGet(string solutionName, string projectName)
        {
            return $"            stage ('Build') {{\r\n" +
                   $"                steps {{\r\n" +
                   $"                    timeout(time: 10, unit: 'MINUTES') {{\r\n" +
                   $"                        echo 'Restoring NuGet Packages'\r\n" +
                   $"                        bat 'C:\\\\tools\\\\nuget.exe locals all -clear'\r\n" +
                   $"                        bat 'C:\\\\tools\\\\nuget.exe restore {solutionName}.sln -ConfigFile C:\\\\tools\\\\NuGet.config'\r\n" +
                   $"\r\n" +
                   $"                        echo 'Running MSBuild'\r\n" +
                   $"                        bat '{msBuildlocation} {solutionName}.sln /p:Configuration=Debug /p:Platform=\\\"Any CPU\\\"'\r\n" +
                   $"                    }}\r\n" +
                   $"                }}\r\n" +
                   $"            }}\r\n" +
                   $"\r\n";
        }

        //TODO: Create Build Artifacts
        //public override string BuildArtifacts(string solutionName, string projectName, string OctopusProjectName)
        //{
        //    return $"            stage ('Create Build Artifacts') {{\r\n" +
        //           $"                steps {{\r\n" +
        //           $"                    powershell ('''\r\n" +
        //           $"                        $projectPath = \"$env:WORKSPACE\\\\{solutionName}\\\\{projectName}.sqlproj\"\r\n" +
        //           $"                        $ver = \"1.0.$env:BUILD_NUMBER\"\r\n" +
        //           $"                        Import-Module  \"C:\\\\Program Files (x86)\\\\Red Gate\\\\SQL Change Automation PowerShell\\\\Modules\\\\SqlChangeAutomation\" -Global\r\n" +
        //           $"                        $validatedProject = Invoke-DatabaseBuild $projectPath\r\n" +
        //           $"                        $buildArtifact = New-DatabaseBuildArtifact $validatedProject -PackageId {OctopusProjectName} -PackageVersion $ver\r\n" +
        //           $"                        Export-DatabaseBuildArtifact $buildArtifact -Path buildArtifacts\r\n" +
        //           $"                    ''')\r\n" +
        //           $"                }}\r\n" +
        //           $"            }}\r\n" +
        //           $"\r\n";
        //} 

        //TODO: Publish to Unit Testing Instance
        //public override string DeployToUnitTestingInstance(string pipelineName, string OctopusProjectName)
        //{
        //    var encodedPipelineName = HttpUtility.UrlEncode(pipelineName);
        //
        //    return $"            stage ('Publish to Unit Testing Instance') {{\r\n" +
        //           $"                steps {{\r\n" +
        //           $"                    timeout(time: 5, unit: 'MINUTES')\r\n" +
        //           $"                        bat '"%Octo%" push --package "%WORKSPACE%\\\\buildArtifacts\\\\{OctopusProjectName}.1.0.%BUILD_NUMBER%.nupkg" --server %OctopusServer% --apiKey %OctopusApiKey% --replace-existing'\r\n" +
        //           $"                        bat '"%Octo%" create-release --project "{OctopusProjectName}" --version 1.0.%BUILD_NUMBER% --packageversion 1.0.%BUILD_NUMBER% --server %OctopusServer%/ --apiKey %OctopusApiKey% --releaseNotes "Jenkins Build[%BUILD_NUMBER%] (%JenkinsServer%/blue/organizations/jenkins/{encodedPipelineName}/detail/%BRANCH_NAME%/%BUILD_NUMBER%/changes)" --deployto="SQLTEST01 - Unit Testing" --progress'\r\n" +
        //           $"                    }}\r\n" +
        //           $"                }}\r\n" +
        //           $"            }}\r\n" +
        //           $"\r\n";
        //}

        public override string TestsAndCoverage(string projectName, string xunitVersion, string FrameworkVersion, string OctopusProjectName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"            stage ('{projectName} Tests and Coverage') {{\r\n" +
                      $"                steps {{\r\n" +
                      $"                    timeout(time: 5, unit: 'MINUTES') {{\r\n" +
                      $"                        echo 'Generating dotCover Report'\r\n" +
                      $"                        bat '\"%dotCover%\" a /TargetExecutable=\"%VSTest%\" /TargetArguments=\".\\\\{projectName}.Tests\\\\bin\\\\Debug\\\\{projectName}.Tests.dll /logger:trx;LogFileName=%workspace%\\\\{projectName}TestResults.trx\" /Output=\"Reports\\\\{projectName}DotCoverCoverageReport.html\" /ReportType=\"HTML\" /Filters=-:*.Tests /Attributefilters=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;'\r\n" +
                      $"\r\n" +
                      $"                        echo 'Generating CodeCoverage Report'\r\n" +
                      $"                        bat '\"%CodeCoverage%\" collect /output:.\\\\{projectName}CodeCoverage.coverage \"%vstest%\" \"{projectName}.tests\\\\bin\\\\Debug\\\\{projectName}.tests.dll\"'\r\n" +
                      $"                        bat '\"%CodeCoverage%\" analyze /output:.\\\\{projectName}CodeCoverage.coveragexml .\\\\{projectName}CodeCoverage.coverage'\r\n" +
                      $"\r\n" +
                      $"                        echo 'Generating {projectName} MSTest Result Report'\r\n" +
                      $"\r\n" +
                      $"                        step([$class: 'MSTestPublisher', testResultsFile:'**/*.trx', failOnError: true, keepLongStdio: true])\r\n" +
                      $"\r\n" +
                      $"                    }}\r\n" +
                      $"                }}\r\n" +
                      $"            }}\r\n" +
                      $"\r\n"
            );
            return sb.ToString();
        }

        public override string Deploy(string solutionName, string pipelineName, string projectName, string deployToLocation, string OctopusProjectName, string FrameworkVersion)
        {
            var encodedPipelineName = HttpUtility.UrlEncode(pipelineName);

            StringBuilder sb = new StringBuilder();

                sb.Append($"            stage ('Deploy {OctopusProjectName} to {deployToLocation}') {{\r\n" +
                          $"			    steps {{\r\n" +
                          $"				    timeout(time: 10, unit: 'MINUTES') {{\r\n" +
                          $"					    echo 'Publishing {OctopusProjectName} to Octopus Deploy'\r\n" +
                          $"					    bat '{msBuildlocation} {projectName}\\\\ /p:Configuration=Debug;Platform=Default'\r\n" +
                          $"                        bat '{msBuildlocation} {projectName}\\\\ /t:Rebuild /p:Configuration=Release;Platform=Default'\r\n" +
                          $"                        bat '\"%Octo%\" pack --id=\"{OctopusProjectName}.Debug\" --version=1.0.%BUILD_NUMBER%+%BRANCH_NAME% --basePath {projectName}/bin/Debug'\r\n" +
                          $"                        bat '\"%Octo%\" pack --id=\"{OctopusProjectName}.Production\" --version=1.0.%BUILD_NUMBER%+%BRANCH_NAME% --basePath {projectName}/bin/Release'\r\n" +
                          $"                        bat '\"%Octo%\" push --package {OctopusProjectName}.1.0.%BUILD_NUMBER%+%BRANCH_NAME%.nupkg --server %OctopusServer% --apiKey %OctopusApiKey% --replace-existing'\r\n" +
                          $"                        bat '\"%Octo%\" delete-releases --project \"{OctopusProjectName}\" --minversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --maxversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --server %OctopusServer%/ --apiKey %OctopusApiKey%'\r\n" +
                          $"					    bat '\"%Octo%\" create-release --project \"{OctopusProjectName}\" --version 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --packageversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --server %OctopusServer%/ --apiKey %OctopusApiKey% --releaseNotes \"Jenkins Build[%BUILD_NUMBER%] (%JenkinsServer%/blue/organizations/jenkins/{encodedPipelineName}/detail/%BRANCH_NAME%/%BUILD_NUMBER%/changes)\" --deployto=\"{deployToLocation}\" --progress'\r\n" +
                          $"                    }}\r\n" +
                          $"                }}\r\n" +
                          $"            }}\r\n" +
                          $"\r\n"
                );
            return sb.ToString();
        }
    }
}