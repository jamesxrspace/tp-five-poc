using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Addressable.Command.Editor
{
    using TPFive.Extended.Addressable.Editor;

    public static class Utility
    {
        // TODO: May extract the calling process as this shares among upload and download.
        internal static async Task HandleDownload(
            ILogger logger,
            string commandLineArguments,
            System.Action<float> progressCallback)
        {
            var platform = TPFive.Game.Editor.Utility.GetPlatform();
            if (string.IsNullOrEmpty(platform))
            {
                logger.LogError(
                    "{Method} - platform: {platform}",
                    nameof(HandleDownload),
                    platform);
                return;
            }

            var combinedExePath = Path.Combine(
                Define.ExeParentPath,
                platform,
                Define.ExeName);
            var absoluteExePath = Path.GetFullPath(combinedExePath);

            logger.LogDebug(
                "{Method} - absoluteExePath: {absoluteExePath}",
                nameof(HandleDownload),
                absoluteExePath);

            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = absoluteExePath;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            logger.LogDebug("{Method} - startInfo: {startInfo}", nameof(HandleDownload), startInfo);

            // Keep the pipe usage for now even though the use in Unity is not making any difference.
            await using var pipeRead = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

            commandLineArguments = $@"{commandLineArguments} --to-write-pipe ""{pipeRead.GetClientHandleAsString()}""";
            startInfo.Arguments = commandLineArguments;

            var process = System.Diagnostics.Process.Start(startInfo);

            logger.LogDebug("{Method} - process: {process}", nameof(HandleDownload), process);
            if (process is null)
            {
                return;
            }

            logger.LogDebug("Start another process {fileName} {arguments}", startInfo.FileName, startInfo.Arguments);

            pipeRead.DisposeLocalCopyOfClientHandle();

            using var sr = new StreamReader(pipeRead);

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

                var pipeTask = Task.Run(() =>
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        var result = float.TryParse(line, out var progress);
                        if (result)
                        {
                            progressCallback?.Invoke(progress);
                        }
                    }
                });

                await Task.WhenAll(consoleTask, pipeTask);
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

                sr.Close();

                logger.LogDebug($"ExitCode: {exitCode}", exitCode);
            }
        }
    }
}
