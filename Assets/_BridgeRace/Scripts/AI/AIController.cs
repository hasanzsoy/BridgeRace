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


    // =====================================================
    // BRICK AREAS
    // =====================================================

    [Header("Brick Areas")]

    [SerializeField]
    private BoxCollider startBrickArea;

    // Eski middleBrickArea ismini özellikle korudum.
    // Inspector bağlantın kaybolmasın.
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
    // GENERAL AI SETTINGS
    // =====================================================

    [Header("AI Settings")]

    [SerializeField]
    private int bricksNeededForBridge = 20;

    [SerializeField]
    private float pointReachedDistance = 0.8f;


    // =====================================================
    // NAVMESH SETTINGS
    // =====================================================

    [Header("NavMesh Settings")]

    [SerializeField]
    private float navMeshSampleDistance = 2.5f;


    // =====================================================
    // DIFFICULTY
    // =====================================================

    [Header("Difficulty Settings")]

    private AIDifficulty difficulty;


    [Header("Easy")]

    [SerializeField]
    private float easyMoveSpeed = 4.2f;

    [SerializeField]
    private float easySearchInterval = 0.45f;

    [SerializeField]
    private int easyMinBricks = 5;

    [SerializeField]
    private int easyMaxBricks = 12;


    [Header("Normal")]

    [SerializeField]
    private float normalMoveSpeed = 5f;

    [SerializeField]
    private float normalSearchInterval = 0.25f;

    [SerializeField]
    private int normalBrickGoal = 8;


    [Header("Hard")]

    [SerializeField]
    private float hardMoveSpeed = 5.7f;

    [SerializeField]
    private float hardSearchInterval = 0.12f;

    [SerializeField]
    private float hardClusterRadius = 4f;

    [SerializeField]
    private float hardAggressionRange = 3f;

    [SerializeField]
    private int hardAggressionMinimumBricks = 4;


    // =====================================================
    // BRIDGE STALL
    // =====================================================

    [Header("Bridge Recovery")]

    [SerializeField]
    private float bridgeStallTime = 0.8f;

    [SerializeField]
    private float bridgeProgressThreshold = 0.05f;


    // =====================================================
    // RUNTIME
    // =====================================================

    private AIState currentState =
        AIState.CollectingStart;


    private Brick targetBrick;


    private int currentBrickGoal;


    private float nextBrickSearchTime;

    private float brickSearchInterval;


    private float lastBridgeDistance =
        Mathf.Infinity;

    private float bridgeStallTimer;


    private CharacterBase[] allCharacters;


    private Vector3 lastNavDestination;

    private bool hasNavDestination;


    private bool navMeshWarningShown;


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


        if (characterStack == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterStack bulunamadı!"
            );
        }


        if (navMeshAgent == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde NavMeshAgent bulunamadı!"
            );

            return;
        }


        difficulty =
            GameSettings.SelectedDifficulty;


        ApplyDifficulty();


        SetupNavMeshAgent();
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        allCharacters =
            FindObjectsByType<CharacterBase>(
                FindObjectsSortMode.None
            );


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
            // =============================================
            // START
            // =============================================

            case AIState.CollectingStart:

                CollectBricks(
                    startBrickArea,
                    AIState.MovingToBridge1
                );

                break;


            // =============================================
            // BRIDGE 1
            // =============================================

            case AIState.MovingToBridge1:

                MoveToPointWithNavMesh(
                    bridge1Start,
                    AIState.CrossingBridge1
                );

                break;


            case AIState.CrossingBridge1:

                CrossBridge(
                    bridge1End,
                    AIState.CollectingMiddle01,
                    AIState.ReturningFromBridge1
                );

                break;


            case AIState.ReturningFromBridge1:

                ReturnToCollectionArea(
                    bridge1Start,
                    AIState.CollectingStart
                );

                break;


            // =============================================
            // MIDDLE 01
            // =============================================

            case AIState.CollectingMiddle01:

                CollectBricks(
                    middleBrickArea,
                    AIState.MovingToBridge2
                );

                break;


            // =============================================
            // BRIDGE 2
            // =============================================

            case AIState.MovingToBridge2:

                MoveToPointWithNavMesh(
                    bridge2Start,
                    AIState.CrossingBridge2
                );

                break;


            case AIState.CrossingBridge2:

                CrossBridge(
                    bridge2End,
                    AIState.CollectingMiddle02,
                    AIState.ReturningFromBridge2
                );

                break;


            case AIState.ReturningFromBridge2:

                ReturnToCollectionArea(
                    bridge2Start,
                    AIState.CollectingMiddle01
                );

                break;


            // =============================================
            // MIDDLE 02
            // =============================================

            case AIState.CollectingMiddle02:

                CollectBricks(
                    middle02BrickArea,
                    AIState.MovingToBridge3
                );

                break;


            // =============================================
            // FINAL BRIDGE
            // =============================================

            case AIState.MovingToBridge3:

                MoveToPointWithNavMesh(
                    bridge3Start,
                    AIState.CrossingBridge3
                );

                break;


            case AIState.CrossingBridge3:

                CrossBridge(
                    bridge3End,
                    AIState.MovingToFinish,
                    AIState.ReturningFromBridge3
                );

                break;


            case AIState.ReturningFromBridge3:

                ReturnToCollectionArea(
                    bridge3Start,
                    AIState.CollectingMiddle02
                );

                break;


            // =============================================
            // FINISH
            // =============================================

            case AIState.MovingToFinish:

                MoveToFinish();

                break;


            case AIState.Finished:

                StopAllMovement();

                break;
        }
    }


    // =====================================================
    // NAVMESH SETUP
    // =====================================================

    private void SetupNavMeshAgent()
    {
        if (navMeshAgent == null)
        {
            return;
        }


        // NavMeshAgent yolu hesaplasın.
        // Rigidbody gerçek hareketi yapsın.
        navMeshAgent.updatePosition = false;

        navMeshAgent.updateRotation = false;


        navMeshAgent.speed =
            moveSpeed;


        navMeshAgent.angularSpeed =
            720f;


        navMeshAgent.acceleration =
            20f;


        navMeshAgent.stoppingDistance =
            0.2f;


        navMeshAgent.autoBraking =
            true;
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


        Debug.Log(
            gameObject.name +
            " | Difficulty: " +
            difficulty +
            " | Speed: " +
            moveSpeed
        );
    }


    // =====================================================
    // COLLECTION GOAL
    // =====================================================

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
    // COLLECT BRICKS
    // =====================================================

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


        // Yeterli Brick toplandı.
        if (characterStack.BrickCount >=
            currentBrickGoal)
        {
            targetBrick = null;


            ChangeState(
                nextState
            );


            return;
        }


        // HARD modda yakın ve bizden daha zayıf
        // rakip varsa kısa süreli agresif hareket.
        if (difficulty ==
            AIDifficulty.Hard)
        {
            if (TryGetHardAggressionTarget(
                    out Vector3 aggressionTarget))
            {
                MoveUsingNavMesh(
                    aggressionTarget
                );

                return;
            }
        }


        // Mevcut Brick artık geçerli değilse
        // yeni Brick ara.
        if (!IsTargetBrickValid(
                targetBrick,
                brickArea))
        {
            targetBrick = null;


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


        if (targetBrick == null)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        MoveUsingNavMesh(
            targetBrick.transform.position
        );
    }


    // =====================================================
    // FIND BRICK ACCORDING TO DIFFICULTY
    // =====================================================

    private Brick FindBrickTarget(
        BoxCollider brickArea)
    {
        List<Brick> candidates =
            GetValidBricks(
                brickArea
            );


        if (candidates.Count <= 0)
        {
            return null;
        }


        switch (difficulty)
        {
            case AIDifficulty.Easy:

                return FindRandomBrick(
                    candidates
                );


            case AIDifficulty.Normal:

                return FindNearestBrick(
                    candidates
                );


            case AIDifficulty.Hard:

                return FindClusterBrick(
                    candidates
                );
        }


        return null;
    }


    // =====================================================
    // VALID BRICK LIST
    // =====================================================

    private List<Brick> GetValidBricks(
        BoxCollider brickArea)
    {
        List<Brick> validBricks =
            new List<Brick>();


        if (brickArea == null)
        {
            return validBricks;
        }


        Brick[] bricks =
            FindObjectsByType<Brick>(
                FindObjectsSortMode.None
            );


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


            validBricks.Add(
                brick
            );
        }


        return validBricks;
    }


    // =====================================================
    // EASY
    // RANDOM BRICK
    // =====================================================

    private Brick FindRandomBrick(
        List<Brick> bricks)
    {
        if (bricks.Count <= 0)
        {
            return null;
        }


        int randomIndex =
            Random.Range(
                0,
                bricks.Count
            );


        return bricks[randomIndex];
    }


    // =====================================================
    // NORMAL
    // NEAREST BRICK
    // =====================================================

    private Brick FindNearestBrick(
        List<Brick> bricks)
    {
        Brick nearestBrick = null;


        float nearestDistance =
            Mathf.Infinity;


        foreach (Brick brick in bricks)
        {
            float distance =
                GetHorizontalDistanceSquared(
                    transform.position,
                    brick.transform.position
                );


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


    // =====================================================
    // HARD
    // CLUSTER SELECTION
    // =====================================================

    private Brick FindClusterBrick(
        List<Brick> bricks)
    {
        Brick bestBrick = null;


        float bestScore =
            Mathf.NegativeInfinity;


        float clusterRadiusSquared =
            hardClusterRadius *
            hardClusterRadius;


        foreach (Brick candidate in bricks)
        {
            int nearbyBrickCount = 0;


            foreach (Brick otherBrick in bricks)
            {
                if (otherBrick == candidate)
                {
                    continue;
                }


                Vector3 difference =
                    otherBrick.transform.position -
                    candidate.transform.position;


                difference.y = 0f;


                if (difference.sqrMagnitude <=
                    clusterRadiusSquared)
                {
                    nearbyBrickCount++;
                }
            }


            float distance =
                GetHorizontalDistanceSquared(
                    transform.position,
                    candidate.transform.position
                );


            // Yakınında çok Brick olması
            // skoru yükseltir.
            //
            // Çok uzakta olması skoru düşürür.
            float score =
                nearbyBrickCount * 10f -
                distance * 0.1f;


            if (score >
                bestScore)
            {
                bestScore =
                    score;


                bestBrick =
                    candidate;
            }
        }


        return bestBrick;
    }


    // =====================================================
    // HARD AGGRESSION
    // =====================================================

    private bool TryGetHardAggressionTarget(
        out Vector3 targetPosition)
    {
        targetPosition =
            Vector3.zero;


        if (characterStack == null)
        {
            return false;
        }


        if (characterStack.BrickCount <
            hardAggressionMinimumBricks)
        {
            return false;
        }


        if (allCharacters == null)
        {
            return false;
        }


        CharacterBase nearestOpponent =
            null;


        float nearestDistance =
            hardAggressionRange *
            hardAggressionRange;


        foreach (CharacterBase otherCharacter
                 in allCharacters)
        {
            if (otherCharacter == null)
            {
                continue;
            }


            if (otherCharacter == this)
            {
                continue;
            }


            float heightDifference =
                Mathf.Abs(
                    otherCharacter.transform.position.y -
                    transform.position.y
                );


            if (heightDifference > 1.5f)
            {
                continue;
            }


            if (!otherCharacter.TryGetComponent<CharacterStack>(
                    out CharacterStack otherStack))
            {
                continue;
            }


            // Bizde daha fazla Brick yoksa
            // saldırma.
            if (characterStack.BrickCount <=
                otherStack.BrickCount)
            {
                continue;
            }


            float distance =
                GetHorizontalDistanceSquared(
                    transform.position,
                    otherCharacter.transform.position
                );


            if (distance <
                nearestDistance)
            {
                nearestDistance =
                    distance;


                nearestOpponent =
                    otherCharacter;
            }
        }


        if (nearestOpponent == null)
        {
            return false;
        }


        targetPosition =
            nearestOpponent.transform.position;


        return true;
    }


    // =====================================================
    // BRICK VALIDATION
    // =====================================================

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


    // =====================================================
    // MOVE TO BRIDGE START USING NAVMESH
    // =====================================================

    private void MoveToPointWithNavMesh(
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


        MoveUsingNavMesh(
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
            ResetNavigation();


            ChangeState(
                nextState
            );
        }
    }


    // =====================================================
    // CROSS BRIDGE
    // =====================================================

    private void CrossBridge(
        Transform bridgeEnd,
        AIState nextState,
        AIState returnState)
    {
        if (bridgeEnd == null)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        // Köprü üzerinde NavMesh kullanmıyoruz.
        // CharacterBase + CharacterBridgeBuilder
        // mevcut köprü mantığını yönetsin.
        ResetNavigation();


        MoveDirectlyTowards(
            bridgeEnd.position
        );


        float distance =
            GetHorizontalDistance(
                transform.position,
                bridgeEnd.position
            );


        // Köprünün sonuna ulaştık.
        if (distance <=
            pointReachedDistance)
        {
            SetMoveDirection(
                Vector3.zero
            );


            SnapAgentToCurrentPosition();


            ChangeState(
                nextState
            );


            return;
        }


        CheckBridgeStall(
            distance,
            returnState
        );
    }


    // =====================================================
    // BRIDGE STALL
    // =====================================================

    private void CheckBridgeStall(
        float currentDistance,
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
                currentDistance;

            return;
        }


        float progress =
            lastBridgeDistance -
            currentDistance;


        // Gerçek ilerleme varsa timer sıfırlanır.
        if (progress >=
            bridgeProgressThreshold)
        {
            bridgeStallTimer = 0f;
        }

        // Brick kalmadı ve ilerleme yok.
        else if (characterStack.BrickCount <= 0)
        {
            bridgeStallTimer +=
                Time.deltaTime;
        }

        else
        {
            bridgeStallTimer = 0f;
        }


        lastBridgeDistance =
            currentDistance;


        // Brick bitti ve karakter köprüde
        // ilerleyemiyorsa geri dön.
        if (bridgeStallTimer >=
            bridgeStallTime)
        {
            ChangeState(
                returnState
            );
        }
    }


    // =====================================================
    // RETURN TO BRICK AREA
    // =====================================================

    private void ReturnToCollectionArea(
        Transform bridgeStart,
        AIState collectingState)
    {
        if (bridgeStart == null)
        {
            return;
        }


        ResetNavigation();


        MoveDirectlyTowards(
            bridgeStart.position
        );


        float distance =
            GetHorizontalDistance(
                transform.position,
                bridgeStart.position
            );


        if (distance <=
            pointReachedDistance)
        {
            SetMoveDirection(
                Vector3.zero
            );


            SnapAgentToCurrentPosition();


            ChangeState(
                collectingState
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
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        float distance =
            GetHorizontalDistance(
                transform.position,
                finishTarget.position
            );


        if (distance <=
            pointReachedDistance)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        MoveUsingNavMesh(
            finishTarget.position
        );
    }


    // =====================================================
    // NAVMESH MOVEMENT
    // =====================================================

    private void MoveUsingNavMesh(
        Vector3 targetPosition)
    {
        if (navMeshAgent == null)
        {
            MoveDirectlyTowards(
                targetPosition
            );

            return;
        }


        if (!SnapAgentIfNecessary())
        {
            // NavMesh bulunamazsa oyun tamamen
            // kilitlenmesin.
            MoveDirectlyTowards(
                targetPosition
            );


            if (!navMeshWarningShown)
            {
                Debug.LogWarning(
                    gameObject.name +
                    " NavMesh üzerinde değil! " +
                    "Bake ve başlangıç konumunu kontrol et."
                );


                navMeshWarningShown =
                    true;
            }


            return;
        }


        navMeshWarningShown =
            false;


        if (navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped =
                false;
        }


        // Rigidbody transform'u hareket ettiriyor.
        // NavMesh simülasyonunu karakterin
        // X/Z konumuna senkronluyoruz.
        Vector3 syncedPosition =
            transform.position;


        syncedPosition.y =
            navMeshAgent.nextPosition.y;


        navMeshAgent.nextPosition =
            syncedPosition;


        Vector3 destination =
            targetPosition;


        if (NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit targetHit,
                navMeshSampleDistance,
                NavMesh.AllAreas))
        {
            destination =
                targetHit.position;
        }


        bool destinationChanged =
            !hasNavDestination ||
            (
                destination -
                lastNavDestination
            ).sqrMagnitude >
            0.04f;


        if (destinationChanged)
        {
            bool destinationSet =
                navMeshAgent.SetDestination(
                    destination
                );


            if (!destinationSet)
            {
                SetMoveDirection(
                    Vector3.zero
                );

                return;
            }


            lastNavDestination =
                destination;


            hasNavDestination =
                true;
        }


        if (navMeshAgent.pathPending)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        if (navMeshAgent.pathStatus ==
            NavMeshPathStatus.PathInvalid)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        Vector3 desiredDirection =
            navMeshAgent.desiredVelocity;


        desiredDirection.y = 0f;


        if (desiredDirection.sqrMagnitude <
            0.001f)
        {
            SetMoveDirection(
                Vector3.zero
            );

            return;
        }


        SetMoveDirection(
            desiredDirection.normalized
        );
    }


    // =====================================================
    // DIRECT MOVEMENT
    // BRIDGE ONLY
    // =====================================================

    private void MoveDirectlyTowards(
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


    // =====================================================
    // NAVMESH POSITION
    // =====================================================

    private bool SnapAgentIfNecessary()
    {
        if (navMeshAgent == null)
        {
            return false;
        }


        if (!navMeshAgent.enabled)
        {
            return false;
        }


        if (navMeshAgent.isOnNavMesh)
        {
            return true;
        }


        return SnapAgentToCurrentPosition();
    }


    private bool SnapAgentToCurrentPosition()
    {
        if (navMeshAgent == null)
        {
            return false;
        }


        if (!navMeshAgent.enabled)
        {
            return false;
        }


        bool foundPosition =
            NavMesh.SamplePosition(
                transform.position,
                out NavMeshHit hit,
                navMeshSampleDistance,
                NavMesh.AllAreas
            );


        if (!foundPosition)
        {
            return false;
        }


        bool warped =
            navMeshAgent.Warp(
                hit.position
            );


        if (!warped)
        {
            return false;
        }


        navMeshAgent.updatePosition =
            false;


        navMeshAgent.updateRotation =
            false;


        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped =
                false;
        }


        hasNavDestination =
            false;


        return true;
    }


    // =====================================================
    // RESET NAVIGATION
    // =====================================================

    private void ResetNavigation()
    {
        hasNavDestination =
            false;


        if (navMeshAgent == null)
        {
            return;
        }


        if (!navMeshAgent.enabled)
        {
            return;
        }


        if (!navMeshAgent.isOnNavMesh)
        {
            return;
        }


        navMeshAgent.ResetPath();


        navMeshAgent.isStopped =
            true;
    }


    // =====================================================
    // STATE CHANGE
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


        hasNavDestination =
            false;


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
    // KNOCKBACK
    // =====================================================

    private void OnCharacterKnockback(
        CharacterBase knockedCharacter)
    {
        if (knockedCharacter != this)
        {
            return;
        }


        targetBrick =
            null;


        ResetNavigation();


        switch (currentState)
        {
            // START
            case AIState.CollectingStart:
            case AIState.MovingToBridge1:

                ChangeState(
                    AIState.CollectingStart
                );

                break;


            case AIState.CrossingBridge1:

                ChangeState(
                    AIState.ReturningFromBridge1
                );

                break;


            // MIDDLE 01
            case AIState.CollectingMiddle01:
            case AIState.MovingToBridge2:

                ChangeState(
                    AIState.CollectingMiddle01
                );

                break;


            case AIState.CrossingBridge2:

                ChangeState(
                    AIState.ReturningFromBridge2
                );

                break;


            // MIDDLE 02
            case AIState.CollectingMiddle02:
            case AIState.MovingToBridge3:

                ChangeState(
                    AIState.CollectingMiddle02
                );

                break;


            case AIState.CrossingBridge3:

                ChangeState(
                    AIState.ReturningFromBridge3
                );

                break;


            case AIState.MovingToFinish:

                ChangeState(
                    AIState.MovingToFinish
                );

                break;
        }
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
    // STOP
    // =====================================================

    private void StopAllMovement()
    {
        SetMoveDirection(
            Vector3.zero
        );


        ResetNavigation();
    }


    // =====================================================
    // DISTANCE
    // =====================================================

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


    private float GetHorizontalDistanceSquared(
        Vector3 firstPosition,
        Vector3 secondPosition)
    {
        firstPosition.y = 0f;

        secondPosition.y = 0f;


        return (
            firstPosition -
            secondPosition
        ).sqrMagnitude;
    }
}