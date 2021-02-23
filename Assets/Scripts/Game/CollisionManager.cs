using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    Queue<SpaceshipAsteroidCollision> _spaceshipAsteroidCollisionsQueue = new Queue<SpaceshipAsteroidCollision>();
    Queue<ShotAsteroidCollision> _shotAsteroidCollisionsQueue = new Queue<ShotAsteroidCollision>();
    Queue<SpaceshipPowerUpCollision> _spaceshipPowerUpCollisionsQueue = new Queue<SpaceshipPowerUpCollision>();

    private void Update() {
        while (_spaceshipAsteroidCollisionsQueue.Count > 0) {
            ResolveSpaceshipAsteroidCollision(_spaceshipAsteroidCollisionsQueue.Dequeue());
        }

        while (_shotAsteroidCollisionsQueue.Count > 0) {
            ResolveShotAsteroidCollision(_shotAsteroidCollisionsQueue.Dequeue());
        }

        while (_spaceshipPowerUpCollisionsQueue.Count > 0) {
            ResolveSpaceshipPowerUpCollision(_spaceshipPowerUpCollisionsQueue.Dequeue());
        }
    }

    void ResolveSpaceshipAsteroidCollision(SpaceshipAsteroidCollision collision) {
        var spaceshipSpawner = Game.instance.spaceshipSpawner;
        var asteroidSpawner = Game.instance.asteroidSpawner;
        if (collision.asteroidSize > 1) {
            asteroidSpawner.SpawnAsteroidsFromAsteroid(collision.asteroidPos, collision.asteroidSize);
        }
        spaceshipSpawner.OnSpaceshipDestroyed(collision.spaceshipPos);
        Game.instance.DecreaseLives();
    }

    void ResolveShotAsteroidCollision(ShotAsteroidCollision collision) {
        var asteroidSpawner = Game.instance.asteroidSpawner;
        var fxManager = Game.instance.fxManager;
        if (collision.asteroidSize > 1) {
            asteroidSpawner.SpawnAsteroidsFromAsteroid(collision.asteroidPos, collision.asteroidSize);
        }
        fxManager.PlayShotHitAsteroid(collision.shotPos);
    }

    void ResolveSpaceshipPowerUpCollision(SpaceshipPowerUpCollision collision) {
        var spaceshipSpawner = Game.instance.spaceshipSpawner;
        spaceshipSpawner.OnSpaceshipPickUpPowerUp(collision.powerUpType);
    }

    public void OnSpaceshipCollidedWithAsteroid(Vector3 asteroidPos, int asteroidSize, Vector3 spaceshipPos) {
        _spaceshipAsteroidCollisionsQueue.Enqueue(new SpaceshipAsteroidCollision(asteroidPos, asteroidSize, spaceshipPos));
    }

    public void OnShotCollidedWithAsteroid(Vector3 asteroidPos, int asteroidSize, Vector3 shotPos) {
        _shotAsteroidCollisionsQueue.Enqueue(new ShotAsteroidCollision(asteroidPos, asteroidSize, shotPos));
    }

    public void OnSpaceshipCollidedWithPowerUp(PowerUpType powerUpType) {
        _spaceshipPowerUpCollisionsQueue.Enqueue(new SpaceshipPowerUpCollision(powerUpType));
    }
}

struct SpaceshipAsteroidCollision
{
    public Vector3 asteroidPos;
    public int asteroidSize;
    public Vector3 spaceshipPos;

    public SpaceshipAsteroidCollision(Vector3 asteroidPos, int asteroidSize, Vector3 spaceshipPos) {
        this.asteroidPos = asteroidPos;
        this.asteroidSize = asteroidSize;
        this.spaceshipPos = spaceshipPos;
    }
}

struct ShotAsteroidCollision
{
    public Vector3 asteroidPos;
    public int asteroidSize;
    public Vector3 shotPos;

    public ShotAsteroidCollision(Vector3 asteroidPos, int asteroidSize, Vector3 shotPos) {
        this.asteroidPos = asteroidPos;
        this.asteroidSize = asteroidSize;
        this.shotPos = shotPos;
    }
}

struct SpaceshipPowerUpCollision
{
    public PowerUpType powerUpType;

    public SpaceshipPowerUpCollision(PowerUpType powerUpType) {
        this.powerUpType = powerUpType;
    }
}
