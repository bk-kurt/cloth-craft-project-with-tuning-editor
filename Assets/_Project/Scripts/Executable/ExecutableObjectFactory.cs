using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutableObjectFactory : ObjectFactory
{
    private ExecutableObject prefab;
    private Transform parentTransform;

    // Constructor that takes the prefab to instantiate and the parent transform.
    public ExecutableObjectFactory(ExecutableObject prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parentTransform = parent;
    }

    // This method creates an instance of the executable object from the prefab.
    public override ExecutableObject CreateObject()
    {
        return Object.Instantiate(prefab, parentTransform.position, Quaternion.identity, parentTransform);
    }
}



