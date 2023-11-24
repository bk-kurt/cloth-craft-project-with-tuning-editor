using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public Image image;
    public ClothType type;
    public int colorCode;
    
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private List<Color> colors;


    public void InitializeWithMission(ClothType missionType,int missionColorCode)
    {
        colorCode = missionColorCode;
        type = missionType;
        AssignIcon();
    }

    private void AssignIcon()
    {
        Array enumValues = Enum.GetValues(typeof(ClothType));
        int currentIndex = Array.IndexOf(enumValues, type);

        // Use the index to get the corresponding sprite from the icons list
        image.sprite = icons[currentIndex];
        image.color = colors[colorCode];
    }


    public void Complete()
    {
        GameManager.Instance.audioManager.PlayAudioClip(13);
        Destroy(gameObject);
    }
}