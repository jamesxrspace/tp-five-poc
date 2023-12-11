using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TPFive.Extended.LocalConfig.Editor.Tests
{
    public class LocalConfigTest
    {
        private ILoggerFactory _factory = new NullLoggerFactory();
        
        [SetUp]
        public void Setup()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CheckSetInt()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, int>("Port", 8080);
            
            var getResult = serviceProvider.IntValueTable.TryGetValue("Port", out var actual);
            
            var expected = 8080;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckSetFloat()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, float>("PI", 3.1415926f);
            
            var getResult = serviceProvider.FloatValueTable.TryGetValue("PI", out var actual);
            
            var expected = 3.1415926f;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.True(Mathf.Approximately(expected, actual));
        }

        [Test]
        public void CheckSetString()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, string>("Company", "XrSpace");
            
            var getResult = serviceProvider.StringValueTable.TryGetValue("Company", out var actual);
            
            var expected = "XrSpace";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckSetScriptableObject()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseSO = ScriptableObject.CreateInstance<TestUseSO>();
            testUseSO.Name = "Abc";
            var setResult = serviceProvider.SetT<string, ScriptableObject>("TestUseConfig", testUseSO);
            
            var getResult = serviceProvider.ScriptableObjectValueTable.TryGetValue("TestUseConfig", out var scriptableObject);

            var castedTestUseSO = scriptableObject as TestUseSO;
            
            var expected = "Abc";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseSO);
            Assert.AreEqual(expected, castedTestUseSO.Name);
        }

        [Test]
        public void CheckSetSystemObject()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseClass = new TestUseClass();
            testUseClass.Id = 1234;
            var setResult = serviceProvider.SetT<string, System.Object>("TestUseClass", testUseClass);
            
            var getResult = serviceProvider.ObjectValueTable.TryGetValue("TestUseClass", out var testUseClassObject);

            var castedTestUseClass = testUseClassObject as TestUseClass;
            
            var expected = 1234;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseClass);
            Assert.AreEqual(expected, castedTestUseClass.Id);
        }

        [Test]
        public void CheckGetInt()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, int>("Port", 8080);

            var (getResult, actual) = serviceProvider.GetIntValue("Port");
            
            var expected = 8080;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckGetFloat()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, float>("PI", 3.1415926f);

            var (getResult, actual) = serviceProvider.GetFloatValue("PI");
            
            var expected = 3.1415926f;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.True(Mathf.Approximately(expected, actual));
        }

        [Test]
        public void CheckGetString()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, string>("Company", "XrSpace");
            
            var (getResult, actual) = serviceProvider.GetT<string, string>("Company");
            
            var expected = "XrSpace";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckGetScriptableObject()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseSO = ScriptableObject.CreateInstance<TestUseSO>();
            testUseSO.Name = "Abc";
            var setResult = serviceProvider.SetT<string, ScriptableObject>("TestUseConfig", testUseSO);
            
            var (getResult, scriptableObject) = serviceProvider.GetT<string, ScriptableObject>("TestUseConfig");

            var castedTestUseSO = scriptableObject as TestUseSO;
            
            var expected = "Abc";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseSO);
            Assert.AreEqual(expected, castedTestUseSO.Name);
        }

        [Test]
        public void CheckGetSystemObject()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseClass = new TestUseClass();
            testUseClass.Id = 1234;
            var setResult = serviceProvider.SetT<string, System.Object>("TestUseClass", testUseClass);
            
            var (getResult, testUseClassObject) = serviceProvider.GetT<string, System.Object>("TestUseClass");

            var castedTestUseClass = testUseClassObject as TestUseClass;
            
            var expected = 1234;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseClass);
            Assert.AreEqual(expected, castedTestUseClass.Id);
        }

        [Test]
        public void CheckRemoveInt()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, int>("Port", 8080);

            var (getResult1, value) = serviceProvider.GetIntValue("Port");
            
            var removeResult = serviceProvider.RemoveT<string, int>("Port");
            
            var (getResult2, valueOnceAgain) = serviceProvider.GetIntValue("Port");
            
            var count = serviceProvider.IntValueTable.Count;
            
            var expected = 8080;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.AreEqual(expected, value);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(0, valueOnceAgain);
            Assert.Zero(count);
        }

        [Test]
        public void CheckRemoveFloat()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, float>("PI", 3.1415926f);
            
            var (getResult1, value) = serviceProvider.GetFloatValue("PI");

            var removeResult = serviceProvider.RemoveT<string, float>("PI");
            
            var (getResult2, valueOnceAgain) = serviceProvider.GetFloatValue("PI");

            var count = serviceProvider.FloatValueTable.Count;
            
            var expected = 3.1415926f;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.True(Mathf.Approximately(expected, value));
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(0, valueOnceAgain);
            Assert.Zero(count);
        }

        [Test]
        public void CheckRemoveString()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = serviceProvider.SetT<string, string>("Company", "XrSpace");
            
            var (getResult1, value) = serviceProvider.GetT<string, string>("Company");
            
            var removeResult = serviceProvider.RemoveT<string, string>("Company");
            
            var (getResult2, valueOnceAgain) = serviceProvider.GetT<string, string>("Company");
            
            var count = serviceProvider.StringValueTable.Count;
            
            var expected = "XrSpace";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.AreEqual(expected, value);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(default(string), valueOnceAgain);
            Assert.Zero(count);            
        }

        [Test]
        public void CheckRemoveScriptableObject()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseSO = ScriptableObject.CreateInstance<TestUseSO>();
            testUseSO.Name = "Abc";
            var setResult = serviceProvider.SetT<string, ScriptableObject>("TestUseConfig", testUseSO);
            
            var (getResult1, value) = serviceProvider.GetT<string, ScriptableObject>("TestUseConfig");

            var removeResult = serviceProvider.RemoveT<string, ScriptableObject>("TestUseConfig");
            
            var (getResult2, valueOnceAgain) = serviceProvider.GetT<string, ScriptableObject>("TestUseConfig");

            var count = serviceProvider.ScriptableObjectValueTable.Count;

            var castedTestUseSO = value as TestUseSO;
            
            var expected = "Abc";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.NotNull(castedTestUseSO);
            Assert.AreEqual(expected, castedTestUseSO.Name);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(default(ScriptableObject), valueOnceAgain);
            Assert.Zero(count);            
        }

        [Test]
        public void CheckRemoveSystemObject()
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseClass = new TestUseClass();
            testUseClass.Id = 1234;
            var setResult = serviceProvider.SetT<string, System.Object>("TestUseClass", testUseClass);
            
            var getResult = serviceProvider.ObjectValueTable.TryGetValue("TestUseClass", out var testUseClassObject);
            
            var (getResult1, value) = serviceProvider.GetT<string, System.Object>("TestUseClass");

            var removeResult = serviceProvider.RemoveT<string, object>("TestUseClass");
            
            var (getResult2, valueOnceAgain) = serviceProvider.GetT<string, object>("TestUseClass");

            var count = serviceProvider.ObjectValueTable.Count;
            
            var castedTestUseClass = testUseClassObject as TestUseClass;
            
            var expected = 1234;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.NotNull(castedTestUseClass);
            Assert.AreEqual(expected, castedTestUseClass.Id);
            Assert.AreEqual(removeResult, true);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(default(object), valueOnceAgain);
            Assert.Zero(count);            
        }

        [UnityTest]
        public IEnumerator CheckSetIntAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, int>("Port", 8080);
            
            var getResult = serviceProvider.IntValueTable.TryGetValue("Port", out var actual);
            
            var expected = 8080;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        });
        
        [UnityTest]
        public IEnumerator CheckSetFloatAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, float>("PI", 3.1415926f);
            
            var getResult = serviceProvider.FloatValueTable.TryGetValue("PI", out var actual);
            
            var expected = 3.1415926f;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.True(Mathf.Approximately(expected, actual));
        });

        [UnityTest]
        public IEnumerator CheckSetStringAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, string>("Company", "XrSpace");
            
            var getResult = serviceProvider.StringValueTable.TryGetValue("Company", out var actual);
            
            var expected = "XrSpace";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        });
        
        [UnityTest]
        public IEnumerator CheckSetScriptableObjectAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseSO = ScriptableObject.CreateInstance<TestUseSO>();
            testUseSO.Name = "Abc";
            var setResult = await serviceProvider.SetAsync<string, ScriptableObject>("TestUseConfig", testUseSO);
            
            var getResult = serviceProvider.ScriptableObjectValueTable.TryGetValue("TestUseConfig", out var scriptableObject);

            var castedTestUseSO = scriptableObject as TestUseSO;
            
            var expected = "Abc";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseSO);
            Assert.AreEqual(expected, castedTestUseSO.Name);
        });
        
        [UnityTest]
        public IEnumerator CheckSetSystemObjectAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseClass = new TestUseClass();
            testUseClass.Id = 1234;
            var setResult = await serviceProvider.SetAsync<string, System.Object>("TestUseClass", testUseClass);
            
            var getResult = serviceProvider.ObjectValueTable.TryGetValue("TestUseClass", out var testUseClassObject);

            var castedTestUseClass = testUseClassObject as TestUseClass;
            
            var expected = 1234;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseClass);
            Assert.AreEqual(expected, castedTestUseClass.Id);
        });
        
        [UnityTest]
        public IEnumerator CheckGetIntAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, int>("Port", 8080);

            var (getResult, actual) = await serviceProvider.GetIntValueAsync("Port");
            
            var expected = 8080;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        });
        
        [UnityTest]
        public IEnumerator CheckGetFloatAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, float>("PI", 3.1415926f);

            var (getResult, actual) = await serviceProvider.GetFloatValueAsync("PI");
            
            var expected = 3.1415926f;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.True(Mathf.Approximately(expected, actual));
        });
        
        [UnityTest]
        public IEnumerator CheckGetStringAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, string>("Company", "XrSpace");
            
            var (getResult, actual) = await serviceProvider.GetAsync<string, string>("Company");
            
            var expected = "XrSpace";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.AreEqual(expected, actual);
        });
        
        [UnityTest]
        public IEnumerator CheckGetScriptableObjectAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseSO = ScriptableObject.CreateInstance<TestUseSO>();
            testUseSO.Name = "Abc";
            var setResult = await serviceProvider.SetAsync<string, ScriptableObject>("TestUseConfig", testUseSO);
            
            var (getResult, scriptableObject) = await serviceProvider.GetAsync<string, ScriptableObject>("TestUseConfig");

            var castedTestUseSO = scriptableObject as TestUseSO;
            
            var expected = "Abc";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseSO);
            Assert.AreEqual(expected, castedTestUseSO.Name);
        });
        
        [UnityTest]
        public IEnumerator CheckGetSystemObjectAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseClass = new TestUseClass();
            testUseClass.Id = 1234;
            var setResult = await serviceProvider.SetAsync<string, System.Object>("TestUseClass", testUseClass);
            
            var (getResult, testUseClassObject) = await serviceProvider.GetAsync<string, System.Object>("TestUseClass");

            var castedTestUseClass = testUseClassObject as TestUseClass;
            
            var expected = 1234;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult);
            Assert.NotNull(castedTestUseClass);
            Assert.AreEqual(expected, castedTestUseClass.Id);
        });
        
        [UnityTest]
        public IEnumerator CheckRemoveIntAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, int>("Port", 8080);

            var (getResult1, value) = await serviceProvider.GetIntValueAsync("Port");
            
            var removeResult = await serviceProvider.RemoveAsync<string, int>("Port");
            
            var (getResult2, valueOnceAgain) = await serviceProvider.GetIntValueAsync("Port");
            
            var count = serviceProvider.IntValueTable.Count;
            
            var expected = 8080;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.AreEqual(expected, value);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(0, valueOnceAgain);
            Assert.Zero(count);
        });
        
        [UnityTest]
        public IEnumerator CheckRemoveFloatAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, float>("PI", 3.1415926f);
            
            var (getResult1, value) = await serviceProvider.GetFloatValueAsync("PI");

            var removeResult = await serviceProvider.RemoveAsync<string, float>("PI");
            
            var (getResult2, valueOnceAgain) = await serviceProvider.GetFloatValueAsync("PI");

            var count = serviceProvider.FloatValueTable.Count;
            
            var expected = 3.1415926f;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.True(Mathf.Approximately(expected, value));
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(0, valueOnceAgain);
            Assert.Zero(count);
        });
        
        [UnityTest]
        public IEnumerator CheckRemoveStringAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var setResult = await serviceProvider.SetAsync<string, string>("Company", "XrSpace");
            
            var (getResult1, value) = await serviceProvider.GetAsync<string, string>("Company");
            
            var removeResult = await serviceProvider.RemoveAsync<string, string>("Company");
            
            var (getResult2, valueOnceAgain) = await serviceProvider.GetAsync<string, string>("Company");
            
            var count = serviceProvider.StringValueTable.Count;
            
            var expected = "XrSpace";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.AreEqual(expected, value);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(default(string), valueOnceAgain);
            Assert.Zero(count);
        });
        
        [UnityTest]
        public IEnumerator CheckRemoveScriptableObjectAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseSO = ScriptableObject.CreateInstance<TestUseSO>();
            testUseSO.Name = "Abc";
            var setResult = await serviceProvider.SetAsync<string, ScriptableObject>("TestUseConfig", testUseSO);
            
            var (getResult1, value) = await serviceProvider.GetAsync<string, ScriptableObject>("TestUseConfig");

            var removeResult = await serviceProvider.RemoveAsync<string, ScriptableObject>("TestUseConfig");
            
            var (getResult2, valueOnceAgain) = await serviceProvider.GetAsync<string, ScriptableObject>("TestUseConfig");

            var count = serviceProvider.ScriptableObjectValueTable.Count;

            var castedTestUseSO = value as TestUseSO;
            
            var expected = "Abc";
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.NotNull(castedTestUseSO);
            Assert.AreEqual(expected, castedTestUseSO.Name);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(default(ScriptableObject), valueOnceAgain);
            Assert.Zero(count);
        });
        
        [UnityTest]
        public IEnumerator CheckRemoveSystemObjectAsync() => UniTask.ToCoroutine(async () =>
        {
            TPFive.Extended.LocalConfig.ServiceProvider serviceProvider = new (
                _factory,
                new TPFive.Game.Config.NullServiceProvider(null),
                null);

            var testUseClass = new TestUseClass();
            testUseClass.Id = 1234;
            var setResult = await serviceProvider.SetAsync<string, System.Object>("TestUseClass", testUseClass);
            
            var getResult = serviceProvider.ObjectValueTable.TryGetValue("TestUseClass", out var testUseClassObject);
            
            var (getResult1, value) = await serviceProvider.GetAsync<string, System.Object>("TestUseClass");

            var removeResult = await serviceProvider.RemoveAsync<string, object>("TestUseClass");
            
            var (getResult2, valueOnceAgain) = await serviceProvider.GetAsync<string, object>("TestUseClass");

            var count = serviceProvider.ObjectValueTable.Count;
            
            var castedTestUseClass = testUseClassObject as TestUseClass;
            
            var expected = 1234;
            
            Assert.AreEqual(true, setResult);
            Assert.AreEqual(true, getResult1);
            Assert.NotNull(castedTestUseClass);
            Assert.AreEqual(expected, castedTestUseClass.Id);
            Assert.AreEqual(removeResult, true);
            Assert.AreEqual(false, getResult2);
            Assert.AreEqual(default(object), valueOnceAgain);
            Assert.Zero(count);
        });        
    }
}
