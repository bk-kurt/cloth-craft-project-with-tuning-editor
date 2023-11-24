using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<Slot> slots;
    private ObjectFactory factory;
    private float spawnInterval;

    public void Initialize(List<Slot> slots, ObjectFactory factory, float spawnInterval)
    {
        this.slots = slots;
        this.factory = factory;
        this.spawnInterval = spawnInterval;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnExecutableForIdleState());
    }

    private IEnumerator SpawnExecutableForIdleState()
    {
        yield return new WaitForSeconds(1f); // Initial delay if required

        while (true)
        {
            // Only try to spawn if there's an available slot.
            if (IsSlotAvailable())
            {
                ExecutableObject newObject = factory.CreateObject();
                TrySpawn(newObject);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private bool IsSlotAvailable()
    {
        return FindAvailableSlot() != null;
    }

    public void TrySpawn(ExecutableObject executableObject)
    {
        Slot availableSlot = FindAvailableSlot();
        if (availableSlot != null)
        {
            // Spawn the provided object if there's an available slot
            PlaceExecutableObjectInSlot(executableObject, availableSlot);
        }
        else
        {
            // If no slot is available, handle it accordingly
            // For instance, you can destroy the object as it should not have been created
            Destroy(executableObject.gameObject);
            Debug.LogWarning("Attempted to spawn an object, but no slots were available.");
        }
    }

    private void PlaceExecutableObjectInSlot(ExecutableObject executableObject, Slot slot)
    {
        slot.executableObject = executableObject;
        executableObject.currentSlot = slot;
        executableObject.transform.position = slot.dropTransform.position;
        // Additional initialization logic for the spawned object can be added here
    }


    private Slot FindAvailableSlot()
    {
        foreach (var slot in slots)
        {
            if (slot.isEmpty)
            {
                return slot;
            }
        }
        return null; // If no slot is available
    }

    public void StopSpawning()
    {
        StopAllCoroutines(); // Stop the spawning coroutine
    }
}



