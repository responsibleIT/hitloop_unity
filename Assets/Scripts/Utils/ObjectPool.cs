using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
	private Transform parent;
	private GameObject prefab;

	private Action<T> initAction;
	private Action<T> cleanUpAction;

	private List<T> pooledObjects = new List<T>();
	internal List<T> usedObjects = new List<T>();

	// if this is -1, there is no poolsize
	internal int maxPoolSize;

	public ObjectPool(string prefabPath, Action<T> initAction, Action<T> cleanUpAction, Transform parent, int maxPoolSize = -1)
	{
		this.prefab = Resources.Load<GameObject>(prefabPath);
		this.maxPoolSize = maxPoolSize;

		this.initAction = initAction;
		this.cleanUpAction = cleanUpAction;

		this.parent = parent;
	}

	public ObjectPool(GameObject prefab, Action<T> initAction, Action<T> cleanUpAction, Transform parent, int maxPoolSize = -1)
	{
		this.prefab = prefab;
		this.maxPoolSize = maxPoolSize;

		this.initAction = initAction;
		this.cleanUpAction = cleanUpAction;

		this.parent = parent;
	}

	public T Get()
	{
		if (pooledObjects.Count > 0)
		{
			// get a pooled object
			T obj = pooledObjects[0];
			usedObjects.Add(obj);
			pooledObjects.Remove(obj);
			return obj;
		}
		else
		{
			// if a max pool size is defined, check it
			if (maxPoolSize > -1 && usedObjects.Count > maxPoolSize)
			{
				Debug.LogError("Max pool size exceeded");
				return null;
			}

			// get a new object
			T obj = GameObject.Instantiate(prefab, parent).GetComponent<T>();
			initAction?.Invoke(obj);
			usedObjects.Add(obj);
			return obj;
		}
	}

	public void Release(T obj)
	{
		CleanUp(obj);
		usedObjects.Remove(obj);
		pooledObjects.Add(obj);
	}

	public void ReleaseAll()
	{
		while (usedObjects.Count > 0)
			Release(usedObjects[0]);
	}

	private void CleanUp(T obj)
	{
		cleanUpAction?.Invoke(obj);
	}

	internal string GetStatus()
	{
		return "used objects: " + usedObjects.Count + ", pooled objects " + pooledObjects.Count;
	}
}