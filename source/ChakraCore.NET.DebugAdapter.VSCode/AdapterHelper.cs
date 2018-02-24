using ChakraCore.NET.Debug;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChakraCore.NET.DebugAdapter.VSCode
{
    public static class AdapterHelper
    {
        public static async void RunServer(this VSCodeDebugAdapter session, int port, CancellationToken token)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Loopback, port);
            serverSocket.Start();
            Console.WriteLine($"StartListening at {serverSocket.LocalEndpoint}");
            token.Register(() =>
            {
                Console.WriteLine("Shuting down Server");
                serverSocket.Stop();
            });
            while (!token.IsCancellationRequested)
            {
                var clientSocket = await serverSocket.AcceptSocketAsync();
                if (clientSocket != null)
                {
                    Console.WriteLine(">> accepted connection from client");
                    using (var networkStream = new NetworkStream(clientSocket))
                    {
                        try
                        {
                            await session.Start(networkStream, networkStream);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("Exception: " + e);
                        }
                    }
                    Console.Error.WriteLine(">> client connection closed");
                }
            }

        }
        public static Variable ToVSCodeVarible(this Debug.Variable value, string namePattern = "{0}")
        {
            return new Variable(string.Format(namePattern, value.Name), value.Display ?? value.Value, value.Type, value.PropertyAttributes.HasFlag(PropertyAttributesEnum.HAVE_CHILDRENS) ? (int)value.Handle : 0);
        }
    }
}
