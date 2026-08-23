using TMPro;
using UnityEngine;

public class CharacterNameUI : MonoBehaviour
{
    [Header("Name Settings")]
    [SerializeField] private string displayName = "YOU";

    [Header("Reference")]
    [SerializeField] private TMP_Text nameText;


    private void Awake()
    {
        UpdateName();
    }


    private void UpdateName()
    {
        if (nameText == null)
        {
            return;
        }


        nameText.text =
            displayName;
    }
}