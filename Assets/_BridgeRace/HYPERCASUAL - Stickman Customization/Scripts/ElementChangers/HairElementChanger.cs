using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairElementChanger : ElementChanger
{
    public override void Start()
    {
        base.Start();
        ChangeHairElements();
    }

    public override void GetNextButton()
    {
        base.GetNextButton();
        ChangeHairElements();
    }

    public override void GetPreviousButton()
    {
        base.GetPreviousButton();
        ChangeHairElements();
    }

    private void ChangeHairElements()
    {
        if (elementIndex == meshElements.Count)
            skinnedMeshRenderer.enabled = false;
        else
        {
            if (skinnedMeshRenderer.enabled == false) skinnedMeshRenderer.enabled = true;

            if (isMeshChange) skinnedMeshRenderer.sharedMesh = meshElements[elementIndex];

            Material[] mats = skinnedMeshRenderer.materials;
            mats[0].mainTexture = spriteChanger[elementIndex];
            skinnedMeshRenderer.materials = mats;
        }
    }

    public Material GetHairMaterial()
    {
        Material[] mats = skinnedMeshRenderer.materials;
        if (elementIndex != meshElements.Count)
        {
            if (mats[0] != null) return mats[0];
            else return null;
        }

        return null;
    }
}
