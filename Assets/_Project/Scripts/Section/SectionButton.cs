using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SectionButton : MonoBehaviour
{
    [SerializeField] private SectionType sectionTypeToSwitch;
    [SerializeField] private TextMeshProUGUI text;
    private Vector3 _localScale;

    private void OnEnable()
    {
        _localScale = transform.localScale;
        UpdateUI();
    }

    public void Switch() // change section onclick section button, useful when added new sections, not toggles 2 sections but loops thorough all added sectons. 
    {
        var gMaster=GameManager.Instance;
        gMaster.sectionManager.SwitchToSection(sectionTypeToSwitch);
        sectionTypeToSwitch = GetNextEnumValue(sectionTypeToSwitch);
        UpdateUI();
        gMaster.audioManager.PlayAudioClip(9, 1);
        
        var selectedObject = GameManager.Instance.player.selectedObject;
        if (selectedObject)
        {
            selectedObject.currentSlot.Deselect();
            GameManager.Instance.player.selectedObject = null;
        }
    }

    private T GetNextEnumValue<T>(T enumValue) where T : Enum
    {
        Array enumValues = Enum.GetValues(typeof(T));
        int currentIndex = Array.IndexOf(enumValues, enumValue);
        int nextIndex = (currentIndex + 1) % enumValues.Length;
        return (T)enumValues.GetValue(nextIndex);
    }

    private void UpdateUI()
    {
        var buttonTitle = "";
        switch (sectionTypeToSwitch)
        {
            case SectionType.Coloring:
                buttonTitle = "Paint";
                break;
            case SectionType.Sewing:
                buttonTitle = "Sew";
                break;
        }

        text.text = buttonTitle;
    }

    public void ResponseVisually()
    {
        transform.DOScale(_localScale * 1.3f, 0.15f).OnComplete((() => { transform.DOScale(_localScale, 0.15f); }));
    }
}