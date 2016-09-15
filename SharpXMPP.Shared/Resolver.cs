using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharpXMPP
{
    public struct SRVRecord
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public long Ttl { get; set; }
    }


    public static class Resolver
    {
        public async static Task<List<SRVRecord>> ResolveXMPPClient(string domain)
        {
            var result = new List<SRVRecord>();
            var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse("8.8.8.8"), 53);
            var stream = client.GetStream();
            var message = EncodeQuery(domain);
            var lengthPrefix = IPAddress.HostToNetworkOrder((short)message.Length);
            var lengthPrefixBytes = BitConverter.GetBytes(lengthPrefix);
            stream.Write(lengthPrefixBytes, 0, 2);
            stream.Write(message, 0, message.Length);

            var responseLengthBytes = new byte[2];
            stream.Read(responseLengthBytes, 0, 2);
            var responseMessage = new byte[IPAddress.NetworkToHostOrder(BitConverter.ToInt16(responseLengthBytes, 0))];
            stream.Read(responseMessage, 0, responseMessage.Length);
            result = Decode(responseMessage);
            return result;
        }

        private static byte[] EncodeQuery(string domain)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            writer.Write(IPAddress.HostToNetworkOrder((short)0x666));
            writer.Write((byte)0x01);
            // QR+Opcode+AA+TC+RD = 0000-0001b - RECURSION_DESIRED
            writer.Write((byte)0x00); // RA+Z+RCode = 0000-0000b
            writer.Write(IPAddress.HostToNetworkOrder((short)0x0001)); // queries count
            writer.Write((short)0x0000); // answers count
            writer.Write((short)0x0000); // authority record count
            writer.Write((short)0x0000); // additions count
            var buff = new StringBuilder();
            buff.Append("_xmpp-client._tcp.").Append(domain);
            foreach (var chr in buff.ToString().Split('.')) { writer.Write(chr); };
            writer.Write((byte)0x00);
            writer.Write(IPAddress.HostToNetworkOrder((short)0x0021));
            writer.Write(IPAddress.HostToNetworkOrder((short)0x0001));
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        private static List<SRVRecord> Decode(byte[] response)
        {

            var ms = new MemoryStream(response);
            var reader = new BinaryReader(ms);
            short id = IPAddress.NetworkToHostOrder(reader.ReadInt16()); // id
            short flags = IPAddress.NetworkToHostOrder(reader.ReadInt16()); // flags
            short questions = IPAddress.NetworkToHostOrder(reader.ReadInt16());
            int answers = IPAddress.NetworkToHostOrder(reader.ReadInt16());
            reader.ReadInt16();
            reader.ReadInt16();
            for (int i = 0; i < questions; ++i)
            {
                string question;
                do
                {
                    question = reader.ReadString();
                } while (question != string.Empty);
                reader.ReadUInt16();
                reader.ReadUInt16();
            }
            var res = new List<SRVRecord>();
            for (var i = 0; i < answers; ++i)
            {
                reader.ReadInt16(); ; // ...
                short type = IPAddress.NetworkToHostOrder((short)reader.ReadUInt16()); // type
                reader.ReadUInt16(); // class
                int ttl = IPAddress.NetworkToHostOrder(reader.ReadInt32()); // ttl
                int rdlength = reader.ReadUInt16(); // length
                if (type == 0x0021)
                {
                    // SRV
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    short port = IPAddress.NetworkToHostOrder((short)reader.ReadUInt16()); // port
                    var result = new StringBuilder();
                    string name;
                    do
                    {
                        name = reader.ReadString();
                        result.Append(name);
                        result.Append('.');
                    } while (name != string.Empty);
                    var item = new SRVRecord { Host = result.ToString().Substring(0, result.Length - 2), Port = port, Ttl = ttl };
                    res.Add(item);
                }
            }
            return res;
        }
    }
}