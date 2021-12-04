using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kadmium_Udp
{
	public class UdpWrapper : IUdpWrapper
	{
		private UdpClient UdpClient { get; set; }
		private CancellationTokenSource TokenSource { get; set; }
		public event EventHandler<UdpReceiveResult> OnPacketReceived;
		public IPEndPoint HostEndPoint => UdpClient.Client.LocalEndPoint as IPEndPoint;

		private void SetupEvents()
		{
			TokenSource = new CancellationTokenSource();
			var token = TokenSource.Token;
			Task.Run(async () =>
			{
				while (!token.IsCancellationRequested)
				{
					var result = await UdpClient.ReceiveAsync();
					OnPacketReceived?.Invoke(this, result);
				}
			});
		}

		public void Listen(IPEndPoint endPoint)
		{
			if (UdpClient == null)
			{
				UdpClient = new UdpClient(endPoint);
			}
			SetupEvents();
		}

		public async Task Send(IPEndPoint remoteEndPoint, ReadOnlyMemory<byte> bytes)
		{
			if (UdpClient == null)
			{
				UdpClient = new UdpClient(remoteEndPoint.AddressFamily);
			}
			await UdpClient.SendAsync(bytes.ToArray(), bytes.Span.Length, remoteEndPoint);
		}

		public async Task Send(IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, ReadOnlyMemory<byte> bytes)
		{
			if (UdpClient == null)
			{
				UdpClient = new UdpClient(localEndPoint);
			}
			await UdpClient.SendAsync(bytes.ToArray(), bytes.Span.Length, remoteEndPoint);
		}

		public void Dispose()
		{
			TokenSource?.Cancel();
			UdpClient?.Dispose();
		}

		public void JoinMulticastGroup(IPAddress address)
		{
			if (UdpClient == null)
			{
				throw new InvalidOperationException("Listen must be called before joining a multicast group");
			}
			UdpClient.JoinMulticastGroup(address);
		}

		public void DropMulticastGroup(IPAddress address)
		{
			if (UdpClient == null)
			{
				throw new InvalidOperationException("Listen must be called before leaving a multicast group");
			}
			UdpClient.DropMulticastGroup(address);
		}
	}
}
