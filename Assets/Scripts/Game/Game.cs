using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public AsteroidSpawner asteroidSpawner;
    public ShotSpawner shotSpawner;
    public Spaceship spaceship;
    public static Game instance;

    private void Awake() {
        instance = this;

        ScreenCorners.LowerLeft.Data = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        ScreenCorners.UpperRight.Data = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
    }

    void Start() {
        asteroidSpawner.SpawnInitialAsteroids();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) { // Reiniciar juego
            World.DisposeAllWorlds();
            DefaultWorldInitialization.Initialize("Default World", false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnSpaceshipCollidedWithAsteroid(Vector3 asteroidPos, int asteroidSize) {
        if (asteroidSize > 1) {
            asteroidSpawner.ScheduleSpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }
    }

    public void OnShotCollidedWithAsteroid(Vector3 asteroidPos, int asteroidSize) {
        if (asteroidSize > 1) {
            asteroidSpawner.ScheduleSpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }
    }
}