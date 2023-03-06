using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace com.kodai100.Sacn
{
    public class DmxViewer : MonoBehaviour
    {

        [SerializeField] private WaveformVisualizer _waveformVisualizer;
        [SerializeField] private Material _visualizerMaterial;
        
        private SynchronizationContext _synchronizationContext;

        private List<WaveformVisualizer> _visualizers = new();

        private void Awake()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        public void OnDiscoverUniverse(IEnumerable<ushort> universes)
        {
            _synchronizationContext.Post(_ =>
            {
                
                DestroyAllVisualizers();

                foreach (var universe in universes)
                {
                    var visualizer = Instantiate(_waveformVisualizer, transform);
                    visualizer.Initialize(_visualizerMaterial);
                    _visualizers.Add(visualizer);
                }
                
                
            }, null);
        }
        
        public void OnReceiveDmx(ushort universe, IEnumerable<byte> data)
        {
            _synchronizationContext.Post(_ =>
            {
                
            }, null);
        }

        private void DestroyAllVisualizers()
        {
            _visualizers.ForEach(v =>
            {
                Destroy(v.gameObject);
            });
            _visualizers.Clear();
        }
        
    }

}

