pipeline {
   agent any

       stages {
           stage ('Checkout') {
               steps {
                   timeout(time: 3, unit: 'MINUTES') {
                       retry(3) {
                           checkout scm
                       }
                   }
               }
           }
           stage ('Build') {
               steps {
                   timeout(time: 10, unit: 'MINUTES') {
                       echo 'Restoring NuGet Packages'
                       bat 'C:\\tools\\nuget.exe restore JenkinsFileGenerator.sln -ConfigFile C:\\tools\\NuGet.config'

                       echo 'Running MSBuild'
                       bat 'MSBuild JenkinsFileGenerator.sln /p:Configuration=Debug /p:Platform=\"Any CPU\" '
                   }
               }
           }

           stage ('DevAudit') {
               steps {
                   dir ("C:\\Tools\\DevAudit") {
                       bat 'devaudit aspnet --non-interactive -r "%WORKSPACE%\\JenkinsFileGenerator" -m "JenkinsFileGenerator" -c @web.config --list-packages'
                       bat 'devaudit aspnet --non-interactive -r "%WORKSPACE%\\JenkinsFileGenerator" -m "JenkinsFileGenerator" -c @web.config'
                   }
               }
           }
		    stage ('Deploy JenkinsFileGenerator to Azure - Test') {
			    steps {
				    timeout(time: 10, unit: 'MINUTES') {
					    echo 'Publishing JenkinsFileGenerator to Octopus Deploy'
					    bat 'MSBuild JenkinsFileGenerator\\ /p:Configuration=Debug /p:RunOctoPack=true /p:OctoPackPackageVersion=1.0.%BUILD_NUMBER%+%BRANCH_NAME% /p:OctoPackPublishPackageToHttp=%OctopusServer%/nuget/packages?replace=true /p:OctoPackPublishApiKey=%OctopusApiKey% /p:OctoPackAppendToPackageId=Debug'
                        bat 'MSBuild JenkinsFileGenerator\\ /t:Rebuild /p:Configuration=Release /p:RunOctoPack=true /p:OctoPackPackageVersion=1.0.%BUILD_NUMBER%+%BRANCH_NAME% /p:OctoPackPublishPackageToHttp=%OctopusServer%/nuget/packages?replace=true /p:OctoPackPublishApiKey=%OctopusApiKey% /p:OctoPackAppendToPackageId=Release'
                        bat '"C:\\Tools\\Octopus\\Octo.exe" delete-releases --project "JenkinsFileGenerator" --minversion="1.0.%BUILD_NUMBER%+%BRANCH_NAME%" --maxversion="1.0.%BUILD_NUMBER%+%BRANCH_NAME%" --server %OctopusServer%/ --apiKey %OctopusApiKey%'
					    bat '"C:\\Tools\\Octopus\\Octo.exe" create-release --project "JenkinsFileGenerator" --version 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --packageversion 1.0.%BUILD_NUMBER%+%BRANCH_NAME% --server %OctopusServer%/ --apiKey %OctopusApiKey% --releaseNotes "Jenkins Build[%BUILD_NUMBER%] (%JenkinsServer%/blue/organizations/jenkins/JenkinsFileGenerator/detail/%BRANCH_NAME%/%BUILD_NUMBER%/changes)" --deployto="Azure - Production" --progress'
                   }
               }
           }
       }
   post {
       success {
           emailext (  to: "jnewman@cmnhospitals.org", 
               subject: "SUCCESS: ${env.JOB_NAME} - ${env.BRANCH_NAME} Branch - Build # ${env.BUILD_NUMBER} - SUCCESS!", 
               body: """<br /><h1>SUCCESS: </h1><h3>${env.JOB_NAME} - Build # ${env.BUILD_NUMBER} - SUCCESS:</h3><br /><p>Check console output at ${env.RUN_DISPLAY_URL} to view the results.</p>""" 
           )

           slackSend color: "good", message: "SUCCESS: Job - '${env.JOB_NAME}' [Build # ${env.BUILD_NUMBER}] (<${env.RUN_DISPLAY_URL}|View Build>)" 
       }
       failure {
           emailext (  to: "jnewman@cmnhospitals.org", 
               subject: "FAILURE: ${env.JOB_NAME} - ${env.BRANCH_NAME} Branch - Build # ${env.BUILD_NUMBER} - FAILURE!", 
               body: """<br /><h1>FAILURE: </h1><h3>${env.JOB_NAME} - Build # ${env.BUILD_NUMBER} - FAILURE:</h3><br /><p>Check console output at ${env.RUN_DISPLAY_URL} to view the results.</p>""" 
           )

           slackSend color: "danger", message: "FAILURE: Job - '${env.JOB_NAME}' [Build # ${env.BUILD_NUMBER}] (<${env.RUN_DISPLAY_URL}|View Build>)" 
       }
   }
}
