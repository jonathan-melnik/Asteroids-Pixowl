using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootManager : MonoBehaviour
{
    public HomingMissileManager homingMissileManager;
    public ShotSpawner shotSpawner;
    Queue<ShootInfo> _shootInfoQueue = new Queue<ShootInfo>();

    public void ScheduleShoot(float3 pos, float angle, ShotType type, bool isEnemyFire) {
        var shootInfo = new ShootInfo(pos, angle, type, isEnemyFire);
        _shootInfoQueue.Enqueue(shootInfo);
    }

    private void Update() {
        while (_shootInfoQueue.Count > 0) {
            var shootInfo = _shootInfoQueue.Dequeue();
            Shoot(shootInfo.pos, shootInfo.angle, shootInfo.type, shootInfo.isEnemyFire);
        }
    }

    public void Shoot(float3 pos, float angle, ShotType type, bool isEnemyFire) {
        if (type == ShotType.Normal) {
            shotSpawner.Spawn(pos, angle, isEnemyFire);
        } else if (type == ShotType.HomingMissile) {
            homingMissileManager.Spawn(pos, angle);
        }
    }

    public void OnPickedUpHomingMissileAmmo() {
        homingMissileManager.OnPickedUpAmmo();
    }

    public void OnHomingMissileSelfDestroyed(Entity entity) {
        homingMissileManager.OnMissileSelfDestroyed(entity);
    }

    public void OnHomingMissileDestroyed(Entity entity) {
        homingMissileManager.OnMissileDestroyed(entity);
    }
}

struct ShootInfo
{
    public float3 pos;
    public float angle;
    public ShotType type;
    public bool isEnemyFire;

    public ShootInfo(float3 pos, float angle, ShotType type, bool isEnemyFire) {
        this.pos = pos;
        this.angle = angle;
        this.type = type;
        this.isEnemyFire = isEnemyFire;
    }
}