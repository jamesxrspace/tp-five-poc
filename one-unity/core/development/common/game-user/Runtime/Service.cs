namespace TPFive.Game.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using MessagePipe;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Logging;
    using TPFive.Game.Messages;
    using TPFive.OpenApi.GameServer;
    using TPFive.OpenApi.GameServer.Model;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using UnityEngine.Pool;
    using VContainer;
    using Assert = UnityEngine.Assertions.Assert;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [AsyncStartable]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private static readonly int StringListPoolInitialCapacity = 5;

        /// <summary>
        /// Assuming a space can accommodate up to 50 players,
        /// each player gets an AvatarProfile once, so the maximum
        /// requirement for the stringList is 50.
        /// </summary>
        private static readonly int StringListPoolMaxCapacity = 50;

        private readonly ILoginApi loginApi;
        private readonly IAvatarApi avatarApi;
        private readonly IObjectPool<List<string>> stringListPool;
        private readonly IDisposable msgSubscription;
        private bool disposed = false;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory,
            ILoginApi loginApi,
            IAvatarApi avatarApi,
            IAsyncSubscriber<QueryEntityData<User>> userDataQuerySubscriber)
        {
            Logger = Utility.CreateLogger<Service>(loggerFactory);
            this.loginApi = loginApi;
            msgSubscription = DisposableBag.Create(userDataQuerySubscriber.Subscribe(OnQueryUserData));
            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);
            this.avatarApi = avatarApi;
            stringListPool = new LinkedPool<List<string>>(
                () => new List<string>(1),
                x => x.Capacity = StringListPoolInitialCapacity,
                x => x.Clear(),
                x => x.Clear(),
                true,
                StringListPoolMaxCapacity);
        }

        public IServiceProvider NullServiceProvider
        {
            get
            {
                return _serviceProviderTable[(int)ServiceProviderKind.NullServiceProvider]
                    as IServiceProvider;
            }
        }

        private ILogger Logger { get; set; }

        public async UniTask<User> GetUserAsync()
        {
            Logger.LogDebug("enter {Method}()", nameof(GetUserAsync));
            var response = await loginApi.GetUserProfileAsync();
            if (!response.IsSuccess)
            {
                if (Logger.IsErrorEnabled())
                {
                    Logger.LogError(
                        "{Method}(): Failed. http_code: {httpCode} , err_code: {errCode} , err_msg: {msg}",
                        nameof(GetUserAsync),
                        response.HttpStatusCode,
                        response.ErrorCode,
                        response.Message);
                }

                return null;
            }

            var userProfile = response.Data;
            if (userProfile == null)
            {
                if (Logger.IsErrorEnabled())
                {
                    Logger.LogError("{Method}(): Failed. Response data (User profile) is null", nameof(GetUserAsync));
                }

                return null;
            }

            if (Logger.IsDebugEnabled())
            {
                Logger.LogDebug("{Method}(): Success.", nameof(GetUserAsync));
            }

            return CreateUser(userProfile);
        }

        public async UniTask<IAvatarProfile> GetAvatarProfile(string userId, CancellationToken token)
        {
            Assert.IsFalse(string.IsNullOrEmpty(userId));
            Assert.IsNotNull(avatarApi);

            var userIds = stringListPool.Get();
            userIds.Add(userId);
            try
            {
                var result = await avatarApi.GetCurrentAvatarMetadataListAsync(
                    userIds,
                    userIds.Count,
                    0,
                    cancellationToken: token).AsUniTask();
                var metadata = result?.Data?.Items?.Find(x =>
                {
                    return x != null && string.Equals(x.Xrid, userId, StringComparison.Ordinal);
                });
                if (metadata != null)
                {
                    return new ReadOnlyAvatarProfile(metadata);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.LogError(e, "GetAvatarProfile failed.");
            }
            finally
            {
                stringListPool.Release(userIds);
            }

            return default;
        }

        public async UniTask<int> GetAvatarProfiles(
            List<string> userIds,
            List<IAvatarProfile> profiles,
            CancellationToken token)
        {
            Assert.IsNotNull(userIds);
            Assert.IsNotNull(profiles);
            Assert.IsNotNull(avatarApi);

            if (userIds.Count == 0)
            {
                return 0;
            }

            try
            {
                var result = await avatarApi.GetCurrentAvatarMetadataListAsync(
                    userIds,
                    userIds.Count,
                    0,
                    cancellationToken: token).AsUniTask();

                if (result != null && result.Data != null && result.Data.Items != null)
                {
                    var newProfiles = result.Data.Items.Where(x => x != null)
                        .Select(x => new ReadOnlyAvatarProfile(x));
                    profiles.AddRange(newProfiles);
                    return newProfiles.Count();
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.LogError(e, "GetAvatarProfiles failed.");
            }

            return default;
        }

        private User CreateUser(Profile profile)
        {
            return new User
            {
                Uid = profile.XrId,
                Id = profile.Username,
                Nickame = profile.Nickname,
            };
        }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncOperationCanceledException(
            OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            Exception e,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        private async UniTask OnQueryUserData(QueryEntityData<User> query, CancellationToken token)
        {
            Logger.LogInformation("Recive {Message} local message", nameof(QueryEntityData<User>));
            var user = await GetUserAsync();
            query.OnQueryResult?.Invoke(user, user != null);
        }

        private void HandleDispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                stringListPool.Clear();
                msgSubscription?.Dispose();
                disposed = true;
            }
        }
    }
}
