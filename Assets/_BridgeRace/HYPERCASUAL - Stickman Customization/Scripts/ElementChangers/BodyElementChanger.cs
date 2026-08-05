using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyElementChanger : ElementChanger
{
    public Mesh emptyModel;

    private List<int> _materialRevertIndex = new List<int>() { 4};

    public override void Start()
    {
        base.Start();

        Actions.instance.onSetSkinColour += SetSkinColour;

        ChangeBodyElement();
    }

    public override void GetNextButton()
    {
        base.GetNextButton();
        ChangeBodyElement();
    }

    public override void GetPreviousButton()
    {
        base.GetPreviousButton();
        ChangeBodyElement();
    }

    public Material GetBodyMaterial(out bool isReverse)
    {
        if (elementIndex != meshElements.Count)
        {
            Material[] mats = skinnedMeshRenderer.materials;
            if (ElementReverseChecker())
            {
                isReverse = true;
                if (mats[1] != null) return mats[1];
                else return null;
            }
            else
            {
                isReverse = false;
                if (mats[0] != null) return mats[0];
                else return null;
            }
        }

        isReverse = false;
        return null;
    }

    private void ChangeBodyElement()
    {
        if (elementIndex == meshElements.Count)
        {
            if (isMeshChange) skinnedMeshRenderer.sharedMesh = emptyModel;

            Material[] mats = skinnedMeshRenderer.materials;
            mats[0] = standartSecondMaterial;
            mats[1] = null;
            skinnedMeshRenderer.materials = mats;
        }
        else
        {
            if (isMeshChange) skinnedMeshRenderer.sharedMesh = meshElements[elementIndex];

            Material[] mats = skinnedMeshRenderer.materials;

            if (ElementReverseChecker())
            {
                mats[0] = standartSecondMaterial;

                mats[1] = standartFirstMaterial;
                mats[1].mainTexture = spriteChanger[elementIndex];
            }
            else
            {
                mats[0] = standartFirstMaterial;
                mats[0].mainTexture = spriteChanger[elementIndex];

                mats[1] = standartSecondMaterial;
            }


            skinnedMeshRenderer.materials = mats;
        }
    }

    private bool ElementReverseChecker()
    {
        for(int i = 0; i < _materialRevertIndex.Count; i++)
        {
            if(elementIndex == _materialRevertIndex[i])
            {
                return true;
            }
        }
        return false;
    }

    private void SetSkinColour(Color newColor)
    {
        Material[] mats = skinnedMeshRenderer.materials;
        if (elementIndex != meshElements.Count)
        {
            if (ElementReverseChecker())
                mats[0].color = newColor;
            else mats[1].color = newColor;
        }
        else mats[0].color = newColor;

        skinnedMeshRenderer.materials = mats;
    }

    private void OnDestroy()
    {
        Actions.instance.onSetSkinColour -= SetSkinColour;
    }
}
