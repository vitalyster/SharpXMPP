using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SharpXMPP.Tests
{
    public class AsynchronousClient
    {
        // The port number for the remote device.
        private readonly int _port = 5222;

        // A timeout for all socket communication
        private const int TIMEOUT_MILLISECONDS = 3000;

        // Notify the callee or user of this class through a custom event
        internal event ResponseReceivedEventHandler ResponseReceived = delegate {};
        internal event ConnectedHandler Connected = delegate { }; 

        private string _serverName = string.Empty;

        public AsynchronousClient(string serverName, int portNumber)
        {
            if (String.IsNullOrWhiteSpace(serverName))
            {
                throw new ArgumentNullException("serverName");
            }

            if (portNumber < 0 || portNumber > 65535)
            {
                throw new ArgumentNullException("portNumber");
            }

            _serverName = serverName;
            _port = portNumber;
        }

        private Socket sock;
        private SocketAsyncEventArgs socketEventArg;
        public void Connect()
        {
            socketEventArg = new SocketAsyncEventArgs();

            var hostEntry = new DnsEndPoint(_serverName, _port);

            // Create a socket and connect to the server 

            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketEventArg.Completed += SocketEventArg_Completed;
            
            socketEventArg.RemoteEndPoint = hostEntry;

            socketEventArg.UserToken = sock;

            try
            {
                sock.ConnectAsync(socketEventArg);
            }
            catch (SocketException ex)
            {
                throw new SocketException((int)ex.ErrorCode);
            }
        }

        /// <summary>
        /// Send data to the server
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <remarks> This is an asynchronous call, with the result being passed to the callee
        /// through the ResponseReceived event</remarks>
        public void SendData(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data");
            }
            socketEventArg.SetBuffer(Encoding.UTF8.GetBytes(data), 0, data.Length);
            sock.SendAsync(socketEventArg);

        }
        #region

        // A single callback is used for all socket operations. 
        // This method forwards execution on to the correct handler 
        // based on the type of completed operation 
        void SocketEventArg_Completed(object sender, SocketAsyncEventArgs e) 
        { 
            switch (e.LastOperation) 
            { 
                case SocketAsyncOperation.Connect: 
                    ProcessConnect(e); 
                    break; 
                case SocketAsyncOperation.Receive: 
                    ProcessReceive(e); 
                    break; 
                case SocketAsyncOperation.Send: 
                    ProcessSend(e); 
                    break; 
                default: 
                    throw new Exception("Invalid operation completed"); 
            } 
        }

        // Called when a ReceiveAsync operation completes  
        private void ProcessReceive(SocketAsyncEventArgs e) 
        { 
            if (e.SocketError == SocketError.Success) 
            {
                // Received data from server 
                string dataFromServer = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                
               /* var sock = e.UserToken as Socket; 
                sock.Shutdown(SocketShutdown.Send); 
                sock.Close(); 
                ClientDone.Set();*/

                // Respond to the client in the UI thread to tell him that data was received
                //System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                    var args = new ResponseReceivedEventArgs {Response = dataFromServer};
                    ResponseReceived(this ,args);
                //});

            } 
            else 
            {
                throw new SocketException((int)e.SocketError); 
            } 
        }

        
        // Called when a SendAsync operation completes 
        private void ProcessSend(SocketAsyncEventArgs e) 
        { 
            if (e.SocketError == SocketError.Success) 
            { 
                //Read data sent from the server 
                var sock = e.UserToken as Socket; 

                sock.ReceiveAsync(e); 
            } 
            else 
            {
                var args = new ResponseReceivedEventArgs
                                                     {Response = e.SocketError.ToString(), IsError = true};
                ResponseReceived(this, args);
            } 
        }

        // Called when a ConnectAsync operation completes 
        private void ProcessConnect(SocketAsyncEventArgs e) 
        { 
            if (e.SocketError == SocketError.Success) 
            { 
                // Successfully connected to the server 
                Connected(this);
            } 
            else 
            {
                var args = new ResponseReceivedEventArgs
                                                     {Response = e.SocketError.ToString(), IsError = true};
                ResponseReceived(this, args);

            } 
        } 
        #endregion
    }

    // A delegate type for hooking up change notifications.
    public delegate void ResponseReceivedEventHandler(object sender, ResponseReceivedEventArgs e);

    public delegate void ConnectedHandler(object sender);

    public class ResponseReceivedEventArgs : EventArgs
    {
        // True if an error occured, False otherwise
        public bool IsError { get; set; }

        // If there was an erro, this will contain the error message, data otherwise
        public string Response { get; set; }
    }

}
