using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.kodai100.Sacn
{
    public class DmxViewer : MonoBehaviour
    {

        [SerializeField] private WaveformVisualizer _waveformVisualizer;
        [SerializeField] private Material _visualizerMaterial;
        
        private List<WaveformVisualizer> _visualizers = new();

        public void OnDiscoverUniverse(IEnumerable<ushort> universes)
        {
            DestroyAllVisualizers();

            foreach (var universe in universes)
            {
                var visualizer = Instantiate(_waveformVisualizer, transform);
                visualizer.Initialize(_visualizerMaterial, universe,18f, 1f, 1.2f * universe);
                _visualizers.Add(visualizer);
            }
        }
        
        public void OnReceiveDmx(ushort universe, IEnumerable<byte> data)
        {
            _visualizers.ForEach(v =>
            {
                if (v.Id == universe)
                {
                    v.Refresh(data.ToArray());
                }
            });
        }

        private void DestroyAllVisualizers()
        {
            _visualizers.ForEach(v =>
            {
                Destroy(v.gameObject);
            });
            _visualizers.Clear();
            
            Debug.Log("Destroyed");
        }
        
    }

}

