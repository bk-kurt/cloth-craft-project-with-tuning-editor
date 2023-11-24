using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pool", menuName = "Object Pooling/Pool")]
public class PoolSO : ScriptableObject
{
    public GameObject prefab;
    public int size;
}
