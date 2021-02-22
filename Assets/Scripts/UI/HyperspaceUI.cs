using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HyperspaceUI : MonoBehaviour
{
    public TMP_Text hyperspaceTxt;
    public Color txtColorHyperspaceDisabled;
    public Color txtColorHyperspaceEnabled;
    public Transform progressBar;
    float lastProgress;

    void Awake() {
        //Reset(0);
    }

    public void Reset(float progress) {
        lastProgress = progress;
        //SetProgress(progress);
    }

    public void SetProgress(float progress) {
        progressBar.localScale = new Vector3(1 - progress, 1, 1);

        hyperspaceTxt.color = progress == 1 ? txtColorHyperspaceEnabled : txtColorHyperspaceDisabled;

        if (progress == 1 && lastProgress != 1) {
            Debug.Log("Full");
        }
    }
}
