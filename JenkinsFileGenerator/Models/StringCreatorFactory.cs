using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JenkinsFileGenerator.Models
{
    public static class StringCreatorFactory
    {
        public static IStringCreatorRepository Create(RepositoryType repositoryType)
        {
            switch (repositoryType)
            {
                //case RepositoryType.DotNetCore: return new StringCreatorNetCoreRepository();
                //case RepositoryType.FullFramework: return new StringCreatorFullFrameworkRepository();
                case RepositoryType.Web: return new StringCreatorWebRepository();
                case RepositoryType.Ssrs: return new StringCreatorSSRSRepository();
                default: throw new Exception($"Repository type {repositoryType} not supported");
            }
        }
    }
}