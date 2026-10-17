using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace EDD;

public class Install
{
    //Installs copy of what's on the disc and returns the path of the copy
    public async Task<string?> InstallGame(string gamePath, IProgress<double>? progress = null)
    {
        if (App.Services.Settings == null) return null;
        if (!App.Services.Settings.WillCache) return null;
            
        var ret = await Task.Run(async () =>
        {
            try
            {
                string destination = App.Services.InstallPath;
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

                string subfolder = System.IO.Path.Combine(destination, Path.GetFileNameWithoutExtension(gamePath));
                if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);
                
                var source = new FileInfo(gamePath);
                long totalBytes = source.Length;
                string finalDestination = Path.Combine(subfolder, source.Name);
                
                //check to see if file exist
                if (File.Exists(finalDestination))
                {
                    FileInfo installedFile = new(finalDestination);
                    if(installedFile.Length == totalBytes) return finalDestination;
                    File.Delete(finalDestination);
                }
                
                Console.WriteLine("Copy started");
                long copiedBytes = 0;
                const int bufferSize = 8 * 1024 * 1024; //8mb buffer
                byte[] buffer = new byte[bufferSize];
                await using (FileStream input = new(
                                 gamePath,
                                 FileMode.Open,
                                 FileAccess.Read,
                                 FileShare.Read,
                                 bufferSize,
                                 FileOptions.SequentialScan | FileOptions.Asynchronous))
                await using (FileStream output = new(
                                 finalDestination,
                                 FileMode.Create,
                                 FileAccess.Write,
                                 FileShare.None,
                                 bufferSize,
                                 FileOptions.SequentialScan | FileOptions.Asynchronous))
                {
                    double lastReportedPercentage = -1;

                    while (App.Services.Settings.WillCache)
                    {
                        var bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead <= 0)
                        {
                            break;
                        }

                        await output.WriteAsync(buffer, 0, bytesRead);

                        copiedBytes += bytesRead;

                        double percentage = (double)copiedBytes / totalBytes * 100.0;

                        if (percentage - lastReportedPercentage >= 1.0 || percentage >= 100.0)
                        {
                            lastReportedPercentage = percentage;
                            Console.WriteLine($"PROGRESS: {percentage:F1}%");
                            progress?.Report(percentage);
                        }
                    }
                }

                return !App.Services.Settings.WillCache ? null : finalDestination;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {ex.Message}");
                return null;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Source file not found: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred: {ex.Message}");
                return null;
            }
        });
        return ret;
    }

    public static async Task<string?> CallMPF(string consoleType, DriveInfo disc)
    {        
        var ret = await Task.Run(() =>
        {
            if (App.Services.Settings?.MPF == null) return null;
            var mpf = App.Services.Settings.MPF;
            FileInfo fi = new FileInfo(mpf);
            
            var combination = disc.VolumeLabel + disc.TotalSize;
            var path = Path.Combine(App.Services.InstallPath, combination);
            path = Path.Combine(path, combination + ".iso");
            Console.WriteLine(path);
            
            if(File.Exists(path)) return path; //check if size are the same
            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                WorkingDirectory = fi.DirectoryName,
                FileName = fi.Name,
                Arguments = consoleType + " -d " + disc.Name +  " -f " + "\"" + path + "\"",
                UseShellExecute = true,
                CreateNoWindow = false
            };
            Process? p = Process.Start(psi);
            p?.WaitForExit();
            Console.WriteLine("Done");
            
            return File.Exists(path) ? path : null;
        });
        
        //since we dont check if the install was complete, make sure to compare size to see if it was a full install
        
        return ret;
    }
}