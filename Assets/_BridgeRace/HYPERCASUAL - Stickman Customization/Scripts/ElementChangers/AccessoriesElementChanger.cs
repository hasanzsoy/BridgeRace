using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoriesElementChanger : ElementChanger
{
    public override void Start()
    {
        base.Start();
        ChangeAccessoryElements();
    }

    public override void GetNextButton()
    {
        base.GetNextButton();
        ChangeAccessoryElements();
    }

    public override void GetPreviousButton()
    {
        base.GetPreviousButton();
        ChangeAccessoryElements();
    }

    public Material GetAccessoryMaterial()
    {
        if (elementIndex != meshElements.Count)
        {
            Material[] mats = skinnedMeshRenderer.materials;

            if (mats[0] != null) return mats[0];
            else return null;
        }

        return null;
    }

    private void ChangeAccessoryElements()
    {
        if (elementIndex == meshElements.Count)
        {
            //if (isMeshChange) skinnedMeshRenderer.sharedMesh = null;

            //Material[] mats = skinnedMeshRenderer.materials;
            //mats[0] = null;
            //skinnedMeshRenderer.materials = mats;
            skinnedMeshRenderer.enabled = false;
        }
        else
        {
            //if (isMeshChange) skinnedMeshRenderer.sharedMesh = meshElements[elementIndex];

            //Material[] mats = skinnedMeshRenderer.materials;
            //if (mats[0] == null) mats[0] = standartFirstMaterial;

            //mats[0].mainTexture = spriteChanger[elementIndex];

            //skinnedMeshRenderer.materials = mats;

            if (skinnedMeshRenderer.enabled == false) skinnedMeshRenderer.enabled = true;

            if (isMeshChange) skinnedMeshRenderer.sharedMesh = meshElements[elementIndex];

            Material[] mats = skinnedMeshRenderer.materials;
            mats[0].mainTexture = spriteChanger[elementIndex];
            skinnedMeshRenderer.materials = mats;
        }
    }
}
