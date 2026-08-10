using UnityEngine;

public class AIController : CharacterBase
{
    private enum AIState
    {
        CollectingStart,
        MovingToBridge1,
        CrossingBridge1,

        CollectingMiddle,
        MovingToBridge2,
        CrossingBridge2,

        MovingToFinish,
        Finished
    }


    [Header("AI References")]
    [SerializeField] private CharacterStack characterStack;

    [Header("Brick Areas")]
    [SerializeField] private BoxCollider startBrickArea;
    [SerializeField] private BoxCollider middleBrickArea;

    [Header("Bridge 1 Route")]
    [SerializeField] private Transform bridge1Start;
    [SerializeField] private Transform bridge1End;

    [Header("Bridge 2 Route")]
    [SerializeField] private Transform bridge2Start;
    [SerializeField] private Transform bridge2End;

    [Header("Finish")]
    [SerializeField] private Transform finishTarget;

    [Header("AI Settings")]
    [SerializeField] private int bricksNeededForBridge = 20;
    [SerializeField] private float pointReachedDistance = 0.8f;
    [SerializeField] private float brickSearchInterval = 0.25f;


    private AIState currentState =
        AIState.CollectingStart;

    private Brick targetBrick;

    private float nextBrickSearchTime;


    protected override void Awake()
    {
        base.Awake();

        if (characterStack == null)
        {
            characterStack =
                GetComponent<CharacterStack>();
        }

        if (characterStack == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterStack bulunamadı!"
            );
        }
    }


    private void OnEnable()
    {
        EventManager.OnCharacterKnockback +=
            OnCharacterKnockback;
    }


    private void OnDisable()
    {
        EventManager.OnCharacterKnockback -=
            OnCharacterKnockback;
    }


    private void Update()
    {
        switch (currentState)
        {
            case AIState.CollectingStart:

                CollectBricks(
                    startBrickArea,
                    AIState.MovingToBridge1
                );

                break;


            case AIState.MovingToBridge1:

                MoveToPoint(
                    bridge1Start,
                    AIState.CrossingBridge1
                );

                break;


            case AIState.CrossingBridge1:

                MoveToPoint(
                    bridge1End,
                    AIState.CollectingMiddle
                );

                break;


            case AIState.CollectingMiddle:

                CollectBricks(
                    middleBrickArea,
                    AIState.MovingToBridge2
                );

                break;


            case AIState.MovingToBridge2:

                MoveToPoint(
                    bridge2Start,
                    AIState.CrossingBridge2
                );

                break;


            case AIState.CrossingBridge2:

                MoveToPoint(
                    bridge2End,
                    AIState.MovingToFinish
                );

                break;


            case AIState.MovingToFinish:

                MoveToFinish();

                break;


            case AIState.Finished:

                SetMoveDirection(
                    Vector3.zero
                );

                break;
        }
    }


    private void CollectBricks(
        BoxCollider brickArea,
        AIState nextState)
    {
        if (characterStack == null)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        if (characterStack.BrickCount >=
            bricksNeededForBridge)
        {
            targetBrick = null;

            currentState =
                nextState;

            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        if (!IsTargetBrickValid(
                targetBrick,
                brickArea))
        {
            targetBrick = null;


            if (Time.time >=
                nextBrickSearchTime)
            {
                targetBrick =
                    FindNearestBrick(
                        brickArea
                    );

                nextBrickSearchTime =
                    Time.time +
                    brickSearchInterval;
            }
        }


        if (targetBrick == null)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        MoveTowards(
            targetBrick.transform.position
        );
    }


    private Brick FindNearestBrick(
        BoxCollider brickArea)
    {
        if (brickArea == null)
        {
            return null;
        }


        Brick[] bricks =
            FindObjectsByType<Brick>(
                FindObjectsSortMode.None
            );


        Brick nearestBrick = null;

        float nearestDistance =
            Mathf.Infinity;


        foreach (Brick brick in bricks)
        {
            if (brick == null)
            {
                continue;
            }


            if (!brick.CanBeCollected)
            {
                continue;
            }


            if (brick.CollectableColor !=
                CharacterTeamColor)
            {
                continue;
            }


            if (!IsInsideArea(
                    brickArea,
                    brick.transform.position))
            {
                continue;
            }


            float distance =
                (
                    brick.transform.position -
                    transform.position
                ).sqrMagnitude;


            if (distance <
                nearestDistance)
            {
                nearestDistance =
                    distance;

                nearestBrick =
                    brick;
            }
        }


        return nearestBrick;
    }


    private bool IsTargetBrickValid(
        Brick brick,
        BoxCollider area)
    {
        if (brick == null)
        {
            return false;
        }


        if (!brick.CanBeCollected)
        {
            return false;
        }


        if (brick.CollectableColor !=
            CharacterTeamColor)
        {
            return false;
        }


        return IsInsideArea(
            area,
            brick.transform.position
        );
    }


    private bool IsInsideArea(
        BoxCollider area,
        Vector3 position)
    {
        if (area == null)
        {
            return false;
        }


        Bounds bounds =
            area.bounds;


        bool insideX =
            position.x >= bounds.min.x &&
            position.x <= bounds.max.x;

        bool insideZ =
            position.z >= bounds.min.z &&
            position.z <= bounds.max.z;


        return insideX &&
               insideZ;
    }


    private void MoveToPoint(
        Transform target,
        AIState nextState)
    {
        if (target == null)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        MoveTowards(
            target.position
        );


        float distance =
            GetHorizontalDistance(
                transform.position,
                target.position
            );


        if (distance <=
            pointReachedDistance)
        {
            currentState =
                nextState;

            targetBrick = null;
        }
    }


    private void MoveToFinish()
    {
        if (finishTarget == null)
        {
            FinishRace();

            return;
        }


        MoveTowards(
            finishTarget.position
        );


        float distance =
            GetHorizontalDistance(
                transform.position,
                finishTarget.position
            );


        if (distance <=
            pointReachedDistance)
        {
            FinishRace();
        }
    }


    private void FinishRace()
    {
        currentState =
            AIState.Finished;


        SetMoveDirection(
            Vector3.zero
        );


        EventManager.CharacterFinished(
            this
        );
    }


    private void MoveTowards(
        Vector3 targetPosition)
    {
        Vector3 direction =
            targetPosition -
            transform.position;


        direction.y = 0f;


        if (direction.sqrMagnitude <
            0.001f)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        SetMoveDirection(
            direction.normalized
        );
    }


    private float GetHorizontalDistance(
        Vector3 firstPosition,
        Vector3 secondPosition)
    {
        firstPosition.y = 0f;
        secondPosition.y = 0f;


        return Vector3.Distance(
            firstPosition,
            secondPosition
        );
    }


    private void OnCharacterKnockback(
        CharacterBase knockedCharacter)
    {
        if (knockedCharacter != this)
        {
            return;
        }


        targetBrick = null;


        switch (currentState)
        {
            case AIState.CollectingStart:
            case AIState.MovingToBridge1:
            case AIState.CrossingBridge1:

                currentState =
                    AIState.CollectingStart;

                break;


            case AIState.CollectingMiddle:
            case AIState.MovingToBridge2:
            case AIState.CrossingBridge2:

                currentState =
                    AIState.CollectingMiddle;

                break;
        }
    }
}