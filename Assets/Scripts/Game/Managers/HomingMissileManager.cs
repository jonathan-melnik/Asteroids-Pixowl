using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class HomingMissileManager : MonoBehaviour
{
    public GameObject missilePrefab;
    EntityManager _entityManager;
    Entity _missileEntityPrefab;
    List<Entity> _missiles = new List<Entity>();
    BlobAssetStore _blobAssetStore;
    List<Tuple<Entity, Entity>> _missileTargetPairs = new List<Tuple<Entity, Entity>>();
    float _angleOffset = math.radians(-90);
    int ammo;

    const float TIME_TO_DIE = 4;
    const float EASE_VALUE = 0.1f;
    const float RETARGET_DELAY = 0.4f;
    const int AMMO = 3;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _missileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(missilePrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    void FixedUpdate() {
        bool missingTargets = false;
        // Hago que los misiles busquen su target
        foreach (var pair in _missileTargetPairs) {
            var missile = pair.Item1;
            var target = pair.Item2;

            // Puede pasar que el target haya sido destruido, en ese caso hago retargeting
            if (!_entityManager.Exists(target)) {
                missingTargets = true;
                continue;
            }

            var movement = _entityManager.GetComponentData<ConstantMovementData>(missile);
            var missilePos = _entityManager.GetComponentData<Translation>(missile).Value;
            var targetPos = _entityManager.GetComponentData<Translation>(target).Value;

            float speed = math.length(movement.velocity);
            float3 dir = movement.velocity / speed;
            float3 targetDir = math.normalize(targetPos - missilePos);
            dir += (targetDir - dir) * EASE_VALUE;
            dir = math.normalize(dir);
            movement.velocity = dir * speed;

            var rotation = new Rotation() {
                Value = quaternion.Euler(0, 0, math.atan2(dir.y, dir.x) + _angleOffset)
            };

            _entityManager.SetComponentData<ConstantMovementData>(missile, movement);
            _entityManager.SetComponentData<Rotation>(missile, rotation);
        }

        if (missingTargets) {
            RetargetMissiles();
        }
    }

    public void Spawn(Vector3 pos, float angle) {
        var missile = _entityManager.Instantiate(_missileEntityPrefab);
        _entityManager.SetName(missile, "Homing Missile");

        var shotData = _entityManager.GetComponentData<ShotData>(missile);

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(missile, translation);

        var rotation = new Rotation() {
            Value = quaternion.Euler(0, 0, angle + _angleOffset)
        };
        _entityManager.AddComponentData(missile, rotation);

        var movement = new ConstantMovementData() {
            velocity = new float3(math.cos(angle), math.sin(angle), 0) * shotData.speed
        };
        _entityManager.AddComponentData(missile, movement);

        _missiles.Add(missile);

        var target = GetRandomTarget();
        if (target != Entity.Null) {
            _missileTargetPairs.Add(new Tuple<Entity, Entity>(missile, target));
        }

        ammo--;
        if (ammo <= 0) {
            Game.instance.spaceshipManager.SetShotType(ShotType.Normal);
        }
    }

    public void OnMissileDestroyed(Entity missile) {
        _missiles.Remove(missile);

        var target = Entity.Null;
        for (int i = 0; i < _missileTargetPairs.Count; i++) {
            if (_missileTargetPairs[i].Item1 == missile) {
                target = _missileTargetPairs[i].Item2;
                _missileTargetPairs.RemoveAt(i);
                break;
            }
        }

        // Remuevo los pares de los misiles que estaban siguiendo al mismo targets
        bool needToRetargetMissiles = false;
        if (target != Entity.Null) {
            for (int i = 0; i < _missileTargetPairs.Count; i++) {
                if (_missileTargetPairs[i].Item2 == target) {
                    _missileTargetPairs.RemoveAt(i);
                    i--;
                    needToRetargetMissiles = true;
                }
            }
        }

        if (needToRetargetMissiles) {
            StartCoroutine(ScheduleRetargetMissiles());
        }
    }

    public void OnMissileSelfDestroyed(Entity missile) {
        if (_missiles.IndexOf(missile) >= 0) {
            _missiles.Remove(missile);

            for (int i = 0; i < _missileTargetPairs.Count; i++) {
                if (_missileTargetPairs[i].Item1 == missile) {
                    _missileTargetPairs.RemoveAt(i);
                    break;
                }
            }

            var missilePos = _entityManager.GetComponentData<Translation>(missile).Value;
            Game.instance.fxManager.PlayHomingMissileSelfDestroyed(missilePos);
        }
    }

    IEnumerator ScheduleRetargetMissiles() {
        yield return new WaitForSeconds(RETARGET_DELAY);
        RetargetMissiles();
    }

    void RetargetMissiles() {
        var newPairs = new List<Tuple<Entity, Entity>>();
        foreach (var missile in _missiles) {
            var found = false;
            foreach (var pair in _missileTargetPairs) {
                if (pair.Item1 == missile) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                var target = GetRandomTarget();
                if (target != Entity.Null) {
                    var newPair = new Tuple<Entity, Entity>(missile, target);
                    newPairs.Add(newPair);
                }
            }
        }

        _missileTargetPairs.AddRange(newPairs);
    }

    public void OnPickedUpAmmo() {
        ammo += AMMO;

        Game.instance.spaceshipManager.SetShotType(ShotType.HomingMissile);
    }

    Entity GetRandomTarget() {
        if (UnityEngine.Random.value < 0.5f) {
            var asteroid = Game.instance.asteroidManager.GetRandomAsteroidEntity();
            if (asteroid != Entity.Null) {
                return asteroid;
            }
            return Game.instance.ufoManager.GetRandomUFOEntity();
        } else {
            var ufo = Game.instance.ufoManager.GetRandomUFOEntity();
            if (ufo != Entity.Null) {
                return ufo;
            }
            return Game.instance.asteroidManager.GetRandomAsteroidEntity();
        }
    }
}
