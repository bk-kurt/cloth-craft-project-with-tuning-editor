using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : PersistentSingleton<ObjectPool>
{
    public List<PoolSO> poolsSO;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var poolSO in poolsSO)
        {
            CreatePool(poolSO.prefab, poolSO.size);
        }
    }

    private void CreatePool(GameObject prefab, int size)
    {
        Queue<GameObject> objectQueue = new Queue<GameObject>();
        string poolKey = prefab.name;

        for (int i = 0; i < size; i++)
        {
            GameObject newObj = Instantiate(prefab, transform);
            newObj.name = prefab.name;
            newObj.SetActive(false);
            objectQueue.Enqueue(newObj);
        }

        poolDictionary.Add(poolKey, objectQueue);
    }

    public GameObject GetObjectFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag {tag} does not exist.");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"All objects in pool {tag} are in use. Instantiating a new one.");
            return Instantiate(poolDictionary[tag].Peek()); // Peek to get the prefab reference.
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        ResetObjectState(objectToSpawn);
        return objectToSpawn;
    }

    private void ResetObjectState(GameObject objectToReset)
    {
        objectToReset.transform.position = Vector3.zero;
        objectToReset.transform.rotation = Quaternion.identity;
    }

    public void ReturnObjectToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag {tag} does not exist.");
            return;
        }

        ResetObjectState(objectToReturn);
        objectToReturn.transform.SetParent(transform, false);
        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}
