using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace MqClient
{
    public class EchoClientHandler : ChannelHandlerAdapter
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine("Client connected to server.");
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            // Console.WriteLine($"Received from server: {buffer.ToString(Encoding.UTF8)}");
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
            context.CloseAsync();
        }
    }
}