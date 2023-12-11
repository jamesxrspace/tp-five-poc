namespace TPFive.Extended.Space
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Space;
    using TPFive.OpenApi.GameServer;
    using GameSpaceGroup = Game.Space.SpaceGroup;
    using GameSpace = Game.Space.Space;
    using System.Linq;
    using Cysharp.Threading.Tasks;

    public sealed partial class ServiceProvider :
        IServiceProvider
    {
        private const int maxPageSize = 50;
        private readonly ISpaceApi SpaceApi;
        private readonly ILogger logger;

        public ServiceProvider(ISpaceApi spaceApi, ILoggerFactory loggerFactory)
        {
            this.SpaceApi = spaceApi;
            this.logger = loggerFactory.CreateLogger<ServiceProvider>();
        }

        public async UniTask<List<GameSpaceGroup>> GetSpaceGroups(CancellationToken cancellationToken)
        {
            var response = await this.SpaceApi.GetSpaceGroupListAsync(0, maxPageSize, null, cancellationToken);
            if (!response.IsSuccess)
            {
                this.logger.LogWarning($"Failed to get space group list, code={response.HttpStatusCode}, error_code={response.ErrorCode}, msg={response.Message}");
                return new List<GameSpaceGroup>();
            }

            return response.Data.Items.Select(MakeGameSpaceGroup).ToList();
        }

        public async UniTask<List<GameSpace>> GetSpaces(string spaceGroupId, CancellationToken cancellationToken)
        {
            var response = await this.SpaceApi.GetSpaceListAsync(0, maxPageSize, spaceGroupId, null, cancellationToken);
            if (!response.IsSuccess)
            {
                this.logger.LogWarning($"Failed to get sapce list, code={response.HttpStatusCode}, error_code={response.ErrorCode}, msg={response.Message}");
                return new List<GameSpace>();
            }
            return response.Data.Items.Select(MakeGameSpace).ToList();
        }

        private static GameSpaceGroup MakeGameSpaceGroup(OpenApi.GameServer.Model.SpaceGroup spaceGroup)
        {
            return new GameSpaceGroup()
            {
                Id = spaceGroup.SpaceGroupId,
                Name = spaceGroup.Name,
                ThumbnailUrl = spaceGroup.Thumbnail,
                Description = spaceGroup.Description,
                SpaceIds = spaceGroup.Spaces?.Select(space => space.SpaceId).ToList(),
            };
        }

        private static GameSpace MakeGameSpace(OpenApi.GameServer.Model.Space space)
        {
            return new GameSpace()
            {
                Id = space.SpaceId,
                Name = space.Name,
                ThumbnailUrl = space.Thumbnail,
                SceneKey = space.Addressable,
            };
        }
    }
}
