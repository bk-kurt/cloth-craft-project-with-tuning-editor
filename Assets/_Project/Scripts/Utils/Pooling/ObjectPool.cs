using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    public List<PoolSO> poolsSO;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
            GameObject newObj = Instantiate(prefab, transform); // Instantiate under the pool container
            newObj.name = prefab.name; // Ensure the name is consistent for pooling logic.
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
            // Optionally instantiate a new one if the pool is empty.
            // This is a design choice and should be used with caution.
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
        // Reset the object's state here. This method should be customized to fit your game's needs.
        objectToReset.transform.position = Vector3.zero;
        objectToReset.transform.rotation = Quaternion.identity;
        // Additionally, reset any other necessary components to their default states.
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
    
    // This method has been removed to avoid confusion with objects not managed by the pool.
    // If an object not managed by the pool needs to be destroyed,
    // it should be handled explicitly outside of the pooling logic.
}
