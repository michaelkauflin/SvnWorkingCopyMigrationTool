using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnWorkingCopyMigrationTool.Model
{
    class WorkingCopyFinder
    {
        public async Task<IEnumerable<WorkingCopy>> FindAll()
        {
            var workingCopies = new List<WorkingCopy>();
            List<string> driveInfos = await Task.Run(() =>
            {
                var directories = new List<string>();

                DriveInfo.GetDrives()
                    .Where(d => d.IsReady)
                    .Where(d => !d.Name.Equals("G:\\", StringComparison.InvariantCultureIgnoreCase))
                    .Where(d => !d.Name.Equals("T:\\", StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
                    .ForEach(
                        drive =>
                        {
                            try
                            {
                                directories.AddRange(Directory.EnumerateDirectories(drive.RootDirectory.FullName));
                            }
                            catch (IOException)
                            {
                            }
                        });

                return directories;
            });

            driveInfos = driveInfos.Where(d => !d.Equals(Environment.GetEnvironmentVariable("windir"))).ToList();

            await Task.Run(() =>
            {
                driveInfos.AsParallel().ToList().ForEach(di =>
                {
                    var wcs = FindInDirectory(di);
                    workingCopies.AddRange(wcs);
                });
            });

            var result = new StringBuilder();
            workingCopies.ForEach(wc => result.AppendLine(wc.RootPath));

            return workingCopies;
        }

        public IEnumerable<WorkingCopy> FindInDirectory(string rootDirectory)
        {
            var workingCopies = new List<WorkingCopy>();
            try
            {
                IEnumerable<string> subDirectories = Directory.EnumerateDirectories(rootDirectory);
                if (IsSvnWorkingCopy(rootDirectory))
                {
                    workingCopies.Add(WorkingCopy.Parse(rootDirectory));
                }
                workingCopies.AddRange(from sub in subDirectories where IsSvnWorkingCopy(sub) select WorkingCopy.Parse(sub));
            }
            catch (UnauthorizedAccessException)
            {
            }

            return workingCopies;
        }

        private static bool IsSvnWorkingCopy(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            try
            {
                string svnDir = Directory.EnumerateDirectories(folderPath).First(d => d.EndsWith("\\.svn"));
                return
                    Directory.EnumerateFiles(svnDir)
                        .Any(f => f.EndsWith("wc.db", StringComparison.InvariantCultureIgnoreCase));
            }
            catch
            {
                // ignored
                return false;
            }
        }
    }
}
