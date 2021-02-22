using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenFade : MonoBehaviour
{
    public Material material;
    bool _isFading = false;

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (_isFading) {
            Graphics.Blit(src, dest, material);
        } else {
            Graphics.Blit(src, dest);
        }
    }

    public void FadeOut(float time) {
        _isFading = true;
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", time,
            "OnUpdate", "OnFadeTweenUpdate", "OnComplete", "OnFadeOutTweenComplete"));
    }

    public void FadeIn(float time) {
        _isFading = true;
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", time,
            "OnUpdate", "OnFadeTweenUpdate", "OnComplete", "OnFadeInTweenComplete"));
    }

    void OnFadeTweenUpdate(float t) {
        material.SetFloat("_Fade", t);
    }

    void OnFadeOutTweenComplete() {
        //isFading = false;
    }

    void OnFadeInTweenComplete() {
        _isFading = false;
    }
}
