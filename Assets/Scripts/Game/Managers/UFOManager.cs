using System.Collections.Generic;
using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UFOManager : MonoBehaviour
{
    public GameObject bigUFOPrefab;
    public GameObject smallUFOPrefab;
    EntityManager _entityManager;
    Entity _bigUFOEntityPrefab;
    Entity _smallUFOEntityPrefab;
    BlobAssetStore _blobAssetStore;
    List<Entity> _ufos = new List<Entity>();

    const float UFO_BIG_SPEED = 10;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _bigUFOEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bigUFOPrefab, settings);
        _smallUFOEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(smallUFOPrefab, settings);
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    public void SpawnUFOAtRandomPos(int size = 1) {
        SpawnUFO(GetRandomScreenPosition(), size);
    }

    void SpawnUFO(Vector3 pos, int size) {
        var prefab = GetEntityPrefabWithSize(size);
        Entity ufo = _entityManager.Instantiate(prefab);
        _entityManager.SetName(ufo, "UFO");
        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(ufo, translation);

        var movement = _entityManager.GetComponentData<MovementData>(ufo);
        float angle = math.radians(UnityEngine.Random.Range(0, 360f));
        float angleOffseted = -angle + math.radians(movement.angleOffset);
        float3 direction = new float3(math.cos(angleOffseted), math.sin(angleOffseted), 0);
        movement.velocity = direction * UFO_BIG_SPEED;
        movement.angle = angle;
        _entityManager.AddComponentData(ufo, movement);

        var control = _entityManager.GetComponentData<UFOControlData>(ufo);
        control.startTurnTimer = 0.1f;
        control.turningTimer = 0;
        _entityManager.AddComponentData(ufo, control);

        var ufoData = _entityManager.GetComponentData<UFOData>(ufo);
        ufoData.shootTimer = UnityEngine.Random.Range(ufoData.minShootTime, ufoData.maxShootTime);
        _entityManager.AddComponentData(ufo, ufoData);

        _ufos.Add(ufo);
    }

    Vector3 GetRandomScreenPosition() {
        float x = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.x, ScreenCorners.UpperRight.Data.x);
        float y = UnityEngine.Random.Range(ScreenCorners.LowerLeft.Data.y, ScreenCorners.UpperRight.Data.y);
        return new Vector3(x, y, 0);
    }

    Entity GetEntityPrefabWithSize(int size) {
        if (size == 2) {
            return _bigUFOEntityPrefab;
        }
        return _smallUFOEntityPrefab;
    }

    public void OnUFODestroyed(Entity ufo) {
        _ufos.Remove(ufo);
    }

    public Entity GetRandomUFOEntity() {
        if (_ufos.Count == 0) {
            return Entity.Null;
        }
        return _ufos[UnityEngine.Random.Range(0, _ufos.Count)];
    }
}

