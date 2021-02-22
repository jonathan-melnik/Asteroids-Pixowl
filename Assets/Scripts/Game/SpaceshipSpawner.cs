using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpaceshipSpawner : MonoBehaviour
{
    public GameObject spaceshipPrefab;
    public SpaceshipThrusters thrusters;
    EntityManager _entityManager;
    Entity _spaceshipEntityPrefab;
    BlobAssetStore _blobAssetStore;
    bool spaceshipDestroyedFlag = false;
    float timerRespawn = 0;
    const float TIME_TO_RESPAWN = 2;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _spaceshipEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(spaceshipPrefab, settings);

        thrusters.Deactivate();
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    void Update() {
        if (spaceshipDestroyedFlag) {
            thrusters.Deactivate();
            spaceshipDestroyedFlag = false;
            timerRespawn = TIME_TO_RESPAWN;
        }

        if (timerRespawn > 0) {
            timerRespawn -= Time.deltaTime;
            if (timerRespawn <= 0) {
                SpawnSpaceship();
            }
        }
    }

    public void SpawnSpaceship() {
        Entity spaceship = _entityManager.Instantiate(_spaceshipEntityPrefab);

        Game.instance.uiManager.hyperspace.Reset(1);

        thrusters.Activate();
    }

    public void OnSpaceshipDestroyed() {
        spaceshipDestroyedFlag = true; // Uso un flag porque no puedo desactivar el gameobject de los thrusters desde el main thead
    }

}
