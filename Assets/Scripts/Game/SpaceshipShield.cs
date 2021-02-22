using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpaceshipShield : MonoBehaviour
{
    float _timer;

    const float SPAWN_TIME = 3;
    const float POWER_UP_TIME = 7;

    void Update() {
        if (_timer > 0) {
            _timer -= Time.deltaTime;
            if (_timer <= 0) {
                Deactivate();
            }
        }
    }

    public void ActivateAtSpawn() {
        gameObject.SetActive(true);
        _timer = SPAWN_TIME;
    }

    public void ActivateWithPowerUp() {
        gameObject.SetActive(true);
        _timer = POWER_UP_TIME;
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }

    public void OnSpaceshipEntityMoved(float3 pos) {
        transform.position = pos;
    }

    public bool IsActive() {
        return _timer > 0;
    }
}
