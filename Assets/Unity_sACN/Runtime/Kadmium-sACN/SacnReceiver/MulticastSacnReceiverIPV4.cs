﻿using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kadmium_sACN.SacnReceiver
{
	public class MulticastSacnReceiverIPV4 : MulticastSacnReceiver
	{
		internal MulticastSacnReceiverIPV4(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpWrapper, multicastAddressProvider)
		{
		}

		public MulticastSacnReceiverIPV4() : base(new UdpWrapper(), new SacnMulticastAddressProviderIPV4())
		{
		}

		public override void Listen(IPAddress ipAddress)
		{
			if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new ArgumentException("The given IP Address was not an IPv4 Address");
			}
			ListenInternal(ipAddress);
		}
	}
}
