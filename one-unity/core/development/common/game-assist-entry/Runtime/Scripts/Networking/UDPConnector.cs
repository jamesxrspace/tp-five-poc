using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Assist.Entry
{
    public class UDPConnector : IDisposable
    {
        /// <summary>
        /// This size is less than iPhone default setting (9xxx)
        /// </summary>
        public const int MaxBufferSize = 8192;
        public static int port = 53715;
        public bool verbose;
        public OnProcessDataIn onDataIn = null;
        private CancellationTokenSource serviceCancellationTokenSource;
        private CancellationTokenSource listenerCancellationTokenSource;
        private bool isDisposing = false;
        private UdpClient client = null;
        private object locker = new object();
        private byte[] dataToSend = null;
        private bool waitForResponse;

        /// <summary>
        /// Callback delegate process connection data.
        /// </summary>
        /// <param name="bytes">bytes data</param>
        /// <returns>need to respond</returns>
        public delegate bool OnProcessDataIn(byte[] bytes);

        public void RunListener(string tag = "Listener")
        {
            listenerCancellationTokenSource?.Cancel();
            listenerCancellationTokenSource = new CancellationTokenSource();
            var token = listenerCancellationTokenSource.Token;
            _ = Task.Factory.StartNew(
                () =>
                {
                    client?.Close();
                    try
                    {
                        client = new UdpClient(port);

                        var endPoint = new IPEndPoint(IPAddress.Any, port);

                        if (verbose)
                        {
                            Debug.Log($"UDP Connection ({tag}): Initialized ({endPoint})");
                        }

                        while (true)
                        {
                            try
                            {
                                var data = client.Receive(ref endPoint);
                                if (verbose)
                                {
                                    Debug.Log($"UDP Connection ({tag}): Received ({data.Length})");
                                }

                                var wait = OnDataIn(data);
                                if (wait)
                                {
                                    data = null;
                                    while (data == null)
                                    {
                                        Thread.Sleep(100);
                                        lock (locker)
                                        {
                                            if (dataToSend != null && dataToSend.Length > 0)
                                            {
                                                data = dataToSend;
                                                dataToSend = null;
                                            }
                                        }
                                    }

                                    client.Send(data, data.Length, endPoint);
                                    if (verbose)
                                    {
                                        Debug.Log($"UDP Connection ({tag}): Sent ({data.Length})");
                                    }
                                }
                            }
                            catch (ThreadAbortException)
                            {
                                break;
                            }
                            catch (System.Exception e)
                            {
                                if (!isDisposing || verbose)
                                {
                                    Debug.LogError($"UDP Connection ({tag}) error({e.GetType()}):\n{e}");
                                }
                            }

                            token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        if (verbose)
                        {
                            Debug.Log($"UDP Connection ({tag}): Cancelled");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (verbose)
                        {
                            Debug.Log($"UDP Connection ({tag}): Cancelled");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                    finally
                    {
                        client?.Close();
                        client = null;
                    }
                }, token);
        }

        public void RunService(string ip, int port, string tag = "Sender")
        {
            serviceCancellationTokenSource?.Cancel();
            serviceCancellationTokenSource = new CancellationTokenSource();
            var token = serviceCancellationTokenSource.Token;
            _ = Task.Factory.StartNew(
                () =>
                {
                    client?.Close();
                    try
                    {
                        client = new UdpClient();
                        var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                        if (verbose)
                        {
                            Debug.Log($"UDP Connection ({tag}): Initialized ({endPoint})");
                        }

                        while (true)
                        {
                            try
                            {
                                byte[] data = null;
                                bool wait = false;
                                lock (locker)
                                {
                                    if (dataToSend != null && dataToSend.Length > 0)
                                    {
                                        data = dataToSend;
                                        wait = waitForResponse;
                                        dataToSend = null;
                                    }
                                }

                                if (data != null)
                                {
                                    client.Send(data, data.Length, endPoint);
                                    if (verbose)
                                    {
                                        Debug.Log($"UDP Connection ({tag}): Sent ({data.Length})");
                                    }

                                    if (wait)
                                    {
                                        data = client.Receive(ref endPoint);
                                        if (verbose)
                                        {
                                            Debug.Log($"UDP Connection ({tag}): Received ({data.Length})");
                                        }

                                        OnDataIn(data);
                                    }
                                }
                            }
                            catch (ThreadAbortException)
                            {
                                break;
                            }
                            catch (System.Exception e)
                            {
                                if (!isDisposing || verbose)
                                {
                                    Debug.LogError($"UDP Connection ({tag}) error:\n{e}");
                                }
                            }

                            token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        if (verbose)
                        {
                            Debug.Log($"UDP Connection ({tag}): Cancelled");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (verbose)
                        {
                            Debug.Log($"UDP Connection ({tag}): Cancelled");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"UDP Connection ({tag}) error:\n{e}");
                    }
                    finally
                    {
                        serviceCancellationTokenSource = null;
                        client?.Close();
                        client = null;
                    }
                }, token);
        }

        public void Send(byte[] dataToSend, bool waitForResponse)
        {
            if (dataToSend != null)
            {
                lock (locker)
                {
                    this.dataToSend = dataToSend;
                    this.waitForResponse = waitForResponse;
                }
            }
        }

        public void Dispose()
        {
            isDisposing = true;
            client?.Dispose();
            client = null;
            serviceCancellationTokenSource?.Cancel();
            serviceCancellationTokenSource = null;
            listenerCancellationTokenSource?.Cancel();
            listenerCancellationTokenSource = null;
        }

        private bool OnDataIn(byte[] bytes)
        {
            var res = onDataIn?.Invoke(bytes);
            return res.HasValue ? res.Value : false;
        }
    }
}
