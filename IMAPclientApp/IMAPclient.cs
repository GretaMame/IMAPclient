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

        public void Connect()
        {
            try
            {
                client = new TcpClient(host, port);
                ssl = new SslStream(client.GetStream());
                ssl.AuthenticateAsClient(host);
                ReceiveResponse("");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: "+ e.Message);
            }
        }

        public void LogIn()
        {
            ReceiveResponse("$ LOGIN " + email + " " + password + "  \r\n");

            ReceiveResponse("$ LIST " + "\"\"" + " \"*\"" + "\r\n");

            ReceiveResponse("$ SELECT INBOX\r\n");

            ReceiveResponse("$ STATUS INBOX (MESSAGES)\r\n");



            int number = 4196;

            ReceiveResponse("$ FETCH " + number + " body[header]\r\n");
            ReceiveResponse("$ FETCH " + number + " body[text]\r\n");


            
        }

        public void LogOut()
        {
            ReceiveResponse("$ LOGOUT\r\n");
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

        private void ReceiveResponse(string command)
        {
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
                sb.Append(Encoding.ASCII.GetString(buffer));


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
