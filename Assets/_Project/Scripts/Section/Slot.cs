using System;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour, ISelectable
{
    public ExecutableObject executableObject;
    public Transform dropTransform;
    public BoxCollider boxCollider;
    public Band band;

    public bool isEmpty => !executableObject;

    [SerializeField] private Image circleImage;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;

    private Vector3 _initialScale;

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += Select;
        _initialScale = transform.localScale;
        band.MoveAvailableExecutableObjectsToLeft();
    }

    public void Select(LeanFinger leanFinger)
    {
        Ray ray = Camera.main.ScreenPointToRay(leanFinger.ScreenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (isEmpty)
                {
                    Debug.LogWarning("Slot is empty");
                    return;
                }

                band.DeselectAllSlots();
                GameManager.Instance.player.selectedObject = executableObject;
                circleImage.color = selectedColor;
                GameManager.Instance.audioManager.PlayAudioClip(11);
                DoVisualResponse(true, circleImage.gameObject);
                DoVisualResponse(true, executableObject.gameObject);
            }
        }
    }


    public void DoVisualResponse(bool scaleUp, GameObject gameObject)
    {
        if (scaleUp)
        {
            gameObject.transform.DOScale(_initialScale * 1.2f, 0.05f).OnComplete(() =>
            {
                gameObject.transform.DOScale(_initialScale * 1.1f, 0.1f);
            });

            executableObject.gameObject.transform.DORotateQuaternion(Quaternion.Euler(new Vector3(0, 0, -10)), 0.055f)
                .OnComplete(() =>
                {
                    executableObject.gameObject.transform.DORotateQuaternion(Quaternion.Euler(Vector3.zero), 0.05f);
                });
        }
        else
        {
            gameObject.transform.DOScale(_initialScale * 0.8f, 0.05f).OnComplete(() =>
            {
                gameObject.transform.DOScale(_initialScale, 0.1f);
            });
        }
    }

    public void Deselect()
    {
        if (executableObject)
        {
            DoVisualResponse(false, circleImage.gameObject);
            DoVisualResponse(false, executableObject.gameObject);
        }

        circleImage.color = deselectedColor;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= Select;
    }
}