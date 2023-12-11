using System;
using System.Collections;
using System.Collections.Generic;

namespace TPFive.Game.UI.Collections
{
    // [DropOutStack with threshold line]
    //
    // ├╴╴╴╴╴╴╴╴╴╴┤ → top       │
    // ├╴╴╴╴╴╴╴╴╴╴┤              │ → upper than the threshold is TopArea (for stackable window to reload)
    // ├╴╴╴╴╴╴╴╴╴╴┤ → threshold │
    // ├──────────┤              │
    // ├──────────┤              │
    // ├──────────┤              │ → lower than the threshold is DownArea (for stackable window to release)
    // ├──────────┤              │
    // └──────────┘ → buttom    │

    /// <summary>
    /// An Enumerable Drop Out Stack Class.
    /// </summary>
    /// <typeparam name="T">Some Generic Class.</typeparam>
    [Serializable]
    public class DropOutStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// The stack collection.
        /// </summary>
        private readonly T[] _items;

        /// <summary>
        /// The top - where the next item will be pushed in the array.
        /// </summary>
        private int _top; // The position in the array that the next item will be placed.

        /// <summary>
        /// The current number of items in the stack.
        /// </summary>
        private int _count; // The amount of items in the array.

        /// <summary>
        /// The number of items near the top.
        /// </summary>
        private int _threshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropOutStack{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity of the stack.</param>
        /// <param name="threshold">The size from the stack top.</param>
        public DropOutStack(int capacity, int threshold)
        {
            _items = new T[capacity];
            _threshold = threshold;
        }

        /// <summary>
        /// Triggered when a item is across the threshold from an area near the stack bottom.
        /// </summary>
        public event Action<T> OnItemUpperThanThreshold;

        /// <summary>
        /// Triggered when a item is across the threshold from an area near the stack top.
        /// </summary>
        public event Action<T> OnItemLowerThanThreshold;

        /// <summary>
        /// Pushes the specified item onto the stack.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Push(T item)
        {
            _count += 1;
            _count = _count > _items.Length ? _items.Length : _count;

            _items[_top] = item;

            // Find whick item is pushed to the lower area
            if (OnItemLowerThanThreshold != null)
            {
                if (_count > _threshold)
                {
                    int thresholdlineIndex = _top - _threshold;
                    thresholdlineIndex = thresholdlineIndex >= 0 ? thresholdlineIndex : thresholdlineIndex + _items.Length;

                    OnItemLowerThanThreshold.Invoke(_items[thresholdlineIndex]);
                }
            }

            _top = (_top + 1) % _items.Length; // After filling the array the next item will be placed at the beginning of the array!
        }

        /// <summary>
        /// Pops last item from the stack.
        /// </summary>
        /// <returns>T.</returns>
        public T Pop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("stack is empty");
            }

            _count -= 1;
            _count = _count < 0 ? 0 : _count;

            _top = (_items.Length + _top - 1) % _items.Length;

            // Find whick item is poped to the upper area
            if (OnItemUpperThanThreshold != null)
            {
                if (_count >= _threshold)
                {
                    int thresholdlineIndex = _top - _threshold;
                    thresholdlineIndex = thresholdlineIndex >= 0 ? thresholdlineIndex : thresholdlineIndex + _items.Length;

                    OnItemUpperThanThreshold.Invoke(_items[thresholdlineIndex]);
                }
            }

            return _items[_top];
        }

        /// <summary>
        /// Peeks at last item on the stack.
        /// </summary>
        /// <returns>T.</returns>
        public T Peek()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("stack is empty");
            }

            // Same as pop but without changing the value of top.
            return _items[(_items.Length + _top - 1) % _items.Length];
        }

        /// <summary>
        /// Returns the amount of elements on the stack.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int Count()
        {
            return _count;
        }

        /// <summary>
        /// Returns is stack is empty.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool IsEmpty()
        {
            return _count == 0;
        }

        /// <summary>
        /// Returns is stack is full.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool IsFull()
        {
            return _count == _items.Length;
        }

        /// <summary>
        /// Gets an item from the stack.
        /// Index 0 is the last item pushed onto the stack.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.InvalidOperationException">Index out of bounds.</exception>
        public T GetItem(int index)
        {
            if (index > Count())
            {
                throw new InvalidOperationException("Index out of bounds");
            }

            // The first element = last element entered = index 0 is at Peek - see above.
            // index 0 = items[(items.Length + top - 1) % items.Length];
            // index 1 = items[(items.Length + top - 2) % items.Length];
            // index 2 = items[(items.Length + top - 3) % items.Length]; etc...
            // So to get an item at a certain index is:
            // items[(items.Length + top - (index+1)) % items.Length];
            return _items[(_items.Length + _top - (index + 1)) % _items.Length];
        }

        /// <summary>
        /// Clears the stack.
        /// </summary>
        public void Clear()
        {
            _count = 0;
        }

        /// <summary>
        /// Returns an enumerator for a generic stack that iterates through the stack.
        /// The iterator start at the last item pushed onto the stack and goes backwards.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count(); i++)
            {
                yield return GetItem(i);
            }
        }

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
