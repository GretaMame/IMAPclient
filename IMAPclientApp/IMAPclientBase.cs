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
        private static string EOL = "\r\n";
        /// <summary>
        /// IMAP commands
        /// </summary>
        private static string IMAP_LOGIN_CMD = "Login";
        private static string IMAP_LOGOUT_CMD = "Logout";
        
        private static string IMAP_SELECT = "Select INBOX";

        /// <summary>
        /// variables declared for unique command identifier
        /// </summary>
        private int imapCommandValue = 0;
        private static string imapCommandPrefix = "IMAP";
        private string stringFormat = "000";

        public string imapCommandIdentifier
        {
            get
            {
                return imapCommandPrefix + imapCommandValue.ToString(stringFormat);
            }
        }

        Stream netwStream;
        StreamReader readStream;

        private string host;
        private int port;
        private TcpClient client= null;
        private SslStream ssl = null;
        byte[] buffer;
        byte[] dummy;
        int bytes = -1;
        StringBuilder sb = new StringBuilder();
        private string email, password;

        public IMAPclientBase(string host, int port)
        {
            this.host = host;
            this.port = port;
        }


        /// <summary>
        /// Connects to server and displays it's response to console
        /// </summary>
        /// <returns>true if connection successful, false if not</returns>
        public bool Connect()
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
                Debug.WriteLine(result);
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
            ReceiveResponse("LOGOUT\r\n");
            if (ssl != null)
            {
                ssl.Close();
                ssl.Dispose();
            }

            if (client != null)
            {
                client.Close();
            }

            
        }

        //private bool 

        protected void ReceiveResponse(string cmd)
        {
            imapCommandValue++;
            string command = imapCommandIdentifier + " " + cmd;
            Debug.WriteLine(command);
            try
            {

                if (command != "")
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
                }
                ssl.Flush();


                buffer = new byte[2048];
                bytes = ssl.Read(buffer, 0, 2048);
                sb.Append(Encoding.UTF7.GetString(buffer));


                Debug.WriteLine(sb.ToString());
                sb = new StringBuilder();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    
    }
}
