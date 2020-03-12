using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public class StringCreatorSSRSRepository : StringCreatorBaseRepository
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
                   $"                        bat '{msBuildlocation} {solutionName}.sln /p:Configuration=Debug;Platform=Default'\r\n" +
                   $"                    }}\r\n" +
                   $"                }}\r\n" +
                   $"            }}\r\n" +
                   $"\r\n";
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
                          $"					    bat '\"%Octo%\" create-release --project \"{OctopusProjectName}\" --version 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --packageversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --server %OctopusServer%/ --apiKey %OctopusApiKey% --releaseNotes \"Jenkins Build[%BUILD_NUMBER%] (%JenkinsServer%/blue/organizations/jenkins/{encodedPipelineName}/detail/%BRANCH_NAME%/%BUILD_NUMBER%/changes)\" --deployto=\"%DEPLOY_TO%\" --progress'\r\n" +
                          $"                    }}\r\n" +
                          $"                }}\r\n" +
                          $"            }}\r\n" +
                          $"\r\n"
                );
            return sb.ToString();
        }
    }
}