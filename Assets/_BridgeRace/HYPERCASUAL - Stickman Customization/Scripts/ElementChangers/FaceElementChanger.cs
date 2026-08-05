using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceElementChanger : ElementChanger
{
    public override void Start()
    {
        base.Start();
        ChangeHeadElements();

        Actions.instance.onSetSkinColour += SetSkinColour;
    }

    public override void GetNextButton()
    {
        base.GetNextButton();
        ChangeHeadElements();
    }

    public override void GetPreviousButton()
    {
        base.GetPreviousButton();
        ChangeHeadElements();
    }

    public Material GetFaceMaterial()
    {
        Material[] mats = skinnedMeshRenderer.materials;

        if (elementIndex != spriteChanger.Count)
        {
            if (mats[1] != null) return mats[1];
            else return null;
        }

        return null;
    }

    private void ChangeHeadElements()
    {
        Material[] mats = skinnedMeshRenderer.materials;
        //if (mats[0] == null) mats[0] = standartFirstMaterial;

        //if (mats[1] == null) mats[1] = standartSecondMaterial;

        if (elementIndex == spriteChanger.Count)
        {
            //mats[1] = null;
            mats[1].mainTexture = null;
            mats[1].color = Color.clear;
        }
        else
        {
            if (mats[1] == null) mats[1] = standartSecondMaterial;
            mats[1].mainTexture = spriteChanger[elementIndex];
            mats[1].color = Color.white;
        }

        skinnedMeshRenderer.materials = mats;
    }

    private void SetSkinColour(Color newColor)
    {
        Debug.Log("SetSkinColour");
        Material[] mats = skinnedMeshRenderer.materials;
        mats[0].color = newColor;
        skinnedMeshRenderer.materials = mats;
    }

    private void OnDestroy()
    {
        Actions.instance.onSetSkinColour -= SetSkinColour;
    }
}