using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public AsteroidManager asteroidManager;
    public SpaceshipManager spaceshipManager;
    public BombSpawner bombSpawner;
    public UIManager uiManager;
    public CameraScreenFade cameraScreenFade;
    public CollisionManager collisionManager;
    public FxManager fxManager;
    public ShootManager shootManager;
    public UFOManager ufoManager;
    [SerializeField] int lives = 1;
    bool _checkRespawnOrGameOver = false;
    bool _isRestarting = false;
    public static Game instance;

    private void Awake() {
        instance = this;

        ScreenCorners.LowerLeft.Data = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        ScreenCorners.UpperRight.Data = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        cameraScreenFade.FadeIn(CameraScreenFade.FADE_IN_TIME);
    }

    void Start() {
        spaceshipManager.SpawnSpaceship();
        asteroidManager.SpawnInitialAsteroids();
        ufoManager.SpawnUFOAtRandomPos();
        uiManager.lives.SetCount(lives);
    }

    private void Update() {
        if (_isRestarting) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) { // Reiniciar juego
            Retry();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            bombSpawner.Spawn(spaceshipManager.GetSpaceshipPos());
        }
    }

    public void Retry() {
        float fadeOutTime = CameraScreenFade.FADE_OUT_TIME;
        cameraScreenFade.FadeOut(fadeOutTime);
        _isRestarting = true;
        StartCoroutine(RestartAfterFadeOut(fadeOutTime));
    }

    IEnumerator RestartAfterFadeOut(float fadeOutTime) {
        yield return new WaitForSeconds(fadeOutTime);
        World.DisposeAllWorlds();
        DefaultWorldInitialization.Initialize("Default World", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu() {
        World.DisposeAllWorlds();
        DefaultWorldInitialization.Initialize("Default World", false);
        SceneManager.LoadScene("MainMenu");
    }

    public void DecreaseLives() {
        lives--;
        uiManager.lives.SetCount(lives);
        if (lives > 0) {
            spaceshipManager.ScheduleRespawn();
        } else {
            uiManager.ShowGameOver();
        }
    }
}
