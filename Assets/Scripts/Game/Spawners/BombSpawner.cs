using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;
    EntityManager _entityManager;
    Entity _bombEntityPrefab;
    BlobAssetStore _blobAssetStore;

    private void Awake() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    }

    private void OnDestroy() {
        if (_blobAssetStore != null) {
            _blobAssetStore.Dispose();
        }
    }

    public void Spawn(Vector3 pos) {
        // Hack:
        // Tengo que recrear el bombEntityPrefab cada vez que instancio una bomba porque
        // en el BombSystem estoy modificando el radio de la bomba para que funcione como una onda expansiva
        // pero el problema es que al modificar el radio en el sistema se modifica el radio del prefab en vez
        // de la entidad, entonces cuando instanciaba una segunda bomba se destruia automaticamente porque el 
        // radio inicial quedaba cargado con el radio de la primera bomba que como ya exploto es mayor al radio target
        if (_blobAssetStore != null) {
            _blobAssetStore.Dispose();
        }
        _blobAssetStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _bombEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bombPrefab, settings);
        Entity bomb = _entityManager.Instantiate(_bombEntityPrefab);
        _entityManager.SetName(bomb, "Bomb");

        var translation = new Translation() {
            Value = pos
        };
        _entityManager.AddComponentData(bomb, translation);

        Game.instance.fxManager.PlayBombExplosion(pos);
    }
}
