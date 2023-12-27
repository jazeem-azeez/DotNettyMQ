using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace MqClient
{
    public class EchoClientHandler : ChannelHandlerAdapter
    {
        public override void ChannelActive(IChannelHandlerContext context)
        { 
            Console.WriteLine($"Client connected to server.{context.Channel.Id} {context.Channel.RemoteAddress}");
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            if (byteBuffer != null)
            {
                Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
                // context.WriteAndFlushAsync(message);
            }
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