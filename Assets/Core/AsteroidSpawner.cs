using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    EntityManager _entityManager;
    Entity _asteroidEntityPrefab;
    BlobAssetStore _blobAssetStore;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _asteroidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidPrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    public void SpawnAsteroids() {
        SpawnAsteroid();
        SpawnAsteroid();
        SpawnAsteroid();
    }

    void SpawnAsteroid() {
        Entity asteroid = _entityManager.Instantiate(_asteroidEntityPrefab);

        float x = Random.Range(ScreenCorners.LowerLeft.Data.x, ScreenCorners.UpperRight.Data.x);
        float z = Random.Range(ScreenCorners.LowerLeft.Data.z, ScreenCorners.UpperRight.Data.z);
        var translation = new Translation() {
            Value = new Vector3(x, 0, z)
        };
        _entityManager.AddComponentData(asteroid, translation);

        var movement = new MovementData() {
            speed = 10,
            angle = Random.Range(0, 360f),
            angleOffset = Random.Range(0, 360f)
        };
        _entityManager.AddComponentData(asteroid, movement);
    }
}
