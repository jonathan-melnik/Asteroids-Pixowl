using EazyTools.SoundManager;
using JonMelnik.Game;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    public GameObject bigAsteroidPrefab;
    public GameObject mediumAsteroidPrefab;
    public GameObject smallAsteroidPrefab;
    EntityManager _entityManager;
    Entity _bigAsteroidEntityPrefab;
    Entity _mediumAsteroidEntityPrefab;
    Entity _smallAsteroidEntityPrefab;
    BlobAssetStore _blobAssetStore;
    List<Entity> _asteroids = new List<Entity>();
    public float minSpeed;
    public float maxSpeed;

    const int INIT_ASTEROIDS_COUNT = 3;

    delegate void Eventhandler(object sender, EventArgs args);
    public event EventHandler asteroidDestroyed;

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

    public void SpawnInitialAsteroids() {
        for (int i = 0; i < INIT_ASTEROIDS_COUNT; i++) {
            SpawnAsteroidAtRandomPos();
        }
    }

    void SpawnAsteroidAtRandomPos(int size = 3) {
        SpawnAsteroid(GetRandomScreenPosition(), size);
    }

    public void SpawnAsteroidsFromAsteroid(Vector3 asteroidPos, int asteroidSize) {
        SpawnAsteroid(asteroidPos, asteroidSize - 1);
        SpawnAsteroid(asteroidPos, asteroidSize - 1);
    }

    void SpawnAsteroid(Vector3 pos, int size) {
        var prefab = GetEntityPrefabWithSize(size);
        Entity asteroid = _entityManager.Instantiate(prefab);
#if UNITY_EDITOR
        _entityManager.SetName(asteroid, "Asteroid");
#endif

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(asteroid, translation);

        float angle = math.radians(UnityEngine.Random.Range(0, 360f));
        float3 direction = new float3(math.cos(angle), math.sin(angle), 0);
        var movement = new ConstantMovementData() {
            velocity = direction * UnityEngine.Random.Range(minSpeed, maxSpeed),
        };
        _entityManager.AddComponentData(asteroid, movement);

        var rotation = new Rotation() {
            Value = quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360f))
        };
        _entityManager.AddComponentData(asteroid, rotation);

        _asteroids.Add(asteroid);
    }

    Vector3 GetRandomScreenPosition() {
        float x = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.x, ScreenCorners.UpperRight.Data.x);
        float y = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.y, ScreenCorners.UpperRight.Data.y);
        return new Vector3(x, y, 0);
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

    public void OnAsteroidDestroyed(Entity asteroid) {
        _asteroids.Remove(asteroid);
        asteroidDestroyed?.Invoke(this, EventArgs.Empty);

        SoundManager.PlaySound(SFX.game.asteroid.explode);
    }

    public Entity GetRandomAsteroidEntity() {
        if (_asteroids.Count == 0) {
            return Entity.Null;
        }
        return _asteroids[UnityEngine.Random.Range(0, _asteroids.Count)];
    }

    public int asteroidsRemaining {
        get {
            return _asteroids.Count;
        }
    }
}

