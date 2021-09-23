using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SharpXMPP.Compat
{
    internal static class TcpClientEx
    {
#if !NET5_0_OR_GREATER
        private static async Task AbandonOnCancel(this Task task, CancellationToken cancellationToken)
        {
            // See https://devblogs.microsoft.com/pfxteam/how-do-i-cancel-non-cancelable-async-operations/ for details.
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => tcs.TrySetResult(true)))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            await task;
        }
#endif

        // Unfortunately, only .NET 5+ supports TcpClient connection cancellation. We'll do the best effort here,
        // though.
        //
        // TcpClient uses Socket::BeginConnect under the covers for all the connection methods on .NET Framework, which
        // is documented to be cancelled on Close(). So, ideally, if the caller eventually disposes the client, then all
        // the resources will be freed upon its destruction. Which means we are free to just abandon the task in
        // question.

        public static Task ConnectWithCancellationAsync(
            this TcpClient tcpClient,
            IPAddress address,
            int port,
            CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            return tcpClient.ConnectAsync(address, port, cancellationToken).AsTask();
#else
            return tcpClient.ConnectAsync(address, port).AbandonOnCancel(cancellationToken);
#endif
        }

        public static Task ConnectWithCancellationAsync(
            this TcpClient tcpClient,
            IPAddress[] addresses,
            int port,
            CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            return tcpClient.ConnectAsync(addresses, port, cancellationToken).AsTask();
#else
            return tcpClient.ConnectAsync(addresses, port).AbandonOnCancel(cancellationToken);
#endif
        }
    }
}
