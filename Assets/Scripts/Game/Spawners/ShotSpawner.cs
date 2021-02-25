using EazyTools.SoundManager;
using JonMelnik.Game;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShotSpawner : MonoBehaviour
{
    public GameObject shotPrefab;
    public GameObject ufoShotPrefab;
    EntityManager _entityManager;
    Entity _shotEntityPrefab;
    Entity _ufoShotEntityPrefab;
    BlobAssetStore _blobAssetStore;
    float _angleOffset = math.radians(90);

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _shotEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(shotPrefab, settings);
        _ufoShotEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ufoShotPrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    public void Spawn(Vector3 pos, float angle, bool isFromEnemy) {
        Entity shot = _entityManager.Instantiate(isFromEnemy ? _ufoShotEntityPrefab : _shotEntityPrefab);
#if UNITY_EDITOR
        _entityManager.SetName(shot, "Shot");
#endif

        var shotData = _entityManager.GetComponentData<ShotData>(shot);

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(shot, translation);

        var rotation = new Rotation() {
            Value = quaternion.Euler(0, 0, angle + _angleOffset)
        };
        _entityManager.AddComponentData(shot, rotation);

        var movement = new ConstantMovementData() {
            velocity = new float3(math.cos(angle), math.sin(angle), 0) * shotData.speed
        };
        _entityManager.AddComponentData(shot, movement);

        if (!isFromEnemy) {
            SoundManager.PlaySound(SFX.game.spaceship.shootNormal, 0.7f);
        } else {
            SoundManager.PlaySound(SFX.game.ufo.shoot);
        }
    }
}
