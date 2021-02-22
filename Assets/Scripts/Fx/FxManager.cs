using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public ParticleSystem hyperspace;

    public void PlayHyperspace(Vector3 fromPos, Vector3 toPos) {
        hyperspace.transform.position = fromPos;
        hyperspace.transform.rotation = Quaternion.LookRotation(toPos - fromPos, Vector3.forward);
        hyperspace.Play();
    }
}
