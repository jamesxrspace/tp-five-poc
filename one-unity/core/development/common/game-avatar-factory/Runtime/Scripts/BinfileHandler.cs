using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BestHTTP;

namespace TPFive.Game.Avatar.Factory
{
    public interface IBinfileDownloader
    {
        /// <summary>
        /// Downloads a file from a specified URL asynchronously and returns a Task with
        /// a tuple containing the file path and any potential error messages.
        /// </summary>
        /// <param name="urlString">The URL from which the file should be downloaded.</param>
        /// <param name="maxRetries">The maximum number of retry attempts in case of download failure (default is 3).</param>
        /// <param name="connectTimeout">Maximum time we wait to establish the connection to the target server.
        /// If set to TimeSpan.Zero or lower, no connect timeout logic is executed. Default value is 8 seconds.</param>
        /// <param name="requestTimeout">Maximum time we want to wait to the request to finish after the connection is established.
        /// Default value is 300 seconds..</param>
        /// <param name="cancellationToken">A CancellationToken to allow for cancellation of the download operation
        /// (default is CancellationToken.None).</param>
        /// <returns>A Task containing a tuple with the file path and any error message encountered during the download.</returns>
        Task<(string filePath, string error)> DownloadAsync(
            string urlString,
            int maxRetries = 3,
            int connectTimeout = 8,
            int requestTimeout = 300,
            CancellationToken cancellationToken = default);
    }

    public sealed class BinfileHandler : IBinfileDownloader
    {
        private const int DefaultBufferSize = 4096;

        public async Task<(string filePath, string error)> DownloadAsync(
            string urlString,
            int maxRetries,
            int connectTimeout,
            int requestTimeout,
            CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(urlString, UriKind.Absolute, out var url))
            {
                throw new ArgumentException($"Invalid {nameof(urlString)}");
            }

            if (maxRetries < 0)
            {
                throw new ArgumentException($"Invalid {nameof(maxRetries)}");
            }

            if (connectTimeout <= 0)
            {
                throw new ArgumentException($"Invalid {nameof(connectTimeout)}");
            }

            if (requestTimeout <= 0)
            {
                throw new ArgumentException($"Invalid {nameof(requestTimeout)}");
            }

            string etag;
            byte[] data;
            try
            {
                var request = new HTTPRequest(url)
                {
                    DisableCache = false,
                    ConnectTimeout = TimeSpan.FromSeconds(connectTimeout),
                    Timeout = TimeSpan.FromSeconds(requestTimeout),
                    MaxRetries = maxRetries,
                };
                var response = await request.GetHTTPResponseAsync(cancellationToken);
                etag = response.GetFirstHeaderValue("ETag")?.Replace("\"", string.Empty);
                data = response.Data;
            }
            catch (AsyncHTTPException e)
            {
                return (null, e.Message);
            }

            if (etag == null)
            {
                return (null, "ETag is null");
            }

            var outputDir = GetBinfileOuputDir(etag);
            var filePath = Path.Combine(outputDir, AvatarConstants.DefaultBinfileFileName);
            if (File.Exists(filePath))
            {
                return (filePath, null);
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string error = null;
            try
            {
                using var fileStream = new FileStream(
                    filePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: DefaultBufferSize,
                    useAsync: true);
                await fileStream.WriteAsync(data, 0, data.Length, cancellationToken);
            }
            catch (ArgumentNullException)
            {
                error = "Data is null";
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch
#pragma warning disable ERP022 // Unobserved exception in generic exception handler
                    {
                    }
#pragma warning restore ERP022 // Unobserved exception in generic exception handler
                }

                throw;
            }
            catch (Exception e)
            {
                error = $"Exception: {e}";
            }

            return (filePath, error);
        }

        private string GetBinfileOuputDir(string dirName)
        {
            return Path.Combine(AvatarConstants.GetBinfileRootDir(), dirName);
        }
    }
}
