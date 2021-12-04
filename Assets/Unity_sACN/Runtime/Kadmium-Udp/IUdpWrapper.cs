using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kadmium_Udp
{
	public interface IUdpWrapper : IDisposable
	{
		event EventHandler<UdpReceiveResult> OnPacketReceived;
		void Listen(IPEndPoint endPoint);
		Task Send(IPEndPoint remoteEndPoint, ReadOnlyMemory<byte> packet);
		Task Send(IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, ReadOnlyMemory<byte> packet);
		void JoinMulticastGroup(IPAddress address);
		void DropMulticastGroup(IPAddress address);
		IPEndPoint HostEndPoint { get; }
	}
}
