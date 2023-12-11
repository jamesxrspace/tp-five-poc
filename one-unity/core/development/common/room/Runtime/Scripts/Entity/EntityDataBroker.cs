using Fusion;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Messages;
using VContainer;

namespace TPFive.Room
{
    public abstract class EntityDataBroker<TEntityData, TNetEntityData> : NetworkBehaviour
        where TNetEntityData : struct, INetworkStruct
    {
        [Inject]
        private IAsyncPublisher<QueryEntityData<TEntityData>> _queryPublisher;
        [Inject]
        private IPublisher<EntityData<TNetEntityData>> _dataPublisher;
        [Inject]
        private ILoggerFactory _loggerFactory;
        private Microsoft.Extensions.Logging.ILogger _logger;

        private Microsoft.Extensions.Logging.ILogger Logger
        {
            get
            {
                return _logger ??= _loggerFactory.CreateLogger<EntityDataBroker<TEntityData, TNetEntityData>>();
            }
        }

        public override void Spawned()
        {
            if (Runner.LocalPlayer == PlayerRef.None)
            {
                // This peer is dedicated server and therefore doesn't represent a player.
                return;
            }

            // Send out message to query entity data.
            Logger.LogInformation("Publish {Message}<{EntityDataType}> locally", nameof(QueryEntityData<TEntityData>), GetEntityDataTypeName());
            _queryPublisher.Publish(new QueryEntityData<TEntityData>(OnQueryResult));
        }

        // In derived class's implementation, the given entity data should be sent to host/server through Fusion RPC.
        protected abstract void SendEntityData(TEntityData data);

        protected abstract string GetEntityDataTypeName();

        protected abstract string GetNetEntityDataTypeName();

        // Derived class should invoke OnEntityDataReceived when it receives entity data through Fusion RPC.
        protected void OnEntityDataReceived(PlayerRef playerRef, ref TNetEntityData netEntityData)
        {
            Logger.LogInformation("Receive {NetEntityDataType} from remote player({PlayerRef})", GetNetEntityDataTypeName(), playerRef);
            Logger.LogInformation("Publish {Message}<{NetEntityDataType}> locally", nameof(EntityData<TNetEntityData>), GetNetEntityDataTypeName());

            // Send out the entity data received from client locally through message.
            _dataPublisher.Publish(new EntityData<TNetEntityData>(playerRef, ref netEntityData));
        }

        private void OnQueryResult(TEntityData entityData, bool isSuccess)
        {
            if (isSuccess)
            {
                Logger.LogInformation("Receive {EntityDataType} locally", GetEntityDataTypeName());
                Logger.LogInformation("Send {NetEntityDataType} to host/server through RPC", GetNetEntityDataTypeName());
                SendEntityData(entityData);
            }
            else
            {
                Logger.LogError("Failed querying {EntityDataType} locally", GetEntityDataTypeName());
            }
        }
    }
}
