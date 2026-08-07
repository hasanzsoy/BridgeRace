using System.Collections.Generic;
using UnityEngine;

public class CharacterStack : MonoBehaviour
{
    [Header("Stack Settings")]
    [SerializeField] private Transform stackPoint;
    [SerializeField] private float verticalSpacing = 0.28f;

    private List<Brick> collectedBricks = new List<Brick>();

    private CharacterBase ownerCharacter;

    public int BrickCount => collectedBricks.Count;


    private void Awake()
    {
        ownerCharacter = GetComponent<CharacterBase>();

        if (stackPoint == null)
        {
            Debug.LogError(
                gameObject.name + " için StackPoint atanmadı!"
            );
        }
    }


    public void AddBrick(Brick brick)
    {
        if (brick == null)
        {
            return;
        }

        if (stackPoint == null)
        {
            return;
        }


        int brickIndex = collectedBricks.Count;

        collectedBricks.Add(brick);


        brick.transform.SetParent(stackPoint);

        brick.transform.localRotation =
            Quaternion.identity;

        brick.transform.localPosition =
            new Vector3(
                0f,
                brickIndex * verticalSpacing,
                0f
            );


        EventManager.BrickCollected(
            ownerCharacter,
            BrickCount
        );
    }
}