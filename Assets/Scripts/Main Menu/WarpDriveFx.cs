using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDriveFx : MonoBehaviour
{
    public ParticleSystem warpDrive1;
    public ParticleSystem warpDrive2;

    public void SpeedUpAndFade() {
        GetComponent<Animator>().SetTrigger("SpeedUp");
        StartCoroutine(StopWarpDrivesWithDelay(0.5f));
    }

    IEnumerator StopWarpDrivesWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        warpDrive1.Stop();
        warpDrive2.Stop();
    }
}
