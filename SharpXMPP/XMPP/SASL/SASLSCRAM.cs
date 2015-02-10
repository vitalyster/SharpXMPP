using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharpXMPP.XMPP.SASL
{
    public class SASLSCRAM : SASLHandler
    {
        private bool _verifystep = false;
        private string _clientFirstMessageBare;
        private byte[] _serverSignature;
        public SASLSCRAM()
        {
            SASLMethod = "SCRAM-SHA-1";
        }

        public override string Initiate()
        {
            _clientFirstMessageBare = string.Format("n={0},r={1}", ClientJID.User, Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("n,,{0}", _clientFirstMessageBare)));
        }

        NameValueCollection parseResponse(string arr)
        {
            var result = new NameValueCollection();
            var parts = arr.Split(',');
            foreach (var part in parts)
            {
                var item = part.Split(new[] {'='}, 2);
                result.Add(item[0], item[1]);
            }
            return result;
        }

        public override string NextChallenge(string previousResponse)
        {
            var serverResponse = Encoding.UTF8.GetString(Convert.FromBase64String(previousResponse));
            var parts = parseResponse(serverResponse);
            if (_verifystep) return Convert.FromBase64String(parts["v"]).SequenceEqual(_serverSignature) ? "" : "error";
            _verifystep = true;
            var iters = int.Parse(parts["i"]);
            var saltedPassword = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(Password),
                Convert.FromBase64String(parts["s"]),
                iters).GetBytes(20);
            var hmac1 = new HMACSHA1(saltedPassword);
            var clientKey = hmac1.ComputeHash(Encoding.UTF8.GetBytes("Client Key"));
            var clientFinalMessageWithoutProof = string.Format("c={0},r={1}", "biws", parts["r"]);
            var authMessage = string.Format("{0},{1},{2}", _clientFirstMessageBare, serverResponse,
                clientFinalMessageWithoutProof);
            var hmac2 = new HMACSHA1(new SHA1Managed().ComputeHash(clientKey));
            var clientSignature = hmac2.ComputeHash(Encoding.UTF8.GetBytes(authMessage));
            var clientProof = clientKey.Zip(clientSignature, (x, y) => (byte)(x ^ y)).ToArray();
            var serverKey = new HMACSHA1(saltedPassword).ComputeHash(Encoding.UTF8.GetBytes("Server Key"));
            _serverSignature = new HMACSHA1(serverKey).ComputeHash(Encoding.UTF8.GetBytes(authMessage));
            return
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(string.Format("{0},p={1}", clientFinalMessageWithoutProof,
                        Convert.ToBase64String(clientProof))));
        }
    }
}
