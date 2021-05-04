using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;

namespace SBC_Control
{
    
    public class Api
    {
        private const byte CmdActivateHeadphones = 0x01;
        private const byte CmdActivateSrs = 0x02;
        private const byte CmdGetActiveDevice = 0x03;

        private const byte ResHeadphonesActive = 0x01;
        private const byte ResSrsActive = 0x02;
        
        private CmdBottomBarView _bottomBar;
        
        public static void Init(Dispatcher dispatcher, object bottomBar)
        {
            var api = new Api
            {
                _bottomBar = new CmdBottomBarView(dispatcher, bottomBar)
            };
            api.StartServer();
        }

        private void StartServer()
        {
            Console.WriteLine("Starting SBC-Control server");
            var thread = new Thread(() =>
            {
                var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9051)
                {
                    ExclusiveAddressUse = false
                };
                server.Start();

                while (true)
                {
                    Socket clientSocket = null;
                    try
                    {
                        clientSocket = server.AcceptSocket();
                        clientSocket.Blocking = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Socket accept error", e);
                        Thread.Sleep(100);
                    }

                    if (clientSocket != null)
                    {
                        HandleClient(clientSocket);
                    }
                }
                // ReSharper disable once FunctionNeverReturns
            }) {IsBackground = true};
            thread.Start();
        }

        private void HandleClient(Socket clientSocket)
        {
            var clientWorker = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        var bytes = new byte[1];
                        var len = clientSocket.Receive(bytes);
                        if (len != 1)
                        {
                            break;
                        }

                        switch (bytes[0])
                        {
                            case CmdActivateHeadphones:
                                _bottomBar.IsHeadphone = true;
                                break;
                            case CmdActivateSrs:
                                _bottomBar.IsHeadphone = false;
                                break;
                            case CmdGetActiveDevice:
                                var hpActive = _bottomBar.IsHeadphone;
                                var res = hpActive ? ResHeadphonesActive : ResSrsActive;
                                clientSocket.Send(new[] {res});
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    clientSocket.Close();
                }
                // ReSharper disable once FunctionNeverReturns
            }) {IsBackground = true};
            clientWorker.Start();
        }
    }
}
