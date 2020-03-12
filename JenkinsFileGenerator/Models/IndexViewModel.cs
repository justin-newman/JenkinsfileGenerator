using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public class IndexViewModel
    {

        public string solutionName { get; set; }

        public string pipelineName { get; set; }

        public string xunitVersion { get; set; }

        public string FrameworkVersion { get; set; }

        public bool gulpTask { get; set; }

        public string deployToLocation { get; set; }

        [Required(ErrorMessage = "Jenkins Pipeline Name is required")]
        [Display(Name = "What type of project is this?")]
        public int RepoType { get; set; }

        [Required(ErrorMessage = "Project is required")]
        public string project { get; set; }

        [Required(ErrorMessage = "Octopus Project Name is required")]
        [Display(Name = "Octopus Project Name")]
        public string OctopusProjectName { get; set; }

        [Required(ErrorMessage = "Agent is required")]
        [Display(Name = "Agent")]
        public string agent { get; set; }

        [Display(Name = "Skip Tests")]
        public bool skipTests { get; set; }

        [Display(Name = "Add Automated Tests")]
        public bool automatedTests { get; set; }

        [Required(ErrorMessage = "Filename is required")]
        public string jenkinsfileName { get; set; }
    }
}