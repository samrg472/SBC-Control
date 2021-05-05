using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SBC_Control
{

    // ReSharper disable once UnusedType.Global
    public class Api
    {
        private const byte CmdActivateHeadphones = 0x01;
        private const byte CmdActivateSrs = 0x02;
        private const byte CmdGetActiveDevice = 0x03;

        private const byte ResHeadphonesActive = 0x01;
        private const byte ResSrsActive = 0x02;

        private CmdBottomBarView _bottomBar;

        // ReSharper disable once UnusedMember.Global
        public static void Init(object bottomBar)
        {
            var api = new Api
            {
                _bottomBar = new CmdBottomBarView(bottomBar)
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
                        clientSocket.NoDelay = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Socket accept error: {0}", e);
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
                            {
                                _bottomBar.IsHeadphone = true;

                                var floatBytes = BitConverter.GetBytes(_bottomBar.MasterVolume);
                                var muteBytes = BitConverter.GetBytes(_bottomBar.MasterMute);
                                clientSocket.Send(floatBytes);
                                clientSocket.Send(muteBytes);

                                break;
                            }
                            case CmdActivateSrs:
                            {
                                _bottomBar.IsHeadphone = false;

                                var floatBytes = BitConverter.GetBytes(_bottomBar.MasterVolume);
                                var muteBytes = BitConverter.GetBytes(_bottomBar.MasterMute);
                                clientSocket.Send(floatBytes);
                                clientSocket.Send(muteBytes);

                                break;
                            }
                            case CmdGetActiveDevice:
                            {
                                var hpActive = _bottomBar.IsHeadphone;
                                var res = hpActive ? ResHeadphonesActive : ResSrsActive;
                                clientSocket.Send(new[] {res});

                                break;
                            }
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
