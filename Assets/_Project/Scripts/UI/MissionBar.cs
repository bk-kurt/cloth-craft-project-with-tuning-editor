using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionBar : MonoBehaviour
{
    public List<Mission> currentMissions = new List<Mission>();
    public Action OnNewMissionGenerated; // for future use

    [SerializeField] private Mission baseMissionPrefab;
    [SerializeField] private Transform gridTransform;
    [SerializeField] private int maxMissionAmount;


    public void AddMission(ClothType type, int colorCode)
    {
        if (currentMissions.Count >= maxMissionAmount)
        {
            Debug.LogWarning("Reached to max amount of mission");
            return;
        }

        var newMission = Instantiate(baseMissionPrefab, gridTransform);
        newMission.InitializeWithMission(type, colorCode);
        currentMissions.Add(newMission);
        OnNewMissionGenerated?.Invoke();
    }


    public bool HasCompletedAnyMission(ClothType type, int colorCode)
    {
        Mission completedMission = null;

        for (int i = 0; i < currentMissions.Count; i++)
        {
            var checkedMission = currentMissions[i];
            if (type == checkedMission.type && colorCode == checkedMission.colorCode)
            {
                completedMission = checkedMission;
                currentMissions.Remove(completedMission);
                completedMission.Complete();
                break;
            }
        }

        if (currentMissions.Count < 1) // if no missions left, proceed to next day
        {
            GameManager.Instance.audioManager.PlayAudioClip(12);
            GameManager.Instance.stats.Day++;
        }

        if (!completedMission)
        {
            Debug.Log("No matching mission found, earning normal income");
        }

        return completedMission;
    }


    public void ClearMissions()
    {
        foreach (var currentMission in currentMissions)
        {
            Destroy(currentMission.gameObject);
        }
        
        currentMissions.Clear();
    }
}