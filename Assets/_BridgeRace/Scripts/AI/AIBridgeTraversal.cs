using System;
using UnityEngine;

public class AIBridgeTraversal
{
    public enum CrossResult
    {
        Moving,
        ReachedEnd,
        NeedsReturn
    }


    private readonly Transform ownerTransform;
    private readonly CharacterStack characterStack;
    private readonly CharacterBridgeBuilder bridgeBuilder;
    private readonly Rigidbody rb;

    private readonly Action<Vector3> setMoveDirection;
    private readonly Action stopMovement;


    private readonly float pointReachedDistance;

    private readonly float bridgeProgressThreshold;
    private readonly float bridgeStallTime;

    private readonly float bridgeLookAhead;
    private readonly float bridgeCenteringStrength;


    private float lastBridgeDistance =
        Mathf.Infinity;

    private float bridgeStallTimer;


    public AIBridgeTraversal(
        Transform ownerTransform,
        CharacterStack characterStack,
        CharacterBridgeBuilder bridgeBuilder,
        Rigidbody rb,
        Action<Vector3> setMoveDirection,
        Action stopMovement,
        float pointReachedDistance,
        float bridgeProgressThreshold,
        float bridgeStallTime,
        float bridgeLookAhead,
        float bridgeCenteringStrength)
    {
        this.ownerTransform =
            ownerTransform;

        this.characterStack =
            characterStack;

        this.bridgeBuilder =
            bridgeBuilder;

        this.rb =
            rb;

        this.setMoveDirection =
            setMoveDirection;

        this.stopMovement =
            stopMovement;

        this.pointReachedDistance =
            pointReachedDistance;

        this.bridgeProgressThreshold =
            bridgeProgressThreshold;

        this.bridgeStallTime =
            bridgeStallTime;

        this.bridgeLookAhead =
            bridgeLookAhead;

        this.bridgeCenteringStrength =
            bridgeCenteringStrength;
    }


    public CrossResult Cross(
        Transform bridgeStart,
        Transform bridgeEnd,
        float deltaTime)
    {
        if (bridgeStart == null ||
            bridgeEnd == null)
        {
            stopMovement?.Invoke();

            return CrossResult.Moving;
        }


        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                true
            );
        }


        float distance =
            HorizontalDistance(
                ownerTransform.position,
                bridgeEnd.position
            );


        // Köprünün sonuna ulaştı.
        if (distance <=
            pointReachedDistance)
        {
            stopMovement?.Invoke();

            ResetProgress();

            return CrossResult.ReachedEnd;
        }


        // Brick kalmadı ve önümüzde
        // yapılamamış basamak var.
        if (characterStack != null &&
            characterStack.BrickCount <= 0 &&
            bridgeBuilder != null &&
            bridgeBuilder.IsForwardBlocked)
        {
            PrepareReturn();

            return CrossResult.NeedsReturn;
        }


        MoveAlongLane(
            bridgeStart,
            bridgeEnd
        );


        // AI köprüde ilerlemiyorsa ve
        // brick'i de bittiyse geri dön.
        if (CheckStall(
                distance,
                deltaTime))
        {
            PrepareReturn();

            return CrossResult.NeedsReturn;
        }


        return CrossResult.Moving;
    }


    public bool ReturnAcross(
        Transform bridgeEnd,
        Transform bridgeStart)
    {
        if (bridgeStart == null ||
            bridgeEnd == null)
        {
            stopMovement?.Invoke();

            return false;
        }


        // Geri dönüşte boş step kontrolü
        // karakteri durdurmasın.
        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                false
            );
        }


        MoveAlongLane(
            bridgeEnd,
            bridgeStart
        );


        float distance =
            HorizontalDistance(
                ownerTransform.position,
                bridgeStart.position
            );


        if (distance >
            pointReachedDistance)
        {
            return false;
        }


        stopMovement?.Invoke();


        if (bridgeBuilder != null)
        {
            bridgeBuilder.SetBridgeCheckEnabled(
                true
            );
        }


        ResetProgress();

        return true;
    }


    public void ResetProgress()
    {
        bridgeStallTimer =
            0f;

        lastBridgeDistance =
            Mathf.Infinity;
    }


    private void PrepareReturn()
    {
        stopMovement?.Invoke();


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


        ResetProgress();
    }


    private void MoveAlongLane(
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
            ownerTransform.position;

        current.y = 0f;


        Vector3 lane =
            end -
            start;


        float laneLength =
            lane.magnitude;


        if (laneLength <
            0.01f)
        {
            stopMovement?.Invoke();

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


        Vector3 centerPoint =
            start +
            laneDirection *
            progress;


        Vector3 centerCorrection =
            centerPoint -
            current;

        centerCorrection.y =
            0f;


        Vector3 forwardDirection =
            laneDirection *
            bridgeLookAhead;


        Vector3 finalDirection =
            forwardDirection +
            centerCorrection *
            bridgeCenteringStrength;


        finalDirection.y =
            0f;


        if (finalDirection.sqrMagnitude <
            0.001f)
        {
            stopMovement?.Invoke();

            return;
        }


        setMoveDirection?.Invoke(
            finalDirection.normalized
        );
    }


    private bool CheckStall(
        float distance,
        float deltaTime)
    {
        if (characterStack == null)
        {
            return false;
        }


        if (float.IsInfinity(
                lastBridgeDistance))
        {
            lastBridgeDistance =
                distance;

            return false;
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
                deltaTime;
        }
        else
        {
            bridgeStallTimer =
                0f;
        }


        lastBridgeDistance =
            distance;


        return bridgeStallTimer >=
               bridgeStallTime;
    }


    private float HorizontalDistance(
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
}