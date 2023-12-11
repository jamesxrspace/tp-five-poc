/*
 * Server API - AIGC
 *
 * The Restful APIs of AIGC.
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

#pragma warning disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BestHTTP;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XRSpace.OpenAPI;
using XRSpace.OpenAPI.Utilities;
using VContainer;
using TPFive.OpenApi.GameServer.Model;

namespace TPFive.OpenApi.GameServer
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IAigcApi
    {
        /// <summary>
        /// Generate motion from uploaded music file. 
        /// </summary>
        /// <param name="generateMotionRequest"></param>
        /// <returns>Task &lt; GenerateMotionResponse &gt;</returns>
        Task<GenerateMotionResponse> GenerateMotionAsync(GenerateMotionRequest generateMotionRequest = default, RequestConfig requestConfig = default, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the AigcApi endpoints.
    /// </summary>
    public class AigcApi : IAigcApi
    {
        private readonly ILogger logger;
        private readonly IServerBaseUriProvider serverBaseUriProvider;
        private readonly IAuthTokenProvider authTokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AigcApi"/> class.
        /// </summary>
        /// <param name="loggerFactory"> an instance of ILoggerFactory</param>        
        /// <param name="serverBaseUriProvider"> an instance of IServerBaseUriProvider</param>
        /// <param name="authTokenProvider"> an instance of IAuthTokenProvider</param>
        [Inject]
        public AigcApi(
            ILoggerFactory loggerFactory,
            IServerBaseUriProvider serverBaseUriProvider,
            IAuthTokenProvider authTokenProvider)
        {
            this.logger = loggerFactory.CreateLogger<AigcApi>();
            this.serverBaseUriProvider = serverBaseUriProvider;
            this.authTokenProvider = authTokenProvider;
        }

        public async Task<GenerateMotionResponse> GenerateMotionAsync(GenerateMotionRequest generateMotionRequest = default, RequestConfig requestConfig = null, CancellationToken cancellationToken = default)
        {
            string path = "/api/v1/aigc/motion/generate";

            try
            {
                return await OpenApiUtil.RequestAsync<GenerateMotionResponse>(CreateRequest, authTokenProvider, requestConfig, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("{Method}(): Be canceled.", nameof(GenerateMotionAsync));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError("{Method}(): Failed. Exception: {Exception}", nameof(GenerateMotionAsync), ex.Message);
                throw;
            }

            HTTPRequest CreateRequest()
            {
                var uri = new Uri(serverBaseUriProvider.BaseUri, path);
                var httpRequest = new HTTPRequest(uri, HTTPMethods.Post);
                httpRequest.DisableCache = true;

                // Bearer authentication required
                httpRequest.AddHeader("Authorization", $"Bearer {authTokenProvider.GetAuthToken()}");

                // Content Type
                httpRequest.AddHeader("Content-Type", "application/json");

                // Accept Type
                httpRequest.AddHeader("Accept", "application/json");

                // Body
                if (generateMotionRequest != null)
                {
                    httpRequest.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(generateMotionRequest));
                }

                return httpRequest;
            }
        }
    }
}
#pragma warning restore