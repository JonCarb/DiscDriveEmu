using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

                        if (bytesRead <= 0) { break; }

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

    public static async Task<string?> CallMPF(string? consoleType, DriveInfo? disc)
    {        
        if(consoleType == null || disc == null) return null;
        var ret = await Task.Run(() =>
        {
            try
            {
                if (App.Services.Settings == null) return null;
                
                string? dumper = App.Services.Settings.Dumper;
                Console.WriteLine($"Dumper: {dumper}");
                if (dumper == null) return null;
                
                string? name = GetName(consoleType, disc);
                Console.WriteLine($"Name: {name}");
                if (name == null) return null;
                
                var path = Path.Combine(App.Services.InstallPath, name);
                Console.WriteLine($"path: {path}");
                
                string? file = null;
                if (Directory.Exists(path))
                { 
                    file = Directory.EnumerateFiles(path).FirstOrDefault(f => Path.GetExtension(f) is ".iso" or ".bin");
                    if (file != null) return file;
                }
                else Directory.CreateDirectory(path);

                FileInfo fi = new(dumper);
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    WorkingDirectory = fi.DirectoryName,
                    FileName = fi.Name,
                    Arguments = $"disc --drive=\"{disc.Name[..2]}\" --image-path=\"{path}\" --image-name=\"{name}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                
                Process? p = Process.Start(psi);
                p?.WaitForExit();
                
                return Directory.EnumerateFiles(path).FirstOrDefault(f => Path.GetExtension(f) is ".iso" or ".bin");
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

    private static string? GetName(string gameConsole, DriveInfo disc)
    {
        string? ret = null;
        switch (gameConsole)
        {
            case "PS2":
                ret = disc.VolumeLabel;
                break;
            case "PS3":
                ret = GetPs3Name(disc.RootDirectory.FullName);
                ret = ret?.Replace("\0", "");
                break;
            case "Xbox360":
                ret  = GetXbox360Name(disc);
                break;
            //to do add original xbox
        }

        return ret;
    }
    private static string? GetPs3Name(string discRoot)
    {
        const int offset = 0x220;
        const int bufLength = 16;
        Console.WriteLine(discRoot);
        string? file = Directory.EnumerateFiles(discRoot).FirstOrDefault("PS3_DISC.SFB");
        try
        {
            using FileStream fs = new(file, FileMode.Open, FileAccess.Read);
            fs.Seek(offset, SeekOrigin.Begin);
            var buf = new byte[bufLength];
            fs.ReadExactly(buf, 0, bufLength);
            return System.Text.Encoding.ASCII.GetString(buf);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }

    private static string? GetXbox360Name(DriveInfo? disc)
    {
        if(disc == null) return null;
        try
        {
            string? file = Directory.EnumerateFiles(disc.RootDirectory.FullName).FirstOrDefault("Default.XEX");
            using FileStream stream = File.OpenRead(file);
            byte[] hashBytes = SHA256.HashData(stream);
            return disc.VolumeLabel + Convert.ToHexString(hashBytes)[..8].ToLowerInvariant();
        }
        catch
        {
            return disc.VolumeLabel + disc.TotalSize;
        }
    }
}