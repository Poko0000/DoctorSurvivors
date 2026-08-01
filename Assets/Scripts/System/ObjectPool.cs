using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _objectQueue;
    private GameObject _prefab;

    // singlrton
    private static ObjectPool<T> _instance = null;
    public static ObjectPool<T> instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ObjectPool<T>();
            }
            return _instance;
        }
    }

    public int queueCount
    {
        get
        {
            return _objectQueue.Count;
        }
    }

    public void InitPool(GameObject prefab, int warnUpCount = 0)
    {
        _prefab = prefab;
        _objectQueue = new Queue<T>();

        List<T> warnUpList = new List<T>();
        for(int i = 0; i < warnUpCount; i++)
        {
            T t = instance.Spawn(Vector3.zero, Quaternion.identity);
            warnUpList.Add(t);
        }
        foreach(var obj in warnUpList) {
            instance.Return(obj);
        }
    }

    public T Spawn(Vector3 position, Quaternion quaternion)
    {
        if(_prefab == null)
        {
            Debug.LogError(typeof(T).ToString() + "prefab not set!");
            return default(T);
        }
        if(queueCount <= 0)
        {
           GameObject g = Object.Instantiate(_prefab,position,quaternion); 
           T t = g.GetComponent<T>();
           if(t == null)
            {
                Debug.LogError(typeof(T).ToString() + "not found! in prefab");
                return default(T);
            }
            _objectQueue.Enqueue(t);
        }
        T obj = _objectQueue.Dequeue();
        obj.gameObject.transform.position = position;
        obj.gameObject.transform.rotation = quaternion;
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        _objectQueue.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
