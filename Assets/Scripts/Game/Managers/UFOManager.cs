using EazyTools.SoundManager;
using JonMelnik.Game;
using System;
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
    public SpaceshipThrusters bigUFOThrustersPrefab;
    public SpaceshipThrusters smallUFOThrustersPrefab;
    Dictionary<Entity, SpaceshipThrusters> _thrustersByUFO = new Dictionary<Entity, SpaceshipThrusters>();
    public float minSpeed;
    public float maxSpeed;
    public int spawningCount = 0;
    bool _canScheduleSpawn = true;

    delegate void Eventhandler(object sender, EventArgs args);
    public event EventHandler ufoDestroyed;

    const float INIT_DELAY = 4f;
    const float SPAWN_MIN_DELAY = 10f;
    const float SPAWN_MAX_DELAY = 16f;
    const float INVOKE_TIME = 2.5f;
    const float SMALL_SIZE_CHANCE = 0.6f;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _bigUFOEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bigUFOPrefab, settings);
        _smallUFOEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(smallUFOPrefab, settings);

        StartCoroutine(ScheduleSpawning());
    }

    private void OnDestroy() {
        _blobAssetStore.Dispose();
    }

    System.Collections.IEnumerator ScheduleSpawning() {
        yield return new WaitForSeconds(INIT_DELAY);
        SpawnUFOAtRandomPos(1);
        while (_canScheduleSpawn) {
            float delay = UnityEngine.Random.Range(SPAWN_MIN_DELAY, SPAWN_MAX_DELAY);
            yield return new WaitForSeconds(delay);
            if (!_canScheduleSpawn) {
                break;
            }
            int size = UnityEngine.Random.value < SMALL_SIZE_CHANCE ? 1 : 2;
            SpawnUFOAtRandomPos(size);
        }
    }

    public void SpawnUFOAtRandomPos(int size = 1) {
        if (!_canScheduleSpawn) {
            return;
        }
        StartCoroutine(InvokeSpawnUFO(GetRandomScreenPosition(), size));
    }

    System.Collections.IEnumerator InvokeSpawnUFO(Vector3 pos, int size) {
        spawningCount++;
        Game.instance.fxManager.SpawnEnemyApproaching(pos, INVOKE_TIME);
        SoundManager.PlaySound(SFX.game.ufo.invoke, 0.2f);
        yield return new WaitForSeconds(INVOKE_TIME);
        SpawnUFO(pos, size);
        spawningCount--;
    }

    void SpawnUFO(Vector3 pos, int size) {
        var prefab = GetEntityPrefabWithSize(size);
        Entity ufo = _entityManager.Instantiate(prefab);
#if UNITY_EDITOR
        _entityManager.SetName(ufo, "UFO");
#endif
        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(ufo, translation);

        var movement = _entityManager.GetComponentData<MovementData>(ufo);
        float angle = math.radians(UnityEngine.Random.Range(0, 360f));
        float angleOffseted = -angle + math.radians(movement.angleOffset);
        float3 direction = new float3(math.cos(angleOffseted), math.sin(angleOffseted), 0);
        movement.velocity = direction * UnityEngine.Random.Range(minSpeed, maxSpeed);
        movement.angle = angle;
        movement.maxSpeed = maxSpeed;
        _entityManager.AddComponentData(ufo, movement);

        var control = _entityManager.GetComponentData<UFOControlData>(ufo);
        control.startTurnTimer = 0.1f;
        control.turningTimer = 0;
        _entityManager.AddComponentData(ufo, control);

        var ufoData = _entityManager.GetComponentData<UFOData>(ufo);
        ufoData.shootTimer = UnityEngine.Random.Range(ufoData.minShootTime, ufoData.maxShootTime);
        _entityManager.AddComponentData(ufo, ufoData);

        _ufos.Add(ufo);

        var thrusters = Instantiate(GetThrustersPrefabWithSize(size));
        _thrustersByUFO.Add(ufo, thrusters);
        thrusters.transform.parent = transform;

        Game.instance.fxManager.PlayUFOAppears(pos);

        SoundManager.PlaySound(SFX.game.ufo.spawn);
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

        Destroy(_thrustersByUFO[ufo].gameObject);
        _thrustersByUFO.Remove(ufo);

        ufoDestroyed?.Invoke(this, EventArgs.Empty);

        SoundManager.PlaySound(SFX.game.ufo.explode);
    }

    public Entity GetRandomUFOEntity() {
        if (_ufos.Count == 0) {
            return Entity.Null;
        }
        return _ufos[UnityEngine.Random.Range(0, _ufos.Count)];
    }

    public void UpdateThrusters(Entity ufo, float3 pos, quaternion rot, bool isThrusting) {
        var thrusters = _thrustersByUFO[ufo];
        thrusters.OnSpaceshipEntityIsThrusting(isThrusting);
        thrusters.OnSpaceshipEntityMoved(pos, rot);
    }

    SpaceshipThrusters GetThrustersPrefabWithSize(int size) {
        if (size == 1) {
            return smallUFOThrustersPrefab;
        }
        return bigUFOThrustersPrefab;
    }

    public int ufosRemaining {
        get {
            return _ufos.Count;
        }
    }

    public bool isUFOBeingSpawned {
        get {
            return spawningCount > 0;
        }
    }

    public void StopSpawning() {
        _canScheduleSpawn = false;
    }
}

