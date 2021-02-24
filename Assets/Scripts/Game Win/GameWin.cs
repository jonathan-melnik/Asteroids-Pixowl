using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{
    public CameraScreenFade cameraScreenFade;
    bool _isTransitioning = false;

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
