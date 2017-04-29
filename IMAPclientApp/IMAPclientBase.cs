using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Diagnostics;

namespace IMAPclientApp
{
    class IMAPclientBase
    {
        protected static string EOL = " \r\n";
        protected static string DEFAULT_MAILBOX = "INBOX";
        // IMAP commands
        protected static string IMAP_LOGIN_CMD = "Login";
        protected static string IMAP_LOGOUT_CMD = "Logout";
        // IMAP autheticated+selected state commands
        protected static string IMAP_SELECT = "Select";
        protected static string IMAP_DELETE = "Delete";
        protected static string IMAP_RENAME = "Rename";
        protected static string IMAP_CREATE = "Create";
        protected static string IMAP_LIST = "List";
        protected static string IMAP_STATUS = "Satus";
        protected static string IMAP_APPEND = "Append";
        // IMAP selected state commands
        protected static string IMAP_CHECK = "Check";
        protected static string IMAP_CLOSE = "Close";
        protected static string IMAP_EXPUNGE = "Expunge";
        protected static string IMAP_SEARCH = "Search";
        protected static string IMAP_FETCH = "Fetch";
        protected static string IMAP_STORE = "Store";
        protected static string IMAP_COPY = "Copy";
        // variables declared for unique command identifier
        private int imapCommandValue = 0;
        private static string imapCommandPrefix = "IMAP";
        private string stringFormat = "000";
        protected string imapCommandIdentifier
        {
            get
            {
                return imapCommandPrefix + imapCommandValue.ToString(stringFormat) + " ";
            }
        }



        Stream netwStream;
        StreamReader readStream;
        private string host;
        private int port;
        private TcpClient client = null;
        private SslStream ssl = null;
        byte[] buffer;
        byte[] dummy;
        int bytes = -1;
        StringBuilder sb = new StringBuilder();

        public IMAPclientBase(string host, int port)
        {
            this.host = host;
            this.port = port;
        }


        /// <summary>
        /// Connects to server and displays it's response to console
        /// </summary>
        /// <returns>true if connection successful, false if not</returns>
        protected bool Connect()
        {
            try
            {
                string result;
                imapCommandValue = 0;
                client = new TcpClient(host, port);
                ssl = new SslStream(client.GetStream());
                ssl.AuthenticateAsClient(host);
                netwStream = ssl;
                readStream = new StreamReader(netwStream);
                result = readStream.ReadLine();
                ServerMessage(result);
                if (result.StartsWith("* OK")) return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: "+ e.Message);
                ssl.Close();
                return false;
            }
        }

        public void Disconnect()
        {
            imapCommandValue = 0;
            if (ssl != null)
            {
                ssl.Close();
                ssl.Dispose();
            }

            if (client != null)
            {
                client.Close();
            }

            if (netwStream != null)
            {
                ssl.Dispose();
            }

            if (readStream != null)
            {
                readStream.Dispose();
            }
        }

        protected void SendReceive(string cmd)
        {
            imapCommandValue++;
            string command = imapCommandIdentifier + cmd;
            //DEBUG
            ClientMessage(command);
            byte[] cmdBuffer = Encoding.ASCII.GetBytes(command.ToCharArray());
            try
            {
                netwStream.Write(cmdBuffer, 0, cmdBuffer.Length);
                bool endMessage = false;
                while (!endMessage)
                {
                    string result = readStream.ReadLine();
                    if(result.StartsWith(imapCommandIdentifier+"OK") || result.StartsWith(imapCommandIdentifier+"BAD") || result.StartsWith(imapCommandIdentifier + "NO"))
                    {
                        ServerMessage(result);
                        endMessage = true;
                    }
                    //else if (result.StartsWith("BAD") || result.StartsWith("NO"))
                    //{
                    //    ServerMessage(result);
                    //    endMessage = true;
                    //}
                    else
                    {
                        ServerMessage(result);
                        endMessage = true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        protected void ReceiveResponse(string cmd)
        {
            imapCommandValue++;
            string command = imapCommandIdentifier + cmd;
            ClientMessage(command);
            try
            {
                if (client.Connected)
                {
                    dummy = Encoding.ASCII.GetBytes(command);
                    ssl.Write(dummy, 0, dummy.Length);
                }
                else
                {
                    throw new ApplicationException("TCP CONNECTION DISCONNECTED");
                }
                ssl.Flush();


                buffer = new byte[2048];
                bytes = ssl.Read(buffer, 0, 2048);
                sb.Append(Encoding.UTF7.GetString(buffer));

                ServerMessage(sb.ToString());
               // Debug.WriteLine(sb.ToString());
                sb = new StringBuilder();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        private void ServerMessage(string response)
        {
            Debug.WriteLine("S: " + response);
        }

        private void ClientMessage(string command)
        {
            Debug.WriteLine("C: " + command);
        }
    }
}
