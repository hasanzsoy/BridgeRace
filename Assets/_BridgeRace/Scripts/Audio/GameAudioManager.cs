using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Audio Source")]

    [SerializeField]
    private AudioSource audioSource;

    private AudioSource playerBrickAudioSource;
    private AudioSource aiBrickAudioSource;

    [Header("Sound Effects")]

    [SerializeField]
    private AudioClip brickCollectClip;

    [SerializeField]
    private AudioClip bridgeBuildClip;

    [SerializeField]
    private AudioClip victoryClip;

    [Header("Brick Volume")]

    [SerializeField]
    [Range(0f, 1f)]
    private float playerBrickVolume = 0.80f;

    [SerializeField]
    [Range(0f, 1f)]
    private float aiBrickVolume = 0.30f;

    [Header("Player Brick Pitch")]

    [SerializeField]
    private float minimumPitch = 0.92f;

    [SerializeField]
    private float maximumPitch = 1.08f;

    [SerializeField]
    private float pitchStep = 0.04f;

    [SerializeField]
    private float pitchResetDelay = 0.70f;

    [Header("AI Brick Sound")]

    [SerializeField]
    private float aiMinimumSoundInterval = 0.08f;

    [SerializeField]
    private float aiMinimumPitch = 0.94f;

    [SerializeField]
    private float aiMaximumPitch = 1.04f;

    private bool victorySoundPlayed;

    private float currentPlayerPitch;

    private int playerPitchDirection = 1;

    private float lastPlayerBrickTime = -100f;

    private float nextAISoundTime;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        playerBrickAudioSource = gameObject.AddComponent<AudioSource>();

        playerBrickAudioSource.playOnAwake =false;

        playerBrickAudioSource.spatialBlend = 0f;

        aiBrickAudioSource = gameObject.AddComponent<AudioSource>();

        aiBrickAudioSource.playOnAwake = false;

        aiBrickAudioSource.spatialBlend = 0f;

        currentPlayerPitch = minimumPitch;

        ApplyGameSound(AudioSettings.GameSoundEnabled);
    }

    private void OnEnable()
    {
        EventManager.OnBrickCollected += PlayBrickCollectSound;

        EventManager.OnBrickSpent += PlayBridgeBuildSound;

        EventManager.OnRaceFinished += PlayVictorySound;

        EventManager.OnGameSoundChanged += ApplyGameSound;

        EventManager.OnBrickSoundChanged += OnBrickSoundChanged;

        EventManager.OnBridgeSoundChanged += OnBridgeSoundChanged;
    }


    private void OnDisable()
    {
        EventManager.OnBrickCollected -= PlayBrickCollectSound;

        EventManager.OnBrickSpent -=PlayBridgeBuildSound;

        EventManager.OnRaceFinished -= PlayVictorySound;

        EventManager.OnGameSoundChanged -= ApplyGameSound;

        EventManager.OnBrickSoundChanged -= OnBrickSoundChanged;

        EventManager.OnBridgeSoundChanged -= OnBridgeSoundChanged;
    }

    private void PlayBrickCollectSound(CharacterBase character,int stackCount)
    {
        if (!AudioSettings.GameSoundEnabled)
        {
            return;
        }

        if (!AudioSettings.BrickSoundEnabled)
        {
            return;
        }

        if (brickCollectClip == null || character == null)
        {
            return;
        }

        if (character is PlayerController)
        {
            PlayPlayerBrickSound();

            return;
        }

        PlayAIBrickSound();
    }
    private void PlayPlayerBrickSound()
    {
        if (playerBrickAudioSource == null)
        {
            return;
        }

        float currentTime = Time.unscaledTime;

        if (currentTime - lastPlayerBrickTime > pitchResetDelay)
        {
            currentPlayerPitch = minimumPitch;

            playerPitchDirection = 1;
        }
        else
        {
            currentPlayerPitch +=pitchStep * playerPitchDirection;


            if (currentPlayerPitch >= maximumPitch)
            {
                currentPlayerPitch = maximumPitch;

                playerPitchDirection = -1;
            }


            else if (currentPlayerPitch <= minimumPitch)
            {
                currentPlayerPitch = minimumPitch;

                playerPitchDirection = 1;
            }
        }


        lastPlayerBrickTime =currentTime;

        playerBrickAudioSource.pitch = currentPlayerPitch;

        playerBrickAudioSource.PlayOneShot(brickCollectClip,playerBrickVolume);
    }

    private void PlayAIBrickSound()
    {
        if (aiBrickAudioSource == null)
        {
            return;
        }

        if (Time.unscaledTime < nextAISoundTime)
        {
            return;
        }

        nextAISoundTime = Time.unscaledTime + aiMinimumSoundInterval;

        aiBrickAudioSource.pitch = Random.Range(aiMinimumPitch,aiMaximumPitch);

        aiBrickAudioSource.PlayOneShot(brickCollectClip,aiBrickVolume);
    }


    private void PlayBridgeBuildSound(CharacterBase character,int stackCount)
    {
        if (!AudioSettings.GameSoundEnabled)
        {
            return;
        }


        if (!AudioSettings.BridgeSoundEnabled)
        {
            return;
        }

        if (audioSource == null || bridgeBuildClip == null)
        {
            return;
        }

        audioSource.pitch = 1f;

        audioSource.PlayOneShot(bridgeBuildClip);
    }

    private void PlayVictorySound()
    {
        if (!AudioSettings.GameSoundEnabled)
        {
            return;
        }

        if (victorySoundPlayed)
        {
            return;
        }

        if (audioSource == null || victoryClip == null)
        {
            return;
        }

        victorySoundPlayed =true;
        audioSource.pitch =1f;
        audioSource.PlayOneShot(victoryClip);

        Debug.Log("Victory sesi oynatıldı!");
    }

    private void ApplyGameSound(bool enabled)
    {

        AudioListener.volume =enabled ? 1f : 0f;
    }


    private void OnBrickSoundChanged(bool enabled)
    {
        if (enabled)
        {
            return;
        }

        if (playerBrickAudioSource != null)
        {
            playerBrickAudioSource.Stop();
        }

        if (aiBrickAudioSource != null)
        {
            aiBrickAudioSource.Stop();
        }
    }

    private void OnBridgeSoundChanged(bool enabled){}
}