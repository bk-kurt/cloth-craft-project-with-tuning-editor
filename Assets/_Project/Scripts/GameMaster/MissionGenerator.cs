using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionGenerator : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.stats.OnDayChanged += GenerateRandomMissionForPlayerLevel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateRandomMissionForPlayerLevel(2);
        }
    }

    private void GenerateRandomMissionForPlayerLevel(int missionAmountForLevel)
    {
        var missionBar = GameManager.Instance.UIManager.header.missionBar;
        missionBar.ClearMissions();

        for (int i = 0; i < missionAmountForLevel; i++)
        {
            var enumList = Enum.GetValues(typeof(ClothType)).Cast<ClothType>().ToList();
            var stats = GameManager.Instance.stats;
            var sL = stats.SewLevelMax / 3;
            var pL = stats.PotLevelMax / 2;

            ClothType typeMax = enumList[Random.Range(0, sL + 1)];
            int colorCodeMax = Random.Range(0, pL + 1);

            missionBar.AddMission(typeMax, colorCodeMax);
        }
    }


    private void OnDisable()
    {
        GameManager.Instance.stats.OnDayChanged -= GenerateRandomMissionForPlayerLevel;
    }
}