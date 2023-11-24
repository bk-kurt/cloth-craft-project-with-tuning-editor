using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Preferences : MonoBehaviour
{
    [SerializeField] 
    private PlayerData playerData;

    public Action OnPreferenceChanged;

    private const string SoldClothPriceMultiplierKey = "soldClothPricePriceMultiplier";
    private const string SewMachinePriceMultiplierKey = "sewMachinePriceMultiplier";
    private const string ColoringPotPriceMultiplierKey = "coloringPotPriceMultiplier";
    private const string SewMachineExecutionTimeMultiplierKey = "sewMachineExecutionTimeMultiplier";
    private const string SewMachineExecutionTimeBaseKey = "sewMachineExecutionTimeBase";
    private const string ColoringPotExecutionTimeMultiplierKey = "coloringPotExecutionTimeMultiplier";
    private const string ColoringPotExecutionTimeBaseKey = "coloringPotExecutionTimeBase";

    public int SoldClothPricePriceMultiplier
    {
        get => playerData.soldClothPricePriceMultiplier;
        set => SetField(ref playerData.soldClothPricePriceMultiplier, value, SoldClothPriceMultiplierKey);
    }

    public int SewMachinePriceMultiplier
    {
        get => playerData.sewMachinePriceMultiplier;
        set => SetField(ref playerData.sewMachinePriceMultiplier, value, SewMachinePriceMultiplierKey);
    }

    public int ColoringPotPriceMultiplier
    {
        get => playerData.coloringPotPriceMultiplier;
        set => SetField(ref playerData.coloringPotPriceMultiplier, value, ColoringPotPriceMultiplierKey);
    }

    public float SewMachineExecutionTimeMultiplier
    {
        get => playerData.sewMachineExecutionTimeMultiplier;
        set => SetField(ref playerData.sewMachineExecutionTimeMultiplier, value, SewMachineExecutionTimeMultiplierKey);
    }

    public float SewMachineExecutionTimeBase
    {
        get => playerData.sewMachineExecutionTimeBase;
        set => SetField(ref playerData.sewMachineExecutionTimeBase, value, SewMachineExecutionTimeBaseKey);
    }

    public float ColoringPotExecutionTimeMultiplier
    {
        get => playerData.coloringPotExecutionTimeMultiplier;
        set => SetField(ref playerData.coloringPotExecutionTimeMultiplier, value, ColoringPotExecutionTimeMultiplierKey);
    }

    public float ColoringPotExecutionTimeBase
    {
        get => playerData.coloringPotExecutionTimeBase;
        set => SetField(ref playerData.coloringPotExecutionTimeBase, value, ColoringPotExecutionTimeBaseKey);
    }

    private void Awake()
    {
        LoadGameData();
    }

    private void SetField<T>(ref T field, T value, string key)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPreferenceChanged?.Invoke();
            SaveSinglePreference(key, value);
        }
    }

    private void SaveSinglePreference<T>(string key, T value)
    {
        // Determine the type of value and save it accordingly
        if (typeof(T) == typeof(int))
        {
            PlayerPrefs.SetInt(key, Convert.ToInt32(value));
        }
        else if (typeof(T) == typeof(float))
        {
            PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
        }
        PlayerPrefs.Save();
    }

    private void LoadGameData()
    {
        playerData.soldClothPricePriceMultiplier = PlayerPrefs.GetInt(SoldClothPriceMultiplierKey, playerData.soldClothPricePriceMultiplier);
        playerData.sewMachinePriceMultiplier = PlayerPrefs.GetInt(SewMachinePriceMultiplierKey, playerData.sewMachinePriceMultiplier);
        playerData.coloringPotPriceMultiplier = PlayerPrefs.GetInt(ColoringPotPriceMultiplierKey, playerData.coloringPotPriceMultiplier);
        playerData.sewMachineExecutionTimeMultiplier = PlayerPrefs.GetFloat(SewMachineExecutionTimeMultiplierKey, playerData.sewMachineExecutionTimeMultiplier);
        playerData.sewMachineExecutionTimeBase = PlayerPrefs.GetFloat(SewMachineExecutionTimeBaseKey, playerData.sewMachineExecutionTimeBase);
        playerData.coloringPotExecutionTimeMultiplier = PlayerPrefs.GetFloat(ColoringPotExecutionTimeMultiplierKey, playerData.coloringPotExecutionTimeMultiplier);
        playerData.coloringPotExecutionTimeBase = PlayerPrefs.GetFloat(ColoringPotExecutionTimeBaseKey, playerData.coloringPotExecutionTimeBase);
    }
}
