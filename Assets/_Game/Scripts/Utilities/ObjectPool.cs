

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPool
{
    private GameObject template;

    private List<GameObject> pooledObjects;
    private List<GameObject> usedObjects;

    public  ObjectPool(GameObject temp)
    {
        template = temp;
        pooledObjects = new List<GameObject>();
        usedObjects = new List<GameObject>();
    }
    
    public T get<T>()
    {
        return get().GetComponent<T>();
    }

    public void recycle(MonoBehaviour obj)
    {
        recycle(obj.gameObject);
    }

    private GameObject get()
    {
        GameObject result;
        if (pooledObjects.Count != 0)
        {
            result = pooledObjects[0];
            pooledObjects.Remove(result);
        }
        else
        {

            result = GameObject.Instantiate(template);
        }

        usedObjects.Add(result);
        return result;
    }

    private void recycle(GameObject obj)
    {
        usedObjects.Remove(obj);
        pooledObjects.Add(obj);
        
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.one * 99999;
    }
}
