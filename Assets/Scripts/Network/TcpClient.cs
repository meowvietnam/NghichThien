using NativeTcp = System.Net.Sockets.TcpClient;
namespace Network
{
    public class TcpClient : Tcp<MessageTag>
    {
        public TcpClient(NativeTcp client) : base(client)
        {
        }
    }
}