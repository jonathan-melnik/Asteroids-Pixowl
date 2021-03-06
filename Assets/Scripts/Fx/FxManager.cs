using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public ParticleSystem hyperspace;
    public ParticleSystem spaceshipExplosion;
    public ParticleSystem bombExplosion;
    public ParticleSystem shotHitAsteroid;
    public ParticleSystem homingMissileHitAsteroid;
    public ParticleSystem homingMissileSelfDestroyed;
    public ParticleSystem ufoExplosionBig;
    public ParticleSystem ufoExplosionSmall;
    public PowerUpTextFx powerUpTextPrefab;
    public ParticleSystem enemyApproachingPrefab;
    public ParticleSystem ufoAppears;

    public void PlayHyperspace(Vector3 fromPos, Vector3 toPos) {
        hyperspace.transform.position = fromPos;
        hyperspace.transform.rotation = Quaternion.LookRotation(toPos - fromPos, Vector3.forward);
        hyperspace.Play();
    }

    public void PlaySpaceshipExplosion(Vector3 pos) {
        spaceshipExplosion.transform.position = pos;
        spaceshipExplosion.Play();
    }

    public void PlayShotHitAsteroid(Vector3 pos) {
        shotHitAsteroid.transform.position = pos;
        shotHitAsteroid.Play();
    }

    public void PlayBombExplosion(Vector3 pos) {
        bombExplosion.transform.position = pos;
        bombExplosion.Play();
    }

    public void PlayHomingMissileHitAsteroid(Vector3 pos) {
        homingMissileHitAsteroid.transform.position = pos;
        homingMissileHitAsteroid.Play();
    }

    public void PlayHomingMissileSelfDestroyed(Vector3 pos) {
        homingMissileSelfDestroyed.transform.position = pos;
        homingMissileSelfDestroyed.Play();
    }

    public void ShowPowerUpText(Vector3 pos, PowerUpType powerUpType) {
        var powerUpText = Instantiate(powerUpTextPrefab);
        powerUpText.transform.position = pos;
        powerUpText.Show(powerUpType);
    }

    public void PlayUFOExplosionBig(Vector3 pos) {
        ufoExplosionBig.transform.position = pos;
        ufoExplosionBig.Play();
    }

    public void PlayUFOExplosionSmall(Vector3 pos) {
        ufoExplosionSmall.transform.position = pos;
        ufoExplosionSmall.Play();
    }

    public void SpawnEnemyApproaching(Vector3 pos, float killTime) {
        var enemyApproaching = Instantiate(enemyApproachingPrefab);
        enemyApproaching.transform.position = pos;
        Destroy(enemyApproaching.gameObject, killTime);
    }

    public void PlayUFOAppears(Vector3 pos) {
        ufoAppears.transform.position = pos;
        ufoAppears.Play();
    }
}
