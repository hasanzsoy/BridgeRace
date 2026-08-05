using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ElementChanger : MonoBehaviour
{
    public Material standartFirstMaterial;
    public Material standartSecondMaterial;

    public Text numberText;

    public bool isCanAbsence = false;
    public bool isMeshChange = true;

    public SkinnedMeshRenderer skinnedMeshRenderer;

    public List<Mesh> meshElements = new List<Mesh>();

    [Space(10)]
    public List<Texture> spriteChanger = new List<Texture>();

    [Space(10)]
    //private GameObject _checkedObject;

    public int elementIndex = 0;

    public virtual void Start()
    {
        if (isCanAbsence) elementIndex = meshElements.Count;
        else elementIndex = meshElements.Count - 1;

        UpdateNumberText();
    }

    public virtual void GetNextButton()
    {
        elementIndex++;

        if (isCanAbsence && (elementIndex > meshElements.Count))
        {
            elementIndex = 0;
        }
        else if (!isCanAbsence && (elementIndex >= meshElements.Count))
        {
            elementIndex = 0;
        }

        //SetElement();
        UpdateNumberText();
    }

    public virtual void GetPreviousButton()
    {
        elementIndex--;
        if (elementIndex < 0) {
            if (isCanAbsence)
            {
                elementIndex = meshElements.Count;
            }
            else
            {
                elementIndex = meshElements.Count - 1;
            }
        }

        //SetElement();
        UpdateNumberText();
    }

    //private void SetElement()
    //{
    //    if (elementNumber == meshElements.Count)
    //    {
    //        if(isMeshChange) skinnedMeshRenderer.sharedMesh = null;

    //        Material[] mats = skinnedMeshRenderer.materials;
    //        mats[0] = null;
    //        if(mats.Length == 2) mats[1] = null;
    //        skinnedMeshRenderer.materials = mats;
    //    }
    //    else
    //    {
    //        if (isMeshChange) skinnedMeshRenderer.sharedMesh = meshElements[elementNumber];


    //        Material[] mats = skinnedMeshRenderer.materials;
    //        if (mats[0] == null) mats[0] = standartFirstMaterial;

    //        mats[0].mainTexture = spriteChanger[elementNumber];

    //        if (secondSpriteChanger.Count > 0)
    //        {
    //            if (mats[1] == null) mats[1] = standartSecondMaterial;

    //            if (secondSpriteChanger[elementNumber] != null) {
    //                mats[1].mainTexture = secondSpriteChanger[elementNumber];
    //            }
    //            else
    //            {
    //                mats[1].mainTexture = standartSecondMaterial;
    //            }
    //        }

    //        skinnedMeshRenderer.materials = mats;
    //    }
    //}

    private void UpdateNumberText()
    {
        int index = elementIndex + 1;

        if ((isCanAbsence && elementIndex == meshElements.Count) || 
            (!isCanAbsence && elementIndex == meshElements.Count - 1)) 
            index = 0;


        numberText.text = index.ToString();
    }
}
