using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterStack : MonoBehaviour
{
    [Header("Stack Settings")]
    [SerializeField] private Transform stackPoint;
    [SerializeField] private float verticalSpacing = 0.28f;

    [Header("Collect Animation")]
    [SerializeField] private float collectDuration = 0.25f;
    [SerializeField] private float jumpPower = 0.5f;

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


        Vector3 targetLocalPosition = new Vector3(
            0f,
            brickIndex * verticalSpacing,
            0f
        );


        brick.transform.DOKill();


        brick.transform.SetParent(
            stackPoint,
            true
        );


        brick.transform.DOLocalJump(
            targetLocalPosition,
            jumpPower,
            1,
            collectDuration
        )
        .SetEase(Ease.OutQuad);


        brick.transform.DOLocalRotate(
            Vector3.zero,
            collectDuration
        );


        EventManager.BrickCollected(
            ownerCharacter,
            BrickCount
        );
    }
}