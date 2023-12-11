using System;
using System.Collections.Generic;

namespace TPFive.Room
{
    /// <summary>
    /// Active list maintains a list of active items and 'active' here means 'not expired'.
    /// </summary>
    /// <typeparam name="TItem">type of itms in list.</typeparam>
    public class ActiveList<TItem>
    {
        private readonly LinkedList<ItemInfo> itemInfos = new ();
        private readonly Dictionary<TItem, LinkedListNode<ItemInfo>> indexes = new ();

        public ActiveList(TimeSpan itemLifetime)
        {
            ItemLifetime = itemLifetime;
        }

        public TimeSpan ItemLifetime { get; private set; }

        public int Count => itemInfos.Count;

        public bool Add(TItem item)
        {
            if (indexes.ContainsKey(item))
            {
                return false;
            }

            indexes[item] = itemInfos.AddLast(new ItemInfo(item, DateTime.UtcNow + ItemLifetime));
            return true;
        }

        public bool Remove(TItem item)
        {
            if (indexes.Remove(item, out var node))
            {
                itemInfos.Remove(node);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// trim the expired items from the list.
        /// </summary>
        /// <param name="dueDate">the time considered as the due date for all the items in the list.</param>
        /// <param name="expiredItems">the set containing the trimmed expired items.</param>
        /// <param name="checkLimit">
        /// limit of the amount of items which are gonna be checked.
        /// Caller can use this parameter to prevent the method from using too much time.
        /// Zero means no limit.
        /// </param>
        /// <returns>
        /// true if all expired items have been trimmed from the list.
        /// false if some expired items might be still left in the list but the trimming process stops because check count has reached the given 'checkLimit'.
        /// </returns>
        public bool TrimExpiredItems(DateTime dueDate, ISet<TItem> expiredItems, int checkLimit = 0)
        {
            int checkCount = 0;
            var nextItemInfoNode = itemInfos.First;
            while (nextItemInfoNode != null)
            {
                var itemInfo = nextItemInfoNode.Value;
                if (dueDate < itemInfo.DueDate)
                {
                    break;
                }

                if (checkLimit > 0 && checkCount >= checkLimit)
                {
                    return false;
                }

                expiredItems?.Add(itemInfo.Item);
                nextItemInfoNode = nextItemInfoNode.Next;
                Remove(itemInfo.Item);

                checkCount += 1;
            }

            return true;
        }

        public Enumerator GetEnumerator()
        {
            var listEnumerator = itemInfos.GetEnumerator();
            return new Enumerator(
                () => listEnumerator.MoveNext(),
                () => listEnumerator.Current.Item);
        }

        public readonly struct Enumerator
        {
            private readonly Func<TItem> currentFunc;
            private readonly Func<bool> moveNextFunc;

            public Enumerator(Func<bool> moveNextFunc, Func<TItem> currentFunc)
            {
                this.moveNextFunc = moveNextFunc;
                this.currentFunc = currentFunc;
            }

            public readonly TItem Current => currentFunc();

            public readonly bool MoveNext()
            {
                return moveNextFunc();
            }
        }

        private class ItemInfo : Tuple<TItem, DateTime>
        {
            public ItemInfo(TItem item, DateTime dueDate)
                : base(item, dueDate)
            {
            }

            public TItem Item => Item1;

            public DateTime DueDate => Item2;
        }
    }
}
