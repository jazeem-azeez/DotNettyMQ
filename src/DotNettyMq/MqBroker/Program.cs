using System.Net;
using DotNetty.Codecs;
using DotNetty.Codecs.Compression;
using DotNetty.Codecs.Json;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace MqBroker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bossGroup = new MultithreadEventLoopGroup();
            var workerGroup = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new ServerBootstrap()
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 1000)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        // pipeline.AddLast(new StringEncoder());
                        // pipeline.AddLast(new StringDecoder());
                        pipeline.AddLast(new DelimiterBasedFrameDecoder(16384, Delimiters.LineDelimiter()));
                        pipeline.AddLast(new JsonObjectDecoder());
                        pipeline.AddLast(new EchoServerHandler());
                    }));

                var serverChannel = bootstrap.BindAsync(new IPEndPoint(IPAddress.Any, 39500)).Result;
                Console.WriteLine("Server started on port 8888. Press Enter to exit.");
                Console.ReadLine();

                // Close the server channel
                serverChannel.CloseAsync().Wait();
            }
            finally
            {
                bossGroup.ShutdownGracefullyAsync().Wait();
                workerGroup.ShutdownGracefullyAsync().Wait();
            }
        }
    }
 
}