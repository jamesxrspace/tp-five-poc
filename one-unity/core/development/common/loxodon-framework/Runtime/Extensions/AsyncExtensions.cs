using System;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Asynchronous;
using UnityEngine.Assertions;

namespace TPFive.Extended.LoxodonFramework
{
    public static class LoxodonFrameworkAsyncExtensions
    {
        /// <summary>
        /// Convert IAsyncResult -> UniTask.
        /// </summary>
        public static UniTask ToUniTask(this Loxodon.Framework.Asynchronous.IAsyncResult asyncResult)
        {
            Assert.IsNotNull(asyncResult);

            if (asyncResult.IsCancelled)
            {
                return UniTask.FromCanceled();
            }
            else if (asyncResult.Exception != null)
            {
                return UniTask.FromException(asyncResult.Exception);
            }
            else if (asyncResult.IsDone)
            {
                return UniTask.CompletedTask;
            }

            var promise = new UniTaskCompletionSource();
            asyncResult.Callbackable().OnCallback(r =>
            {
                if (r.IsCancelled)
                {
                    _ = promise.TrySetCanceled();
                }
                else if (r.Exception != null)
                {
                    _ = promise.TrySetException(r.Exception);
                }
                else
                {
                    _ = promise.TrySetResult();
                }
            });
            return promise.Task;
        }

        /// <summary>
        /// Convert IAsyncResult<![CDATA[<T>]]> -> UniTask<![CDATA[<T>]]>.
        /// </summary>
        public static UniTask<T> ToUniTask<T>(this IAsyncResult<T> asyncResult)
        {
            Assert.IsNotNull(asyncResult);

            if (asyncResult.IsCancelled)
            {
                return UniTask.FromCanceled<T>();
            }
            else if (asyncResult.Exception != null)
            {
                return UniTask.FromException<T>(asyncResult.Exception);
            }
            else if (asyncResult.IsDone)
            {
                return UniTask.FromResult(asyncResult.Result);
            }

            var promise = new UniTaskCompletionSource<T>();
            asyncResult.Callbackable().OnCallback(r =>
            {
                if (r.IsCancelled)
                {
                    _ = promise.TrySetCanceled();
                }
                else if (r.Exception != null)
                {
                    _ = promise.TrySetException(r.Exception);
                }
                else
                {
                    _ = promise.TrySetResult(r.Result);
                }
            });
            return promise.Task;
        }

        /// <summary>
        /// Convert IProgressResult<![CDATA[<TProgress>]]> -> UniTask.
        /// </summary>
        public static UniTask ToUniTask<TProgress>(
            this IProgressResult<TProgress> asyncResult,
            IProgress<TProgress> onProgress = null)
        {
            Assert.IsNotNull(asyncResult);

            if (asyncResult.IsCancelled)
            {
                onProgress?.Report(asyncResult.Progress);
                return UniTask.FromCanceled();
            }
            else if (asyncResult.Exception != null)
            {
                onProgress?.Report(asyncResult.Progress);
                return UniTask.FromException(asyncResult.Exception);
            }
            else if (asyncResult.IsDone)
            {
                onProgress?.Report(asyncResult.Progress);
                return UniTask.FromResult(asyncResult.Result);
            }

            var promise = new UniTaskCompletionSource();
            asyncResult.Callbackable().OnCallback(r =>
            {
                if (r.IsCancelled)
                {
                    _ = promise.TrySetCanceled();
                }
                else if (r.Exception != null)
                {
                    _ = promise.TrySetException(r.Exception);
                }
                else
                {
                    _ = promise.TrySetResult();
                }
            });
            if (onProgress != null)
            {
                asyncResult.Callbackable()
                           .OnProgressCallback(p => onProgress.Report(p));
            }

            return promise.Task;
        }

        /// <summary>
        /// Convert IProgressResult<![CDATA[<TProgress, TResult>]]> -> UniTask<![CDATA[<TResult>]]>.
        /// </summary>
        public static UniTask<TResult> ToUniTask<TProgress, TResult>(
            this IProgressResult<TProgress, TResult> progressResult,
            IProgress<TProgress> progress = null)
        {
            Assert.IsNotNull(progressResult);

            if (progressResult.IsCancelled)
            {
                progress?.Report(progressResult.Progress);
                return UniTask.FromCanceled<TResult>();
            }
            else if (progressResult.Exception != null)
            {
                progress?.Report(progressResult.Progress);
                return UniTask.FromException<TResult>(progressResult.Exception);
            }
            else if (progressResult.IsDone)
            {
                progress?.Report(progressResult.Progress);
                return UniTask.FromResult(progressResult.Result);
            }

            var promise = new UniTaskCompletionSource<TResult>();
            progressResult.Callbackable().OnCallback(r =>
            {
                if (r.IsCancelled)
                {
                    _ = promise.TrySetCanceled();
                }
                else if (r.Exception != null)
                {
                    _ = promise.TrySetException(r.Exception);
                }
                else
                {
                    _ = promise.TrySetResult(progressResult.Result);
                }
            });
            if (progress != null)
            {
                progress.Report(progressResult.Progress);
                progressResult.Callbackable()
                              .OnProgressCallback(p => progress.Report(p));
            }

            return promise.Task;
        }
    }
}
