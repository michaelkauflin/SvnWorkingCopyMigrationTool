using System;
using System.Collections.Generic;
using System.Linq;

namespace SvnWorkingCopyMigrationTool.Model
{
    class WorkingCopyMigrationAnalyzer
    {
        private const string NewHost = "svn-pcsw.sonova.com";
        private const string NewProtocole = "https";

        private readonly List<string> _repositoriesMigrated;

        public WorkingCopyMigrationAnalyzer()
        {
            _repositoriesMigrated = new List<string>
            {
                "chinook2", "chinook2ReferenceData", "chinook2docs", "chinookmedia", "cocoondocs", "swiftdocs", "testbed2", "chinook1", "pfg"
            };
        }

        public bool RequiresMigration(WorkingCopy workingCopy)
        {
            if (!Uri.IsWellFormedUriString(workingCopy.RepositoryRoot, UriKind.Absolute))
            {
                return false;
            }

            var repositoryUri = new Uri(workingCopy.RepositoryRoot);
            string repositoryName = repositoryUri.Segments.Last();

            return _repositoriesMigrated.Contains(repositoryName, StringComparer.InvariantCultureIgnoreCase)
                && !repositoryUri.Host.Equals(NewHost, StringComparison.InvariantCultureIgnoreCase);
        }

        public string ComputeMigrationUrl(WorkingCopy workingCopy)
        {
            var repositoryRoot = new Uri(workingCopy.URL);

            var migrationUrl = repositoryRoot.ToString();
            migrationUrl = migrationUrl.Replace(repositoryRoot.Host, NewHost);
            migrationUrl = migrationUrl.Replace(repositoryRoot.Scheme, NewProtocole);

            return migrationUrl;
        }
    }
}
