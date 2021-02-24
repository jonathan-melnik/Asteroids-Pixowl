using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject canvas;
    public WarpDriveFx warpDriveFx;
    public CameraScreenFade cameraScreenFade;
    bool _isTransitioning = false;

    private void Start() {
        cameraScreenFade.FadeIn(CameraScreenFade.FADE_IN_TIME);
    }

    void Update() {
        if (_isTransitioning) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            TransitionToGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    void TransitionToGame() {
        _isTransitioning = true;
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 0.5f, "OnUpdate", "OnCanvasAlphaTweenUpdate"));
        warpDriveFx.SpeedUpAndFade();
        StartCoroutine(FadeOutWithDelay(0.7f));
    }

    IEnumerator FadeOutWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        float fadeOutDelay = CameraScreenFade.FADE_OUT_TIME;
        cameraScreenFade.FadeOut(fadeOutDelay);
        yield return new WaitForSeconds(fadeOutDelay);
        LoadGame();
    }

    void LoadGame() {
        SceneManager.LoadScene("Game");
    }

    void OnCanvasAlphaTweenUpdate(float alpha) {
        canvas.GetComponent<CanvasGroup>().alpha = alpha;
    }
}
