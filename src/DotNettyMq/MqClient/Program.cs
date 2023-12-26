using System.Diagnostics;
using System.Net;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace MqClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap()
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddLast(new EchoClientHandler());
                    }));

                var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
                var channel = await bootstrap.ConnectAsync(endPoint);

                // Send a message to the server
                var stopWatch = Stopwatch.StartNew();
                for (int i = 0; i < 1000000; i++)
                {
                    await channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes("Hello, DotNetty".PadRight(4096,'*'))));
                }
                stopWatch.Stop();
                Console.WriteLine($"Message sent to server. Press Enter to exit.{stopWatch.ElapsedMilliseconds} ms");
                Console.ReadLine();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }
    }
}