using System.Collections.Generic;
using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject bigAsteroidPrefab;
    public GameObject mediumAsteroidPrefab;
    public GameObject smallAsteroidPrefab;
    EntityManager _entityManager;
    Entity _bigAsteroidEntityPrefab;
    Entity _mediumAsteroidEntityPrefab;
    Entity _smallAsteroidEntityPrefab;
    BlobAssetStore _blobAssetStore;
    Queue<AsteroidCreationInfo> _spawnAsteroidQueue = new Queue<AsteroidCreationInfo>();

    const float ASTEROID_BIG_SPEED = 10;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _bigAsteroidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bigAsteroidPrefab, settings);
        _mediumAsteroidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(mediumAsteroidPrefab, settings);
        _smallAsteroidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(smallAsteroidPrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    void Update() {
        while (_spawnAsteroidQueue.Count > 0) {
            var info = _spawnAsteroidQueue.Dequeue();
            SpawnAsteroid(info.pos, info.size);
        }
    }

    public void SpawnInitialAsteroids() {
        SpawnAsteroidAtRandomPos();
        SpawnAsteroidAtRandomPos();
        SpawnAsteroidAtRandomPos();
    }

    void SpawnAsteroidAtRandomPos(int size = 3) {
        SpawnAsteroid(GetRandomScreenPosition(), size);
    }

    public void ScheduleSpawnAsteroidsFromAsteroid(Vector3 asteroidPos, int asteroidSize) {
        var info = new AsteroidCreationInfo(asteroidPos, asteroidSize - 1);
        _spawnAsteroidQueue.Enqueue(info);
        _spawnAsteroidQueue.Enqueue(info);
    }

    void SpawnAsteroid(Vector3 pos, int size) {
        var prefab = GetEntityPrefabWithSize(size);
        Entity asteroid = _entityManager.Instantiate(prefab);

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(asteroid, translation);

        float angle = UnityEngine.Random.Range(0, 360f);
        float3 direction = new float3(math.cos(angle), 0, math.sin(angle));
        var movement = new ConstantMovementData() {
            velocity = direction * ASTEROID_BIG_SPEED,
        };
        _entityManager.AddComponentData(asteroid, movement);

        var rotation = new Rotation() {
            Value = quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0)
        };
        _entityManager.AddComponentData(asteroid, rotation);
    }

    Vector3 GetRandomScreenPosition() {
        float x = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.x, ScreenCorners.UpperRight.Data.x);
        float z = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.z, ScreenCorners.UpperRight.Data.z);
        return new Vector3(x, 0, z);
    }

    Entity GetEntityPrefabWithSize(int size) {
        if (size == 3) {
            return _bigAsteroidEntityPrefab;
        }
        if (size == 2) {
            return _mediumAsteroidEntityPrefab;
        }
        return _smallAsteroidEntityPrefab;
    }

    struct AsteroidCreationInfo
    {
        public Vector3 pos;
        public int size;

        public AsteroidCreationInfo(Vector3 pos, int size) {
            this.pos = pos;
            this.size = size;
        }
    }
}
