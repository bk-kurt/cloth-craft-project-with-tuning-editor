using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ExecutableObject : MonoBehaviour
{
    [SerializeField] private float arrivalDuration;

    public Slot currentSlot;
    protected Executer executer;
    
    private bool _isComplete;

    // Sends this object to the executer for processing
    public void SendToExecution(Executer executer)
    {
        if (!executer.IsAvailableForNewExecution)
        {
            Debug.LogWarning("Executer is not available");
            return;
        }

        ResetSlot();
        PrepareForExecution(executer);
        MoveToExecutionPoint(executer);
    }

    // Resets the current slot and clears references
    private void ResetSlot()
    {
        currentSlot?.Deselect();
        GameManager.Instance.player.selectedObject = null;
        currentSlot.executableObject = null;
        currentSlot?.band.MoveAvailableExecutableObjectsToLeft();
        currentSlot = null;
    }

    // Prepares the object for execution
    private void PrepareForExecution(Executer executer)
    {
        this.executer = executer;
        (executer as ColoringPot)?.timerCircle.Hide(false);
    }

    // Moves the object to the executer's position
    private void MoveToExecutionPoint(Executer executer)
    {
        Vector3 targetPosition = executer is ColoringPot pot ? 
            pot.arrivalTransform.position : 
            executer.dropTransform.position;

        StartCoroutine(MoveTo(targetPosition,false));
        GameManager.Instance.audioManager.PlayAudioClip(10, 1);
    }

    // Coroutine to move the object
    public IEnumerator MoveTo(Vector3 target, bool shouldVisitCenter = true)
    {
        if (shouldVisitCenter)
        {
            yield return MoveToCenterImagePosition();
        }

        transform.DOMove(target, arrivalDuration).OnComplete(OnArrived);
    }

    // Sends the object to the coloring section
    public void SendToColoringSection()
    {
        Vector3 sectionButtonPosition = GetSectionButtonPosition();
        StartCoroutine(MoveTo(sectionButtonPosition, false));
    }

// Retrieves the position of the section button with an offset
    private Vector3 GetSectionButtonPosition()
    {
        Vector3 sectionButtonPos = GameManager.Instance.UIManager.sectionButton.transform.position;
        return sectionButtonPos - new Vector3(0.1f, 0, 0);
    }
    
    // Moves the object to the center image position
    public IEnumerator MoveToCenterImagePosition()
    {
        var centerPos = GetCenterImagePosition();
        yield return transform.DOMove(centerPos, arrivalDuration).WaitForCompletion();
    }

    // Gets the center image position with an offset
    private Vector3 GetCenterImagePosition()
    {
        Vector3 centerPos = GameManager.Instance.UIManager.centerImage.transform.position;
        return new Vector3(centerPos.x, centerPos.y + 0.3f, -2);
    }

    // Method called when the object arrives at the target position
    private void OnArrived()
    {
        if (_isComplete)
        {
            StartCoroutine(CompleteAllExecutionAndSell());
        }
        else
        {
            ExecuteOrPassToNextSection();
        }
    }

    // Completes the execution and handles selling the object
    private IEnumerator CompleteAllExecutionAndSell()
    {
        yield return new WaitForSeconds(0.3f);

        int amountToEarn = CalculateEarnings();
        GameManager.Instance.UIManager.SpawnMoney(amountToEarn, transform.position + new Vector3(0, 0.2f, 0));
        Destroy(gameObject);
    }
    public void Sell(Mission mission)
    {
        _isComplete = true;
        var targetPosition =
            mission
                ? GetMissionPosition(mission)
                : GetCenterImagePosition();
        StartCoroutine(MoveTo(targetPosition));
    }

    private Vector3 GetMissionPosition(Mission mission)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, mission.image.rectTransform.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 2));


        return worldPos;
    }


    // Calculates the earnings for selling the object
    private int CalculateEarnings()
    {
        if (!(this is Cloth cloth)) return 0;

        int clothTypeIndex = Array.IndexOf(Enum.GetValues(typeof(ClothType)), cloth.type);
        int colorMultiplier = cloth.colorCode + 1;
        int typeMultiplier = clothTypeIndex + 1;
        int basePriceMultiplier = GameManager.Instance.preferences.SoldClothPricePriceMultiplier;
        int missionMultiplier = GameManager.Instance.UIManager.header.missionBar.HasCompletedAnyMission(cloth.type, cloth.colorCode) ? 2 : 1;

        return colorMultiplier * typeMultiplier * basePriceMultiplier * missionMultiplier;
    }

    // Executes or passes the object to the next section
    private void ExecuteOrPassToNextSection()
    {
        if (executer != null)
        {
            executer.Execute(this);
        }
        else
        {
            GameManager.Instance.UIManager.sectionButton.ResponseVisually();
            GameManager.Instance.sectionManager.sections[1].band.spawner.TrySpawn(this);
        }
    }
    
    // Add other methods and logic here as needed
}
