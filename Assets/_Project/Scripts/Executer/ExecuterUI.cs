using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExecuterUI : MonoBehaviour
{
    [SerializeField] private Image lockImage;
    [SerializeField] private TextMeshProUGUI levelText;

    public void ShowStatus(int level, bool hasBeenPaid, int price)
    {
        
        if (level>GameManager.Instance.stats.Day)
        {
            lockImage.enabled = true;
            lockImage.raycastTarget = false;
            levelText.text = "Level \n" + level;
        }
        else if (hasBeenPaid || level<=1)
        {
            lockImage.enabled = false;
            levelText.text = "";
        }
        else
        {
            lockImage.enabled = true;
            lockImage.raycastTarget = true;
            levelText.text = price.ToString();
        }
    }
}