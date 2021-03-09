namespace GenericUtility.ObjectPool
{
    using GenericUtility.Singleton;
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        [SerializeField] private List<Pool> _objectPoolsList = default;

        //Dictionary storing the objects in pool, key is prefab ID, value is the queue of pooled objects.
        private Dictionary<int, Queue<GameObject>> _pooledObjectsDic;

        //Dictionary storing the objects spawned, key is the object, value is the prefab ID.
        private Dictionary<GameObject, int> _spawnedObjectDic;

        private bool _initialized = false;

        private void Awake()
        { InitializePools(); }

        #region Public Method

        public void CreatePool<T>(T componentPrefab, byte initialPoolSize) where T : Component => CreatePool(componentPrefab.gameObject, initialPoolSize);

        public void CreatePool(GameObject objectPrefab, byte initialPoolSize)
        {
            if (!_initialized) { InitializePools(); }
            if (objectPrefab == null) { Debug.LogError("Object Prefab is null!"); return; }
            if (_pooledObjectsDic.ContainsKey(objectPrefab.GetInstanceID())) { Debug.LogError(objectPrefab.name + "_Pool already exists!"); return; }

            var pool = new Pool(objectPrefab, initialPoolSize);
            CreatePool(pool);
        }

        public T Spawn<T>(T prefab) where T : Component => Spawn(prefab, Vector3.zero, Quaternion.identity, transform.Find(prefab.name + "_Pool"));

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component => Spawn(prefab.gameObject, position, rotation, parent).GetComponent<T>();

        public GameObject Spawn(GameObject prefab) => Spawn(prefab, Vector3.zero, Quaternion.identity, transform.Find(prefab.name + "_Pool"));

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!_initialized) { InitializePools(); }
            if (prefab == null) { Debug.LogError("Object Prefab is null!"); return null; }

            var objectToSpawn = GrabFromPool(prefab);
            objectToSpawn.transform.parent = parent;
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            IPooledObject pooledObjectInterface = objectToSpawn.GetComponentInChildren<IPooledObject>();
            if (pooledObjectInterface != null) { pooledObjectInterface.OnObjectSpawn(); }

            _spawnedObjectDic.Add(objectToSpawn, prefab.GetInstanceID());

            return objectToSpawn;
        }

        public void Recycle<T>(T objectToRecycle) where T : Component => Recycle(objectToRecycle.gameObject);

        public void Recycle(GameObject objectToRecycle)
        {
            if (!_initialized) { InitializePools(); }
            if (!TryReturnToPool(objectToRecycle)) { Destroy(objectToRecycle); }
        }

        #endregion Public Method

        private void InitializePools()
        {
            if (_initialized) { return; }

            _pooledObjectsDic = new Dictionary<int, Queue<GameObject>>();
            _spawnedObjectDic = new Dictionary<GameObject, int>();

            for (int i = 0; i < _objectPoolsList.Count; i++)
            {
                var pool = _objectPoolsList[i];

                pool.InitializePool();
                CreatePool(pool);
            }

            _initialized = true;
        }

        private void CreatePool(Pool pool)
        {
            if (_pooledObjectsDic.ContainsKey(pool.PrefabID)) { Debug.LogError(pool.PoolName + " already exists!"); return; }

            var poolTransform = new GameObject(pool.PoolName).transform;
            var poolQueue = new Queue<GameObject>();

            poolTransform.SetParent(transform);
            for (int j = 0; j < pool.InitialPoolSize; j++)
            {
                var poolObject = Instantiate(pool.ObjectPrefab, poolTransform);
                poolObject.SetActive(false);
                poolQueue.Enqueue(poolObject);
            }

            _pooledObjectsDic.Add(pool.PrefabID, poolQueue);
        }

        private GameObject GrabFromPool(GameObject prefab)
        {
            if (!_initialized) { InitializePools(); }
            if (prefab == null) { Debug.LogError("Object Prefab is null!"); return null; }

            if (_pooledObjectsDic.TryGetValue(prefab.GetInstanceID(), out Queue<GameObject> queue))
            {
                var objectToSpawn = (queue.Count > 0) ? queue.Dequeue() : null;
                if (objectToSpawn == null) { objectToSpawn = Instantiate(prefab); }
                return objectToSpawn;
            }
            CreatePool(prefab, 1);
            return GrabFromPool(prefab);
        }

        private bool TryReturnToPool(GameObject objectToRecycle)
        {
            if (!_initialized) { InitializePools(); }

            if (!_spawnedObjectDic.TryGetValue(objectToRecycle, out int id)) { return false; }
            if (!_pooledObjectsDic.ContainsKey(id)) { return false; }

            objectToRecycle.SetActive(false);
            objectToRecycle.transform.SetParent(transform.Find(id + "_Pool"));

            _pooledObjectsDic[id].Enqueue(objectToRecycle);
            _spawnedObjectDic.Remove(objectToRecycle);
            return true;
        }
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private GameObject _objectPrefab = default;
        [SerializeField] private byte _initialPoolSize = default;

        public int PrefabID { get; private set; }
        public string PoolName { get; private set; }

        public GameObject ObjectPrefab => _objectPrefab;
        public byte InitialPoolSize => _initialPoolSize;

        public Pool(GameObject objectPrefab, byte initialPoolSize)
        {
            if (objectPrefab == null) { Debug.LogError("Object Prefab is null!"); return; }

            _objectPrefab = objectPrefab;
            _initialPoolSize = initialPoolSize;
            InitializePool();
        }

        public bool InitializePool()
        {
            if (_objectPrefab == null) { Debug.LogError("Object Prefab is null!"); return false; }

            PrefabID = _objectPrefab.GetInstanceID();
            PoolName = PrefabID + "_Pool";
            return true;
        }
    }
}