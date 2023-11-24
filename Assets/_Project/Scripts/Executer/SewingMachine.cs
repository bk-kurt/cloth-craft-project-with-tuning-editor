using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using Unity.Mathematics;
using UnityEngine;

public class SewingMachine : Executer
{
    [SerializeField] private Cloth clothPrefabToProduce;
    [SerializeField] private Animator animator;
    [SerializeField] private UIMissionMachine uIMissionMachine;

    [SerializeField] private Transform cylinder;
    [SerializeField] private List<GameObject> circles = new List<GameObject>();
    [SerializeField] private Transform sewingTransform;
    [SerializeField] private Transform sewingTransformPointStart;
    [SerializeField] private Transform sewingTransformPointEnd;

    [SerializeField] private ParticleSystem circleDisappearPoofPs;

    private Cloth _sewedCloth;

    public override void Initialize()
    {
        base.Initialize();
        GameManager.Instance.stats.OnMoneyChanged += UpdateUIMissionMachineAvailability;
        UpdateUIMissionMachineAvailability();
        SetAdjustedPriceAndExecutionTime();
    }

    private void UpdateUIMissionMachineAvailability()
    {
        var isAvailable = IsAvailableForNewExecution;
        uIMissionMachine.Hide(!isAvailable);

        if (isAvailable)
        {
            uIMissionMachine.SetMission(clothPrefabToProduce.type);
        }
    }

    protected override void SetAdjustedPriceAndExecutionTime()
    {
        base.SetAdjustedPriceAndExecutionTime();
        var preferences = GameManager.Instance.preferences;

        price = level*level * preferences.SewMachinePriceMultiplier;
        executionTime = preferences.SewMachineExecutionTimeBase +
                        preferences.SewMachineExecutionTimeMultiplier * level * level;
    }


    public override void Execute(ExecutableObject executableObject)
    {
        executableObject.gameObject.SetActive(false); // for sewing machine, hide the executable object.
        base.Execute(executableObject);
        if (!clothPrefabToProduce)
        {
            Debug.LogWarning("Object to produce was not found");
            return;
        }

        UseRopeAndSew();
    }

    public void UseRopeAndSew()
    {
        Rope rope = objectToUse as Rope;
        if (rope)
        {
            //  rope.GetUsed(executionTime); // deprecation of old. (actual use)
            StartCoroutine(UseRopeCirclesOneByOne(executionTime));
        }
        else
        {
            Debug.LogWarning("Rope is null");
        }

        StartCoroutine(Sew());
    }

    private Quaternion initialRotation = Quaternion.identity;

    private IEnumerator Sew()
    {
        GameManager.Instance.audioManager.PlayAudioClipWithLoop(0, executionTime, 2f, 0.7f);
        isReadyForCollection = false;

        animator.SetBool("IsSewing", true);
        _sewedCloth = Instantiate(clothPrefabToProduce, transform.position, quaternion.identity,
            sewingTransform);
        initialRotation = _sewedCloth.transform.rotation;
        _sewedCloth.GetSewed(executionTime);

        sewingTransform.transform.position = sewingTransformPointStart.position;
        sewingTransform.rotation = sewingTransformPointStart.rotation;
        sewingTransform.transform.DOMove(sewingTransformPointEnd.position, executionTime); // slide cloth parent
        sewingTransform.transform.DORotateQuaternion(sewingTransformPointEnd.rotation,
            executionTime); // slide cloth parent

        uIMissionMachine.Hide(true);

        yield return new WaitForSeconds(executionTime);

        animator.SetBool("IsSewing", false);

        isExecuting = false;
        isReadyForCollection = true;
        objectToUse = null;
        uIMissionMachine.Hide(false);
        uIMissionMachine.CompleteResponse();

        CompleteAndResetTheAppearanceOfUsedObject();
        // ready to collect with tap.
    }

    public override void Collect()
    {
        if (!isReadyForCollection) return;
        if (!GameManager.Instance.sectionManager.sections[1].band.FindAvailableSlot())
        {
            Debug.LogWarning("No available slot left on coloring section, please color some awaiting cloths!");
            GameManager.Instance.UIManager.sectionButton.ResponseVisually();
            return;
        }

        base.Collect();
        _sewedCloth.transform.rotation = initialRotation; 
        isReadyForCollection = false;
        _sewedCloth.currentSlot = null;
        _sewedCloth.SendToColoringSection();
        uIMissionMachine.ResetColor();
    }

    private IEnumerator UseRopeCirclesOneByOne(float executionTime)
    {
        foreach (var circle in circles)
        {
            circle.SetActive(true);
        }

        executionTime /= circles.Count;
        for (int i = 0; i < circles.Count; i++)
        {
            var cylinderRot = cylinder.transform.rotation.eulerAngles;
            cylinder.transform.DORotate(new Vector3(cylinderRot.x, cylinderRot.y + 40, cylinderRot.z + 0),
                executionTime);
            var circlePos = circles[i].transform.position;
            circleDisappearPoofPs.transform.position = new Vector3(circleDisappearPoofPs.transform.position.x,
                circlePos.y, circleDisappearPoofPs.transform.position.z);
            circleDisappearPoofPs.Play();
            circles[i].SetActive(false);
            yield return new WaitForSeconds(executionTime);
        }

        //used totally
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.stats.OnMoneyChanged -= UpdateUIMissionMachineAvailability;
    }
}