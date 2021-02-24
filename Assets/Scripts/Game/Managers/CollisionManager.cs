using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    Queue<Tuple<Entity, Entity>> _playerEnemyCollisionsQueue = new Queue<Tuple<Entity, Entity>>();
    Queue<Tuple<Entity, Entity>> _playerItemCollisionsQueue = new Queue<Tuple<Entity, Entity>>();
    EntityManager _entityManager;
    AsteroidManager _asteroidManager;
    SpaceshipManager _spaceshipManager;
    UFOManager _ufoManager;
    ShootManager _shootManager;
    FxManager _fxManager;

    private void Start() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _asteroidManager = Game.instance.asteroidManager;
        _spaceshipManager = Game.instance.spaceshipManager;
        _shootManager = Game.instance.shootManager;
        _ufoManager = Game.instance.ufoManager;
        _fxManager = Game.instance.fxManager;
    }

    private void LateUpdate() {
        while (_playerEnemyCollisionsQueue.Count > 0) {
            var pair = _playerEnemyCollisionsQueue.Dequeue();
            ResolvePlayerEnemyCollision(pair.Item1, pair.Item2);
        }

        while (_playerItemCollisionsQueue.Count > 0) {
            var pair = _playerItemCollisionsQueue.Dequeue();
            ResolvePlayerItemCollision(pair.Item1, pair.Item2);
        }
    }

    void ResolvePlayerEnemyCollision(Entity player, Entity enemy) {
        if (_entityManager.HasComponent<SpaceshipTag>(player)) {
            if (!_spaceshipManager.shield.IsActive()) {
                if (_entityManager.HasComponent<AsteroidTag>(enemy)) {
                    ResolveSpaceshipAsteroidCollision(player, enemy);
                } else if (_entityManager.HasComponent<UFOTag>(enemy)) {
                    ResolveSpaceshipUFOCollision(player, enemy);
                } else if (_entityManager.HasComponent<ShotTag>(enemy)) {
                    ResolveSpaceshipShotCollision(player, enemy);
                }
            }
        } else if (_entityManager.HasComponent<ShotTag>(player)) {
            if (_entityManager.HasComponent<AsteroidTag>(enemy)) {
                ResolveShotAsteroidCollision(player, enemy);
            } else if (_entityManager.HasComponent<UFOTag>(enemy)) {
                ResolveShotUFOCollision(player, enemy);
            }
        } else if (_entityManager.HasComponent<HomingMissileTag>(player)) {
            if (_entityManager.HasComponent<AsteroidTag>(enemy)) {
                ResolveHomingMissileAsteroidCollision(player, enemy);
            } else if (_entityManager.HasComponent<UFOTag>(enemy)) {
                ResolveHomingMissileUFOCollision(player, enemy);
            }
        } else if (_entityManager.HasComponent<BombTag>(player)) {
            if (_entityManager.HasComponent<AsteroidTag>(enemy)) {
                ResolveBombAsteroidCollision(player, enemy);
            } else if (_entityManager.HasComponent<UFOTag>(enemy)) {
                ResolveBombUFOCollision(player, enemy);
            }
        }
    }

    void ResolvePlayerItemCollision(Entity player, Entity item) {
        if (_entityManager.HasComponent<SpaceshipTag>(player)) {
            if (_entityManager.HasComponent<PowerUpTag>(item)) {
                ResolveSpaceshipPowerUpCollision(player, item);
            }
        }
    }

    void ResolveSpaceshipAsteroidCollision(Entity spaceship, Entity asteroid) {
        var asteroidSize = _entityManager.GetComponentData<AsteroidData>(asteroid).size;
        var asteroidPos = _entityManager.GetComponentData<Translation>(asteroid).Value;
        var spaceshipPos = _entityManager.GetComponentData<Translation>(spaceship).Value;

        _entityManager.GetComponentData<AsteroidData>(asteroid);
        if (asteroidSize > 1) {
            _asteroidManager.SpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }

        _entityManager.DestroyEntity(asteroid);
        _entityManager.DestroyEntity(spaceship);

        _spaceshipManager.OnSpaceshipDestroyed(spaceshipPos);
        _asteroidManager.OnAsteroidDestroyed(asteroid);

        Game.instance.DecreaseLives();
    }

    void ResolveShotAsteroidCollision(Entity shot, Entity asteroid) {
        var asteroidSize = _entityManager.GetComponentData<AsteroidData>(asteroid).size;
        var asteroidPos = _entityManager.GetComponentData<Translation>(asteroid).Value;
        var shotPos = _entityManager.GetComponentData<Translation>(shot).Value;

        if (asteroidSize > 1) {
            _asteroidManager.SpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }
        _fxManager.PlayShotHitAsteroid(shotPos);

        _entityManager.DestroyEntity(shot);
        _entityManager.DestroyEntity(asteroid);

        _asteroidManager.OnAsteroidDestroyed(asteroid);
    }

    void ResolveSpaceshipShotCollision(Entity spaceship, Entity shot) {
        var spaceshipPos = _entityManager.GetComponentData<Translation>(spaceship).Value;

        _fxManager.PlaySpaceshipExplosion(spaceshipPos);

        _entityManager.DestroyEntity(shot);
        _entityManager.DestroyEntity(spaceship);

        _spaceshipManager.OnSpaceshipDestroyed(spaceshipPos);

        Game.instance.DecreaseLives();
    }

    void ResolveShotUFOCollision(Entity shot, Entity ufo) {
        var ufoPos = _entityManager.GetComponentData<Translation>(ufo).Value;
        var ufoSize = _entityManager.GetComponentData<UFOData>(ufo).size;

        if (ufoSize == 1) {
            _fxManager.PlayUFOExplosionSmall(ufoPos);
        } else {
            _fxManager.PlayUFOExplosionBig(ufoPos);
        }

        _entityManager.DestroyEntity(shot);
        _entityManager.DestroyEntity(ufo);

        _ufoManager.OnUFODestroyed(ufo);
    }

    void ResolveSpaceshipPowerUpCollision(Entity spaceship, Entity powerUp) {
        var powerUpType = _entityManager.GetComponentData<PowerUpData>(powerUp).type;
        var powerUpPos = _entityManager.GetComponentData<Translation>(powerUp).Value;
        _spaceshipManager.OnSpaceshipPickUpPowerUp(powerUpType);
        _fxManager.ShowPowerUpText(powerUpPos, powerUpType);

        _entityManager.DestroyEntity(powerUp);
    }

    void ResolveBombAsteroidCollision(Entity bomb, Entity asteroid) {
        _entityManager.DestroyEntity(asteroid);

        _asteroidManager.OnAsteroidDestroyed(asteroid);
    }

    void ResolveBombUFOCollision(Entity bomb, Entity ufo) {
        var ufoPos = _entityManager.GetComponentData<Translation>(ufo).Value;
        var ufoSize = _entityManager.GetComponentData<UFOData>(ufo).size;

        if (ufoSize == 1) {
            _fxManager.PlayUFOExplosionSmall(ufoPos);
        } else {
            _fxManager.PlayUFOExplosionBig(ufoPos);
        }

        _entityManager.DestroyEntity(ufo);

        _ufoManager.OnUFODestroyed(ufo);
    }

    void ResolveHomingMissileAsteroidCollision(Entity missile, Entity asteroid) {
        var asteroidSize = _entityManager.GetComponentData<AsteroidData>(asteroid).size;
        var asteroidPos = _entityManager.GetComponentData<Translation>(asteroid).Value;
        var missilePos = _entityManager.GetComponentData<Translation>(missile).Value;

        if (asteroidSize > 1) {
            _asteroidManager.SpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }

        _fxManager.PlayHomingMissileHitAsteroid(missilePos);

        _entityManager.DestroyEntity(asteroid);
        _entityManager.DestroyEntity(missile);

        _asteroidManager.OnAsteroidDestroyed(asteroid);
        _shootManager.OnHomingMissileDestroyed(missile);
    }

    void ResolveHomingMissileUFOCollision(Entity missile, Entity ufo) {
        var ufoPos = _entityManager.GetComponentData<Translation>(ufo).Value;
        var ufoSize = _entityManager.GetComponentData<UFOData>(ufo).size;
        var missilePos = _entityManager.GetComponentData<Translation>(missile).Value;

        if (ufoSize == 1) {
            _fxManager.PlayUFOExplosionSmall(ufoPos);
        } else {
            _fxManager.PlayUFOExplosionBig(ufoPos);
        }
        _fxManager.PlayHomingMissileHitAsteroid(missilePos);

        _entityManager.DestroyEntity(ufo);
        _entityManager.DestroyEntity(missile);

        _ufoManager.OnUFODestroyed(ufo);
        _shootManager.OnHomingMissileDestroyed(missile);
    }

    void ResolveSpaceshipUFOCollision(Entity spaceship, Entity ufo) {
        var spaceshipPos = _entityManager.GetComponentData<Translation>(spaceship).Value;
        var ufoPos = _entityManager.GetComponentData<Translation>(ufo).Value;
        var ufoSize = _entityManager.GetComponentData<UFOData>(ufo).size;

        _entityManager.DestroyEntity(spaceship);
        _entityManager.DestroyEntity(ufo);

        if (ufoSize == 1) {
            _fxManager.PlayUFOExplosionSmall(ufoPos);
        } else {
            _fxManager.PlayUFOExplosionBig(ufoPos);
        }
        _spaceshipManager.OnSpaceshipDestroyed(spaceshipPos);
        _ufoManager.OnUFODestroyed(ufo);

        Game.instance.DecreaseLives();
    }

    public void OnPlayerCollidedWithEnemy(Entity player, Entity enemy) {
        _playerEnemyCollisionsQueue.Enqueue(new Tuple<Entity, Entity>(player, enemy));
    }

    public void OnPlayerCollidedWithItem(Entity player, Entity item) {
        _playerItemCollisionsQueue.Enqueue(new Tuple<Entity, Entity>(player, item));
    }
}
