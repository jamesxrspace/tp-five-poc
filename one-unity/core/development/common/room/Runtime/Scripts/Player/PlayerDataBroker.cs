using Fusion;
using TPFive.Game.User;

namespace TPFive.Room
{
    public class PlayerDataBroker : EntityDataBroker<User, NetPlayerData>
    {
        protected override void SendEntityData(User user)
        {
            SendPlayerDataRPC(new NetPlayerData(user));
        }

        protected override string GetEntityDataTypeName()
        {
            return nameof(User);
        }

        protected override string GetNetEntityDataTypeName()
        {
            return nameof(NetPlayerData);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority, InvokeResim = true, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void SendPlayerDataRPC(NetPlayerData playerData, RpcInfo rpcInfo = default)
        {
            OnEntityDataReceived(rpcInfo.Source, ref playerData);
        }
    }
}