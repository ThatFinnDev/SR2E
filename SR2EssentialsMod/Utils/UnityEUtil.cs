using System.Linq;

namespace SR2E.Utils;

public static class UnityEUtil
{
    public static T GetObjectRecursively<T>(this GameObject obj, string name) where T : class
    {
        var transform = obj.transform;

        List<GameObject> totalChildren = GetAllChildren(transform);
        for (int i = 0; i < totalChildren.Count; i++)
            if (totalChildren[i].name == name)
            {
                if (typeof(T) == typeof(GameObject))
                    return totalChildren[i] as T;
                if (typeof(T) == typeof(Transform))
                    return totalChildren[i].transform as T;
                if (totalChildren[i].GetComponent<T>() != null)
                    return totalChildren[i].GetComponent<T>();
            }

        return null;
    }

    public static T GetObjectRecursively<T>(this Transform transform, string name) where T : class
    {
        List<GameObject> totalChildren = GetAllChildren(transform);
        for (int i = 0; i < totalChildren.Count; i++)
            if (totalChildren[i].name == name)
            {
                if (typeof(T) == typeof(GameObject))
                    return totalChildren[i] as T;
                if (typeof(T) == typeof(Transform))
                    return totalChildren[i].transform as T;
                if (totalChildren[i].GetComponent<T>() != null)
                    return totalChildren[i].GetComponent<T>();
            }

        return null;
    }
    public static List<Transform> GetChildren(this Transform obj)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < obj.childCount; i++)
            children.Add(obj.GetChild(i));
        return children;
    }
    public static void DestroyAllChildren(this Transform obj)
    {
        for (int i = 0; i < obj.childCount; i++) 
            GameObject.Destroy(obj.GetChild(i).gameObject);
    }
    public static void DestroyAllChildren(this GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++) 
            GameObject.Destroy(obj.transform.GetChild(i).gameObject);
    }
    public static List<GameObject> GetAllChildren(this GameObject obj)
    {
        var container = obj.transform;
        List<GameObject> allChildren = new List<GameObject>();
        for (int i = 0; i < container.childCount; i++)
        {
            var child = container.GetChild(i);
            allChildren.Add(child.gameObject);
            allChildren.AddRange(GetAllChildren(child));
        }

        return allChildren;
    }

    public static List<GameObject> GetAllChildren(this Transform container)
    {
        List<GameObject> allChildren = new List<GameObject>();
        for (int i = 0; i < container.childCount; i++)
        {
            var child = container.GetChild(i);
            allChildren.Add(child.gameObject);
            allChildren.AddRange(GetAllChildren(child));
        }

        return allChildren;
    }

    public static T[] GetAllChildrenOfType<T>(this GameObject obj) where T : Component
    {
        List<T> children = new List<T>();
        foreach (var child in obj.GetAllChildren())
        {
            if (child.GetComponent<T>() != null)
            {
                children.Add(child.GetComponent<T>());
            }
        }

        return children.ToArray();
    }

    public static T[] GetAllChildrenOfType<T>(this Transform obj) where T : Component
    {
        List<T> children = new List<T>();
        foreach (var child in obj.GetAllChildren())
        {
            if (child.GetComponent<T>() != null)
            {
                children.Add(child.GetComponent<T>());
            }
        }

        return children.ToArray();
    }
    
    public static T? Get<T>(string name) where T : Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
    public static List<T> GetAll<T>() where T : Object => Resources.FindObjectsOfTypeAll<T>().ToList();
    
    public static T AddComponent<T>(this Component obj) where T : Component => obj.gameObject.AddComponent<T>();
    public static bool AddComponent<T>(this Transform obj) where T : Component => obj.gameObject.AddComponent<T>();
    public static bool AddComponent(this Transform obj, Il2CppSystem.Type componentType) => obj.gameObject.AddComponent(componentType);
    public static bool AddComponent(this Transform obj, System.Type componentType) => obj.gameObject.AddComponent(componentType.il2cppTypeof());
    public static bool AddComponent(this GameObject obj, System.Type componentType) => obj.AddComponent(componentType.il2cppTypeof());
    public static bool HasComponent<T>(this Transform obj) where T : Component => HasComponent<T>(obj.gameObject);
    public static bool HasComponent<T>(this GameObject obj) where T : Component
    {
        try { return obj.GetComponent<T>()!=null; } catch { return false; }
    }
    public static bool RemoveComponent<T>(this Transform obj) where T : Component => RemoveComponent<T>(obj.gameObject);
    public static bool RemoveComponent<T>(this GameObject obj) where T : Component
    {
        try
        {
            T comp = obj.GetComponent<T>();
            var check = comp.gameObject;
            Object.Destroy(comp);
            return true;
        }
        catch { return false; }
    }
    
    public static GameObject CopyObject(this GameObject obj) => Object.Instantiate(obj, prefabHolder.transform);

    public static void MakePrefab(this GameObject obj)
    {
        Object.DontDestroyOnLoad(obj);
        obj.transform.parent = prefabHolder.transform;
    }

}