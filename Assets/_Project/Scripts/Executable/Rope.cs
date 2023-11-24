using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : ExecutableObject
{
    [SerializeField] private List<GameObject> circles = new List<GameObject>();
    public void GetUsed(float executionTime) // instead use the models circles for now, not this
    {
        StartCoroutine(GetUsedEnum(executionTime));
    }


    private IEnumerator GetUsedEnum(float time)
    {
        time /= circles.Count;
        for (int i = 0; i < circles.Count; i++)
        {
            circles[i].SetActive(false);
            yield return new WaitForSeconds(time);
        }
    }
}
