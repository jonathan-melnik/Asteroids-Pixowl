using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{
    public GameObject spaceshipPrefab;
    public SpaceshipThrusters thrusters;
    public SpaceshipShield shield;
    EntityManager _entityManager;
    Entity _spaceshipEntityPrefab;
    Entity _spaceship = Entity.Null;
    BlobAssetStore _blobAssetStore;
    float _timerRespawn = 0;

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
    }

    public void SpawnSpaceship() {
        _spaceship = _entityManager.Instantiate(_spaceshipEntityPrefab);
        _entityManager.SetName(_spaceship, "Spaceship");
        Game.instance.uiManager.hyperspace.Reset(1);

        thrusters.Activate();
        shield.ActivateAtSpawn();
    }

    public void OnSpaceshipDestroyed(Vector3 spaceshipPos) {
        Game.instance.fxManager.PlaySpaceshipExplosion(spaceshipPos);
        _spaceship = Entity.Null;
        thrusters.Deactivate();
    }

    public void ScheduleRespawn() {
        _timerRespawn = TIME_TO_RESPAWN;
    }

    public void OnSpaceshipPickUpPowerUp(PowerUpType powerUpType) {
        ApplyPowerUp(powerUpType);
    }

    void ApplyPowerUp(PowerUpType powerUpType) {
        if (powerUpType == PowerUpType.Shield) {
            shield.ActivateWithPowerUp();
        } else if (powerUpType == PowerUpType.Bomb) {
            Game.instance.bombSpawner.Spawn(GetSpaceshipPos());
        } else if (powerUpType == PowerUpType.HomingMissile) {

        }
    }

    public Vector3 GetSpaceshipPos() {
        return _entityManager.GetComponentData<Translation>(_spaceship).Value;
    }

}
