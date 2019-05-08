using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

     [Header("GameObjects")]
    public Text scoreText;
    public BoxCollider2D scoreBox;
    public static int score;

    private void Awake() {
        scoreBox = gameObject.GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("Collided with " + col.name);
        // Update player score and text
        score++;
        scoreText.text = score.ToString();
    }

    public void ResetScore() {
        score = 0;
        scoreText.text = score.ToString();
    }
}
