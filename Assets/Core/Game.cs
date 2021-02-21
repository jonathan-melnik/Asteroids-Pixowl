using Unity.Entities;
using UnityEngine;

public class Game : MonoBehaviour
{
    public AsteroidSpawner asteroidSpawner;
    public ShotSpawner shotSpawner;
    public static Game instance;
    EntityManager entityManager;

    private void Awake() {
        instance = this;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Start() {
        ScreenCorners.LowerLeft.Data = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        ScreenCorners.UpperRight.Data = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        asteroidSpawner.SpawnAsteroids();
    }

    public void OnSpaceshipCollidedWithAsteroid(Vector3 asteroidPos) {
        Debug.Log(asteroidPos);
    }
}
