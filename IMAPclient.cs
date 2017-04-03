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
    class IMAPclient
    {
        /// <summary>
        /// IMAP commands
        /// </summary>

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

        public IMAPclient(string host, int port, string email, string password)
        {
            this.host = host;
            this.port = port;
            this.email = email;
            this.password = password;
        }

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

        public void LogIn()
        {
            ReceiveResponse("LOGIN " + email + " " + password + "  \r\n");

            ReceiveResponse("LIST " + "\"\"" + " \"*\"" + "\r\n");

            ReceiveResponse("SELECT INBOX\r\n");

            ReceiveResponse("STATUS INBOX (MESSAGES)\r\n");



            int number = 4196;

            ReceiveResponse("FETCH " + number + " body[header]\r\n");
            ReceiveResponse("FETCH " + number + " body[text]\r\n");


            
        }

        public void LogOut()
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

        private void ReceiveResponse(string cmd)
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
