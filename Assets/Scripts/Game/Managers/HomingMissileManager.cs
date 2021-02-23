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

    const float TIME_TO_DIE = 5;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _missileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(missilePrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    public void Spawn(Vector3 pos, float angle, float speed) {
        var missile = _entityManager.Instantiate(_missileEntityPrefab);
        _entityManager.SetName(missile, "Homing Missile");

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(missile, translation);

        var rotation = new Rotation() {
            Value = quaternion.Euler(0, 0, angle + _angleOffset)
        };
        _entityManager.AddComponentData(missile, rotation);

        var movement = new ConstantMovementData() {
            velocity = new float3(math.cos(angle), math.sin(angle), 0) * speed
        };
        _entityManager.AddComponentData(missile, movement);

        _missiles.Add(missile);
    }

    int GetRandomAsteroidEntityIndex() {
        return Game.instance.asteroidManager.GetRandomAsteroidEntityIndex();
    }

    public void OnMissileDestroyed(Entity missile) {
        _missiles.Remove(missile);
    }

    public void OnEntitySelfDestroyed(Entity entity) {
        if (_missiles.IndexOf(entity) >= 0) {
            OnMissileDestroyed(entity);
        }
    }
}
