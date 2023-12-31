/*
 * Server API - Space
 *
 * The Restful APIs of Space.
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
    public interface ISpaceApi
    {
        /// <summary>
        /// Get the list of available space groups 
        /// </summary>
        /// <param name="offset">current page</param>
        /// <param name="size">number of items per page</param>
        /// <returns>Task &lt; GetSpaceGroupListResponse &gt;</returns>
        Task<GetSpaceGroupListResponse> GetSpaceGroupListAsync(int offset, int size, RequestConfig requestConfig = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the list of available space 
        /// </summary>
        /// <param name="offset">current page (start from 0)</param>
        /// <param name="size">number of items per page</param>
        /// <param name="spaceGroupId">related space group id</param>
        /// <returns>Task &lt; GetSpaceListResponse &gt;</returns>
        Task<GetSpaceListResponse> GetSpaceListAsync(int offset, int size, string spaceGroupId = default, RequestConfig requestConfig = default, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the SpaceApi endpoints.
    /// </summary>
    public class SpaceApi : ISpaceApi
    {
        private readonly ILogger logger;
        private readonly IServerBaseUriProvider serverBaseUriProvider;
        private readonly IAuthTokenProvider authTokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceApi"/> class.
        /// </summary>
        /// <param name="loggerFactory"> an instance of ILoggerFactory</param>        
        /// <param name="serverBaseUriProvider"> an instance of IServerBaseUriProvider</param>
        /// <param name="authTokenProvider"> an instance of IAuthTokenProvider</param>
        [Inject]
        public SpaceApi(
            ILoggerFactory loggerFactory,
            IServerBaseUriProvider serverBaseUriProvider,
            IAuthTokenProvider authTokenProvider)
        {
            this.logger = loggerFactory.CreateLogger<SpaceApi>();
            this.serverBaseUriProvider = serverBaseUriProvider;
            this.authTokenProvider = authTokenProvider;
        }

        public async Task<GetSpaceGroupListResponse> GetSpaceGroupListAsync(int offset, int size, RequestConfig requestConfig = null, CancellationToken cancellationToken = default)
        {
            string path = "/api/v1/space/group/list";

            // Build the query string into the path
            var paramMap = new Multimap<string, string>();
            HttpUtil.ParameterToMultiMap("multi", "offset", offset, paramMap);
            HttpUtil.ParameterToMultiMap("multi", "size", size, paramMap);
            path = HttpUtil.SetQueryParameter(path, paramMap);

            try
            {
                return await OpenApiUtil.RequestAsync<GetSpaceGroupListResponse>(CreateRequest, authTokenProvider, requestConfig, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("{Method}(): Be canceled.", nameof(GetSpaceGroupListAsync));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError("{Method}(): Failed. Exception: {Exception}", nameof(GetSpaceGroupListAsync), ex.Message);
                throw;
            }

            HTTPRequest CreateRequest()
            {
                var uri = new Uri(serverBaseUriProvider.BaseUri, path);
                var httpRequest = new HTTPRequest(uri, HTTPMethods.Get);
                httpRequest.DisableCache = true;

                // Bearer authentication required
                httpRequest.AddHeader("Authorization", $"Bearer {authTokenProvider.GetAuthToken()}");

                // Accept Type
                httpRequest.AddHeader("Accept", "application/json");

                return httpRequest;
            }
        }

        public async Task<GetSpaceListResponse> GetSpaceListAsync(int offset, int size, string spaceGroupId = default, RequestConfig requestConfig = null, CancellationToken cancellationToken = default)
        {
            string path = "/api/v1/space/list";

            // Build the query string into the path
            var paramMap = new Multimap<string, string>();
            HttpUtil.ParameterToMultiMap("multi", "offset", offset, paramMap);
            HttpUtil.ParameterToMultiMap("multi", "size", size, paramMap);
            HttpUtil.ParameterToMultiMap("multi", "space_group_id", spaceGroupId, paramMap);
            path = HttpUtil.SetQueryParameter(path, paramMap);

            try
            {
                return await OpenApiUtil.RequestAsync<GetSpaceListResponse>(CreateRequest, authTokenProvider, requestConfig, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("{Method}(): Be canceled.", nameof(GetSpaceListAsync));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError("{Method}(): Failed. Exception: {Exception}", nameof(GetSpaceListAsync), ex.Message);
                throw;
            }

            HTTPRequest CreateRequest()
            {
                var uri = new Uri(serverBaseUriProvider.BaseUri, path);
                var httpRequest = new HTTPRequest(uri, HTTPMethods.Get);
                httpRequest.DisableCache = true;

                // Bearer authentication required
                httpRequest.AddHeader("Authorization", $"Bearer {authTokenProvider.GetAuthToken()}");

                // Accept Type
                httpRequest.AddHeader("Accept", "application/json");

                return httpRequest;
            }
        }
    }
}
#pragma warning restore