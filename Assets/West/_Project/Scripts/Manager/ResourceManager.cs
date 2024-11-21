using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public static T GetResource<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public static List<T> GetResources<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path).ToList();
    }

    public static GameObject Instantiate(string path, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = GetResource<GameObject>(path);
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}
