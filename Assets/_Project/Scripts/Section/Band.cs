using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Band : MonoBehaviour
{
    [SerializeField] private bool shouldProduceInIdle;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private ExecutableObject sectionExecutableObject;
    [SerializeField] private Transform sectionObjectTransform;
    [SerializeField] private List<Slot> slots;
    [SerializeField] private Spawner spawnerPrefab; // Assign this prefab in the Unity Inspector

    [HideInInspector] public Spawner spawner;
    private ExecutableObjectFactory factory; // Declare the factory interface

    private void Start()
    {
        // Instantiate the factory with the specific object it should produce.
        // For example, if sectionExecutableObjectPrefab is the prefab you want to produce:
        factory = new ExecutableObjectFactory(sectionExecutableObject, sectionObjectTransform);

        // Instantiate the spawner and initialize it with the factory and other parameters.
        spawner = Instantiate(spawnerPrefab, Vector3.zero, Quaternion.identity);
        spawner.Initialize(slots, new ExecutableObjectFactory(sectionExecutableObject, sectionObjectTransform), spawnInterval);
        
        if (shouldProduceInIdle)
        {
            spawner.StartSpawning();
        }
    }

    public Slot FindAvailableSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isEmpty)
            {
                return slots[i];
            }
        }
        return null;
    }



    public void DeselectAllSlots()
    {
        foreach (var slot in slots)
        {
            slot.Deselect();
        }
    }


    public void MoveAvailableExecutableObjectsToLeft()
    {
        for (int i = 1; i < slots.Count; i++) // dont need check the left one, start from mid.
        {
            var slotExecutableObject = slots[i].executableObject;
            if (slotExecutableObject && slotExecutableObject != GameManager.Instance.player.selectedObject
                                     && slotExecutableObject.transform.position.x == slots[i].dropTransform.position.x
                                     && slots[i - 1].isEmpty)
            {
                slots[i].executableObject = null;
                slots[i - 1].boxCollider.enabled = false;
                slotExecutableObject.currentSlot = slots[i - 1];
                slotExecutableObject.currentSlot.executableObject = slotExecutableObject;
                slotExecutableObject.transform.DOMove(slotExecutableObject.currentSlot.dropTransform.position, 0.5f).OnComplete(() =>
                {
                    slotExecutableObject.currentSlot.boxCollider.enabled = true;
                });
            }
        }
    }
}