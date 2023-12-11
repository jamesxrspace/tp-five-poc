using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Views;
using UnityEngine;

namespace TPFive.Extended.LoxodonFramework
{
    public class ResourcesViewLocator : UIViewLocatorBase
    {
        private GlobalWindowManagerBase globalWindowManager;
        private Dictionary<string, WeakReference> templates = new Dictionary<string, WeakReference>();

        public override T LoadView<T>(string name)
        {
            return DoLoadView<T>(name);
        }

        public override IProgressResult<float, T> LoadViewAsync<T>(string name)
        {
            var result = new ProgressResult<float, T>();
            Executors.RunOnCoroutineNoReturn(DoLoad<T>(result, name));
            return result;
        }

        public override T LoadWindow<T>(string name)
        {
            return LoadWindow<T>(null, name);
        }

        public override T LoadWindow<T>(IWindowManager windowManager, string name)
        {
            if (windowManager == null)
            {
                windowManager = this.GetDefaultWindowManager();
            }

            T target = this.DoLoadView<T>(name);
            if (target != null)
            {
                target.WindowManager = windowManager;
            }

            return target;
        }

        public override IProgressResult<float, T> LoadWindowAsync<T>(string name)
        {
            return this.LoadWindowAsync<T>(null, name);
        }

        public override IProgressResult<float, T> LoadWindowAsync<T>(IWindowManager windowManager, string name)
        {
            if (windowManager == null)
            {
                windowManager = this.GetDefaultWindowManager();
            }

            var result = new ProgressResult<float, T>();
            Executors.RunOnCoroutineNoReturn(DoLoad(result, name, windowManager));
            return result;
        }

        protected string Normalize(string name)
        {
            int index = name.IndexOf('.');
            return index < 0 ? name : name[..index];
        }

        protected virtual IWindowManager GetDefaultWindowManager()
        {
            if (globalWindowManager != null)
            {
                return globalWindowManager;
            }

            globalWindowManager = UnityEngine.Object.FindObjectOfType<GlobalWindowManagerBase>();
            if (globalWindowManager == null)
            {
                throw new NotFoundException("GlobalWindowManager");
            }

            return globalWindowManager;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "<None>")]
        protected virtual T DoLoadView<T>(string name)
        {
            name = Normalize(name);
            GameObject viewTemplateGo = null;
            try
            {
                if (this.templates.TryGetValue(name, out WeakReference weakRef) && weakRef.IsAlive)
                {
                    viewTemplateGo = (GameObject)weakRef.Target;

                    // Check if the object is valid because it may have been destroyed.
                    // Unmanaged objects,the weak caches do not accurately track the validity of objects.
                    if (viewTemplateGo != null)
                    {
                        string goName = viewTemplateGo.name;
                    }
                }
            }
            catch (Exception ex) when (ex is InvalidCastException ||
                                       ex is NullReferenceException)
            {
                viewTemplateGo = null;
            }

            if (viewTemplateGo == null)
            {
                viewTemplateGo = Resources.Load<GameObject>(name);
                if (viewTemplateGo != null)
                {
                    viewTemplateGo.SetActive(false);
                    this.templates[name] = new WeakReference(viewTemplateGo);
                }
            }

            if (viewTemplateGo == null || viewTemplateGo.GetComponent<T>() == null)
            {
                return default;
            }

            GameObject go = UnityEngine.Object.Instantiate(viewTemplateGo);
            go.name = viewTemplateGo.name;
            T view = go.GetComponent<T>();
            if (view == null && go != null)
            {
                UnityEngine.Object.Destroy(go);
            }

            return view;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "<None>")]
        protected virtual IEnumerator DoLoad<T>(IProgressPromise<float, T> promise, string name, IWindowManager windowManager = null)
        {
            name = Normalize(name);
            GameObject viewTemplateGo = null;
            try
            {
                if (this.templates.TryGetValue(name, out WeakReference weakRef) && weakRef.IsAlive)
                {
                    viewTemplateGo = (GameObject)weakRef.Target;

                    // Check if the object is valid because it may have been destroyed.
                    // Unmanaged objects,the weak caches do not accurately track the validity of objects.
                    if (viewTemplateGo != null)
                    {
                        string goName = viewTemplateGo.name;
                    }
                }
            }
            catch (Exception ex) when (ex is InvalidCastException ||
                                       ex is NullReferenceException)
            {
                viewTemplateGo = null;
            }

            if (viewTemplateGo == null)
            {
                ResourceRequest request = Resources.LoadAsync<GameObject>(name);
                while (!request.isDone)
                {
                    promise.UpdateProgress(request.progress);
                    yield return null;
                }

                viewTemplateGo = (GameObject)request.asset;
                if (viewTemplateGo != null)
                {
                    viewTemplateGo.SetActive(false);
                    this.templates[name] = new WeakReference(viewTemplateGo);
                }
            }

            if (viewTemplateGo == null || viewTemplateGo.GetComponent<T>() == null)
            {
                promise.UpdateProgress(1f);
                promise.SetException(new NotFoundException(name));
                yield break;
            }

            GameObject go = UnityEngine.Object.Instantiate(viewTemplateGo);
            go.name = viewTemplateGo.name;
            if (!go.TryGetComponent<T>(out var view))
            {
                UnityEngine.Object.Destroy(go);
                promise.SetException(new NotFoundException(name));
            }
            else
            {
                if (windowManager != null && view is IWindow)
                {
                    (view as IWindow).WindowManager = windowManager;
                }

                promise.UpdateProgress(1f);
                promise.SetResult(view);
            }
        }
    }
}
