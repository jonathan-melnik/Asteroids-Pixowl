using EazyTools.SoundManager;
using JonMelnik.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    bool _canInteract = false;

    private void Update() {
        if (_canInteract) {
            if (Input.GetKeyDown(KeyCode.X)) {
                Game.instance.Retry();
                SoundManager.PlaySound(SFX.ui.pressToContinue);
            }
            if (Input.GetKeyDown(KeyCode.Z)) {
                Game.instance.GoToMainMenu();
                SoundManager.PlaySound(SFX.ui.pressToContinue);
            }
        }
    }

    public void Show() {
        gameObject.SetActive(true);
        OnShowTweenUpdate(0);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.5f, "delay", 0.6f,
            "OnUpdate", "OnShowTweenUpdate", "OnComplete", "OnShowTweenComplete"));
        StartCoroutine(PlaySoundWithDelay(0.5f));
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    void OnShowTweenUpdate(float alpha) {
        GetComponent<CanvasGroup>().alpha = alpha;
    }

    void OnShowTweenComplete() {
        _canInteract = true;
    }

    IEnumerator PlaySoundWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SoundManager.PlaySound(SFX.fanfare.gameOver);
    }
}
