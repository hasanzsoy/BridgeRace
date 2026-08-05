using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class ColourSkinChanger : MonoBehaviour
{
    public Material skinMaterial;

    public Slider[] colourSliders = new Slider[3];
    public Image skinColourImage;

    float rCanal = 1f;
    float gCanal = 1f;
    float bCanal = 1f;

    public void SetNewColourCanal(int sliderNumber)
    {
        switch (sliderNumber)
        {
            case 0:
                rCanal = colourSliders[0].value;
                break;
            
            case 1:
                gCanal = colourSliders[1].value;
                break;
            
            case 2:
                bCanal = colourSliders[2].value;
                break;
        }

        skinColourImage.color = new Color(rCanal, gCanal, bCanal, 1);
        skinMaterial.color = new Color(rCanal, gCanal, bCanal, 1);

        Actions.instance.OnSetSkinColourEvent(new Color(rCanal, gCanal, bCanal, 1));
    }

    public Material CreateNewSkinMaterial(string newCharacterPrefabName)
    {
        Material newMaterial = new Material(skinMaterial);
        newMaterial.color = new Color(rCanal, gCanal, bCanal, 1);

        AssetDatabase.CreateAsset(newMaterial, "Assets/HYPERCASUAL - Stickman Customization/MaterialsCreator/" + "Skin_" + newCharacterPrefabName+ ".mat");

        return newMaterial;
    }

    private void OnDestroy()
    {
        skinMaterial.color = Color.white;
    }
}
