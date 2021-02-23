using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpTextFx : MonoBehaviour
{
    public TMP_Text txt;

    const float KILL_TIME = 1;
    public void Show(PowerUpType powerUpType) {
        if (powerUpType == PowerUpType.Bomb) {
            txt.text = "";
        } else if (powerUpType == PowerUpType.HomingMissile) {
            txt.text = "MISSILES";
        } else if (powerUpType == PowerUpType.Shield) {
            txt.text = "SHIELD";
        }

        StartCoroutine(KillAfterTime());
    }

    IEnumerator KillAfterTime() {
        yield return new WaitForSeconds(KILL_TIME);
        Destroy(gameObject);
    }
}
