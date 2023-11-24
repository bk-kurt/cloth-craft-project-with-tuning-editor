using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionMachine : MonoBehaviour
{
    [SerializeField] private Image clothImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image ringImage;
    [SerializeField] private Color completeColor;
    [SerializeField] private Color inCompleteColor;
    [SerializeField] private List<Sprite> clothSprites;

    private void OnEnable()
    {
        backgroundImage.color = inCompleteColor;
    }

    public void Hide(bool isHidden)
    {
        clothImage.enabled = !isHidden;
        backgroundImage.enabled = !isHidden;
        ringImage.enabled = !isHidden;

        ResetColor();
    }

    public void SetMission(ClothType clothType)
    {
        Array enumValues = Enum.GetValues(typeof(ClothType));
        int currentIndex = Array.IndexOf(enumValues, clothType);
        clothImage.sprite = clothSprites[currentIndex];
        backgroundImage.DOColor(inCompleteColor, 0.1f);
        ringImage.DOColor((inCompleteColor + Color.white) / 2, 0.1f);
    }

    Tween _colorTween;
    Tween _colorTween1;

    public void CompleteResponse()
    {
        _colorTween = backgroundImage.DOColor(completeColor, 1f).SetLoops(2);
        _colorTween1 = ringImage.DOColor((completeColor + Color.white) / 2, 1f).SetLoops(2);
    }

    public void ResetColor()
    {
        if (_colorTween.IsActive())
        {
            _colorTween.Kill();
        }

        if (_colorTween1.IsActive())
        {
            _colorTween1.Kill();
        }

        backgroundImage.DOColor(inCompleteColor, 0.1f);
        ringImage.DOColor(Color.white, 0.1f);
    }
}