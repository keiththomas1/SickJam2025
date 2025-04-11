using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPool
{
    private readonly List<GameObject> _pooledObjects = new List<GameObject>();

    private int _poolSize;
    private int _currentPoolIndex = 0;

    public List<GameObject> PooledObjects
    {
        get { return _pooledObjects; }
    }

    public GenericPool(GameObject component, Transform transform, int poolSize)
    {
        for (var i = 0; i < poolSize; i++)
        {
            GameObject newObject = GameObject.Instantiate(component);
            newObject.transform.SetParent(transform);
            this._pooledObjects.Add(newObject);
        }

        this._poolSize = poolSize;
    }

    public GameObject GetNewPoolObject()
    {
        var nextObject = this._pooledObjects[this._currentPoolIndex];
        this._currentPoolIndex = (this._currentPoolIndex == this._poolSize - 1) ? 0 : this._currentPoolIndex + 1;

        return nextObject;
    }
}
