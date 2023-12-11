/*
 * Server API - Room
 *
 * The Restful APIs of Room.
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
    public interface IRoomApi
    {
        /// <summary>
        /// Register one user of a specified room with GameServer. Register one user of a specified room with GameServer.
        /// </summary>
        /// <param name="roomUserRegistry"></param>
        /// <returns>Task &lt; RegisterRoomUserResponse &gt;</returns>
        Task<RegisterRoomUserResponse> RegisterRoomUserAsync(RoomUserRegistry roomUserRegistry, RequestConfig requestConfig = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unregister one user of a specified room with GameServer. Unregister one user of a specified room with GameServer.
        /// </summary>
        /// <param name="roomUserRegistry"></param>
        /// <returns>Task &lt; UnregisterRoomUserResponse &gt;</returns>
        Task<UnregisterRoomUserResponse> UnregisterRoomUserAsync(RoomUserRegistry roomUserRegistry, RequestConfig requestConfig = default, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the RoomApi endpoints.
    /// </summary>
    public class RoomApi : IRoomApi
    {
        private readonly ILogger logger;
        private readonly IServerBaseUriProvider serverBaseUriProvider;
        private readonly IAuthTokenProvider authTokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomApi"/> class.
        /// </summary>
        /// <param name="loggerFactory"> an instance of ILoggerFactory</param>        
        /// <param name="serverBaseUriProvider"> an instance of IServerBaseUriProvider</param>
        /// <param name="authTokenProvider"> an instance of IAuthTokenProvider</param>
        [Inject]
        public RoomApi(
            ILoggerFactory loggerFactory,
            IServerBaseUriProvider serverBaseUriProvider,
            IAuthTokenProvider authTokenProvider)
        {
            this.logger = loggerFactory.CreateLogger<RoomApi>();
            this.serverBaseUriProvider = serverBaseUriProvider;
            this.authTokenProvider = authTokenProvider;
        }

        public async Task<RegisterRoomUserResponse> RegisterRoomUserAsync(RoomUserRegistry roomUserRegistry, RequestConfig requestConfig = null, CancellationToken cancellationToken = default)
        {
            string path = "/api/v1/room/join";

            try
            {
                return await OpenApiUtil.RequestAsync<RegisterRoomUserResponse>(CreateRequest, authTokenProvider, requestConfig, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("{Method}(): Be canceled.", nameof(RegisterRoomUserAsync));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError("{Method}(): Failed. Exception: {Exception}", nameof(RegisterRoomUserAsync), ex.Message);
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
                if (roomUserRegistry != null)
                {
                    httpRequest.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(roomUserRegistry));
                }

                return httpRequest;
            }
        }

        public async Task<UnregisterRoomUserResponse> UnregisterRoomUserAsync(RoomUserRegistry roomUserRegistry, RequestConfig requestConfig = null, CancellationToken cancellationToken = default)
        {
            string path = "/api/v1/room/leave";

            try
            {
                return await OpenApiUtil.RequestAsync<UnregisterRoomUserResponse>(CreateRequest, authTokenProvider, requestConfig, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("{Method}(): Be canceled.", nameof(UnregisterRoomUserAsync));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError("{Method}(): Failed. Exception: {Exception}", nameof(UnregisterRoomUserAsync), ex.Message);
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
                if (roomUserRegistry != null)
                {
                    httpRequest.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(roomUserRegistry));
                }

                return httpRequest;
            }
        }
    }
}
#pragma warning restore