using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDynamax.SceneSetup
{
    [ExecuteAlways]
    public class PlanetBuilder : MonoBehaviour
    {
        [SerializeField] private int _resolutionFactor = 100;
        [SerializeField] private float _width = 0.01f;

        [SerializeField] private MeshRenderer _meshRenderer = default;
        [SerializeField] private MeshFilter _meshFilter = default;

        private List<Vector3> _listVertices;
        private List<int> _listIndices;


        [Range(2, 256)]
        [SerializeField] private int _resolution = 10;

        [SerializeField] private MeshFilter[] _meshFilters = default;
        [SerializeField] private TerrainFace[] _terrainFaces = default;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { InitializePlanet(); }
        private void FixedUpdate() { }
        private void Update() { }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion

        private void OnValidate()
        {
            //InitializePlanet();

            //InitializeBuilder();
            //GenerateMesh();
        }

        private void InitializePlanet()
        {
            _listVertices = new List<Vector3>();
            _listIndices = new List<int>();

            BuildPlanet();
        }

        private void InitializeBuilder()
        {
            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

            if (_meshFilters == null || _meshFilters.Length == 0) { _meshFilters = new MeshFilter[directions.Length]; }
            _terrainFaces = new TerrainFace[directions.Length];

            for (int i = 0; i < directions.Length; i++)
            {
                if (_meshFilters[i] == null)
                {
                    GameObject meshObj = new GameObject("mesh");
                    meshObj.transform.parent = transform;

                    meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                    _meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    _meshFilters[i].sharedMesh = new Mesh();
                }

                _terrainFaces[i] = new TerrainFace(_meshFilters[i].sharedMesh, _resolution, directions[i]);
            }
        }
        private void GenerateMesh()
        {
            for (int i = 0; i < _terrainFaces.Length; i++)
            {
                _terrainFaces[i].ConstructMesh();
            }
        }

        private void AddMeshData_BigCube()
        {
            int length = 0;

            // Up
            AddYAxisVertices((_resolutionFactor - 1) * _width);
            AddIndicesCounterClockwise(length);
            length = _listVertices.Count;

            //DOWN
            AddYAxisVertices(0);
            AddIndicesClockwise(length);
            length = _listVertices.Count;

            //FORWARD
            AddZAxisVertices(0);
            AddIndicesCounterClockwise(length);
            length = _listVertices.Count;

            //BACKWARD
            AddZAxisVertices((_resolutionFactor - 1) * _width);
            AddIndicesClockwise(length);
            length = _listVertices.Count;

            //RIGHT
            AddXAxisVertices((_resolutionFactor - 1) * _width);
            AddIndicesCounterClockwise(length);
            length = _listVertices.Count;

            //LEFT
            AddXAxisVertices(0);
            AddIndicesClockwise(length);
            length = _listVertices.Count;
        }

        private void AddMeshData_Sphere()
        {
            AddMeshData_BigCube();

            var o = Vector3.one * ((_resolutionFactor - 1) * _width / 2.0f);
            for (int i = 0; i < _listVertices.Count; i++)
            {
                var newVertex = _listVertices[i] - o;
                newVertex = newVertex.normalized * (_resolutionFactor * _width);
                _listVertices[i] = newVertex;
            }
        }

        private void AddYAxisVertices(float y)
        {
            for (int z = 0; z < _resolutionFactor; z++)
            {
                for (int x = 0; x < _resolutionFactor; x++)
                {
                    _listVertices.Add(new Vector3(x * _width, y, z * _width));
                }
            }
        }
        private void AddZAxisVertices(float z)
        {
            for (int y = 0; y < _resolutionFactor; y++)
            {
                for (int x = 0; x < _resolutionFactor; x++)
                {
                    _listVertices.Add(new Vector3(x * _width, y * _width, z));
                }
            }
        }
        private void AddXAxisVertices(float x)
        {
            for (int y = 0; y < _resolutionFactor; y++)
            {
                for (int z = 0; z < _resolutionFactor; z++)
                {
                    _listVertices.Add(new Vector3(x, y * _width, z * _width));
                }
            }
        }

        private void AddIndicesClockwise(int length)
        {
            for (int k = 0; k < _resolutionFactor - 1; k++)
            {
                for (int j = 0; j < _resolutionFactor - 1; j++)
                {
                    _listIndices.Add(length + k * _resolutionFactor + j);
                    _listIndices.Add(length + k * _resolutionFactor + j + 1);
                    _listIndices.Add(length + (k + 1) * _resolutionFactor + j);

                    _listIndices.Add(length + (k + 1) * _resolutionFactor + j + 1);
                    _listIndices.Add(length + (k + 1) * _resolutionFactor + j);
                    _listIndices.Add(length + k * _resolutionFactor + j + 1);
                }
            }
        }
        private void AddIndicesCounterClockwise(int length)
        {
            for (int k = 0; k < _resolutionFactor - 1; k++)
            {
                for (int j = 0; j < _resolutionFactor - 1; j++)
                {
                    _listIndices.Add(length + k * _resolutionFactor + j);
                    _listIndices.Add(length + (k + 1) * _resolutionFactor + j);
                    _listIndices.Add(length + k * _resolutionFactor + j + 1);

                    _listIndices.Add(length + (k + 1) * _resolutionFactor + j + 1);
                    _listIndices.Add(length + k * _resolutionFactor + j + 1);
                    _listIndices.Add(length + (k + 1) * _resolutionFactor + j);
                }
            }
        }

        private void BuildPlanet()
        {
            ClearMeshData();
            AddMeshData_Sphere();
            Draw();
        }

        private void ClearMeshData()
        {
            _listVertices.Clear();
            _listIndices.Clear();
        }

        private void Draw()
        {
            var tempMesh = new Mesh();
            tempMesh.vertices = _listVertices.ToArray();
            tempMesh.triangles = _listIndices.ToArray();
            tempMesh.RecalculateNormals();
            tempMesh.RecalculateBounds();

            _meshFilter.mesh = tempMesh;

            GetOrAddComponent<SphereCollider>().radius = 1f;
        }

        private T GetOrAddComponent<T>() where T : Component => GetComponent<T>() != null ? GetComponent<T>() : gameObject.AddComponent<T>();
    }

    public class TerrainFace
    {
        private Mesh _mesh;
        private int _resolution;
        private Vector3 _localUp;
        private Vector3 _axisA;
        private Vector3 _axisB;

        public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
        {
            _mesh = mesh;
            _resolution = resolution;
            _localUp = localUp;

            _axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
            _axisB = Vector3.Cross(_localUp, _axisA);
        }

        public void ConstructMesh()
        {
            var vertices = new Vector3[_resolution * _resolution];
            var triangles = new int[(_resolution - 1) * (_resolution - 1) * 6];
            int triIndex = 0;

            for (int y = 0; y < _resolution; y++)
            {
                for (int x = 0; x < _resolution; x++)
                {
                    int i = x + y * _resolution;
                    var percent = new Vector2(x, y) / (_resolution - 1);
                    var pointOnUnitCube = _localUp + (percent.x - 0.5f) * 2 * _axisA + (percent.y - 0.5f) * 2 * _axisB;
                    var pointOnUnitSphere = pointOnUnitCube.normalized;

                    vertices[i] = pointOnUnitSphere;

                    if (x != _resolution - 1 && y != _resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + _resolution + 1;
                        triangles[triIndex + 2] = i + _resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + _resolution + 1; ;

                        triIndex += 6;
                    }
                }
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
        }
    }
}