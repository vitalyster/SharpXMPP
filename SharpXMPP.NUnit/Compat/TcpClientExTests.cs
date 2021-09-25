using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace SharpXMPP.Compat
{
    public class TcpClientExTests
    {
        /// <summary>
        /// This test checks for TcpClient leaks which are guaranteed to happen if the requests aren't properly
        /// cancelled.
        /// </summary>
        /// <remarks>
        /// Run with dotMemoryUnit to check for leaks, run without it for basic sanity check. For details see
        /// https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html#3-run-the-test
        /// </remarks>
        [Test, DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void TestCancellation()
        {
            const int iterations = 100;
            int cancelled = 0;

            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                for (int i = 0; i < iterations; ++i)
                {
                    Task.Run(async () =>
                    {
                        using var cts = new CancellationTokenSource();
                        var token = cts.Token;

                        using var client = new TcpClient();
                        var task = client.ConnectWithCancellationAsync(new[] { IPAddress.Loopback }, port, token);
                        cts.Cancel();

                        try
                        {
                            await task;
                        }
                        catch (OperationCanceledException)
                        {
                            ++cancelled;
                        }
                    }).Wait();
                }
            }
            finally
            {
                listener.Stop();
            }

            if (cancelled == 0)
                Assert.Inconclusive("No cancellations detected, all connections succeeded");

            dotMemory.Check(memory =>
                // note there's 1 task object on pre-.NET 5 runtimes
                Assert.LessOrEqual(memory.GetObjects(o => o.Type.Is<Task>()).ObjectsCount, 1));
        }
    }
}
