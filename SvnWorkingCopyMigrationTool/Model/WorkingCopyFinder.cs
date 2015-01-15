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
        public async Task<IEnumerable<WorkingCopy>> FindAll(int depth)
        {
            var workingCopies = new List<WorkingCopy>();
            List<string> driveInfos = await Task.Run(() =>
            {
                var directories = new List<string>();

                DriveInfo.GetDrives()
                    .Where(d => d.IsReady)
                    .Where(d => !d.Name.Equals("G:\\", StringComparison.InvariantCultureIgnoreCase)) // Global Phonak shared directory, cannot contain working copies
                    .Where(d => !d.Name.Equals("T:\\", StringComparison.InvariantCultureIgnoreCase)) // Transfer shared directory, too huge. User will have to specify his transfer folder
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

            // Exclude Windows folders
            driveInfos = driveInfos
                .Where(d => !d.Equals(Environment.GetEnvironmentVariable("windir")))
                .Where(d => !d.Equals(Environment.GetEnvironmentVariable("temp")))
                .ToList();

            await Task.Run(() =>
            {
                driveInfos.AsParallel().ToList().ForEach(di =>
                {
                    var wcs = FindInDirectory(di, depth - 1);
                    workingCopies.AddRange(wcs);
                });
            });

            var result = new StringBuilder();
            workingCopies.ForEach(wc => result.AppendLine(wc.RootPath));

            return workingCopies;
        }

        public IEnumerable<WorkingCopy> FindInDirectory(string rootDirectory, int depth)
        {
            var workingCopies = new List<WorkingCopy>();

            if (IsSvnWorkingCopy(rootDirectory))
            {
                workingCopies.Add(WorkingCopy.Parse(rootDirectory));
            }

            if (depth == 0)
            {
                return workingCopies;
            }

            try
            {
                Directory.EnumerateDirectories(rootDirectory).AsParallel().ToList().ForEach(d => workingCopies.AddRange(FindInDirectory(d, depth - 1)));
            }
            catch
            {
                // ignored
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
