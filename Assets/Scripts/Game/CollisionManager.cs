using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    Queue<Tuple<Entity, Entity>> _spaceshipAsteroidCollisionsQueue = new Queue<Tuple<Entity, Entity>>();
    Queue<Tuple<Entity, Entity>> _shotAsteroidCollisionsQueue = new Queue<Tuple<Entity, Entity>>();
    Queue<Tuple<Entity, Entity>> _spaceshipPowerUpCollisionsQueue = new Queue<Tuple<Entity, Entity>>();
    Queue<Tuple<Entity, Entity>> _bombAsteroidCollisionsQueue = new Queue<Tuple<Entity, Entity>>();
    EntityManager _entityManager;
    private void Start() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update() {
        while (_spaceshipAsteroidCollisionsQueue.Count > 0) {
            var pair = _spaceshipAsteroidCollisionsQueue.Dequeue();
            ResolveSpaceshipAsteroidCollision(pair.Item1, pair.Item2);
        }

        while (_shotAsteroidCollisionsQueue.Count > 0) {
            var pair = _shotAsteroidCollisionsQueue.Dequeue();
            ResolveShotAsteroidCollision(pair.Item1, pair.Item2);
        }

        while (_spaceshipPowerUpCollisionsQueue.Count > 0) {
            var pair = _spaceshipPowerUpCollisionsQueue.Dequeue();
            ResolveSpaceshipPowerUpCollision(pair.Item1, pair.Item2);
        }

        while (_bombAsteroidCollisionsQueue.Count > 0) {
            var pair = _bombAsteroidCollisionsQueue.Dequeue();
            ResolveBombAsteroidCollision(pair.Item1, pair.Item2);
        }
    }

    void ResolveSpaceshipAsteroidCollision(Entity spaceship, Entity asteroid) {
        var spaceshipSpawner = Game.instance.spaceshipSpawner;
        var asteroidSpawner = Game.instance.asteroidSpawner;
        var asteroidSize = _entityManager.GetComponentData<AsteroidData>(asteroid).size;
        var asteroidPos = _entityManager.GetComponentData<Translation>(asteroid).Value;
        var spaceshipPos = _entityManager.GetComponentData<Translation>(spaceship).Value;

        _entityManager.GetComponentData<AsteroidData>(asteroid);
        if (asteroidSize > 1) {
            asteroidSpawner.SpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }
        spaceshipSpawner.OnSpaceshipDestroyed(spaceshipPos);

        _entityManager.DestroyEntity(asteroid);
        _entityManager.DestroyEntity(spaceship);

        Game.instance.DecreaseLives();
    }

    void ResolveShotAsteroidCollision(Entity shot, Entity asteroid) {
        var asteroidSpawner = Game.instance.asteroidSpawner;
        var fxManager = Game.instance.fxManager;
        var asteroidSize = _entityManager.GetComponentData<AsteroidData>(asteroid).size;
        var asteroidPos = _entityManager.GetComponentData<Translation>(asteroid).Value;
        var shotPos = _entityManager.GetComponentData<Translation>(shot).Value;

        if (asteroidSize > 1) {
            asteroidSpawner.SpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }
        fxManager.PlayShotHitAsteroid(shotPos);

        _entityManager.DestroyEntity(shot);
        _entityManager.DestroyEntity(asteroid);
    }

    void ResolveSpaceshipPowerUpCollision(Entity spaceship, Entity powerUp) {
        var spaceshipSpawner = Game.instance.spaceshipSpawner;
        var powerUpType = _entityManager.GetComponentData<PowerUpData>(powerUp).type;

        spaceshipSpawner.OnSpaceshipPickUpPowerUp(powerUpType);

        _entityManager.DestroyEntity(powerUp);
    }

    void ResolveBombAsteroidCollision(Entity bomb, Entity asteroid) {
        _entityManager.DestroyEntity(asteroid);
    }

    public void OnSpaceshipCollidedWithAsteroid(Entity spaceship, Entity asteroid) {
        _spaceshipAsteroidCollisionsQueue.Enqueue(new Tuple<Entity, Entity>(spaceship, asteroid));
    }

    public void OnShotCollidedWithAsteroid(Entity shot, Entity asteroid) {
        _shotAsteroidCollisionsQueue.Enqueue(new Tuple<Entity, Entity>(shot, asteroid));
    }

    public void OnSpaceshipCollidedWithPowerUp(Entity spaceship, Entity powerUp) {
        _spaceshipPowerUpCollisionsQueue.Enqueue(new Tuple<Entity, Entity>(spaceship, powerUp));
    }

    public void OnBombCollidedWithAsteroid(Entity bomb, Entity asteroid) {
        _bombAsteroidCollisionsQueue.Enqueue(new Tuple<Entity, Entity>(bomb, asteroid));
    }
}
