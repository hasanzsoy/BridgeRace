using DG.Tweening;
using UnityEngine;


public class Brick : MonoBehaviour, ICollectable
{
    [Header("Brick Settings")]
    [SerializeField] private TeamColor brickColor;

    private Collider brickCollider;

    private BrickSpawner ownerSpawner;

    private int ownerSlotIndex = -1;

    private bool isCollected;


    public TeamColor CollectableColor =>
        brickColor;


    private void Awake()
    {
        brickCollider =
            GetComponent<Collider>();
    }


    private void OnEnable()
    {
        isCollected = false;

        if (brickCollider != null)
        {
            brickCollider.enabled = true;
        }
    }


    private void OnDisable()
    {
        transform.DOKill();
    }

    public void SetSpawner(
        BrickSpawner spawner,
        int slotIndex)
    {
        ownerSpawner = spawner;

        ownerSlotIndex = slotIndex;
    }


    public void Collect(
        CharacterBase collector)
    {
        if (isCollected)
        {
            return;
        }


        if (collector == null)
        {
            return;
        }


        if (collector.CharacterTeamColor !=
            brickColor)
        {
            return;
        }


        if (!collector.TryGetComponent<CharacterStack>(
                out CharacterStack characterStack))
        {
            return;
        }


        isCollected = true;


        if (brickCollider != null)
        {
            brickCollider.enabled = false;
        }


        BrickSpawner previousSpawner =
            ownerSpawner;

        int previousSlotIndex =
            ownerSlotIndex;


        ownerSpawner = null;

        ownerSlotIndex = -1;


        characterStack.AddBrick(this);


        if (previousSpawner != null)
        {
            previousSpawner.BrickCollected(
                brickColor,
                previousSlotIndex
            );
        }
    }
}