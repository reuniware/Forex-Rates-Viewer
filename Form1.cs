using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForexRatesViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Logger logger = new Logger();
        private void Form1_Load(object sender, EventArgs e)
        {
            logger.setTxtLog(txtLog);
            log("Main: Application Start (HttpToSqlServer)");

            IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ipAddress in ipAddresses)
            {
                log("Local IP Address = " + ipAddress.ToString());
            }

            Thread serverThread = new Thread(ServerThread);
            serverThread.Start();
        }

        private HttpListener listener;
        int listeningPort = 9090;
        private void ServerThread()
        {
            listener = new HttpListener();
            string[] prefixes = new string[] { "http://*:" + listeningPort + "/send_data/" };
            foreach (string s in prefixes) { listener.Prefixes.Add(s); }

            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                log("(ServerThread) Impossible de démarrer le serveur d'écoute des connexions clients : " + ex.Message);
                Console.WriteLine("(ServerThread) Pressez ENTREE pour terminer");
                Console.ReadLine();
                return;
            }

            HttpListenerContext context;
            HttpListenerRequest request;

            log("(ServerThread) Serveur en écoute sur le port " + listeningPort + " ...");

            while (true)
            {
                try
                {
                    context = listener.GetContext();
                    request = context.Request;

                    if (request.RawUrl.Equals("/send_data/"))
                    {
                        RequestProcessor requestProcessor = new RequestProcessor();
                        requestProcessor.setContext(context);
                        requestProcessor.setRequest(request);
                        requestProcessor.setLogger(logger);
                        //log("(ServerThread) Client IP = " + request.RemoteEndPoint.Address);

                        Thread requestProcessorThread = new Thread(new ThreadStart(requestProcessor.RequestProcessorThread));
                        requestProcessorThread.Start();
                    }
                    //listener.Stop();
                }
                catch (Exception ex)
                {
                    log("Erreur générale : " + ex.Message);
                }
            } // fin du while(true)

        }

        private void log(string str)
        {
            logger.log(str);
        }

    }

    internal class Logger
    {
        private TextBox txtLog;
        public void setTxtLog(TextBox textBox)
        {
            this.txtLog = textBox;
        }
        public void log(string str)
        {
            try
            {
                txtLog.Invoke((MethodInvoker)delegate
                {
                    txtLog.AppendText(str + "\r\n");
                    txtLog.SelectionStart = txtLog.Text.Length;
                    txtLog.ScrollToCaret();
                });
            }
            catch (Exception)
            {
                try
                {
                    txtLog.Invoke((MethodInvoker)delegate
                    {
                        txtLog.Clear();
                    });
                }
                catch(Exception)
                {

                }

            }
        }
    }

    internal class RequestProcessor
    {
        private Logger logger;
        public void setLogger(Logger logger_)
        {
            this.logger = logger_;
        }
        private void log(string str)
        {
            logger.log(str);
        }
        protected HttpListenerContext context;
        public void setContext(HttpListenerContext context)
        {
            this.context = context;
        }
        protected HttpListenerRequest request;
        public void setRequest(HttpListenerRequest request)
        {
            this.request = request;
        }
        HttpListenerResponse response;
        Stream output;
        public void RequestProcessorThread()
        {
            //log("Request method = " + request.HttpMethod);
            Stream input = request.InputStream;
            StreamReader streamReader = new StreamReader(input);
            string line = "";
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
                log("line=" + line);
            }
            streamReader.Close();
            input.Close();

            // Ici répondre au client
            response = context.Response;
            string responseString = "OK";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Flush();
            output.Close();
        }

    }
}
