using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Movement Animation")]
    [SerializeField] private float runningThreshold = 0.1f;

    [Header("Victory Animation")]
    [SerializeField] private float victoryDelay = 0.65f;

    private Rigidbody rb;
    private CharacterBase character;

    private bool victoryStarted;


    private static readonly int IsRunningHash =
        Animator.StringToHash("IsRunning");

    private static readonly int VictoryHash =
        Animator.StringToHash("Victory");


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        character =
            GetComponent<CharacterBase>();


        if (rb == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde Rigidbody bulunamadı!"
            );
        }


        if (character == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterBase bulunamadı!"
            );
        }


        if (animator == null)
        {
            Debug.LogError(
                gameObject.name +
                " CharacterAnimationController " +
                "üzerinde Animator atanmadı!"
            );
        }
    }


    private void OnEnable()
    {
        EventManager.OnCharacterPlaced +=
            OnCharacterPlaced;
    }


    private void OnDisable()
    {
        EventManager.OnCharacterPlaced -=
            OnCharacterPlaced;
    }


    private void Update()
    {
        if (victoryStarted)
        {
            return;
        }


        UpdateMovementAnimation();
    }


    private void UpdateMovementAnimation()
    {
        if (rb == null ||
            animator == null)
        {
            return;
        }


        Vector3 horizontalVelocity =
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );


        bool isRunning =
            horizontalVelocity.magnitude >
            runningThreshold;


        animator.SetBool(
            IsRunningHash,
            isRunning
        );
    }


    private void OnCharacterPlaced(
        CharacterBase placedCharacter,
        int place)
    {
        // Event başka karaktere aitse
        // hiçbir şey yapma.
        if (placedCharacter != character)
        {
            return;
        }


        // İkinci ve üçüncü dans etmez.
        if (place != 1)
        {
            if (animator != null)
            {
                animator.SetBool(
                    IsRunningHash,
                    false
                );
            }

            return;
        }


        // Aynı Victory iki kere çalışmasın.
        if (victoryStarted)
        {
            return;
        }


        victoryStarted = true;


        StartCoroutine(
            PlayVictoryAnimation()
        );
    }


    private IEnumerator PlayVictoryAnimation()
    {
        if (animator == null)
        {
            yield break;
        }


        // Önce koşu animasyonundan çık.
        animator.SetBool(
            IsRunningHash,
            false
        );


        // RacePlacementManager karakteri
        // podiuma yaklaşık 0.6 saniyede taşıyor.
        // Önce podiuma oturmasını bekliyoruz.
        yield return new WaitForSeconds(
            victoryDelay
        );


        animator.SetTrigger(
            VictoryHash
        );


        Debug.Log(
            gameObject.name +
            " Victory Dance oynatıyor!"
        );
    }
}