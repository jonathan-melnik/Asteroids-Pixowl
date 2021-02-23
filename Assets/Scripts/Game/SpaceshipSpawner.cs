using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpaceshipSpawner : MonoBehaviour
{
    public GameObject spaceshipPrefab;
    public SpaceshipThrusters thrusters;
    public SpaceshipShield shield;
    EntityManager _entityManager;
    Entity _spaceshipEntityPrefab;
    Entity _spaceshipEntity = Entity.Null;
    BlobAssetStore _blobAssetStore;
    float _timerRespawn = 0;
    Queue<PowerUpType> _powerUpsPickedUpQueue = new Queue<PowerUpType>();

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
        if (_timerRespawn > 0) {
            _timerRespawn -= Time.deltaTime;
            if (_timerRespawn <= 0) {
                SpawnSpaceship();
            }
        }

        while (_powerUpsPickedUpQueue.Count > 0) {
            ApplyPowerUp(_powerUpsPickedUpQueue.Dequeue());
        }
    }

    public void SpawnSpaceship() {
        _spaceshipEntity = _entityManager.Instantiate(_spaceshipEntityPrefab);
        Game.instance.uiManager.hyperspace.Reset(1);

        thrusters.Activate();
        shield.ActivateAtSpawn();
    }

    public void OnSpaceshipDestroyed(Vector3 spaceshipPos) {
        Game.instance.fxManager.PlaySpaceshipExplosion(spaceshipPos);
        _spaceshipEntity = Entity.Null;
        thrusters.Deactivate();
    }

    public void ScheduleRespawn() {
        _timerRespawn = TIME_TO_RESPAWN;
    }

    public void OnSpaceshipPickUpPowerUp(PowerUpType powerUpType) {
        _powerUpsPickedUpQueue.Enqueue(powerUpType);
    }

    void ApplyPowerUp(PowerUpType powerUpType) {
        if (powerUpType == PowerUpType.Shield) {
            shield.ActivateWithPowerUp();
        } else if (powerUpType == PowerUpType.Mines) {

        } else if (powerUpType == PowerUpType.Shotgun) {

        }
    }

}
