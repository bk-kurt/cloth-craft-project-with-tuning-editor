using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Canvas canvas;
    [SerializeField] public Header header;
    [SerializeField] public Image centerImage;
    [SerializeField] public SectionButton sectionButton;

    [Header("Configuration")]
    [SerializeField] private int moneyImageValue;
    [SerializeField] private string moneyImagePoolKey = "Money_Image";
    [SerializeField] private float spawnDelay = 0.1f;
    [SerializeField] private float moveDuration = 0.75f;
    [SerializeField] private AudioClip moneyAudioClip;

    private const int AudioMoneyClipId = 6; // Example of a named constant for audio clip ID.

    private GameManager gameManager;
    private AudioManager audioManager;
    private Stats stats;

    private void Awake()
    {
        // Cache references to avoid repeated calls to 'Instance'
        gameManager = GameManager.Instance;
        audioManager = gameManager.audioManager;
        stats = gameManager.stats;
    }

    private void OnEnable()
    {
        stats.OnMoneyChanged += UpdateUI;
        stats.OnLevelChanged += UpdateUI;
    }

    private void OnDisable()
    {
        stats.OnMoneyChanged -= UpdateUI;
        stats.OnLevelChanged -= UpdateUI;
    }

    public void SpawnMoney(int value, Vector3 worldPosition)
    {
        StartCoroutine(MoneySpawnRoutine(value, worldPosition));
    }

    private IEnumerator MoneySpawnRoutine(int value, Vector3 worldPosition)
    {
        var moneyImageCountToSpawn = value / moneyImageValue;
        for (int i = 0; i < moneyImageCountToSpawn; i++)
        {
            GameObject obj = ObjectPool.Instance.GetObjectFromPool(moneyImagePoolKey);
            if (obj == null)
            {
                Debug.LogWarning($"Object pool returned null for key {moneyImagePoolKey}.");
                yield break;
            }
            Image newMoney = SetupNewMoneyImage(obj, worldPosition);
            AnimateMoneyImage(newMoney, obj);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Image SetupNewMoneyImage(GameObject obj, Vector3 worldPosition)
    {
        Image newMoney = obj.GetComponent<Image>();
        newMoney.transform.SetParent(canvas.transform, false);
        newMoney.transform.position = worldPosition;
        newMoney.rectTransform.pivot = header.moneyImage.rectTransform.pivot;
        return newMoney;
    }

    private void AnimateMoneyImage(Image moneyImage, GameObject pooledObject)
    {
        moneyImage.rectTransform.DOMove(header.moneyImage.transform.position, moveDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                stats.Money += moneyImageValue;
                ObjectPool.Instance.ReturnObjectToPool(moneyImagePoolKey, pooledObject);
                audioManager.PlayAudioClip(AudioMoneyClipId); // This should ideally be replaced with a direct reference to the clip if possible.
            });
    }

    private void UpdateUI()
    {
        header.moneyText.text = FormatMoney(stats.Money);
        header.dayText.text = $"DAY {stats.Day}!";
    }

    private string FormatMoney(int money)
    {
        return (money / 1000.0).ToString("0.#") + "k";
    }
}
