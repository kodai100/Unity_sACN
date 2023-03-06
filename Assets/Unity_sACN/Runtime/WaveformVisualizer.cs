using UnityEngine;
using System.Linq;
using Unity.Collections;
using UnityEngine.Rendering;

namespace com.kodai100.Sacn
{

    public sealed class WaveformVisualizer : MonoBehaviour
    {

        [SerializeField] private float _width = 1f;
        [SerializeField] private float _height = 1f;
        [SerializeField] private float _yOffset = 0f;

        private const int Resolution = 512;
        private Material _material = null;

        private Mesh _mesh;

        private bool _initialized;

        public void Initialize(Material material, float width = 1f, float height = 1f, float yOffset = 0f)
        {
            _material = material;
            _width = width;
            _height = height;
            _yOffset = yOffset;

            _initialized = true;
        }

        public void Refresh(byte[] data)
        {
            if (!_initialized) return;

            UpdateMesh(data);

            Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, _material, gameObject.layer);
        }

        private void UpdateMesh(byte[] data)
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10);

                // Initial vertices
                using (var vertices = CreateVertexArray(data))
                {
                    var pos = new VertexAttributeDescriptor(
                        VertexAttribute.Position,
                        VertexAttributeFormat.Float32, 3);

                    _mesh.SetVertexBufferParams(vertices.Length, pos);
                    _mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
                }

                // Initial indices
                using (var indices = CreateIndexArray())
                {
                    _mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);
                    _mesh.SetIndexBufferData(indices, 0, 0, indices.Length);

                    var lines = new SubMeshDescriptor
                        (0, indices.Length, MeshTopology.LineStrip);

                    _mesh.SetSubMesh(0, lines);
                }
            }
            else
            {
                // Vertex update
                using var vertices = CreateVertexArray(data);
                _mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
            }
        }

        private NativeArray<int> CreateIndexArray()
        {
            var indices = Enumerable.Range(0, Resolution);
            return new NativeArray<int>(indices.ToArray(), Allocator.Temp);
        }

        private NativeArray<Vector3> CreateVertexArray(byte[] data)
        {
            var buffer = new NativeArray<Vector3>(Resolution, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (var vi = 0; vi < Resolution; vi++)
            {
                var x = (float)vi / Resolution * _width;
                var y = data[vi] / 255f * _height;
                buffer[vi] = new Vector3(x, _yOffset + y, 0);
            }

            return buffer;
        }

        private void OnDestroy()
        {
            if (_mesh != null) Destroy(_mesh);
        }




    }

}