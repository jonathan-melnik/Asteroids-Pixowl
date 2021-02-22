using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShotSpawner : MonoBehaviour
{
    public GameObject shotPrefab;
    EntityManager _entityManager;
    Entity _shotEntityPrefab;
    BlobAssetStore _blobAssetStore;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _shotEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(shotPrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    public void Spawn(Vector3 pos, float angle, float speed) {
        Entity shot = _entityManager.Instantiate(_shotEntityPrefab);

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(shot, translation);

        var movement = new ConstantMovementData() {
            velocity = new float3(math.cos(angle), 0, math.sin(angle)) * speed
        };
        _entityManager.AddComponentData(shot, movement);
    }
}
