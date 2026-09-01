using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterStack : MonoBehaviour, IBrickModifierTarget
{

    [Header("References")]
    [SerializeField]
    private Transform stackPoint;

    [SerializeField]
    private BrickPoolManager brickPoolManager;

    [Header("Stack Settings")]
    [SerializeField]
    private float verticalSpacing = 0.19f;

    [Header("Collect Animation")]
    [SerializeField]
    private float collectDuration = 0.25f;

    [SerializeField]
    private float jumpPower = 0.5f;

    [Header("Drop Settings")]
    [SerializeField]
    private float dropDuration = 0.35f;

    [SerializeField]
    private float dropJumpPower = 0.6f;

    [SerializeField]
    private float dropMinDistance = 1.3f;

    [SerializeField]
    private float dropMaxDistance = 2.2f;

    [SerializeField]
    private float dropGroundOffset = 0.875f;

    private readonly List<Brick> collectedBricks =
        new List<Brick>();

    private CharacterBase ownerCharacter;

    public int BrickCount =>
        collectedBricks.Count;


    public int CurrentBrickCount =>
        BrickCount;

    private void Awake()
    {
        ownerCharacter =
            GetComponent<CharacterBase>();


        if (ownerCharacter == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterBase bulunamadı!"
            );
        }


        if (stackPoint == null)
        {
            Debug.LogError(
                gameObject.name +
                " için StackPoint atanmadı!"
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

        if (brick.TryGetComponent<Collider>(
                out Collider brickCollider))
        {
            brickCollider.enabled = false;
        }


        int brickIndex =
            collectedBricks.Count;


        collectedBricks.Add(
            brick
        );


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
        .SetEase(
            Ease.OutQuad
        );


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

    public int DropBricks(int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }


        if (collectedBricks.Count <= 0)
        {
            return 0;
        }


        int dropCount =
            Mathf.Min(
                amount,
                collectedBricks.Count
            );


        float startAngle =
            Random.Range(
                0f,
                360f
            );


        for (int i = 0;
             i < dropCount;
             i++)
        {
            int lastIndex =
                collectedBricks.Count - 1;


            Brick brickToDrop =
                collectedBricks[lastIndex];


            collectedBricks.RemoveAt(
                lastIndex
            );


            brickToDrop.transform.DOKill();


            brickToDrop.PrepareForDrop();


            brickToDrop.transform.SetParent(
                null,
                true
            );


            float angle =
                startAngle +
                (360f / dropCount) * i;


            float angleRad =
                angle * Mathf.Deg2Rad;


            Vector3 direction =
                new Vector3(
                    Mathf.Cos(angleRad),
                    0f,
                    Mathf.Sin(angleRad)
                );


            float distance =
                Random.Range(
                    dropMinDistance,
                    dropMaxDistance
                );


            Vector3 targetPosition =
                transform.position +
                direction * distance;


            targetPosition.y =
                transform.position.y -
                dropGroundOffset;


            brickToDrop.transform.DOJump(
                targetPosition,
                dropJumpPower,
                1,
                dropDuration
            )
            .SetEase(
                Ease.OutQuad
            )
            .OnComplete(
                brickToDrop.EnableCollection
            );


            brickToDrop.transform.DORotate(
                new Vector3(
                    0f,
                    Random.Range(
                        0f,
                        360f
                    ),
                    0f
                ),
                dropDuration
            );
        }


        EventManager.BrickDropped(
            ownerCharacter,
            BrickCount
        );


        return dropCount;
    }

    public void AddBricks(int amount)
    {
        if (amount <= 0)
        {
            return;
        }


        if (brickPoolManager == null)
        {
            Debug.LogError(
                gameObject.name +
                " için BrickPoolManager bulunamadı!"
            );

            return;
        }


        if (ownerCharacter == null)
        {
            Debug.LogError(
                gameObject.name +
                " için CharacterBase bulunamadı!"
            );

            return;
        }


        TeamColor characterColor =
            ownerCharacter.CharacterTeamColor;


        for (int i = 0;
             i < amount;
             i++)
        {
            Brick newBrick =
                brickPoolManager.GetBrickFromPool(
                    characterColor
                );


            // Pool boşaldıysa daha fazla ekleme.
            if (newBrick == null)
            {
                break;
            }


            newBrick.transform.position =
                transform.position +
                Vector3.up * 0.5f;


            newBrick.transform.rotation =
                Quaternion.identity;


            // Gate sistemi eskisi gibi normal AddBrick kullanıyor.
            AddBrick(
                newBrick
            );
        }
    }

    public void AddBonusBricks(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (brickPoolManager == null)
        {
            Debug.LogError(gameObject.name +" için BrickPoolManager bulunamadı!");

            return;
        }

        if (ownerCharacter == null)
        {
            Debug.LogError(gameObject.name +" için CharacterBase bulunamadı!");

            return;
        }


        TeamColor characterColor = ownerCharacter.CharacterTeamColor;


        for (int i = 0;i < amount;i++)
        {
            Brick newBrick = brickPoolManager.GetBrickFromPool(characterColor);

            if (newBrick == null)
            {
                break;
            }

            newBrick.transform.position = transform.position + Vector3.up * 0.5f;

            newBrick.transform.rotation = Quaternion.identity;

            AddBonusBrick(newBrick);
        }
    }

    private void AddBonusBrick(Brick brick)
    {
        if (brick == null)
        {
            return;
        }

        if (stackPoint == null)
        {
            return;
        }

        if (brick.TryGetComponent<Collider>(out Collider brickCollider))
        {
            brickCollider.enabled = false;
        }

        int brickIndex = collectedBricks.Count;

        collectedBricks.Add(brick);

        Vector3 targetLocalPosition =new Vector3(0f,brickIndex * verticalSpacing,0f);

        brick.transform.DOKill();

        brick.transform.SetParent(stackPoint,true);

        brick.transform.DOLocalJump(targetLocalPosition,jumpPower,1,collectDuration).SetEase(Ease.OutQuad);

        brick.transform.DOLocalRotate(Vector3.zero,collectDuration);

    }

    public void RemoveBricks(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (brickPoolManager == null)
        {
            return;
        }

        if (collectedBricks.Count <= 0)
        {
            return;
        }

        int removeCount = Mathf.Min(amount,collectedBricks.Count);

        for (int i = 0;i < removeCount;i++)
        {
            int lastIndex = collectedBricks.Count - 1;


            Brick brickToRemove = collectedBricks[lastIndex];

            collectedBricks.RemoveAt(lastIndex);

            brickToRemove.transform.DOKill();

            brickPoolManager.ReturnBrickToPool(brickToRemove);
        }


        EventManager.BrickSpent(ownerCharacter,BrickCount);
    }
}