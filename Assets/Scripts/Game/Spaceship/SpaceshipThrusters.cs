using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpaceshipThrusters : MonoBehaviour
{
    public ParticleSystem thrusters;
    bool _wasThrusting = false;

    void Awake() {
        thrusters.Stop();
    }

    public void OnSpaceshipEntityMoved(float3 pos, quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    public void OnSpaceshipEntityIsThrusting(bool isThrusting) {
        if (_wasThrusting == isThrusting) {
            return;
        }
        if (isThrusting) {
            thrusters.Play();
        } else {
            thrusters.Stop();
        }
        _wasThrusting = isThrusting;
    }

    public void Activate() {
        gameObject.SetActive(true);
        thrusters.Stop();
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
