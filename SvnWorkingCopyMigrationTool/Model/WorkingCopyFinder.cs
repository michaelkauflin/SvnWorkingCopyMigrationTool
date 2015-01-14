using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                foreach (string sub in subDirectories)
                {
                    if (IsSvnWorkingCopy(sub))
                    {
                        workingCopies.Add(WorkingCopy.Parse(sub));
                    }
                }
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
                return Directory.EnumerateDirectories(folderPath).Any(d => d.EndsWith("\\.svn"));
            }
            catch
            {
                // ignored
                return false;
            }
        }
    }
}
