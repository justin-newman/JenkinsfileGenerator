using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JenkinsFileGenerator.Configuration
{
    public static class WebConfig
    {
        public static string consumerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["consumerKey"];
            }
        }
        public static string consumerSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["consumerSecretKey"];
            }
        }
    }
}
