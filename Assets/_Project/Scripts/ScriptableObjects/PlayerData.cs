using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public int day;
    public int money;
    
    public int sewMachinePriceMultiplier;
    public int coloringPotPriceMultiplier;
    
    public float sewMachineExecutionTimeMultiplier;
    public float coloringPotExecutionTimeMultiplier;
    
    public float sewMachineExecutionTimeBase;
    public float coloringPotExecutionTimeBase;
    
    public int soldClothPricePriceMultiplier;

    [SerializeField] public List<int> fixedMissionAmountsForDays=new List<int>();
    public int DayMissionCount(int dayNo)
    {
        if (fixedMissionAmountsForDays.Count < dayNo)
        {
            Debug.LogError("Please add mission info for the day from editor tool in the tools menu");
            return 0;
        }
        
        return fixedMissionAmountsForDays[dayNo-1];
    }

    public void ResetToDefault()
    {
        day = 2;
        money = 100;

        soldClothPricePriceMultiplier = 300;
        
        sewMachinePriceMultiplier = 100;
        coloringPotPriceMultiplier = 200;

        sewMachineExecutionTimeMultiplier = 1.1f;
        coloringPotExecutionTimeMultiplier = 1.4f;
        
        sewMachineExecutionTimeBase = 1f;
        coloringPotExecutionTimeBase = 3f;
        
        
        fixedMissionAmountsForDays.Add(2);
        fixedMissionAmountsForDays.Add(3);
        fixedMissionAmountsForDays.Add(3);
        fixedMissionAmountsForDays.Add(4);
        fixedMissionAmountsForDays.Add(2);
        fixedMissionAmountsForDays.Add(4);
        fixedMissionAmountsForDays.Add(3);
        fixedMissionAmountsForDays.Add(4);
        
        
        GameManager.Instance.stats.OnMoneyChanged?.Invoke(); // invoke this as a placeholder for general update
    }
}