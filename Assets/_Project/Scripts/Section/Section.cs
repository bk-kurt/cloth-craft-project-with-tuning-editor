using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public SectionType type;
    public Band band;
    public Camera sectionCamera;
    public List<Executer> executers;


    public void Switch(bool status)
    {
        sectionCamera.enabled = status;
    }
}
