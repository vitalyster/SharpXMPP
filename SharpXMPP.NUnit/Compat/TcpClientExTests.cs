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
        private static int GetFreePort()
        {
            var host = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));
            host.Start();
            try
            {
                return ((IPEndPoint)host.LocalEndpoint).Port;
            }
            finally
            {
                host.Stop();
            }
        }

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

            // Connecting to a free port takes enough time for the tasks to be cancelled in time.
            var port = GetFreePort();
            TcpListener host = null;
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // Since the port trick doesn't work on Linux, do this.
                host = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
                host.Start();
            }

            try
            {
                for (int i = 0; i < iterations; ++i)
                {
                    Task.Run(async () =>
                    {
                        using var cts = new CancellationTokenSource();
                        var token = cts.Token;

                        using var client = new TcpClient();
                        var task = client.ConnectWithCancellationAsync(IPAddress.Loopback, port, token);
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
                host?.Stop();
            }

            Assert.Greater(cancelled, 0);

            dotMemory.Check(memory =>
                // note there's 1 task object on pre-.NET 5 runtimes
                Assert.LessOrEqual(memory.GetObjects(o => o.Type.Is<Task>()).ObjectsCount, 1));
        }
    }
}
