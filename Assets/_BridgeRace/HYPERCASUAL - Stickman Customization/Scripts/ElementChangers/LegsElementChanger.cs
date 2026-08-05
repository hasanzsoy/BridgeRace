using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsElementChanger : ElementChanger
{
    public Mesh emptyMesh;

    private List<int> _doubleMaterial = new List<int>() { 1, 4, 5, 10 };

    private List<int> _materialRevertIndex = new List<int>() { 1, 10 };

    public override void Start()
    {
        base.Start();

        Actions.instance.onSetSkinColour += SetSkinColour;

        ChangeLegsElement();
    }

    public override void GetNextButton()
    {
        base.GetNextButton();
        ChangeLegsElement();
    }

    public override void GetPreviousButton()
    {
        base.GetPreviousButton();
        ChangeLegsElement();
    }

    public Material GetLegsMaterial(out bool isReverse, out bool isTwoElements)
    {
        Material[] mats = skinnedMeshRenderer.materials;
        if (elementIndex != meshElements.Count) {
            if (ElementDoubleMaterialChecker())
            {
                isTwoElements = true;
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
            else
            {
                isReverse = isTwoElements = false;
                if (mats[0] != null) return mats[0];
                else return null;
            }
        }
        else
        {
            isReverse = isTwoElements = false;
            return null;
        }
    }

    private void ChangeLegsElement()
    {
        if (elementIndex == meshElements.Count)
        {
            if (isMeshChange) skinnedMeshRenderer.sharedMesh = emptyMesh;

            Material[] mats = skinnedMeshRenderer.materials;
            mats[0] = standartSecondMaterial;
            mats[1] = null;
            skinnedMeshRenderer.materials = mats;
        }
        else
        {
            if (isMeshChange) skinnedMeshRenderer.sharedMesh = meshElements[elementIndex];

            Material[] mats = skinnedMeshRenderer.materials;
            if (ElementDoubleMaterialChecker())
            {
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
            }
            else
            {
                Debug.Log(123);
                mats[0] = standartFirstMaterial;
                mats[0].mainTexture = spriteChanger[elementIndex];
                mats[1] = null;
            }

            skinnedMeshRenderer.materials = mats;
        }
    }


    private bool ElementDoubleMaterialChecker()
    {
        for (int i = 0; i < _doubleMaterial.Count; i++)
        {
            if (elementIndex == _doubleMaterial[i])
            {
                return true;
            }
        }
        return false;
    }

    private bool ElementReverseChecker()
    {
        for (int i = 0; i < _materialRevertIndex.Count; i++)
        {
            if (elementIndex == _materialRevertIndex[i])
            {
                return true;
            }
        }
        return false;
    }

    private void SetSkinColour(Color newColor)
    {
        Material[] mats = skinnedMeshRenderer.materials;

        if (ElementDoubleMaterialChecker()) 
        { 
            if (ElementReverseChecker())
                mats[0].color = newColor;
            else mats[1].color = newColor;
        }
        else if(elementIndex == meshElements.Count)
        {
            mats[0].color = newColor;
        }

        skinnedMeshRenderer.materials = mats;
    }

    private void OnDestroy()
    {
        Actions.instance.onSetSkinColour -= SetSkinColour;
    }
}
