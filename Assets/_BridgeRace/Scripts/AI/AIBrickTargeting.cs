using System.Collections.Generic;
using UnityEngine;

public class AIBrickTargeting
{
    public Brick FindTarget(
        BoxCollider brickArea,
        TeamColor teamColor,
        AIBrickSearchMode searchMode,
        Vector3 aiPosition,
        float hardClusterRadius)
    {
        List<Brick> validBricks =
            GetValidBricks(
                brickArea,
                teamColor
            );

        if (validBricks.Count == 0)
        {
            return null;
        }

        switch (searchMode)
        {
            case AIBrickSearchMode.Random:

                return FindRandomBrick(
                    validBricks
                );


            case AIBrickSearchMode.Nearest:

                return FindNearestBrick(
                    validBricks,
                    aiPosition
                );


            case AIBrickSearchMode.Cluster:

                return FindClusterBrick(
                    validBricks,
                    aiPosition,
                    hardClusterRadius
                );
        }

        return null;
    }


    public bool IsTargetValid(
        Brick brick,
        BoxCollider brickArea,
        TeamColor teamColor)
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
            teamColor)
        {
            return false;
        }

        return IsInsideArea(
            brickArea,
            brick.transform.position
        );
    }


    private List<Brick> GetValidBricks(
        BoxCollider brickArea,
        TeamColor teamColor)
    {
        List<Brick> result =
            new List<Brick>();

        if (brickArea == null)
        {
            return result;
        }


        Brick[] bricks =
            Object.FindObjectsByType<Brick>(
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
                teamColor)
            {
                continue;
            }

            if (!IsInsideArea(
                    brickArea,
                    brick.transform.position))
            {
                continue;
            }

            result.Add(brick);
        }


        return result;
    }


    private Brick FindRandomBrick(
        List<Brick> bricks)
    {
        if (bricks == null ||
            bricks.Count == 0)
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


    private Brick FindNearestBrick(
        List<Brick> bricks,
        Vector3 aiPosition)
    {
        Brick nearest =
            null;

        float bestDistance =
            Mathf.Infinity;


        foreach (Brick brick in bricks)
        {
            float distance =
                HorizontalDistanceSquared(
                    aiPosition,
                    brick.transform.position
                );


            if (distance <
                bestDistance)
            {
                bestDistance =
                    distance;

                nearest =
                    brick;
            }
        }


        return nearest;
    }


    private Brick FindClusterBrick(
        List<Brick> bricks,
        Vector3 aiPosition,
        float clusterRadius)
    {
        Brick best =
            null;

        float bestScore =
            Mathf.NegativeInfinity;


        float radiusSquared =
            clusterRadius *
            clusterRadius;


        foreach (Brick candidate in bricks)
        {
            int nearbyCount = 0;


            foreach (Brick other in bricks)
            {
                if (other == candidate)
                {
                    continue;
                }


                Vector3 difference =
                    other.transform.position -
                    candidate.transform.position;

                difference.y = 0f;


                if (difference.sqrMagnitude <=
                    radiusSquared)
                {
                    nearbyCount++;
                }
            }


            float distance =
                HorizontalDistanceSquared(
                    aiPosition,
                    candidate.transform.position
                );


            float score =
                nearbyCount * 10f -
                distance * 0.1f;


            if (score >
                bestScore)
            {
                bestScore =
                    score;

                best =
                    candidate;
            }
        }


        return best;
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


        return
            position.x >= bounds.min.x &&
            position.x <= bounds.max.x &&
            position.z >= bounds.min.z &&
            position.z <= bounds.max.z;
    }


    private float HorizontalDistanceSquared(
        Vector3 firstPosition,
        Vector3 secondPosition)
    {
        Vector3 difference =
            firstPosition -
            secondPosition;

        difference.y = 0f;

        return difference.sqrMagnitude;
    }
}