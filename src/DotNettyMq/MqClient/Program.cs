using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Compression;
using DotNetty.Codecs.Json;
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
                        // pipeline.AddLast(new StringEncoder());
                        // pipeline.AddLast(new StringDecoder());
                        pipeline.AddLast(new DelimiterBasedFrameDecoder(16384, Delimiters.LineDelimiter()));
                        pipeline.AddLast(new JsonObjectDecoder());
                        pipeline.AddLast(new EchoClientHandler());
                    }));

                var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 39500);
                var channel = await bootstrap.ConnectAsync(endPoint);
                Console.WriteLine("Press Enter");
                Console.ReadLine();
                var serialize = JsonSerializer.Serialize(new DotNettyMessage
                {
                    CanRead = false,
                    CanSeek = false,
                    CanWrite = false,
                    Length = 100,
                    Position = 0,
                    Message = " \r\n ".PadLeft(10, '*').PadRight(10, '#')
                });
                // Send a message to the server
                var stopWatch = Stopwatch.StartNew();
                // var wrappedBuffer = Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(serialize));
                var tasks = new List<Task>(10000);
                for (int i = 0; i < 10000; i++)
                {
                    tasks.Add(channel.WriteAndFlushAsync(
                        Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes($"{serialize}\r\n"))));
                    // channel.Flush();
                }
                await Task.WhenAll(tasks);

                stopWatch.Stop();
                Console.WriteLine($"Message sent to server. Press Enter to exit.{stopWatch.ElapsedMilliseconds} ms");
                Console.ReadLine();
                await channel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }
    }

    internal class DotNettyMessage
    {
        public bool CanRead { get; set; }
        public bool CanSeek { get; set; }
        public bool CanWrite { get; set; }
        public long Length { get; set; }
        public long Position { get; set; }
        public string Message { get; set; }
    }
}