using EazyTools.SoundManager;
using JonMelnik.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{
    public CameraScreenFade cameraScreenFade;
    bool _isTransitioning = false;

    void Start() {
        SoundManager.PlaySound(SFX.fanfare.gameCompleted);
        cameraScreenFade.FadeIn(CameraScreenFade.FADE_IN_TIME);
    }

    void Update() {
        if (_isTransitioning) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            StartCoroutine(TransitionToMainMenu());
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    IEnumerator TransitionToMainMenu() {
        SoundManager.PlaySound(SFX.ui.pressToContinue);
        _isTransitioning = true;
        float fadeOutDelay = CameraScreenFade.FADE_OUT_TIME;
        cameraScreenFade.FadeOut(fadeOutDelay);
        yield return new WaitForSeconds(fadeOutDelay);
        LoadMainMenu();
    }

    void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
