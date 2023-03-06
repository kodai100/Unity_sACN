using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kadmium_sACN.SacnReceiver;
using UnityEngine;

namespace com.kodai100.Sacn
{
    public class sACNReceiverSample : MonoBehaviour
    {

        [SerializeField] private DmxUniverseDiscoverUnityEvent _discoverEvent;
        [SerializeField] private DmxReceiveUnityEvent _receiveEvent;
        
        private MulticastSacnReceiverIPV4 receiver;
    
        private IEnumerable<ushort> _universes;
        
        private void Start()
        {
            receiver = new MulticastSacnReceiverIPV4(); // IPv6 is also supported
            
            receiver.OnDataPacketReceived += (sender, packet) =>
            {
                // Debug.Log($"{packet.FramingLayer.Universe} : {string.Join(", ", packet.DMPLayer.PropertyValues)}");
                _receiveEvent.Invoke(packet.FramingLayer.Universe, packet.DMPLayer.PropertyValues);
            };
            
            receiver.OnSynchronizationPacketReceived += (sender, packet) =>
            {
                Debug.Log("Sync!");
            };
            
            receiver.OnUniverseDiscoveryPacketReceived += (sender, packet) =>
            {
                var universes = packet.UniverseDiscoveryLayer.Universes.ToList();
                if (!CheckSame(universes))
                {
                    Debug.Log($"Detect universe change. Join to [{string.Join(", ", packet.UniverseDiscoveryLayer.Universes.Select(p=>p.ToString()).ToArray())}]");
                    _universes = universes;
                    receiver.JoinMulticastGroups(_universes);
                    
                    _discoverEvent?.Invoke(universes);
                }
                
            };
            
            receiver.Listen(IPAddress.Any);
            receiver.JoinUniverseDiscoveryGroup();
            
        }
    
        private bool CheckSame(IEnumerable<ushort> universes)
        {
            if (_universes == null) return false;
            
            var list1 = _universes.ToList();
            var list2 = universes.ToList();
            list1.Sort();
            list2.Sort();
            return list1.SequenceEqual(list2);
        }
    
    
        private void OnDestroy()
        {
            receiver?.Dispose();
        }
    }

}

