using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class ColoringPot : Executer
{
    public int colorCode;
    public TimerCircle timerCircle;

    [SerializeField] private List<Material> liquidMaterials;
    [SerializeField] private MeshRenderer liquidMeshRenderer;
    [SerializeField] private ParticleSystem bubbleParticle;
    [SerializeField] private ParticleSystem oilParticle;
    [SerializeField] private ParticleSystem oilParticle2;
    [SerializeField] private List<Transform> mixingPoints;

    private Material _dyeMaterial;

    public override void Initialize()
    {
        base.Initialize();
        MatchVisualsWithColorCode();
    }

    private void MatchVisualsWithColorCode()
    {
        _dyeMaterial = liquidMeshRenderer.materials[1];
        _dyeMaterial.color = liquidMaterials[colorCode].color;

        var psMain = sparklesPs.main;
        psMain.startColor = liquidMaterials[colorCode].color;
    }

    protected override void SetAdjustedPriceAndExecutionTime()
    {
        base.SetAdjustedPriceAndExecutionTime();
        var preferences = GameManager.Instance.preferences;

        price = level* level * preferences.ColoringPotPriceMultiplier;
        executionTime = preferences.ColoringPotExecutionTimeBase +
                        preferences.ColoringPotExecutionTimeMultiplier * level * level;
    }

    public override void Execute(ExecutableObject executableObject)
    {
        base.Execute(executableObject);
        Colorize();
        timerCircle.SetTimer(executionTime);
    }

    public void Colorize()
    {
        Cloth clothToColor = objectToUse as Cloth;
        if (!clothToColor)
        {
            Debug.LogWarning("No Cloth found to colorize");
            return;
        }

        clothToColor.GetColored(executionTime, liquidMaterials[colorCode].color);
        GameManager.Instance.audioManager.PlayAudioClipWithLoop(3, executionTime, 0.5f, 0.5f);
        StartCoroutine(DoCircularMotion(dropTransform.gameObject, executionTime));
    }


    private IEnumerator DoCircularMotion(GameObject objectToDoCircularMotion, float duration)
    {
        bubbleParticle.Play();
        oilParticle.Play();
        var mainOil1 = oilParticle.main;
        mainOil1.startColor = liquidMaterials[colorCode].color;
        var mainOil2 = oilParticle2.main;
        mainOil2.startColor = liquidMaterials[colorCode].color;
        var trailModule = oilParticle2.trails;
        trailModule.colorOverLifetime = liquidMaterials[colorCode].color;


        objectToDoCircularMotion.transform.position = mixingPoints[0].position;
        Sequence seq = DOTween.Sequence();
        for (int i = 1; i <= 5; i++)
        {
            if (i == 5)
            {
                seq.Append(objectToDoCircularMotion.transform.DOMove(mixingPoints[0].position, 0.5f)
                    .SetEase(Ease.Linear));
            }
            else
            {
                seq.Append(objectToDoCircularMotion.transform.DOMove(mixingPoints[i].position, 0.5f)
                    .SetEase(Ease.Linear));
            }
        }

        seq.SetLoops(-1);
        yield return new WaitForSeconds(duration);
        bubbleParticle.Stop();
        oilParticle.Stop();
        seq.Kill();
        objectToDoCircularMotion.transform.position = mixingPoints[0].position;
        isExecuting = false; 
        isReadyForCollection = true;

        CompleteAndResetTheAppearanceOfUsedObject();
    }


    public override void Collect()
    {
        sparklesPs.Stop();
        
        timerCircle.Hide(true);
        Cloth coloredCloth = objectToUse as Cloth;
        Mission completedMission = null;
        var awaitingMissions = GameManager.Instance.UIManager.header.missionBar.currentMissions;
        foreach (var mission in awaitingMissions
                ) // check matches with any of current missions, if yes move towards.
        {
            if (mission.type == coloredCloth.type && mission.colorCode == coloredCloth.colorCode)
            {
                completedMission = mission;

                print("mission for " + mission.type + " with color code: " + mission.colorCode + " has completed!");
                break;
            }
        }


        objectToUse.Sell(completedMission);


        base.Collect();
        isReadyForCollection = false;
    }
}