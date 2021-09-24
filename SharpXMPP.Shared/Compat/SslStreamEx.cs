using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace SharpXMPP.Compat
{
    public static class SslStreamEx
    {
        public static Task AuthenticateAsClientWithCancellationAsync(
            this SslStream sslStream,
            string targetHost,
            CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            return sslStream.AuthenticateAsClientAsync(
                new SslClientAuthenticationOptions { TargetHost = targetHost },
                cancellationToken);
#else
            // No cancellation on older runtimes :(
            cancellationToken.ThrowIfCancellationRequested();
            return sslStream.AuthenticateAsClientAsync(targetHost);
#endif
        }
    }
}
