using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpaceshipThrusters : MonoBehaviour
{
    public ParticleSystem thrusters;
    bool wasThrusting = false;

    void Awake() {
        thrusters.Stop();
    }

    public void OnSpaceshipEntityMoved(float3 pos, quaternion rot) {
        thrusters.transform.position = pos;
        thrusters.transform.rotation = rot;
    }

    public void OnSpaceshipEntityIsThrusting(bool isThrusting) {
        if (wasThrusting == isThrusting) {
            return;
        }
        if (isThrusting) {
            thrusters.Play();
        } else {
            thrusters.Stop();
        }
        wasThrusting = isThrusting;
    }

    public void Activate() {
        gameObject.SetActive(true);
        thrusters.Stop();
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
