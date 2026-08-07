using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterStack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform stackPoint;
    [SerializeField] private BrickPoolManager brickPoolManager;

    [Header("Stack Settings")]
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

        if (brickPoolManager == null)
        {
            brickPoolManager =
                FindFirstObjectByType<BrickPoolManager>();
        }

        if (brickPoolManager == null)
        {
            Debug.LogError(
                gameObject.name +
                " için BrickPoolManager bulunamadı!"
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

        int brickIndex =
            collectedBricks.Count;

        collectedBricks.Add(brick);


        Vector3 targetLocalPosition =
            new Vector3(
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


    public bool TrySpendBrick()
    {
        if (collectedBricks.Count <= 0)
        {
            return false;
        }

        if (brickPoolManager == null)
        {
            return false;
        }


        int lastIndex =
            collectedBricks.Count - 1;


        Brick brickToSpend =
            collectedBricks[lastIndex];


        collectedBricks.RemoveAt(
            lastIndex
        );


        brickToSpend.transform.DOKill();


        brickPoolManager.ReturnBrickToPool(
            brickToSpend
        );


        EventManager.BrickSpent(
            ownerCharacter,
            BrickCount
        );


        return true;
    }
}