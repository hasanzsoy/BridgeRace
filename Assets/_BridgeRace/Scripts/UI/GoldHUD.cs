using TMPro;
using UnityEngine;

public class GoldHUD : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField]
    private TMP_Text goldText;


    private void OnEnable()
    {
        EventManager.OnGoldChanged +=
            UpdateGoldText;


        RefreshGold();
    }


    private void OnDisable()
    {
        EventManager.OnGoldChanged -=
            UpdateGoldText;
    }


    private void RefreshGold()
    {
        int currentGold =
            SaveManager.LoadGold();


        UpdateGoldText(
            currentGold
        );
    }


    private void UpdateGoldText(
        int totalGold)
    {
        if (goldText == null)
        {
            return;
        }


        goldText.text =
            totalGold.ToString();
    }
}