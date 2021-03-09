namespace GenericUtility.Singleton
{
    using UnityEngine;

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting) { return null; }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (FindObjectsOfType(typeof(T)).Length > 1) { return _instance; }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                        }
                    }
                    return _instance;
                }
            }
        }

        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public void OnDestroy()
        {
            if (IsDontDestroyOnLoad()) { _applicationIsQuitting = true; }
        }

        private static bool IsDontDestroyOnLoad()
        {
            if (_instance == null) { return false; }
            if ((_instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave) { return true; }
            return false;
        }
    }
}