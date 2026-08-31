using UnityEngine;

public static class AudioSettings
{
    private const string GameSoundKey ="GameSoundEnabled";

    private const string BrickSoundKey ="BrickSoundEnabled";

    private const string BridgeSoundKey ="BridgeSoundEnabled";

    public static bool GameSoundEnabled =>PlayerPrefs.GetInt(GameSoundKey,1) == 1;

    public static bool BrickSoundEnabled =>PlayerPrefs.GetInt(BrickSoundKey,1) == 1;

    public static bool BridgeSoundEnabled =>PlayerPrefs.GetInt(BridgeSoundKey,1) == 1;

    public static void SetGameSound(bool enabled)
    {
        SaveBool(GameSoundKey,enabled);


        EventManager.GameSoundChanged(enabled);
    }


    public static bool ToggleGameSound()
    {
        bool newValue =!GameSoundEnabled;

        SetGameSound(newValue);

        return newValue;
    }

    public static void SetBrickSound(bool enabled)
    {
        SaveBool(BrickSoundKey,enabled);

        EventManager.BrickSoundChanged(enabled);
    }

    public static bool ToggleBrickSound()
    {
        bool newValue =!BrickSoundEnabled;


        SetBrickSound(newValue);

        return newValue;
    }

    public static void SetBridgeSound(bool enabled)
    {
        SaveBool(BridgeSoundKey,enabled);

        EventManager.BridgeSoundChanged(enabled);
    }


    public static bool ToggleBridgeSound()
    {
        bool newValue =!BridgeSoundEnabled;

        SetBridgeSound(newValue);
        return newValue;
    }

    private static void SaveBool(string key,bool value)
    {
        PlayerPrefs.SetInt(key,value ? 1 : 0);
        PlayerPrefs.Save();
    }
}