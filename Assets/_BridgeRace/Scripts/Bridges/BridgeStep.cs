using UnityEngine;

public class BridgeStep : MonoBehaviour, IBuildable
{
    [Header("References")]
    [SerializeField] private MeshRenderer stepRenderer;
    [SerializeField] private BoxCollider solidCollider;
    [SerializeField] private BoxCollider detectionCollider;

    [Header("Team Materials")]
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material yellowMaterial;

    private bool isBuilt;
    private TeamColor currentColor;


    private void Awake()
    {
        ResetStep();
    }


    public bool NeedsBuild(
        TeamColor builderColor)
    {
        if (!isBuilt)
        {
            return true;
        }

        return currentColor != builderColor;
    }


    public void BuildStep(
        TeamColor builderColor)
    {
        isBuilt = true;

        currentColor = builderColor;


        if (stepRenderer != null)
        {
            stepRenderer.enabled = true;

            stepRenderer.sharedMaterial =
                GetMaterial(builderColor);
        }


        if (solidCollider != null)
        {
            solidCollider.enabled = true;
        }


        if (detectionCollider != null)
        {
            detectionCollider.enabled = true;
        }
    }


    private Material GetMaterial(
        TeamColor color)
    {
        switch (color)
        {
            case TeamColor.Blue:
                return blueMaterial;

            case TeamColor.Red:
                return redMaterial;

            case TeamColor.Green:
                return greenMaterial;

            case TeamColor.Yellow:
                return yellowMaterial;
        }

        return blueMaterial;
    }


    private void ResetStep()
    {
        isBuilt = false;

        if (stepRenderer != null)
        {
            stepRenderer.enabled = false;
        }

        if (solidCollider != null)
        {
            solidCollider.enabled = false;
        }

        if (detectionCollider != null)
        {
            detectionCollider.enabled = true;
        }
    }
}