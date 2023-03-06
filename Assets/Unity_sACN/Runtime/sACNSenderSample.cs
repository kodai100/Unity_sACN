using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Kadmium_sACN;
using Kadmium_sACN.SacnSender;
using UnityEngine;

namespace com.kodai100.Sacn
{
    public class sACNSenderSample : MonoBehaviour
    {
    
        [SerializeField] private List<ushort> _universes;
        [SerializeField] private float _dmxSendRate = 44;   // Hz
        
        private MulticastSacnSenderIPV4 _sender;
        private SacnPacketFactory _factory;
    
        private Timer _timerForDiscovery;
        private Timer _timerForDmx;
        
        private void Start()
        {
            var cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            
            _factory = new SacnPacketFactory(cid, "MySource");
            _sender = new MulticastSacnSenderIPV4(); // IPv6 is also supported
            
            SendDiscoveryPacketEvery5Second();
            SendDmxPacketEverySecond();
        }
    
        private void SendDiscoveryPacketEvery5Second()
        {
            _timerForDiscovery = new Timer(5000);
            _timerForDiscovery.Elapsed += async (sender, e) =>
            {
                var packets = _factory.CreateUniverseDiscoveryPackets(_universes);
                foreach (var packet in packets)
                {
                    await _sender.Send(packet);
                }
            };
            _timerForDiscovery.Start();
        }
    
        private void SendDmxPacketEverySecond()
        {
            var offset = 0;
            _timerForDmx = new Timer(1000f/_dmxSendRate); // DMX supports 44Hz max refresh rate
            _timerForDmx.Elapsed += async (sender, e) =>
            {
                var values = new byte[512];
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = (byte) ((i + offset) % 255);
                }
    
                foreach (var packet in _universes.Select(universe => _factory.CreateDataPacket(universe, values)))
                {
                    await _sender.Send(packet);
                }

                offset++;
            };
            _timerForDmx.Start();
        }
    
        private void OnDestroy()
        {
            _sender.Dispose();
            
            _timerForDiscovery.Dispose();
            _timerForDmx.Dispose();
        }
        
    }
}


