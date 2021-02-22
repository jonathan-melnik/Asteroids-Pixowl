using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public HyperspaceUI hyperspace;
    public GameOver gameOver;

    public void Awake() {
        gameOver.Hide();
    }

    public void ShowGameOver() {
        gameOver.Show();
        hyperspace.Hide();
    }
}
