using System;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public abstract class Executer : MonoBehaviour
{
    private const float RotateDuration = 0.5f;
    private const float ScaleMultiplierForCollection = 1.5f;

    public int level;
    public int price;
    public float executionTime;

    public bool isReadyForCollection;
    public bool isExecuting;

    public Transform dropTransform;
    public Transform arrivalTransform;
    public ExecutableObject objectToUse;


    [SerializeField] protected ParticleSystem sparklesPs;

    [SerializeField] private ExecuterUI executerUI;
    [SerializeField] private ParticleSystem dustPs;

    private Vector3 objectToUseInitialScale;
    private Vector3 objectToUseInitialPos;
    private Quaternion objectToUseInitialRot;

    public bool IsAvailableForNewExecution => !isExecuting && !IsLocked && objectToUse == null;

    public bool IsLocked =>
        level > GameManager.Instance.stats.Day || !GameManager.Instance.stats.HasExecuterBeenPaid(this);

    protected virtual void OnEnable()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        GameManager.Instance.stats.OnLevelChanged += UpdateUI;
        GameManager.Instance.stats.OnMoneyChanged += UpdateUI;
        UpdateUI();
        SetAdjustedPriceAndExecutionTime();
    }

    protected virtual void SetAdjustedPriceAndExecutionTime()
    {
        // Implementation for derived classes
    }

    private void Start()
    {
        if (level <= 1)
        {
            price = 0;
            GameManager.Instance.stats.PayForExecuter(this);
        }
    }

    public virtual void Execute(ExecutableObject executableObject)
    {
        if (!CanExecute(executableObject))
        {
            Debug.LogWarning("Cannot execute: Invalid executable object.");
            return;
        }

        PrepareObjectForExecution(executableObject);
        BeginExecution();
    }

    private bool CanExecute(ExecutableObject executableObject)
    {
        return executableObject != null && !isExecuting;
    }

    private void PrepareObjectForExecution(ExecutableObject executableObject)
    {
        objectToUse = executableObject;
        isExecuting = true;

        Transform objectTransform = objectToUse.transform;
        objectToUseInitialPos = objectTransform.position;
        objectToUseInitialRot = objectTransform.localRotation;
        objectToUseInitialScale = objectTransform.localScale;

        objectTransform.SetParent(dropTransform);
        objectTransform.localPosition = Vector3.zero;
        objectTransform.DOLocalRotate(Vector3.zero, RotateDuration);
        objectTransform.localScale = Vector3.one;
    }

    protected virtual void BeginExecution()
    {
        // Start execution logic here
    }

    public virtual void CompleteAndResetTheAppearanceOfUsedObject()
    {
        if (objectToUse == null) return;

        Transform objectTransform = objectToUse.transform;
        GameManager.Instance.audioManager.PlayAudioClip(5);
        sparklesPs.Play();
        dustPs.Play();

        objectTransform.position = objectToUseInitialPos + Vector3.down * 0.02f;
        objectTransform.localScale = objectToUseInitialScale;
        objectTransform.localRotation = objectToUseInitialRot;
    }

    public virtual void ClaimTheUsedObject()
    {
        if (objectToUse == null) return;
        StartCoroutine(objectToUse.MoveToCenterImagePosition());
        Transform objectTransform = objectToUse.transform;
        objectTransform.localScale = objectToUseInitialScale * ScaleMultiplierForCollection;
        objectTransform.localRotation = objectToUseInitialRot;
        objectToUse = null;

        GameManager.Instance.audioManager.PlayAudioClip(7);
    }

    private void OnFingerDown(LeanFinger finger)
    {
        Ray ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            Select(finger);
        }
    }

    public void Select(LeanFinger leanFinger)
    {
        if (isReadyForCollection)
        {
            Collect();
        }
        else
        {
            var selectedObject = GameManager.Instance.player.selectedObject;
            if (selectedObject != null)
            {
                selectedObject.SendToExecution(this);
            }
        }
    }

    public void Deselect()
    {
        // not impl. yet
    }

    public virtual void Collect()
    {
        ClaimTheUsedObject();
    }

    public virtual void Purchase()
    {
        GameManager.Instance.stats.PayForExecuter(this);
    }

    private void UpdateUI()
    {
        executerUI.ShowStatus(level, GameManager.Instance.stats.HasExecuterBeenPaid(this), price);
    }

    protected virtual void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        GameManager.Instance.stats.OnLevelChanged -= UpdateUI;
        GameManager.Instance.stats.OnMoneyChanged -= UpdateUI;
    }
}