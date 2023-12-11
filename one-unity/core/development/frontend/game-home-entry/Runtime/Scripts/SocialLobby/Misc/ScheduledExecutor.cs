using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TPFive.Home.Entry.SocialLobby
{
    public class ScheduledExecutor : IDisposable
    {
        private readonly ILogger logger;
        private readonly Queue<Func<CancellationToken, UniTask>> tasks = new Queue<Func<CancellationToken, UniTask>>();

        private bool disposed;
        private CancellationTokenSource cancellationTokenSource;

        public ScheduledExecutor(ILogger logger)
        {
            this.logger = logger;

            Start().Forget();
        }

        ~ScheduledExecutor()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void AddTask(Func<CancellationToken, UniTask> task)
        {
            tasks.Enqueue(task);
        }

        private async UniTaskVoid Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            while (true)
            {
                await TryRunTask(cancellationToken);

                await UniTask.Yield(cancellationToken);
            }
        }

        private void Stop()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        private async UniTask TryRunTask(CancellationToken cancellationToken)
        {
            if (tasks.Count > 0)
            {
                try
                {
                    var task = tasks.Dequeue();

                    await task.Invoke(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    logger.LogWarning($"{nameof(TryRunTask)} failed. {e}");
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Stop();

                tasks.Clear();
            }

            disposed = true;
        }
    }
}