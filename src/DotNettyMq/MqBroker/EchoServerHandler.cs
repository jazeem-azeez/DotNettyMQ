using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace MqBroker
{
    internal class DotNettyMessage
    {
        public bool CanRead { get; set; }
        public bool CanSeek { get; set; }
        public bool CanWrite { get; set; }
        public long Length { get; set; }
        public long Position { get; set; }
        public string Message { get; set; }
    }

    public class EchoServerHandler : ChannelHandlerAdapter
    {
        private StringBuilder sb1 = new StringBuilder();
        private long counter = 0;


        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine($"Client connected to client.{context.Channel.Id} {context.Channel.RemoteAddress}");
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            // if (message is IByteBuffer byteBuffer)
            {
                var str = ((IByteBuffer)message).ToString(Encoding.UTF8);
                // Console.WriteLine($"Received from client: {str}");
                // context.Flush();
                JsonSerializer.Deserialize<DotNettyMessage>(str);
                counter++;
            }
        }


        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            Console.WriteLine(counter);
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
            context.CloseAsync();
        }
    }
}