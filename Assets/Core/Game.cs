using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public AsteroidSpawner asteroidSpawner;
    public ShotSpawner shotSpawner;
    public static Game instance;

    private void Awake() {
        instance = this;
    }

    void Start() {
        ScreenCorners.LowerLeft.Data = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        ScreenCorners.UpperRight.Data = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        asteroidSpawner.SpawnAsteroids();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) { // Reiniciar juego
            World.DisposeAllWorlds();
            DefaultWorldInitialization.Initialize("Default World", false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnSpaceshipCollidedWithAsteroid(Vector3 asteroidPos) {
    }

    public void OnShotCollidedWithAsteroid(Vector3 asteroidPos) {
    }
}
