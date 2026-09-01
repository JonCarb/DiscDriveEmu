using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static EmuDiscReader.MainWindow;

namespace EmuDiscReader
{
    internal class Cache
    {
        public async Task<StorageFile?> CacheGame(string gameName, string gamePath, IProgress<double>? progress = null)
        {
            try
            {
                StorageFolder destination;

                try
                {
                    destination = await AppService.DocumentsFolder.GetFolderAsync("CachedGames");
                }
                catch
                {
                    destination = await AppService.DocumentsFolder.CreateFolderAsync("CachedGames");
                }

                StorageFolder subFolder;

                try
                {
                    subFolder = await destination.GetFolderAsync(gameName);
                }
                catch
                {
                    subFolder = await destination.CreateFolderAsync(gameName);
                }

                StorageFile source = await StorageFile.GetFileFromPathAsync(gamePath);

                ulong totalBytes = (await source.GetBasicPropertiesAsync()).Size;

                string destinationPath = Path.Combine(subFolder.Path, gameName);

                if (File.Exists(destinationPath))
                {
                    FileInfo cachedFile = new FileInfo(destinationPath);

                    if ((ulong)cachedFile.Length == totalBytes)
                    {
                        return await StorageFile.GetFileFromPathAsync(destinationPath);
                    }
                    // Cache is incomplete/corrupt
                    File.Delete(destinationPath);
                }

                long copiedBytes = 0;

                const int bufferSize = 8 * 1024 * 1024; //8mb buffer
                byte[] buffer = new byte[bufferSize];

                using (FileStream input = new FileStream(
                    gamePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize,
                    FileOptions.SequentialScan | FileOptions.Asynchronous))
                using (FileStream output = new FileStream(
                    destinationPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize,
                    FileOptions.SequentialScan | FileOptions.Asynchronous))
                {
                    int bytesRead;

                    double lastReportedPercentage = -1;

                    while (true)
                    {

                        var sw = Stopwatch.StartNew();

                        Console.WriteLine(
                            $"BEFORE READ | Position: {input.Position:N0}/{totalBytes:N0}");

                        bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);

                        sw.Stop();

                        Console.WriteLine(
                            $"AFTER READ | {bytesRead:N0} bytes | " +
                            $"READ TIME: {sw.Elapsed.TotalSeconds:F2}s");

                        if (bytesRead <= 0)
                            break;

                        await output.WriteAsync(
                            buffer, 0, bytesRead);

                        Console.WriteLine(
                            $"WRITE | {bytesRead:N0} bytes");

                        copiedBytes += bytesRead;

                        double percentage =
                            (double)copiedBytes / totalBytes * 100.0;

                        if (percentage - lastReportedPercentage >= 1.0 ||
                            percentage >= 100.0)
                        {
                            lastReportedPercentage = percentage;

                            Console.WriteLine(
                                $"PROGRESS: {percentage:F1}%");

                            progress?.Report(percentage);
                        }
                    }
                }

                return await StorageFile.GetFileFromPathAsync(destinationPath);
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
        }
    }
}