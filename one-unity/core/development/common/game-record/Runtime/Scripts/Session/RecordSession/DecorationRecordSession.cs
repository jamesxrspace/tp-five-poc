using System.Collections.Generic;
using System.Linq;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record
{
    public class DecorationRecordSession : RecordSession
    {
        private IEnumerable<DecorationRecordData> _recordData;

        public DecorationRecordSession(ILogger logger)
            : base(logger)
        {
        }

        public override void Setup(RecordData[] recordData)
        {
            _recordData = recordData.OfType<DecorationRecordData>().ToArray();
        }

        public override IEnumerable<RecordData> GetRecordData()
        {
            return _recordData;
        }

        public override void Dispose()
        {
            _recordData = null;
        }
    }
}