using System;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation
{
    private readonly Transform ownerTransform;
    private readonly NavMeshAgent navMeshAgent;

    private readonly Action<Vector3> setMoveDirection;
    private readonly Action stopMovement;

    private readonly float navMeshSampleDistance;

    private Vector3 lastNavDestination;
    private bool hasNavDestination;


    public AINavigation(
        Transform ownerTransform,
        NavMeshAgent navMeshAgent,
        float navMeshSampleDistance,
        Action<Vector3> setMoveDirection,
        Action stopMovement)
    {
        this.ownerTransform =
            ownerTransform;

        this.navMeshAgent =
            navMeshAgent;

        this.navMeshSampleDistance =
            navMeshSampleDistance;

        this.setMoveDirection =
            setMoveDirection;

        this.stopMovement =
            stopMovement;
    }


    public void Setup(
        float moveSpeed)
    {
        if (navMeshAgent == null)
        {
            return;
        }


        navMeshAgent.updatePosition =
            false;

        navMeshAgent.updateRotation =
            false;

        navMeshAgent.speed =
            moveSpeed;

        navMeshAgent.angularSpeed =
            720f;

        navMeshAgent.acceleration =
            20f;

        navMeshAgent.stoppingDistance =
            0.2f;
    }


    public void SetSpeed(
        float moveSpeed)
    {
        if (navMeshAgent == null)
        {
            return;
        }

        navMeshAgent.speed =
            moveSpeed;
    }


    public void MoveTo(
        Vector3 targetPosition)
    {
        if (navMeshAgent == null)
        {
            MoveDirectly(
                targetPosition
            );

            return;
        }


        if (!EnsureAgentOnNavMesh())
        {
            MoveDirectly(
                targetPosition
            );

            return;
        }


        Vector3 sync =
            ownerTransform.position;

        sync.y =
            navMeshAgent.nextPosition.y;

        navMeshAgent.nextPosition =
            sync;


        Vector3 destination =
            targetPosition;


        if (NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit hit,
                navMeshSampleDistance,
                NavMesh.AllAreas))
        {
            destination =
                hit.position;
        }


        bool changed =
            !hasNavDestination ||
            (
                destination -
                lastNavDestination
            ).sqrMagnitude >
            0.04f;


        if (changed)
        {
            navMeshAgent.isStopped =
                false;

            navMeshAgent.SetDestination(
                destination
            );

            lastNavDestination =
                destination;

            hasNavDestination =
                true;
        }


        if (navMeshAgent.pathPending)
        {
            return;
        }


        Vector3 direction =
            navMeshAgent.desiredVelocity;

        direction.y = 0f;


        if (direction.sqrMagnitude <
            0.001f)
        {
            stopMovement?.Invoke();

            return;
        }


        setMoveDirection?.Invoke(
            direction.normalized
        );
    }


    private void MoveDirectly(
        Vector3 target)
    {
        Vector3 direction =
            target -
            ownerTransform.position;

        direction.y = 0f;


        if (direction.sqrMagnitude <
            0.001f)
        {
            stopMovement?.Invoke();

            return;
        }


        setMoveDirection?.Invoke(
            direction.normalized
        );
    }


    public bool EnsureAgentOnNavMesh()
    {
        if (navMeshAgent == null ||
            !navMeshAgent.enabled)
        {
            return false;
        }


        if (navMeshAgent.isOnNavMesh)
        {
            return true;
        }


        return SnapToCurrentPosition();
    }


    public bool SnapToCurrentPosition()
    {
        if (navMeshAgent == null ||
            !navMeshAgent.enabled)
        {
            return false;
        }


        if (!NavMesh.SamplePosition(
                ownerTransform.position,
                out NavMeshHit hit,
                navMeshSampleDistance,
                NavMesh.AllAreas))
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

        hasNavDestination =
            false;


        return true;
    }


    public void Reset()
    {
        hasNavDestination =
            false;


        if (navMeshAgent == null ||
            !navMeshAgent.enabled ||
            !navMeshAgent.isOnNavMesh)
        {
            return;
        }


        navMeshAgent.ResetPath();

        navMeshAgent.isStopped =
            true;
    }
}