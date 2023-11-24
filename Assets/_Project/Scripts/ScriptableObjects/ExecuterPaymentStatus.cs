using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ExecuterPaymentStatus", menuName = "ScriptableObjects/ExecuterPaymentStatus", order = 1)]
public class ExecuterPaymentStatus : ScriptableObject
{
    public List<Executer> Executers;
    public List<bool> PaymentStatus;

    private const string PlayerPrefsKey = "ExecuterPaymentStatusData";

    public void SetPaymentStatus(Executer executer, bool paid)
    {
        int index = Executers.IndexOf(executer);
        if (index < 0)
        {
            Executers.Add(executer);
            PaymentStatus.Add(paid);
        }
        else
        {
            PaymentStatus[index] = paid;
        }
    }
    
    
    public void ResetAllPaymentStatusToNone()
    {
        foreach (var executer in Executers)
        {
            SetPaymentStatus(executer,false);
            GameManager.Instance.stats.OnMoneyChanged?.Invoke(); // money was not refunded but invoke for UI.
        }
    }

    public bool GetPaymentStatus(Executer executer)
    {
        int index = Executers.IndexOf(executer);
        return index >= 0 ? PaymentStatus[index] : false;
    }

    [Serializable]
    private class SerializableData
    {
        public List<string> ExecuterNames;
        public List<bool> PaymentStatus;
    }

    public void SaveData()
    {
        var data = new SerializableData
        {
            ExecuterNames = Executers.Select(e => e.name).ToList(),
            PaymentStatus = new List<bool>(PaymentStatus)
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            var data = JsonUtility.FromJson<SerializableData>(json);

            Executers = data.ExecuterNames.Select(name => GameObject.Find(name).GetComponent<Executer>()).ToList();
            PaymentStatus = new List<bool>(data.PaymentStatus);
        }
        else
        {
            // Handle case where no data was previously saved
            Executers = new List<Executer>();
            PaymentStatus = new List<bool>();
        }
    }
}