using TMPro;
using UnityEngine;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("State Texts")]

    [SerializeField]
    private TMP_Text gameSoundStateText;

    [SerializeField]
    private TMP_Text brickSoundStateText;

    [SerializeField]
    private TMP_Text bridgeSoundStateText;

    private void OnEnable()
    {
        EventManager.OnGameSoundChanged += UpdateGameSoundText;

        EventManager.OnBrickSoundChanged += UpdateBrickSoundText;

        EventManager.OnBridgeSoundChanged += UpdateBridgeSoundText;

        RefreshUI();
    }

    private void OnDisable()
    {
        EventManager.OnGameSoundChanged -= UpdateGameSoundText;


        EventManager.OnBrickSoundChanged -= UpdateBrickSoundText;


        EventManager.OnBridgeSoundChanged -= UpdateBridgeSoundText;
    }

    public void ToggleGameSound()
    {
        AudioSettings.ToggleGameSound();
    }

    public void ToggleBrickSound()
    {
        AudioSettings.ToggleBrickSound();
    }

    public void ToggleBridgeSound()
    {
        AudioSettings.ToggleBridgeSound();
    }
    private void RefreshUI()
    {
        UpdateGameSoundText(AudioSettings.GameSoundEnabled);


        UpdateBrickSoundText(AudioSettings.BrickSoundEnabled);


        UpdateBridgeSoundText(AudioSettings.BridgeSoundEnabled);
    }

    private void UpdateGameSoundText(bool enabled)
    {
        SetStateText(gameSoundStateText,enabled);
    }

    private void UpdateBrickSoundText(bool enabled)
    {
        SetStateText(brickSoundStateText,enabled);
    }

    private void UpdateBridgeSoundText(bool enabled)
    {
        SetStateText(bridgeSoundStateText,enabled);
    }
   private void SetStateText(TMP_Text text,bool enabled)
    {
        if (text == null)
        {
            return;
        }

        text.text =enabled? "ON": "OFF";
    }
}