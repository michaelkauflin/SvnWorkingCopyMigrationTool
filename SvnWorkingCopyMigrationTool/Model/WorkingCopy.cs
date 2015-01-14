using System;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace SvnWorkingCopyMigrationTool.Model
{
    class WorkingCopy
    {
        public string URL { get; private set; }
        public string RootPath {get; private set;}
        public string RepositoryRoot { get; private set; }
        public string Revision { get; private set; }

        private WorkingCopy()
        {
        }

        public void Migrate(string migrationUrl)
        {
            using (var svninfo = new Process())
            {
                svninfo.StartInfo = new ProcessStartInfo("svn")
                {
                    Arguments = String.Format("relocate {0}", migrationUrl),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    WorkingDirectory = RootPath
                };

                svninfo.Start();
                string errorOutput = svninfo.StandardError.ReadToEnd();
                svninfo.WaitForExit();
                if (svninfo.ExitCode != 0)
                {
                    MessageBox.Show(errorOutput, "Error in relocate");
                }
            }
        }

        public static WorkingCopy Parse(string rootFolderPath)
        {
            string svnInfoResponseInXml;
            using (var svninfo = new Process())
            {
                svninfo.StartInfo = new ProcessStartInfo("svn")
                {
                    Arguments = String.Format("info --xml"),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = rootFolderPath
                };

                svninfo.Start();
                svnInfoResponseInXml = svninfo.StandardOutput.ReadToEnd();
                svninfo.WaitForExit();
                if (svninfo.ExitCode != 0)
                {
                    return GetUnknown();
                }
            }

            XDocument svnInfoDocument = XDocument.Parse(svnInfoResponseInXml);
            if (svnInfoDocument.Root == null || svnInfoDocument.Root.Element("entry") == null)
            {
                return GetUnknown();
            }

            XElement entryElement = svnInfoDocument.Root.Element("entry");
            return new WorkingCopy()
            {
                RepositoryRoot = entryElement.Element("repository").Element("root").Value,
                Revision = entryElement.Attribute("revision").Value,
                RootPath = entryElement.Element("wc-info").Element("wcroot-abspath").Value,
                URL = entryElement.Element("url").Value
            };
        }

        private static WorkingCopy GetUnknown()
        {
            return new WorkingCopy()
            {
                RepositoryRoot = "Unknown",
                Revision = "Unknown",
                RootPath = "Unknown",
                URL = "Unknown"
            };
        }

        public void OpenRootFolder()
        {
            Process.Start(RootPath);
        }
    }
}
