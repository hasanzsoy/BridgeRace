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

    private AIBrickTargeting brickTargeting;
    private AINavigation navigation;
    private AIBridgeTraversal bridgeTraversal;
    private IAIDifficultyStrategy difficultyStrategy;
    private AIOpponentBehaviour opponentBehaviour;


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
    // DIFFICULTY DATA - SCRIPTABLE OBJECT
    // =====================================================

    [Header("Difficulty Data")]

    [SerializeField]
    private AIDifficultyData easyDifficultyData;

    [SerializeField]
    private AIDifficultyData normalDifficultyData;

    [SerializeField]
    private AIDifficultyData hardDifficultyData;

    private AIDifficultyData currentDifficultyData;


    // =====================================================
    // NAVMESH
    // =====================================================

    [Header("NavMesh Settings")]

    [SerializeField]
    private float navMeshSampleDistance = 3f;


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

    // =====================================================
    // AWAKE
    // =====================================================

    protected override void Awake()
    {
        // CharacterBase burada:
        // Rigidbody ve CharacterBridgeBuilder'ı hazırlar.
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


        bridgeTraversal =
            new AIBridgeTraversal(
                transform,
                characterStack,

                // CharacterBase'den gelen
                // protected bridgeBuilder.
                bridgeBuilder,

                rb,
                SetMoveDirection,
                StopMovement,
                pointReachedDistance,
                bridgeProgressThreshold,
                bridgeStallTime,
                bridgeLookAhead,
                bridgeCenteringStrength
            );


        difficulty =
            GameSettings.SelectedDifficulty;


        SelectDifficultyData();


        if (currentDifficultyData == null)
        {
            Debug.LogError(
                gameObject.name +
                " için AI Difficulty Data bulunamadı!"
            );

            return;
        }


        opponentBehaviour =
            new AIOpponentBehaviour(
                this,
                characterStack,
                currentDifficultyData.avoidanceRadius,
                currentDifficultyData.avoidanceStrength,
                currentDifficultyData.attackRadius,
                currentDifficultyData.brickAdvantage
            );


        CreateDifficultyStrategy();

        ApplyDifficulty();


        navigation.Setup(
            moveSpeed
        );
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

    private void SelectDifficultyData()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:

                currentDifficultyData =
                    easyDifficultyData;

                break;


            case AIDifficulty.Normal:

                currentDifficultyData =
                    normalDifficultyData;

                break;


            case AIDifficulty.Hard:

                currentDifficultyData =
                    hardDifficultyData;

                break;
        }


        if (currentDifficultyData == null)
        {
            Debug.LogError(
                gameObject.name +
                " | " +
                difficulty +
                " ScriptableObject atanmadı!"
            );

            return;
        }


        Debug.Log(
            gameObject.name +
            " | ScriptableObject: " +
            currentDifficultyData.name
        );
    }


    private void ApplyDifficulty()
    {
        if (difficultyStrategy == null)
        {
            Debug.LogError(
                gameObject.name +
                " için Difficulty Strategy oluşturulamadı!"
            );

            return;
        }


        moveSpeed =
            difficultyStrategy.MoveSpeed;


        brickSearchInterval =
            difficultyStrategy.SearchInterval;


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
            moveSpeed +
            " | Search Mode: " +
            difficultyStrategy.SearchMode
        );
    }


    private void PrepareCollectionGoal()
    {
        if (difficultyStrategy == null)
        {
            currentBrickGoal =
                bricksNeededForBridge;

            return;
        }


        currentBrickGoal =
            difficultyStrategy.GetBrickGoal();


        Debug.Log(
            gameObject.name +
            " yeni Brick hedefi: " +
            currentBrickGoal
        );
    }


    private void CreateDifficultyStrategy()
    {
        if (currentDifficultyData == null)
        {
            Debug.LogError(
                gameObject.name +
                " için currentDifficultyData null!"
            );

            return;
        }


        switch (difficulty)
        {
            case AIDifficulty.Easy:

                difficultyStrategy =
                    new EasyDifficultyStrategy(
                        currentDifficultyData.moveSpeed,
                        currentDifficultyData.searchInterval,
                        currentDifficultyData.minBrickGoal,
                        currentDifficultyData.maxBrickGoal
                    );

                break;


            case AIDifficulty.Normal:

                difficultyStrategy =
                    new NormalDifficultyStrategy(
                        currentDifficultyData.moveSpeed,
                        currentDifficultyData.searchInterval,
                        currentDifficultyData.minBrickGoal
                    );

                break;


            case AIDifficulty.Hard:

                difficultyStrategy =
                    new HardDifficultyStrategy(
                        currentDifficultyData.moveSpeed,
                        currentDifficultyData.searchInterval,
                        currentDifficultyData.minBrickGoal
                    );

                break;
        }
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


        if (targetBrick == null)
        {
            MoveToBrickAreaCenter(
                brickArea
            );

            return;
        }


        Vector3 movementTarget =
            targetBrick.transform.position;


        if (opponentBehaviour != null &&
            difficultyStrategy != null)
        {
            movementTarget =
                opponentBehaviour.GetMovementTarget(
                    movementTarget,
                    difficultyStrategy.OpponentMode
                );
        }


        MoveUsingNavMesh(
            movementTarget
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
        if (brickTargeting == null ||
            difficultyStrategy == null)
        {
            return null;
        }


        return brickTargeting.FindTarget(
            brickArea,
            CharacterTeamColor,
            difficultyStrategy.SearchMode,
            transform.position,
            currentDifficultyData.clusterRadius
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
        if (bridgeTraversal == null)
        {
            StopMovement();

            return;
        }


        ResetNavigation();


        AIBridgeTraversal.CrossResult result =
            bridgeTraversal.Cross(
                bridgeStart,
                bridgeEnd,
                Time.deltaTime
            );


        switch (result)
        {
            case AIBridgeTraversal.CrossResult.ReachedEnd:

                SnapAgentToCurrentPosition();


                ChangeState(
                    nextState
                );

                break;


            case AIBridgeTraversal.CrossResult.NeedsReturn:

                ChangeState(
                    returnState
                );

                break;
        }
    }


    // =====================================================
    // BRIDGE RETURN
    // =====================================================

    private void ReturnAcrossBridge(
        Transform bridgeEnd,
        Transform bridgeStart,
        AIState collectingState)
    {
        if (bridgeTraversal == null)
        {
            StopMovement();

            return;
        }


        ResetNavigation();


        bool reachedStart =
            bridgeTraversal.ReturnAcross(
                bridgeEnd,
                bridgeStart
            );


        if (!reachedStart)
        {
            return;
        }


        SnapAgentToCurrentPosition();


        ChangeState(
            collectingState
        );
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


        if (bridgeTraversal != null)
        {
            bridgeTraversal.ResetProgress();
        }


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