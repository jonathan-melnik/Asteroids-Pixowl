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
    List<Tuple<Entity, Entity>> _missileAsteroidPairs = new List<Tuple<Entity, Entity>>();
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
        foreach (var pair in _missileAsteroidPairs) {
            var missile = pair.Item1;
            var asteroid = pair.Item2;

            if (!_entityManager.Exists(asteroid)) {
                missingTargets = true;
                continue;
            }

            var movement = _entityManager.GetComponentData<ConstantMovementData>(missile);
            var missilePos = _entityManager.GetComponentData<Translation>(missile).Value;
            var asteroidPos = _entityManager.GetComponentData<Translation>(asteroid).Value;

            float speed = math.length(movement.velocity);
            float3 dir = movement.velocity / speed;
            float3 targetDir = math.normalize(asteroidPos - missilePos);
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

        var asteroid = Game.instance.asteroidManager.GetRandomAsteroidEntity();
        if (asteroid != Entity.Null) {
            _missileAsteroidPairs.Add(new Tuple<Entity, Entity>(missile, asteroid));
        }

        ammo--;
        if (ammo <= 0) {
            Game.instance.spaceshipManager.SetShotType(ShotType.Normal);
        }
    }

    public void OnMissileDestroyed(Entity missile) {
        _missiles.Remove(missile);

        var asteroidTarget = Entity.Null;
        for (int i = 0; i < _missileAsteroidPairs.Count; i++) {
            if (_missileAsteroidPairs[i].Item1 == missile) {
                asteroidTarget = _missileAsteroidPairs[i].Item2;
                _missileAsteroidPairs.RemoveAt(i);
                break;
            }
        }

        // Remuevo los pares de los misiles que estaban siguiendo al mismo asteroid
        var newPairs = new List<Tuple<Entity, Entity>>();
        if (asteroidTarget != Entity.Null) {
            for (int i = 0; i < _missileAsteroidPairs.Count; i++) {
                if (_missileAsteroidPairs[i].Item2 == asteroidTarget) {
                    var currMissile = _missileAsteroidPairs[i].Item1;
                    _missileAsteroidPairs.RemoveAt(i);
                    i--;
                }
            }
        }

        StartCoroutine(ScheduleRetargetMissiles());
    }

    public void OnMissileSelfDestroyed(Entity missile) {
        if (_missiles.IndexOf(missile) >= 0) {
            _missiles.Remove(missile);

            for (int i = 0; i < _missileAsteroidPairs.Count; i++) {
                if (_missileAsteroidPairs[i].Item1 == missile) {
                    _missileAsteroidPairs.RemoveAt(i);
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
            foreach (var pair in _missileAsteroidPairs) {
                if (pair.Item1 == missile) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                var asteroid = Game.instance.asteroidManager.GetRandomAsteroidEntity();
                if (asteroid != Entity.Null) {
                    var newPair = new Tuple<Entity, Entity>(missile, asteroid);
                    newPairs.Add(newPair);
                }
            }
        }

        _missileAsteroidPairs.AddRange(newPairs);
    }

    public void OnPickedUpAmmo() {
        ammo += AMMO;

        Game.instance.spaceshipManager.SetShotType(ShotType.HomingMissile);
    }
}
