using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Creator.Editor;

namespace TPFive.Creator.Bundle.Command.Editor
{
    public static class UploadUgcUtility
    {
        internal static async Task HandleUpload(
            ILogger logger,
            string commandLineArguments)
        {
            var platform = Utility.GetPlatform();
            if (string.IsNullOrEmpty(platform))
            {
                logger.LogError("Platform not supported.");
                return;
            }

            var combinedExePath = Path.Combine(
                Define.UgcExeParentPath,
                Utility.GetPlatform(),
                Define.UgcExeName);
            var absoluteExePath = Path.GetFullPath(combinedExePath);

            logger.LogDebug(
                "{Method} - absoluteExePath: {absoluteExePath}",
                nameof(HandleUpload), absoluteExePath);

            // Start external process
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = absoluteExePath;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            logger.LogDebug("{Method} - startInfo: {startInfo}", nameof(HandleUpload), startInfo);

            startInfo.Arguments = commandLineArguments;

            var process = System.Diagnostics.Process.Start(startInfo);

            logger.LogDebug("{Method} - process: {process}", nameof(HandleUpload), process);
            if (process is null) return;

            logger.LogDebug("Start another process {fileName} {arguments}",startInfo.FileName, startInfo.Arguments);

            try
            {
                var consoleTask = Task.Run(() =>
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        logger.LogDebug(line);
                    }
                });

                await consoleTask;
            }
            catch (System.Exception e)
            {
                logger.LogError($"Exception: {e}");
            }
            finally
            {
                process.WaitForExit();
                var exitCode = process.ExitCode;

                process.Close();

                logger.LogDebug($"ExitCode: {exitCode}", exitCode);
            }
        }
    }
}