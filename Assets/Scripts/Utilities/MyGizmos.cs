using UnityEngine;

namespace GenericUtility
{
    public class MyGizmos : MonoBehaviour
    {
        public static void Label(in Vector3 position, string text)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(position, text);
#endif
        }
    }
}