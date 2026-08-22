using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceResultController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private CameraFollow cameraFollow;

    [SerializeField]
    private Transform podiumCameraPoint;

    [SerializeField]
    private float cameraMoveDuration = 0.8f;

    [Header("Confetti")]
    [SerializeField]
    private ParticleSystem winnerConfetti;

    [Header("UI")]
    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private GameObject joystickRoot;

    [Header("Timing")]
    [SerializeField]
    private float confettiDelay = 0.5f;

    [SerializeField]
    private float victoryDelay = 0.5f;

    private bool resultShown;

    private void Awake()
    {
        
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(
                false
            );
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
       
        if (joystickRoot != null)
        {
            joystickRoot.SetActive(
                false
            );
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetFollowEnabled(
                false
            );
        }

        if (mainCamera != null &&
            podiumCameraPoint != null)
        {
            mainCamera.transform.DOKill();


            mainCamera.transform.DOMove(
                    podiumCameraPoint.position,
                    cameraMoveDuration
                )
                .SetEase(
                    Ease.InOutQuad
                );


            mainCamera.transform
                .DORotateQuaternion(
                    podiumCameraPoint.rotation,
                    cameraMoveDuration
                )
                .SetEase(
                    Ease.InOutQuad
                );
        }

        yield return new WaitForSeconds(
            confettiDelay
        );

        if (winnerConfetti != null)
        {
            winnerConfetti.Play();
        }
        yield return new WaitForSeconds(
            victoryDelay
        );

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(
                true
            );
        }
    }

    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f;

        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }
}