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


    private void Start()
    {
        EventManager.ComboChanged(
            currentCombo,
            requiredCombo
        );
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
        int stackCount)
    {
        if (character != player)
        {
            return;
        }


        // Event'teki ikinci değer toplam StackCount.
        // Bu nedenle onu Combo'ya eklemiyoruz.
        // Her gerçek BrickCollected event'i = +1 Combo.
        currentCombo++;


        EventManager.ComboChanged(
            currentCombo,
            requiredCombo
        );


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
        if (knockedCharacter != player)
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


        EventManager.ComboCompleted(
            bonusBrickAmount
        );


        EventManager.ComboChanged(
            currentCombo,
            requiredCombo
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


        EventManager.ComboChanged(
            currentCombo,
            requiredCombo
        );


        Debug.Log(
            "Combo sıfırlandı!"
        );
    }
}