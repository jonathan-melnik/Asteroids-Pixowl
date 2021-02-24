using System;
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
    public bool isInputEnabled { get; private set; } = true;
    public static Game instance;

    const float FADE_OUT_TO_WIN_DELAY = 1f;

    private void Awake() {
        instance = this;

        ScreenCorners.LowerLeft.Data = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        ScreenCorners.UpperRight.Data = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        cameraScreenFade.FadeIn(CameraScreenFade.FADE_IN_TIME);

        asteroidManager.asteroidDestroyed += OnAsteroidDestroyed;
        ufoManager.ufoDestroyed += OnUFODestroyed;
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

        if (isInputEnabled) {
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
    }

    public void Retry() {
        float fadeOutTime = CameraScreenFade.FADE_OUT_TIME;
        cameraScreenFade.FadeOut(fadeOutTime);
        _isRestarting = true;
        StartCoroutine(RestartAfterFadeOut(fadeOutTime));
    }

    IEnumerator RestartAfterFadeOut(float fadeOutTime) {
        yield return new WaitForSeconds(fadeOutTime);
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu() {
        LoadScene("MainMenu");
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

    void OnAsteroidDestroyed(object sender, EventArgs args) {
        CheckEndGame();
    }

    void OnUFODestroyed(object sender, EventArgs args) {
        CheckEndGame();
    }

    void CheckEndGame() {
        if (asteroidManager.asteroidsRemaining == 0 && ufoManager.ufosRemaining == 0 && !ufoManager.isUFOBeingSpawned && lives > 0) {
            ufoManager.StopSpawning();
            isInputEnabled = false;
            StartCoroutine(ScheduleFadeOut(FADE_OUT_TO_WIN_DELAY));
        }
    }

    IEnumerator ScheduleFadeOut(float delay) {
        yield return new WaitForSeconds(delay);
        var fadeOutTime = CameraScreenFade.FADE_OUT_TIME;
        cameraScreenFade.FadeOut(fadeOutTime);
        StartCoroutine(LoadGameWinWithDelay(fadeOutTime));
    }

    IEnumerator LoadGameWinWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        LoadScene("GameWin");
    }

    void LoadScene(string sceneName) {
        World.DisposeAllWorlds();
        DefaultWorldInitialization.Initialize("Default World", false);
        SceneManager.LoadScene(sceneName);
    }
}
