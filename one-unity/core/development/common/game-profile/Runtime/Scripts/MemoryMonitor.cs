using System;
using Tayx.Graphy;

namespace TPFive.Game.Profile
{
    public class MemoryMonitor
    {
        private static int nextDebugPacketID = 1;

        private readonly int debugPacketID;
        private readonly MemAllocKind memAllocKind;
        private readonly GraphyDebugger.DebugPacket warningDebugPacket;
        private readonly GraphyDebugger.DebugPacket normalDebugPacket;

        public MemoryMonitor(MemAllocKind memAllocKind, float memWarningThreshold)
        {
            debugPacketID = nextDebugPacketID++;
            this.memAllocKind = memAllocKind;
            warningDebugPacket = CreateDebugPacket(debugPacketID, memAllocKind, memWarningThreshold, GraphyDebugger.DebugComparer.Greater_than, StartWarning);
            normalDebugPacket = CreateDebugPacket(debugPacketID, memAllocKind, memWarningThreshold, GraphyDebugger.DebugComparer.Equals_or_less_than, StopWarning);
        }

        public event Action<MemAllocKind, bool> OnWarningStatusChanged;

        public void Start()
        {
            GraphyDebugger.Instance.AddNewDebugPacket(warningDebugPacket);
        }

        public void Stop()
        {
            GraphyDebugger.Instance.RemoveAllDebugPacketsWithId(debugPacketID);
        }

        private GraphyDebugger.DebugPacket CreateDebugPacket(
            int id,
            MemAllocKind memAllocKind,
            float memAllocThreshold,
            GraphyDebugger.DebugComparer comparer,
            Action callback)
        {
            var debugPacket = new GraphyDebugger.DebugPacket { Id = id };
            var condition = new GraphyDebugger.DebugCondition
            {
                Comparer = comparer,
                Variable = memAllocKind == MemAllocKind.Unity ? GraphyDebugger.DebugVariable.Ram_Allocated : GraphyDebugger.DebugVariable.Ram_Mono,
                Value = memAllocThreshold,
            };
            debugPacket.DebugConditions.Add(condition);
            debugPacket.Callbacks.Add(callback);
            return debugPacket;
        }

        private void StartWarning()
        {
            OnWarningStatusChanged?.Invoke(memAllocKind, true);
            GraphyDebugger.Instance.AddNewDebugPacket(normalDebugPacket);
        }

        private void StopWarning()
        {
            OnWarningStatusChanged?.Invoke(memAllocKind, false);
            GraphyDebugger.Instance.AddNewDebugPacket(warningDebugPacket);
        }
    }
}