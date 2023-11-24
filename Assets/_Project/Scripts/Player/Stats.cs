using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    public Action OnLevelChanged;
    public Action<int> OnDayChanged; // int passes the mission count for the day
    public Action OnXpChanged;
    public Action OnMoneyChanged;

    [SerializeField]
    private ExecuterPaymentStatus paymentStatus;

    private List<Executer> _potExecuters;
    private List<Executer> _sewExecuters;

    private void Start()
    {
        LoadGameData(); // Load saved game data at start
        
        // Null checks and initialization for executers
        if (GameManager.Instance?.sectionManager?.sections != null &&
            GameManager.Instance.sectionManager.sections.Count > 1)
        {
            GetExecutersToList();
        }

        OnMoneyChanged?.Invoke();
        OnLevelChanged?.Invoke();
        OnXpChanged?.Invoke();
    }

    private void GetExecutersToList()
    {
        _potExecuters = GameManager.Instance.sectionManager.sections[1].executers;
        _sewExecuters = GameManager.Instance.sectionManager.sections[0].executers;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Day++;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Money += 100;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerData.ResetToDefault();
            paymentStatus.ResetAllPaymentStatusToNone();
            SaveGameData();
        }
    }

    public int Day
    {
        get => playerData.day;
        set
        {
            if (playerData.day != value)
            {
                playerData.day = value;
                OnDayChanged?.Invoke(playerData.DayMissionCount(value));
                OnLevelChanged?.Invoke();
            }
        }
    }

    public int Money
    {
        get => playerData.money;
        set
        {
            if (playerData.money != value)
            {
                playerData.money = value;
                OnMoneyChanged?.Invoke();
            }
        }
    }

    public int PotLevelMax
    {
        get
        {
            GetExecutersToList();
            if (_potExecuters != null && _potExecuters.Count > 0)
            {
                List<Executer> availablePotExecuters = new List<Executer>();
                foreach (var potExecuter in _potExecuters)
                {
                    if (HasExecuterBeenPaid(potExecuter))
                    {
                        availablePotExecuters.Add(potExecuter);
                    }
                }

                if (availablePotExecuters.Count <= 0) return 0;
                availablePotExecuters.Sort((x, y) => y.level.CompareTo(x.level));
                return availablePotExecuters[0].level;
            }

            Debug.LogError("potExecuters is null or empty");
            return 0;
        }
    }

    public int SewLevelMax
    {
        get
        {
            GetExecutersToList();
            if (_sewExecuters != null && _sewExecuters.Count > 0)
            {
                List<Executer> availableSewExecuters = new List<Executer>();
                foreach (var sewExecuter in _sewExecuters)
                {
                    if (HasExecuterBeenPaid(sewExecuter))
                    {
                        availableSewExecuters.Add(sewExecuter);
                    }
                }

                if (availableSewExecuters.Count <= 0) return 0;
                availableSewExecuters.Sort((x, y) => y.level.CompareTo(x.level));
                return availableSewExecuters[0].level;
            }

            Debug.LogError("sewExecuters is null or empty");
            return 0;
        }
    }

    public bool HasExecuterBeenPaid(Executer executer)
    {
        return paymentStatus.GetPaymentStatus(executer);
    }

    public void PayForExecuter(Executer executer)
    {
        if (Money >= executer.price)
        {
            paymentStatus.SetPaymentStatus(executer, true);
            Money -= executer.price;
            GameManager.Instance.audioManager.PlayAudioClip(14);
        }
        else
        {
            Debug.LogWarning("Not enough money");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameData();
        }
    }

    private void SaveGameData()
    {
        paymentStatus.SaveData();
        PlayerPrefs.SetInt("PlayerDay", Day);
        PlayerPrefs.SetInt("PlayerMoney", Money);
        PlayerPrefs.Save(); // This line is important to save the data immediately
    }

    private void LoadGameData()
    {
        paymentStatus.LoadData();
        Day = PlayerPrefs.GetInt("PlayerDay", Day); // Use the current day as the default value
        Money = PlayerPrefs.GetInt("PlayerMoney", Money); // Use the current money as the default value
    }
}
