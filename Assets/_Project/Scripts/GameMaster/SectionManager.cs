using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
    public List<Section> sections;


    public void SwitchToSection(SectionType sectionTypeToSwitch)
    {
        for (int i = 0; i < sections.Count; i++)
        {
            var checkedSection = sections[i];
            if (sectionTypeToSwitch == checkedSection.type)
            {
                checkedSection.Switch(true);
                GameManager.Instance.UIManager.canvas.worldCamera = checkedSection.sectionCamera;
            }
            else
            {
                checkedSection.Switch(false);
            }
        }
    }
}

public enum SectionType
{
    Sewing,
    Coloring
}