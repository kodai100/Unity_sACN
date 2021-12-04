using System;
using System.Net;
using Kadmium_sACN.SacnReceiver;
using UnityEngine;

public class sACNReceiverSample : MonoBehaviour
{
    private MulticastSacnReceiverIPV4 receiver;
    
    // Start is called before the first frame update
    private void Start()
    {
        receiver = new MulticastSacnReceiverIPV4(); // IPv6 is also supported
        receiver.OnDataPacketReceived += (sender, packet) =>
        {
            Debug.Log(packet.FramingLayer.Universe + ": ");
            Debug.Log(string.Join(", ", packet.DMPLayer.PropertyValues));
        };
        receiver.OnSynchronizationPacketReceived += (sender, packet) =>
        {
            Debug.Log("Sync!");
        };
        receiver.OnUniverseDiscoveryPacketReceived += (sender, packet) =>
        {
            Debug.Log("Discovery!");
        };
        
        receiver.Listen(IPAddress.Any);
        receiver.JoinMulticastGroup(2);
    }


    private void OnDestroy()
    {
        receiver?.Dispose();
    }
}
