using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text levelText;

    private const string LevelKey = "CurrentLevel";

    private int currentLevel;


    private void Awake()
    {
        LoadLevel();
        UpdateLevelText();
    }


    private void LoadLevel()
    {
        // Daha önce kayıt yoksa oyun
        // LEVEL 1'den başlar.
        currentLevel =
            PlayerPrefs.GetInt(
                LevelKey,
                1
            );
    }


    private void UpdateLevelText()
    {
        if (levelText == null)
        {
            return;
        }


        levelText.text =
            "LEVEL " + currentLevel;
    }


    public void CompleteLevel()
    {
        currentLevel++;


        PlayerPrefs.SetInt(
            LevelKey,
            currentLevel
        );


        PlayerPrefs.Save();


        Debug.Log(
            "Yeni Level kaydedildi: " +
            currentLevel
        );
    }


    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}