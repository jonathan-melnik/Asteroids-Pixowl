using UnityEngine;

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

    void Update() {

    }
}
