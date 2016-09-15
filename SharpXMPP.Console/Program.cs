using SharpXMPP.XMPP;

namespace SharpXMPP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Write("JID: ");
            var jid = System.Console.ReadLine();
            System.Console.Write("Password: ");
            var password = System.Console.ReadLine();
            var client = new XmppWebSocketConnection(new JID(jid), password);
            client.Element += (sender, e) =>
            {
                var direction = e.IsInput ? "<==" : "==>";
                System.Console.WriteLine($"{direction} {e.Stanza}");
            };
            client.SignedIn += (sender, args) =>
            {
                System.Console.WriteLine("Connected!");
            };
            client.ConnectionFailed += (sender, args) =>
            {
                System.Console.WriteLine(args.Message);
            };
            var connection = client.ConnectAsync();
            System.Console.ReadKey();
        }
    }
}
