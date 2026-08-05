using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public static Actions instance;

    private void Awake()
    {
        if (instance == null)
            instance = this; 
        

        DontDestroyOnLoad(gameObject);
    }


    public event System.Action<Color> onSetSkinColour;
    public void OnSetSkinColourEvent(Color newColor)
    {
        if (onSetSkinColour != null)
            onSetSkinColour(newColor);
    }
}
