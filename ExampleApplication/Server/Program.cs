using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main()
        {

            MyMain();
        }

        public static void MyMain()
        {
            TcpListener listener;
            
            try
            {
                listener = new TcpListener(IPAddress.Any, 1234);
                listener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
            while (true)
            {
                var x = listener.AcceptTcpClient();
                new Thread(() => ClientHandler(x)).Start();
            }
        }

        public static void ClientHandler(TcpClient x)
        {
            try
            {
                var r = new Random();
                var stream = x.GetStream();

                int b;
                var request = "";
                var num = 0;
                while ((b = stream.ReadByte()) != '\n')
                {
                    request += (char) b;
                }
                if (request.StartsWith("measure"))
                {
                    num = int.Parse(request.Split(' ')[1]);
                    Console.WriteLine(num + " values requested");
                }
                for(var i=0;i<num;i++)
                {
                    var timestamp = UnixTimestamp;
                    var val1 = (float) r.NextDouble()*50000;
                    var val2 = (float) r.NextDouble()*50000;
                    var b1 = BitConverter.GetBytes(timestamp);
                    var b2 = BitConverter.GetBytes(val1);
                    var b3 = BitConverter.GetBytes(val2);
                    var buffer = new byte[12];
                    using (var ms = new MemoryStream(buffer))
                    {
                        ms.Write(b1, 0, 4);
                        ms.Write(b2, 0, 4);
                        ms.Write(b3, 0, 4);
                    }
                    x.GetStream().Write(buffer, 0, 12);
                    Console.WriteLine("Sent values!" + "Timestamp:" + timestamp + "\r\nval1:" + val1 + "\r\nval2:" +
                                      val2);
                    Thread.Sleep(1000);
                }
                Console.WriteLine("Ended sending values!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static int UnixTimestamp
        {
            get
            {
                return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }
    }
}
