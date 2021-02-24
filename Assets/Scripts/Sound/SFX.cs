using UnityEngine;

namespace JonMelnik.Game
{
    public class SFX : MonoBehaviour
    {
        [SerializeField] GameSounds _game;
        [SerializeField] UISounds _ui;
        [SerializeField] FanfareSounds _fanfare;

        // Uso estas variables estaticas para que haya que escribir menos para refenciar a los sonidos desde otras partes del codigo
        public static GameSounds game;
        public static UISounds ui;
        public static FanfareSounds fanfare;

        private void Awake() {
            game = _game;
            ui = _ui;
            fanfare = _fanfare;
        }
    }

    [System.Serializable]
    public class GameSounds
    {
        public SpaceshipSounds spaceship;
        public AsteroidSounds asteroid;
        public UFOSounds ufo;
        public PowerUpSounds powerUp;
    }

    [System.Serializable]
    public class SpaceshipSounds
    {
        public AudioClip shootNormal;
        public AudioClip shootMissile;
        public AudioClip explode;
        public AudioClip hyperspace;
    }

    [System.Serializable]
    public class AsteroidSounds
    {
        public AudioClip explode;
    }

    [System.Serializable]
    public class UFOSounds
    {
        public AudioClip invoke;
        public AudioClip spawn;
        public AudioClip shoot;
        public AudioClip explode;
    }

    [System.Serializable]
    public class PowerUpSounds
    {
        public AudioClip missiles;
        public AudioClip bomb;
        public AudioClip shield;
    }

    [System.Serializable]
    public class UISounds
    {
        public AudioClip startGame;
        public AudioClip pressToContinue;
    }

    [System.Serializable]
    public class FanfareSounds
    {
        public AudioClip gameCompleted;
    }
}
