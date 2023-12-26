using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace MqBroker
{
    public class EchoServerHandler : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            // Console.WriteLine($"Received: {buffer.ToString(Encoding.UTF8)}");

            // Echo the message back to the client
            context.WriteAndFlushAsync(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
            context.CloseAsync();
        }
    }
}