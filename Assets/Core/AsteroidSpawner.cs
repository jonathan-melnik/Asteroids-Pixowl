using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    EntityManager _entityManager;
    Entity _asteroidEntityPrefab;
    BlobAssetStore _blobAssetStore;

    const float ASTEROID_BIG_SPEED = 10;

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

        float x = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.x, ScreenCorners.UpperRight.Data.x);
        float z = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.z, ScreenCorners.UpperRight.Data.z);
        var translation = new Translation() {
            Value = new Vector3(x, 0, z)
        };
        _entityManager.AddComponentData(asteroid, translation);

        float angle = UnityEngine.Random.Range(0, 360f);
        var movement = new ConstantMovementData() {
            velocity = new float3(math.cos(angle), 0, math.sin(angle)) * ASTEROID_BIG_SPEED,
        };
        _entityManager.AddComponentData(asteroid, movement);

        var rotation = new Rotation() {
            Value = quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0)
        };
        _entityManager.AddComponentData(asteroid, rotation);
    }
}
