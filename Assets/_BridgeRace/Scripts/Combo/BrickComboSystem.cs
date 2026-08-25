using UnityEngine;

public class BrickComboSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    [SerializeField] private int requiredCombo = 5;
    [SerializeField] private int bonusBrickAmount = 2;

    private PlayerController player;
    private CharacterStack characterStack;

    private int currentCombo;


    public int CurrentCombo
    {
        get
        {
            return currentCombo;
        }
    }


    public int RequiredCombo
    {
        get
        {
            return requiredCombo;
        }
    }


    private void Awake()
    {
        player =
            GetComponent<PlayerController>();

        characterStack =
            GetComponent<CharacterStack>();


        if (player == null)
        {
            Debug.LogError(
                "BrickComboSystem yalnızca Player üzerinde kullanılmalı!"
            );
        }


        if (characterStack == null)
        {
            Debug.LogError(
                "BrickComboSystem için CharacterStack bulunamadı!"
            );
        }
    }


    private void OnEnable()
    {
        EventManager.OnBrickCollected +=
            OnBrickCollected;

        EventManager.OnCharacterKnockback +=
            OnCharacterKnockback;
    }


    private void OnDisable()
    {
        EventManager.OnBrickCollected -=
            OnBrickCollected;

        EventManager.OnCharacterKnockback -=
            OnCharacterKnockback;
    }


    private void OnBrickCollected(
        CharacterBase character,
        int amount)
    {
        if (character != player)
        {
            return;
        }


        currentCombo +=
            amount;


        Debug.Log(
            "Combo: " +
            currentCombo +
            "/" +
            requiredCombo
        );


        if (currentCombo >=
            requiredCombo)
        {
            GiveComboReward();
        }
    }


    private void OnCharacterKnockback(
        CharacterBase knockedCharacter)
    {
        if (knockedCharacter !=
            player)
        {
            return;
        }


        ResetCombo();
    }


    private void GiveComboReward()
    {
        if (characterStack == null)
        {
            return;
        }

        currentCombo =
            0;

        characterStack.AddBonusBricks(
            bonusBrickAmount
        );

        Debug.Log(
            "COMBO TAMAMLANDI! +" +
            bonusBrickAmount +
            " bonus brick!"
        );
    }


    private void ResetCombo()
    {
        currentCombo =
            0;


        Debug.Log(
            "Combo sıfırlandı!"
        );
    }
}