﻿using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Collections.Generic;

namespace Kadmium_sACN.SacnReceiver
{
	public abstract class MulticastSacnReceiver : SacnReceiver, IMulticastSacnReceiver
	{
		private ISacnMulticastAddressProvider MulticastAddressProvider { get; }

		protected MulticastSacnReceiver(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpWrapper)
		{
			MulticastAddressProvider = multicastAddressProvider;
		}

		public void JoinMulticastGroups(IEnumerable<UInt16> universes)
		{
			foreach (var universe in universes)
			{
				JoinMulticastGroup(universe);
			}
		}

		public void JoinMulticastGroup(UInt16 universe)
		{
			if (universe < Constants.Universe_MinValue || universe > Constants.Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Constants.Universe_MinValue} and {Constants.Universe_MaxValue} inclusive");
			}

			UdpWrapper.JoinMulticastGroup(MulticastAddressProvider.GetMulticastAddress(universe));
		}

		public void JoinUniverseDiscoveryGroup()
		{
			UdpWrapper.JoinMulticastGroup(MulticastAddressProvider.GetMulticastAddress(UniverseDiscoveryPacket.DiscoveryUniverse));
		}

		public void DropUniverseDiscoveryGroup()
		{
			UdpWrapper.DropMulticastGroup(MulticastAddressProvider.GetMulticastAddress(UniverseDiscoveryPacket.DiscoveryUniverse));
		}

		public void DropMulticastGroups(IEnumerable<ushort> universes)
		{
			foreach (var universe in universes)
			{
				DropMulticastGroup(universe);
			}
		}

		public void DropMulticastGroup(ushort universe)
		{
			if (universe < Constants.Universe_MinValue || universe > Constants.Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Constants.Universe_MinValue} and {Constants.Universe_MaxValue} inclusive");
			}

			UdpWrapper.DropMulticastGroup(MulticastAddressProvider.GetMulticastAddress(universe));
		}
	}
}
