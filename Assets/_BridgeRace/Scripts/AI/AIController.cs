using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : CharacterBase
{
    private enum AIState
    {
        CollectingStart,

        MovingToBridge1,
        CrossingBridge1,
        ReturningFromBridge1,

        CollectingMiddle01,

        MovingToBridge2,
        CrossingBridge2,
        ReturningFromBridge2,

        CollectingMiddle02,

        MovingToBridge3,
        CrossingBridge3,
        ReturningFromBridge3,

        MovingToFinish,

        Finished
    }


    // =====================================================
    // REFERENCES
    // =====================================================

    [Header("AI References")]
    [SerializeField] private CharacterStack characterStack;
    [SerializeField] private NavMeshAgent navMeshAgent;


    private CharacterBridgeBuilder bridgeBuilder;
    private AIBrickTargeting brickTargeting;
    private AINavigation navigation;


    // =====================================================
    // BRICK AREAS
    // =====================================================

    [Header("Brick Areas")]

    [SerializeField]
    private BoxCollider startBrickArea;

    [SerializeField]
    private BoxCollider middleBrickArea;

    [SerializeField]
    private BoxCollider middle02BrickArea;


    // =====================================================
    // BRIDGE 1
    // =====================================================

    [Header("Bridge 1 Route")]

    [SerializeField]
    private Transform bridge1Start;

    [SerializeField]
    private Transform bridge1End;


    // =====================================================
    // BRIDGE 2
    // =====================================================

    [Header("Bridge 2 Route")]

    [SerializeField]
    private Transform bridge2Start;

    [SerializeField]
    private Transform bridge2End;


    // =====================================================
    // FINAL BRIDGE
    // =====================================================

    [Header("Final Bridge Route")]

    [SerializeField]
    private Transform bridge3Start;

    [SerializeField]
    private Transform bridge3End;


    // =====================================================
    // FINISH
    // =====================================================

    [Header("Finish")]

    [SerializeField]
    private Transform finishTarget;


    // =====================================================
    // AI SETTINGS
    // =====================================================

    [Header("AI Settings")]

    [SerializeField]
    private int bricksNeededForBridge = 20;


    [SerializeField]
    private float pointReachedDistance = 0.8f;


    // =====================================================
    // NAVMESH
    // =====================================================

    [Header("NavMesh Settings")]

    [SerializeField]
    private float navMeshSampleDistance = 3f;


    // =====================================================
    // EASY
    // =====================================================

    [Header("Easy")]

    [SerializeField]
    private float easyMoveSpeed = 4.2f;

    [SerializeField]
    private float easySearchInterval = 0.45f;

    [SerializeField]
    private int easyMinBricks = 5;

    [SerializeField]
    private int easyMaxBricks = 12;


    // =====================================================
    // NORMAL
    // =====================================================

    [Header("Normal")]

    [SerializeField]
    private float normalMoveSpeed = 5f;

    [SerializeField]
    private float normalSearchInterval = 0.25f;

    [SerializeField]
    private int normalBrickGoal = 8;


    // =====================================================
    // HARD
    // =====================================================

    [Header("Hard")]

    [SerializeField]
    private float hardMoveSpeed = 5.7f;

    [SerializeField]
    private float hardSearchInterval = 0.12f;

    [SerializeField]
    private float hardClusterRadius = 4f;


    // =====================================================
    // BRIDGE SETTINGS
    // =====================================================

    [Header("Bridge Recovery")]

    [SerializeField]
    private float bridgeStallTime = 0.20f;

    [SerializeField]
    private float bridgeProgressThreshold = 0.015f;

    [SerializeField]
    private float bridgeLookAhead = 0.45f;

    [SerializeField]
    private float bridgeCenteringStrength = 3f;

    [SerializeField]
    private float bridgeCrossSpeed = 3.4f;

    [SerializeField]
    private float bridgeReturnSpeed = 2.5f;


    private float groundMoveSpeed;


    // =====================================================
    // RUNTIME
    // =====================================================

    private AIState currentState =
        AIState.CollectingStart;


    private AIDifficulty difficulty;


    private Brick targetBrick;


    private int currentBrickGoal;


    private float nextBrickSearchTime;

    private float brickSearchInterval;


    private float lastBridgeDistance =
        Mathf.Infinity;


    private float bridgeStallTimer;

    [SerializeField]
    private float bridgeReachedDistance = 0.15f;


    // =====================================================
    // AWAKE
    // =====================================================

    protected override void Awake()
    {
        base.Awake();


        if (characterStack == null)
        {
            characterStack =
                GetComponent<CharacterStack>();
        }

        if (navMeshAgent == null)
        {
            navMeshAgent =
                GetComponent<NavMeshAgent>();
        }


        bridgeBuilder =
            GetComponent<CharacterBridgeBuilder>();


        brickTargeting =
            new AIBrickTargeting();

        navigation =
    new AINavigation(
        transform,
        navMeshAgent,
        navMeshSampleDistance,
        SetMoveDirection,
        StopMovement
    );

        difficulty =
            GameSettings.SelectedDifficulty;


        ApplyDifficulty();

    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        SnapAgentToCurrentPosition();


        PrepareCollectionGoal();
    }


    // =====================================================
    // EVENTS
    // =====================================================

    private void OnEnable()
    {
        EventManager.OnCharacterKnockback +=
            OnCharacterKnockback;


        EventManager.OnCharacterPlaced +=
            OnCharacterPlaced;
    }


    private void OnDisable()
    {
        EventManager.OnCharacterKnockback -=
            OnCharacterKnockback;


        EventManager.OnCharacterPlaced -=
            OnCharacterPlaced;
    }


    // =====================================================
    // UPDATE
    // =====================================================

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

                MoveToPointWithNavMesh(
                    bridge1Start,
                    AIState.CrossingBridge1
                );

                break;


            case AIState.CrossingBridge1:

                CrossBridge(
                    bridge1Start,
                    bridge1End,
                    AIState.CollectingMiddle01,
                    AIState.ReturningFromBridge1
                );

                break;


            case AIState.ReturningFromBridge1:

                ReturnAcrossBridge(
                    bridge1End,
                    bridge1Start,
                    AIState.CollectingStart
                );

                break;


            case AIState.CollectingMiddle01:

                CollectBricks(
                    middleBrickArea,
                    AIState.MovingToBridge2
                );

                break;


            case AIState.MovingToBridge2:

                MoveToPointWithNavMesh(
                    bridge2Start,
                    AIState.CrossingBridge2
                );

                break;


            case AIState.CrossingBridge2:

                CrossBridge(
                    bridge2Start,
                    bridge2End,
                    AIState.CollectingMiddle02,
                    AIState.ReturningFromBridge2
                );

                break;


            case AIState.ReturningFromBridge2:

                ReturnAcrossBridge(
                    bridge2End,
                    bridge2Start,
                    AIState.CollectingMiddle01
                );

                break;


            case AIState.CollectingMiddle02:

                CollectBricks(
                    middle02BrickArea,
                    AIState.MovingToBridge3
                );

                break;


            case AIState.MovingToBridge3:

                MoveToPointWithNavMesh(
                    bridge3Start,
                    AIState.CrossingBridge3
                );

                break;


            case AIState.CrossingBridge3:

                CrossBridge(
                    bridge3Start,
                    bridge3End,
                    AIState.MovingToFinish,
                    AIState.ReturningFromBridge3
                );

                break;


            case AIState.ReturningFromBridge3:

                ReturnAcrossBridge(
                    bridge3End,
                    bridge3Start,
                    AIState.CollectingMiddle02
                );

                break;


            case AIState.MovingToFinish:

                MoveToFinish();

                break;


            case AIState.Finished:

                StopAllMovement();

                break;
        }
    }


    // =====================================================
    // DIFFICULTY
    // =====================================================

    private void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:

                moveSpeed =
                    easyMoveSpeed;

                brickSearchInterval =
                    easySearchInterval;

                break;


            case AIDifficulty.Normal:

                moveSpeed =
                    normalMoveSpeed;

                brickSearchInterval =
                    normalSearchInterval;

                break;


            case AIDifficulty.Hard:

                moveSpeed =
                    hardMoveSpeed;

                brickSearchInterval =
                    hardSearchInterval;

                break;
        }


        // Ada üzerinde kullanılacak normal hız.
        groundMoveSpeed =
            moveSpeed;

        if (navigation != null)
        {
            navigation.SetSpeed(
                moveSpeed
            );
        }

        Debug.Log(
            gameObject.name +
            " | Difficulty: " +
            difficulty +
            " | Speed: " +
            moveSpeed
        );
    }


    private void PrepareCollectionGoal()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:

                currentBrickGoal =
                    Random.Range(
                        easyMinBricks,
                        easyMaxBricks + 1
                    );

                break;


            case AIDifficulty.Normal:

                currentBrickGoal =
                    normalBrickGoal;

                break;


            case AIDifficulty.Hard:

                currentBrickGoal =
                    bricksNeededForBridge;

                break;
        }


        Debug.Log(
            gameObject.name +
            " yeni Brick hedefi: " +
            currentBrickGoal
        );
    }


    // =====================================================
    // COLLECT
    // =====================================================

    private void CollectBricks(
        BoxCollider brickArea,
        AIState nextState)
    {
        if (characterStack == null)
        {
            StopMovement();

            return;
        }


        if (characterStack.BrickCount >=
            currentBrickGoal)
        {
            targetBrick = null;


            ChangeState(
                nextState
            );


            return;
        }


        if (!IsTargetBrickValid(
                targetBrick,
                brickArea))
        {
            targetBrick =
                null;


            if (Time.time >=
                nextBrickSearchTime)
            {
                targetBrick =
                    FindBrickTarget(
                        brickArea
                    );


                nextBrickSearchTime =
                    Time.time +
                    brickSearchInterval;
            }
        }


        // =================================================
        // ÇOK ÖNEMLİ:
        // Brick henüz spawn olmadıysa artık DONMUYOR.
        // Brick alanının merkezine ilerliyor.
        // Böylece ActivationZone'u tetikliyor.
        // =================================================

        if (targetBrick == null)
        {
            MoveToBrickAreaCenter(
                brickArea
            );

            return;
        }


        MoveUsingNavMesh(
            targetBrick.transform.position
        );
    }


    private void MoveToBrickAreaCenter(
        BoxCollider area)
    {
        if (area == null)
        {
            StopMovement();

            return;
        }


        Vector3 target =
            area.bounds.center;


        target.y =
            transform.position.y;


        MoveUsingNavMesh(
            target
        );
    }


    // =====================================================
    // BRICK SEARCH
    // =====================================================

    private Brick FindBrickTarget(
    BoxCollider brickArea)
    {
        if (brickTargeting == null)
        {
            return null;
        }


        return brickTargeting.FindTarget(
            brickArea,
            CharacterTeamColor,
            difficulty,
            transform.position,
            hardClusterRadius
        );
    }


    private bool IsTargetBrickValid(
    Brick brick,
    BoxCollider area)
    {
        if (brickTargeting == null)
        {
            return false;
        }


        return brickTargeting.IsTargetValid(
            brick,
            area,
            CharacterTeamColor
        );
    }


    // =====================================================
    // BRIDGE START
    // =====================================================

    private void MoveToPointWithNavMesh(
    Transform target,
    AIState nextState)
    {
        if (target == null)
        {
            StopMovement();

            return;
        }


        Vector3 reachableTarget =
            target.position;


        // Hedef Bridge'in üzerinde veya
        // NavMesh dışında olabilir.
        // AI'nın gerçekten ulaşabileceği
        // en yakın NavMesh noktasını buluyoruz.
        if (NavMesh.SamplePosition(
                target.position,
                out NavMeshHit hit,
                navMeshSampleDistance,
                NavMesh.AllAreas))
        {
            reachableTarget =
                hit.position;
        }


        MoveUsingNavMesh(
            reachableTarget
        );


        float distance =
            HorizontalDistance(
                transform.position,
                reachableTarget
            );


        if (distance <=
            pointReachedDistance)
        {
            StopMovement();


            ResetNavigation();


            ChangeState(
                nextState
            );
        }
    }


    // =====================================================
    // BRIDGE CROSS
    // =====================================================

    private void CrossBridge(
     Transform bridgeStart,
     Transform bridgeEnd,
     AIState nextState,
     AIState returnState)
    {
        if (bridgeStart == null ||
            bridgeEnd == null)
        {
            StopMovement();

            return;
        }


        ResetNavigation();


        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                true
            );
        }


        float distance =
            HorizontalDistance(
                transform.position,
                bridgeEnd.position
            );


        // Önce köprünün gerçekten bitip
        // bitmediğini kontrol ediyoruz.
        if (distance <=
    bridgeReachedDistance)
        {
            StopMovement();

            ClearHorizontalVelocity();

            ResetNavigation();

            ChangeState(
                nextState
            );

            return;
        }


        // =================================================
        // EN ÖNEMLİ DÜZELTME
        //
        // Brick bitti ve bir sonraki Step boşsa,
        // 0.2 saniye daha ileri yürümek yerine
        // ANINDA geri dönüşe geç.
        // =================================================

        if (characterStack != null &&
            characterStack.BrickCount <= 0 &&
            bridgeBuilder != null &&
            bridgeBuilder.IsForwardBlocked)
        {
            BeginBridgeReturn(
                returnState
            );

            return;
        }


        MoveAlongBridgeLane(
            bridgeStart,
            bridgeEnd
        );


        // Ek güvenlik.
        CheckBridgeStall(
            distance,
            returnState
        );
    }

    private void BeginBridgeReturn(
    AIState returnState)
    {
        // Hareket yönünü hemen sıfırla.
        StopMovement();


        // Önceki ileri momentumunu da temizle.
        // Yoksa state değişse bile Rigidbody
        // bir fizik frame'i ileri taşıyabiliyor.
        if (rb != null)
        {
            Vector3 velocity =
                rb.linearVelocity;


            velocity.x = 0f;
            velocity.z = 0f;


            rb.linearVelocity =
                velocity;


            rb.angularVelocity =
                Vector3.zero;
        }


        ChangeState(
            returnState
        );
    }

    // =====================================================
    // BRIDGE RETURN
    // =====================================================

    private void ReturnAcrossBridge(
        Transform bridgeEnd,
        Transform bridgeStart,
        AIState collectingState)
    {
        if (bridgeStart == null ||
            bridgeEnd == null)
        {
            return;
        }


        ResetNavigation();


        // Geri dönerken bridge builder artık
        // hareketimizi engellemesin.
        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                false
            );
        }


        MoveAlongBridgeLane(
            bridgeEnd,
            bridgeStart
        );


        if (HorizontalDistance(
        transform.position,
        bridgeStart.position) <=
    bridgeReachedDistance)
        {
            StopMovement();

            ClearHorizontalVelocity();

            if (bridgeBuilder != null)
            {
                bridgeBuilder.SetBridgeCheckEnabled(
                    true
                );
            }

            ResetNavigation();

            ChangeState(
                collectingState
            );

            return;
        }
    }


    // =====================================================
    // BRIDGE LANE FOLLOW
    // =====================================================

    private void MoveAlongBridgeLane(
    Transform from,
    Transform to)
    {
        Vector3 start =
            from.position;


        Vector3 end =
            to.position;


        start.y = 0f;
        end.y = 0f;


        Vector3 current =
            transform.position;


        current.y = 0f;


        Vector3 lane =
            end -
            start;


        float laneLength =
            lane.magnitude;


        if (laneLength <
            0.01f)
        {
            StopMovement();

            return;
        }


        Vector3 laneDirection =
            lane /
            laneLength;


        Vector3 fromStart =
            current -
            start;


        float progress =
            Vector3.Dot(
                fromStart,
                laneDirection
            );


        progress =
            Mathf.Clamp(
                progress,
                0f,
                laneLength
            );


        // Karakterin olması gereken
        // merdiven merkez noktası.
        Vector3 centerPoint =
            start +
            laneDirection *
            progress;


        // Merkeze olan yatay sapma.
        Vector3 centerCorrection =
            centerPoint -
            current;


        centerCorrection.y =
            0f;


        // Biraz ileri hedef.
        Vector3 forwardDirection =
            laneDirection *
            bridgeLookAhead;


        // =================================================
        // Düz ileri gitmek yerine:
        //
        // ileri yön
        // +
        // merdiven merkezine güçlü düzeltme
        // =================================================

        Vector3 finalDirection =
            forwardDirection +
            centerCorrection *
            bridgeCenteringStrength;


        finalDirection.y =
            0f;


        if (finalDirection.sqrMagnitude <
            0.001f)
        {
            StopMovement();

            return;
        }


        SetMoveDirection(
            finalDirection.normalized
        );
    }

    // =====================================================
    // BRIDGE STALL
    // =====================================================

    private void CheckBridgeStall(
        float distance,
        AIState returnState)
    {
        if (characterStack == null)
        {
            return;
        }


        if (float.IsInfinity(
                lastBridgeDistance))
        {
            lastBridgeDistance =
                distance;

            return;
        }


        float progress =
            lastBridgeDistance -
            distance;


        if (progress >
            bridgeProgressThreshold)
        {
            bridgeStallTimer =
                0f;
        }
        else if (
            characterStack.BrickCount <= 0)
        {
            bridgeStallTimer +=
                Time.deltaTime;
        }
        else
        {
            bridgeStallTimer =
                0f;
        }


        lastBridgeDistance =
            distance;


        if (bridgeStallTimer >=
    bridgeStallTime)
        {
            BeginBridgeReturn(
                returnState
            );
        }
    }


    // =====================================================
    // FINISH
    // =====================================================

    private void MoveToFinish()
    {
        if (finishTarget == null)
        {
            StopMovement();

            return;
        }


        MoveUsingNavMesh(
            finishTarget.position
        );
    }


    // =====================================================
    // NAVMESH
    // =====================================================

    private void MoveUsingNavMesh(
    Vector3 targetPosition)
    {
        if (navigation == null)
        {
            StopMovement();

            return;
        }


        navigation.MoveTo(
            targetPosition
        );
    }

    private bool EnsureAgentOnNavMesh()
    {
        if (navigation == null)
        {
            return false;
        }


        return navigation.EnsureAgentOnNavMesh();
    }


    private bool SnapAgentToCurrentPosition()
    {
        if (navigation == null)
        {
            return false;
        }


        return navigation.SnapToCurrentPosition();
    }


    private void ResetNavigation()
    {
        if (navigation == null)
        {
            return;
        }


        navigation.Reset();
    }


    // =====================================================
    // RESPAWN
    // =====================================================

    public void HandleRespawn()
    {
        StopMovement();


        ResetNavigation();


        SetMovementEnabled(
            true
        );


        targetBrick =
            null;


        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                true
            );
        }


        switch (currentState)
        {
            case AIState.CollectingStart:
            case AIState.MovingToBridge1:
            case AIState.CrossingBridge1:
            case AIState.ReturningFromBridge1:

                ChangeState(
                    AIState.CollectingStart
                );

                break;


            case AIState.CollectingMiddle01:
            case AIState.MovingToBridge2:
            case AIState.CrossingBridge2:
            case AIState.ReturningFromBridge2:

                ChangeState(
                    AIState.CollectingMiddle01
                );

                break;


            case AIState.CollectingMiddle02:
            case AIState.MovingToBridge3:
            case AIState.CrossingBridge3:
            case AIState.ReturningFromBridge3:

                ChangeState(
                    AIState.CollectingMiddle02
                );

                break;


            case AIState.MovingToFinish:

                ChangeState(
                    AIState.MovingToFinish
                );

                break;


            case AIState.Finished:

                StopAllMovement();

                return;
        }


        SnapAgentToCurrentPosition();


        Debug.Log(
            gameObject.name +
            " Respawn sonrası AI state düzeltildi."
        );
    }


    // =====================================================
    // KNOCKBACK
    // =====================================================

    private void OnCharacterKnockback(
        CharacterBase knockedCharacter)
    {
        if (knockedCharacter != this)
        {
            return;
        }


        // Knockback artık AI state'ini
        // tamamen değiştirmiyor.
        //
        // Böylece küçük bir çarpışmada
        // karakter aniden geri dönmüyor.

        targetBrick =
            null;


        ResetNavigation();
    }


    // =====================================================
    // FINISH EVENT
    // =====================================================

    private void OnCharacterPlaced(
        CharacterBase placedCharacter,
        int place)
    {
        if (placedCharacter != this)
        {
            return;
        }


        ChangeState(
            AIState.Finished
        );


        StopAllMovement();
    }


    // =====================================================
    // STATE
    // =====================================================

    private void ChangeState(
    AIState newState)
    {
        currentState =
            newState;


        targetBrick =
            null;


        nextBrickSearchTime =
            0f;


        bridgeStallTimer =
            0f;


        lastBridgeDistance =
            Mathf.Infinity;


        if (navigation != null)
        {
            navigation.Reset();
        }


        bool crossing =
            newState ==
                AIState.CrossingBridge1 ||
            newState ==
                AIState.CrossingBridge2 ||
            newState ==
                AIState.CrossingBridge3;


        bool returning =
            newState ==
                AIState.ReturningFromBridge1 ||
            newState ==
                AIState.ReturningFromBridge2 ||
            newState ==
                AIState.ReturningFromBridge3;


        // Köprüde daha yavaş ve güvenli hareket.
        if (crossing)
        {
            moveSpeed =
                bridgeCrossSpeed;
        }
        else if (returning)
        {
            moveSpeed =
                bridgeReturnSpeed;
        }
        else
        {
            moveSpeed =
                groundMoveSpeed;
        }


        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                !returning
            );
        }


        if (newState ==
                AIState.CollectingStart ||
            newState ==
                AIState.CollectingMiddle01 ||
            newState ==
                AIState.CollectingMiddle02)
        {
            PrepareCollectionGoal();
        }
    }


    // =====================================================
    // HELPERS
    // =====================================================

    private void StopMovement()
    {
        SetMoveDirection(
            Vector3.zero
        );
    }
    private void ClearHorizontalVelocity()
    {
        if (rb == null)
        {
            return;
        }

        Vector3 velocity =
            rb.linearVelocity;

        velocity.x = 0f;
        velocity.z = 0f;

        rb.linearVelocity =
            velocity;

        rb.angularVelocity =
            Vector3.zero;
    }


    private void StopAllMovement()
    {
        StopMovement();


        ResetNavigation();


        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                false
            );
        }
    }


    private float HorizontalDistance(
        Vector3 a,
        Vector3 b)
    {
        a.y = 0f;

        b.y = 0f;


        return Vector3.Distance(
            a,
            b
        );
    }


    private float HorizontalDistanceSquared(
        Vector3 a,
        Vector3 b)
    {
        a.y = 0f;

        b.y = 0f;


        return (
            a -
            b
        ).sqrMagnitude;
    }
}