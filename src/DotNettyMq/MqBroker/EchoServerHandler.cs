using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace MqBroker
{
    public class EchoServerHandler : ChannelHandlerAdapter
    {
        private StringBuilder sb1 = new StringBuilder();


        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            
            // Console.WriteLine($"Received: {buffer.ToString(Encoding.UTF8)}");

            // Echo the message back to the client
            // context.WriteAndFlushAsync(message);
            sb1.Append(buffer.ToString(Encoding.UTF8));
            Console.WriteLine(sb1.Length);
            Console.WriteLine(context.Channel.Id);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            
            Console.WriteLine(sb1.ToString());
            Console.WriteLine(sb1.Length);
            Console.WriteLine(context.Channel.Id);
            sb1.Clear();
            context.Flush();

        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
            context.CloseAsync();
        }
    }
}