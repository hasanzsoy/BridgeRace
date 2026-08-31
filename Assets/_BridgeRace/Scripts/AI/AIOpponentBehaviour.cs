using UnityEngine;

public class AIOpponentBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    private readonly CharacterBase owner;
    private readonly CharacterStack ownerStack;

    private CharacterBase[] characters;


    // =====================================================
    // EASY SETTINGS
    // =====================================================

    private readonly float avoidanceRadius;
    private readonly float avoidanceStrength;


    // =====================================================
    // HARD SETTINGS
    // =====================================================

    private readonly float hardAttackRadius;
    private readonly int hardBrickAdvantage;

    private readonly float hardAttackDuration;
    private readonly float hardAttackCooldown;


    // =====================================================
    // HARD RUNTIME
    // =====================================================

    private bool hardAttackActive;

    private float hardAttackEndTime;

    private float nextHardAttackTime;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public AIOpponentBehaviour(
        CharacterBase owner,
        CharacterStack ownerStack,
        float avoidanceRadius,
        float avoidanceStrength,
        float hardAttackRadius,
        int hardBrickAdvantage,
        float hardAttackDuration = 1.20f,
        float hardAttackCooldown = 4f)
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


        this.hardAttackDuration =
            Mathf.Max(
                0.20f,
                hardAttackDuration
            );

        this.hardAttackCooldown =
            Mathf.Max(
                0f,
                hardAttackCooldown
            );


        hardAttackActive =
            false;

        hardAttackEndTime =
            0f;

        nextHardAttackTime =
            0f;


        RefreshCharacters();
    }


    // =====================================================
    // REFRESH CHARACTERS
    // =====================================================

    public void RefreshCharacters()
    {
        characters =
            Object.FindObjectsByType<CharacterBase>(
                FindObjectsSortMode.None
            );
    }


    // =====================================================
    // MOVEMENT TARGET
    // =====================================================

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


    // =====================================================
    // EASY
    // AVOID OTHER CHARACTERS
    // =====================================================

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

            away.y =
                0f;


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


    // =====================================================
    // HARD
    // ATTACK PLAYER
    // =====================================================

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
            FinishHardAttack();

            return normalTarget;
        }


        CharacterStack playerStack =
            player.GetComponent<CharacterStack>();


        if (playerStack == null)
        {
            FinishHardAttack();

            return normalTarget;
        }


        // =================================================
        // PLAYER'DA BRICK YOKSA SALDIRMANIN ANLAMI YOK
        //
        // Knockback sonrası bütün Brickler düşeceği için
        // AI burada oyuncuyu hemen bırakır.
        // =================================================

        if (playerStack.BrickCount <= 0)
        {
            FinishHardAttack();

            return normalTarget;
        }


        int brickDifference =
            ownerStack.BrickCount -
            playerStack.BrickCount;


        // =================================================
        // SALDIRI DEVAM EDİYORSA
        // =================================================

        if (hardAttackActive)
        {
            // Saldırı süresi doldu.
            if (Time.time >=
                hardAttackEndTime)
            {
                FinishHardAttack();

                return normalTarget;
            }


            // Artık oyuncuyu düşürecek kadar
            // Brick avantajımız yok.
            if (brickDifference <
                hardBrickAdvantage)
            {
                FinishHardAttack();

                return normalTarget;
            }


            return CreateAttackTarget(
                player,
                normalTarget
            );
        }


        // =================================================
        // COOLDOWN
        // =================================================

        if (Time.time <
            nextHardAttackTime)
        {
            return normalTarget;
        }


        // =================================================
        // AI'NIN BRICK AVANTAJI YETERLİ Mİ?
        // =================================================

        if (brickDifference <
            hardBrickAdvantage)
        {
            return normalTarget;
        }


        Vector3 difference =
            player.transform.position -
            owner.transform.position;

        difference.y =
            0f;


        // =================================================
        // PLAYER SALDIRI MESAFESİNDE Mİ?
        // =================================================

        if (difference.sqrMagnitude >
            hardAttackRadius *
            hardAttackRadius)
        {
            return normalTarget;
        }


        // =================================================
        // SALDIRI BAŞLAT
        // =================================================

        BeginHardAttack();


        return CreateAttackTarget(
            player,
            normalTarget
        );
    }


    // =====================================================
    // BEGIN HARD ATTACK
    // =====================================================

    private void BeginHardAttack()
    {
        hardAttackActive =
            true;


        hardAttackEndTime =
            Time.time +
            hardAttackDuration;
    }


    // =====================================================
    // FINISH HARD ATTACK
    // =====================================================

    private void FinishHardAttack()
    {
        if (!hardAttackActive)
        {
            return;
        }


        hardAttackActive =
            false;


        // AI oyuncuyu tekrar hemen hedeflemesin.
        nextHardAttackTime =
            Time.time +
            hardAttackCooldown;
    }


    // =====================================================
    // ATTACK TARGET
    // =====================================================

    private Vector3 CreateAttackTarget(
        CharacterBase player,
        Vector3 normalTarget)
    {
        Vector3 direction =
            player.transform.position -
            owner.transform.position;

        direction.y =
            0f;


        if (direction.sqrMagnitude >
            0.001f)
        {
            direction.Normalize();
        }
        else
        {
            direction =
                owner.transform.forward;
        }


        // =================================================
        // Sadece oyuncunun merkezine gitmiyoruz.
        //
        // Biraz arkasını hedefliyoruz.
        //
        // Böylece AI oyuncunun dibinde durmak yerine
        // oyuncunun içinden geçmeye çalışıyor ve
        // gerçek bir çarpışma oluşturuyor.
        // =================================================

        Vector3 attackTarget =
            player.transform.position +
            direction * 0.75f;


        attackTarget.y =
            normalTarget.y;


        return attackTarget;
    }


    // =====================================================
    // FIND PLAYER
    // =====================================================

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