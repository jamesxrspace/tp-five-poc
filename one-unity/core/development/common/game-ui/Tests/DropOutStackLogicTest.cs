using NUnit.Framework;
using TPFive.Game.UI.Collections;

namespace TPFive.Game.UI.Tests
{
    public class DropOutStackLogicTest
    {
        [Test]
        public void TestPush()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.Push(0);
            var result = stack.GetItem(0);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void TestItemLowerThanThreshold()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.OnItemLowerThanThreshold += OnItemLowerThanThreshold;
            stack.Push(0);
            stack.Push(1);
            stack.OnItemLowerThanThreshold -= OnItemLowerThanThreshold;

            static void OnItemLowerThanThreshold(int item)
            {
                Assert.AreEqual(item, 0);
            }
        }

        [Test]
        public void TestPushOverCapacity()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.Push(0);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            var result = stack.GetItem(0);
            Assert.AreEqual(result, 3);
            Assert.AreEqual(stack.Count(), 3);
        }

        [Test]
        public void TestPop()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.Push(0);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            var result = stack.Pop();
            Assert.AreEqual(result, 3);
            Assert.AreEqual(stack.Count(), 2);
        }

        [Test]
        public void TestPopEmpty()
        {
            var stack = new DropOutStack<int>(3, 1);
            Assert.Throws<System.InvalidOperationException>(() => stack.Pop());
        }

        [Test]
        public void TestItemUpperThanThreshold()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.OnItemUpperThanThreshold += OnItemUpperThanThreshold;
            stack.Push(0);
            stack.OnItemUpperThanThreshold -= OnItemUpperThanThreshold;

            static void OnItemUpperThanThreshold(int item)
            {
                Assert.AreEqual(item, 0);
            }
        }

        [Test]
        public void TestPeek()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.Push(0);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            var result = stack.Peek();
            Assert.AreEqual(result, 3);
        }

        [Test]
        public void TestPeekEmpty()
        {
            var stack = new DropOutStack<int>(3, 1);
            Assert.Throws<System.InvalidOperationException>(() => stack.Peek());
        }

        [Test]
        public void TestGetItem()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.Push(0);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            var result = stack.GetItem(0);
            Assert.AreEqual(result, 3);
        }

        [Test]
        public void TestGetItemEmpty()
        {
            var stack = new DropOutStack<int>(3, 1);
            stack.Push(0);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            Assert.Throws<System.InvalidOperationException>(() => stack.GetItem(4));
        }
    }
}
