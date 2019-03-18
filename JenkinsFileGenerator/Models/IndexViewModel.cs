using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public class IndexViewModel
    {
        [Required(ErrorMessage = "Jenkins Pipeline Name is required")]
        [Display(Name = "What type of project is this?")]
        public int RepoType { get; set; }

        [Required(ErrorMessage = "Solution Name is required")]
        [Display(Name = "Solution Name")]
        public string solutionName { get; set; }

        [Required(ErrorMessage = "Jenkins Pipeline Name is required")]
        [Display(Name = "Jenkins Pipeline Name")]
        public string pipelineName { get; set; }

        [Required(ErrorMessage = "Octopus Project Name is required")]
        [Display(Name = "Octopus Project Name")]
        public string OctopusProjectName { get; set; }

        [Required(ErrorMessage = "Xunit Version is required")]
        [Display(Name = "Xunit Version")]
        public string xunitVersion { get; set; }

        [Display(Name = "Framework Version")]
        public string FrameworkVersion { get; set; }

        [Required(ErrorMessage = "Agent is required")]
        [Display(Name = "Agent")]
        public string agent { get; set; }

        [Display(Name = "Add Gulp Task")]
        public bool gulpTask { get; set; }

        [Display(Name = "Skip Tests")]
        public bool skipTests { get; set; }

        [Required(ErrorMessage ="Project is required")]
        [Display(Name = "Project")]
        public string project { get; set; }

        [Required(ErrorMessage = "Deploy To Location is required")]
        [Display(Name = "Deploy To Location")]
        public string deployToLocation { get; set; }

        [Required(ErrorMessage = "Filename is required")]
        [Display(Name = "Filename")]
        public string jenkinsfileName { get; set; }
    }
}