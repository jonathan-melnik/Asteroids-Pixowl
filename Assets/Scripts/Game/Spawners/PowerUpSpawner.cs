using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;
    EntityManager _entityManager;
    Entity _powerUpEntityPrefab;
    BlobAssetStore _blobAssetStore;
    bool _canSpawn = true;

    const float INIT_SPAWN_DELAY = 4;
    const float SPAWN_MIN_DELAY = 10;
    const float SPAWN_MAX_DELAY = 15;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _powerUpEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(powerUpPrefab, settings);

        StartCoroutine(SchedulePowerUpCreation());
    }

    IEnumerator SchedulePowerUpCreation() {
        yield return new WaitForSeconds(INIT_SPAWN_DELAY);
        SpawnPowerUpAtRandomPos(GetRandomPowerUpType());
        while (_canSpawn) {
            float delay = UnityEngine.Random.Range(SPAWN_MIN_DELAY, SPAWN_MAX_DELAY);
            yield return new WaitForSeconds(delay);
            if (!_canSpawn) {
                break;
            }
            SpawnPowerUpAtRandomPos(GetRandomPowerUpType());
        }
    }

    PowerUpType[] _powerUpTypes = new PowerUpType[] { PowerUpType.Bomb, PowerUpType.Shield, PowerUpType.HomingMissile };
    private PowerUpType GetRandomPowerUpType() {
        return _powerUpTypes[Random.Range(0, _powerUpTypes.Length)];
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    void SpawnPowerUpAtRandomPos(PowerUpType type) {
        SpawnPowerUp(GetRandomScreenPosition(), type);
    }

    void SpawnPowerUp(Vector3 pos, PowerUpType type) {
        Entity powerUp = _entityManager.Instantiate(_powerUpEntityPrefab);
#if UNITY_EDITOR
        _entityManager.SetName(powerUp, "Power Up");
#endif
        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(powerUp, translation);

        var data = new PowerUpData() {
            type = type
        };
        _entityManager.AddComponentData(powerUp, data);
    }

    Vector3 GetRandomScreenPosition() {
        float x = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.x, ScreenCorners.UpperRight.Data.x);
        float y = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.y, ScreenCorners.UpperRight.Data.y);
        return new Vector3(x, y, 0);
    }

    public void StopSpawning() {
        _canSpawn = false;
    }
}
