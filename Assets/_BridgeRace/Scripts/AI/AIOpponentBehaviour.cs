using UnityEngine;

public class AIOpponentBehaviour
{
    private readonly CharacterBase owner;
    private readonly CharacterStack ownerStack;

    private CharacterBase[] characters;

    private readonly float avoidanceRadius;
    private readonly float avoidanceStrength;

    private readonly float hardAttackRadius;
    private readonly int hardBrickAdvantage;


    public AIOpponentBehaviour(
        CharacterBase owner,
        CharacterStack ownerStack,
        float avoidanceRadius,
        float avoidanceStrength,
        float hardAttackRadius,
        int hardBrickAdvantage)
    {
        this.owner =
            owner;

        this.ownerStack =
            ownerStack;

        this.avoidanceRadius =
            avoidanceRadius;

        this.avoidanceStrength =
            avoidanceStrength;

        this.hardAttackRadius =
            hardAttackRadius;

        this.hardBrickAdvantage =
            hardBrickAdvantage;


        RefreshCharacters();
    }


    public void RefreshCharacters()
    {
        characters =
            Object.FindObjectsByType<CharacterBase>(
                FindObjectsSortMode.None
            );
    }


    public Vector3 GetMovementTarget(
        Vector3 normalTarget,
        AIOpponentMode opponentMode)
    {
        switch (opponentMode)
        {
            case AIOpponentMode.Avoid:

                return GetAvoidanceTarget(
                    normalTarget
                );


            case AIOpponentMode.Neutral:

                return normalTarget;


            case AIOpponentMode.Aggressive:

                return GetAggressiveTarget(
                    normalTarget
                );
        }


        return normalTarget;
    }


    private Vector3 GetAvoidanceTarget(
        Vector3 normalTarget)
    {
        if (characters == null ||
            owner == null)
        {
            return normalTarget;
        }


        Vector3 avoidance =
            Vector3.zero;

        int nearbyCount =
            0;


        foreach (CharacterBase character
                 in characters)
        {
            if (character == null ||
                character == owner)
            {
                continue;
            }


            Vector3 away =
                owner.transform.position -
                character.transform.position;

            away.y = 0f;


            float distance =
                away.magnitude;


            if (distance <= 0.01f ||
                distance > avoidanceRadius)
            {
                continue;
            }


            float strength =
                1f -
                distance /
                avoidanceRadius;


            avoidance +=
                away.normalized *
                strength;


            nearbyCount++;
        }


        if (nearbyCount == 0)
        {
            return normalTarget;
        }


        avoidance /=
            nearbyCount;


        Vector3 target =
            normalTarget +
            avoidance *
            avoidanceStrength;


        target.y =
            normalTarget.y;


        return target;
    }


    private Vector3 GetAggressiveTarget(
        Vector3 normalTarget)
    {
        if (characters == null ||
            owner == null ||
            ownerStack == null)
        {
            return normalTarget;
        }


        CharacterBase player =
            FindPlayer();


        if (player == null)
        {
            return normalTarget;
        }


        CharacterStack playerStack =
            player.GetComponent<CharacterStack>();


        if (playerStack == null)
        {
            return normalTarget;
        }


        int brickDifference =
            ownerStack.BrickCount -
            playerStack.BrickCount;


        if (brickDifference <
            hardBrickAdvantage)
        {
            return normalTarget;
        }


        Vector3 difference =
            player.transform.position -
            owner.transform.position;

        difference.y = 0f;


        if (difference.sqrMagnitude >
            hardAttackRadius *
            hardAttackRadius)
        {
            return normalTarget;
        }


        Vector3 attackTarget =
            player.transform.position;

        attackTarget.y =
            normalTarget.y;


        return attackTarget;
    }


    private CharacterBase FindPlayer()
    {
        if (characters == null)
        {
            return null;
        }


        foreach (CharacterBase character
                 in characters)
        {
            if (character == null ||
                character == owner)
            {
                continue;
            }


            if (character is PlayerController)
            {
                return character;
            }
        }


        return null;
    }
}