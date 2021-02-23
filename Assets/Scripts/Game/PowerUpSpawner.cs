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

    const float TIME_TO_CREATE = 4;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _powerUpEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(powerUpPrefab, settings);

        StartCoroutine(SchedulePowerUpCreation());
    }

    IEnumerator SchedulePowerUpCreation() {
        yield return new WaitForSeconds(TIME_TO_CREATE);
        SpawnPowerUpAtRandomPos(PowerUpType.Shield);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    void SpawnPowerUpAtRandomPos(PowerUpType type) {
        SpawnPowerUp(GetRandomScreenPosition(), type);
    }

    void SpawnPowerUp(Vector3 pos, PowerUpType type) {
        Entity powerUp = _entityManager.Instantiate(_powerUpEntityPrefab);

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
}
