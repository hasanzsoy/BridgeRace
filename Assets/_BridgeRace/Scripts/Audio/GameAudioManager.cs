using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip brickCollectClip;
    [SerializeField] private AudioClip bridgeBuildClip;
    [SerializeField] private AudioClip victoryClip;

    private bool victorySoundPlayed;


    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }


    private void OnEnable()
    {
        EventManager.OnBrickCollected += PlayBrickCollectSound;
        EventManager.OnBrickSpent += PlayBridgeBuildSound;
        EventManager.OnRaceFinished += PlayVictorySound;
    }


    private void OnDisable()
    {
        EventManager.OnBrickCollected -= PlayBrickCollectSound;
        EventManager.OnBrickSpent -= PlayBridgeBuildSound;
        EventManager.OnRaceFinished -= PlayVictorySound;
    }


    private void PlayBrickCollectSound(
        CharacterBase character,
        int amount)
    {
        if (audioSource == null ||
            brickCollectClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(
            brickCollectClip
        );
    }


    private void PlayBridgeBuildSound(
        CharacterBase character,
        int amount)
    {
        if (audioSource == null ||
            bridgeBuildClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(
            bridgeBuildClip
        );
    }


    private void PlayVictorySound()
    {
        if (victorySoundPlayed)
        {
            return;
        }

        if (audioSource == null ||
            victoryClip == null)
        {
            return;
        }

        victorySoundPlayed = true;

        audioSource.PlayOneShot(
            victoryClip
        );

        Debug.Log(
            "Victory sesi oynatıldı!"
        );
    }
}