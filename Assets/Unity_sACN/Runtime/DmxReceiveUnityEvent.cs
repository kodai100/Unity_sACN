using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace com.kodai100.Sacn
{
    [Serializable]
    public class DmxReceiveUnityEvent : UnityEvent<ushort, IEnumerable<byte>>
    {
    }
    
    [Serializable]
    public class DmxUniverseDiscoverUnityEvent : UnityEvent<IEnumerable<ushort>>
    {
    }
}