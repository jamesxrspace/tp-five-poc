namespace TPFive.Game.Messages
{
    public class QueryEntityData<TEntityData>
    {
        public QueryEntityData(QueryResultHandler onQueryResult)
        {
            OnQueryResult = onQueryResult;
        }

        public delegate void QueryResultHandler(TEntityData data, bool isSuccess);

        public QueryResultHandler OnQueryResult { get; private set; }
    }
}