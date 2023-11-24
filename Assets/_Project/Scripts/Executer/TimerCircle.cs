using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TimerCircle : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image fillImage;
    private Tween _colorTween;

    private void OnEnable()
    {
        Hide(true);
    }

    public void SetTimer(float timeToFinish)
    {
        Hide(false);
        var fillAmount = 0f;
        DOTween.To(() => fillAmount, x => fillAmount = x, 1, timeToFinish)
            .OnUpdate(() => { fillImage.fillAmount = fillAmount; }).OnComplete(() =>
            {
                _colorTween = fillImage.DOColor(Color.white, 0.5f).SetLoops(-1,LoopType.Yoyo);
            });
    }

    public void Hide(bool isHidden)
    {
        fillImage.color=Color.green;
        fillImage.fillAmount = 0;
        if (_colorTween != null)
        {
            _colorTween.Kill();
        }
        if (!isHidden)
        {
            fillImage.fillAmount = 0;
        }

        fillImage.enabled = !isHidden;
        backgroundImage.enabled = !isHidden;
    }
}