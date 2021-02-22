using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public AsteroidSpawner asteroidSpawner;
    public ShotSpawner shotSpawner;
    public SpaceshipSpawner spaceshipSpawner;
    public UIManager uiManager;
    public CameraScreenFade cameraScreenFade;
    public FxManager fxManager;
    public int lives = 1;
    bool _checkRespawnOrGameOver = false;
    public static Game instance;

    private void Awake() {
        instance = this;

        ScreenCorners.LowerLeft.Data = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        ScreenCorners.UpperRight.Data = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        cameraScreenFade.FadeIn(0.4f);
    }

    void Start() {
        spaceshipSpawner.SpawnSpaceship();
        asteroidSpawner.SpawnInitialAsteroids();
        uiManager.lives.SetCount(lives);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) { // Reiniciar juego
            Retry();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (_checkRespawnOrGameOver) {
            uiManager.lives.SetCount(lives);
            _checkRespawnOrGameOver = false;
            if (lives > 0) {
                spaceshipSpawner.OnSpaceshipDestroyed();
            } else {
                uiManager.ShowGameOver();
            }
        }
    }

    public void OnSpaceshipCollidedWithAsteroid(Vector3 asteroidPos, int asteroidSize) {
        if (asteroidSize > 1) {
            asteroidSpawner.ScheduleSpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }

        lives--;
        _checkRespawnOrGameOver = true; // seteo un flag y ejecutar despues en el main thread
    }

    public void OnShotCollidedWithAsteroid(Vector3 asteroidPos, int asteroidSize) {
        if (asteroidSize > 1) {
            asteroidSpawner.ScheduleSpawnAsteroidsFromAsteroid(asteroidPos, asteroidSize);
        }
    }

    public void Retry() {
        World.DisposeAllWorlds();
        DefaultWorldInitialization.Initialize("Default World", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu() {
        World.DisposeAllWorlds();
        DefaultWorldInitialization.Initialize("Default World", false);
        SceneManager.LoadScene("MainMenu");
    }
}
