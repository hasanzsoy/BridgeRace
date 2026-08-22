using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceResultController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform podiumCameraPoint;
    [SerializeField] private float cameraMoveDuration = 0.8f;


    [Header("Confetti")]
    [SerializeField] private ParticleSystem winnerConfetti;


    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject joystickRoot;


    [Header("Level")]
    [SerializeField] private LevelManager levelManager;


    [Header("Timing")]
    [SerializeField] private float confettiDelay = 0.5f;
    [SerializeField] private float victoryDelay = 0.5f;


    private bool resultShown;
    private bool loadingNextLevel;


    private void Awake()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }


        if (winnerConfetti != null)
        {
            winnerConfetti.Stop(
                true,
                ParticleSystemStopBehavior
                    .StopEmittingAndClear
            );
        }
    }


    private void OnEnable()
    {
        EventManager.OnRaceFinished +=
            OnRaceFinished;
    }


    private void OnDisable()
    {
        EventManager.OnRaceFinished -=
            OnRaceFinished;
    }


    private void OnRaceFinished()
    {
        if (resultShown)
        {
            return;
        }


        resultShown = true;


        StartCoroutine(
            ShowRaceResult()
        );
    }


    private IEnumerator ShowRaceResult()
    {
        // Joystick kapanır.
        if (joystickRoot != null)
        {
            joystickRoot.SetActive(false);
        }


        // Oyuncu takip kamerası kapanır.
        if (cameraFollow != null)
        {
            cameraFollow.SetFollowEnabled(false);
        }


        // Kamera podiuma gider.
        if (mainCamera != null &&
            podiumCameraPoint != null)
        {
            mainCamera.transform.DOKill();


            mainCamera.transform.DOMove(
                    podiumCameraPoint.position,
                    cameraMoveDuration
                )
                .SetEase(Ease.InOutQuad);


            mainCamera.transform
                .DORotateQuaternion(
                    podiumCameraPoint.rotation,
                    cameraMoveDuration
                )
                .SetEase(Ease.InOutQuad);
        }


        // Karakterlerin podiuma
        // yerleşmesini bekle.
        yield return new WaitForSeconds(
            confettiDelay
        );


        // Konfeti.
        if (winnerConfetti != null)
        {
            winnerConfetti.Play();
        }


        yield return new WaitForSeconds(
            victoryDelay
        );


        // Victory ekranı.
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }


    public void ContinueToNextLevel()
    {
        // Butona hızlıca iki kere basılırsa
        // Level iki defa artmasın.
        if (loadingNextLevel)
        {
            return;
        }


        loadingNextLevel = true;


        // ======================================
        // LEVEL NUMARASINI ARTIR VE KAYDET
        // ======================================

        if (levelManager != null)
        {
            levelManager.CompleteLevel();
        }
        else
        {
            Debug.LogError(
                "RaceResultController üzerinde " +
                "LevelManager atanmadı!"
            );
        }


        Time.timeScale = 1f;


        // Şimdilik sadece bir bölümümüz var.
        // Aynı sahneyi tekrar açıyoruz.
        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }
}