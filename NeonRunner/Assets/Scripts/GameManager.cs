using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("GameObjects")]
    public GameObject player;
    public GameObject spikeObject;

    [Header("JumpHeight")]
    public float[] jumpForce;

    [Header("Properties")]
    [SerializeField]
    private float bigCircleRadius;
    [SerializeField]
    private float smallCircleRadius;
    [SerializeField]
    private float revolutionSpeed;
    [SerializeField]
    private float currentAngle;
    [SerializeField]
    private float gravity;

    private float revolutions;
    private float jumpOffset = 0;
    private int jumps = 0;
    private float currentJumpForce = 0;
    private float spikeHeight;



    void Start() {
        revolutions = bigCircleRadius / smallCircleRadius + 1;
        spikeHeight = spikeObject.GetComponent<Renderer>().bounds.size.x;

        for (int i = 0; i < 6; i++) {
            GameObject spike = Instantiate(spikeObject);
            PlaceSpike(spike, Mathf.FloorToInt(i / 2) + 1);
        }
    }


    void Update() {
        HandleJumps();
        HandleRevolution(Time.deltaTime);
        HandleSpikes();
    }


    void PlaceSpike(GameObject spike, int quadrant) {
        float totalRadius = bigCircleRadius + spikeHeight / 2 - 0.01f;
        int randomAngle = Random.Range(quadrant * 90, (quadrant + 1) * 90);
        spike.transform.eulerAngles = new Vector3(0, 0, randomAngle);
        float radians = randomAngle * Mathf.Deg2Rad;
        spike.transform.position = new Vector2(totalRadius * Mathf.Cos(radians), totalRadius * Mathf.Sin(radians));
        spike.GetComponent<Spike>().isApproaching = false;
        spike.GetComponent<Spike>().quadrant = quadrant;
    }


    void HandleJumps() {
        if (Input.GetButtonDown("Fire1")) {
            if (jumps < 2) {
                jumps++;
                currentJumpForce = jumpForce[jumps - 1];
            }
        }

        if (jumps > 0) {
            jumpOffset += currentJumpForce;
            currentJumpForce -= gravity;
            if (jumpOffset < 0) {
                jumpOffset = 0;
                jumps = 0;
                currentJumpForce = 0;
            }
        }
    }


    void HandleRevolution(float elapsedTime) {
        currentAngle += 360f * elapsedTime / revolutionSpeed;
        currentAngle = (currentAngle + 360) % 360;
        Vector2 newPos = player.transform.position;
        float radians = currentAngle * Mathf.Deg2Rad;
        float totalRadius = bigCircleRadius + smallCircleRadius + jumpOffset;

        newPos.x = totalRadius * Mathf.Cos(radians);
        newPos.y = totalRadius * Mathf.Sin(radians);
        player.transform.position = newPos;
        player.transform.Rotate(0, 0, 360f * revolutions * elapsedTime / revolutionSpeed);
    }


    void HandleSpikes() {
        GameObject[] spikes = GameObject.FindGameObjectsWithTag("Spike");
        foreach (GameObject spike in spikes) {
            float spikeAngle = spike.transform.localRotation.eulerAngles.z;
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(spikeAngle, currentAngle));

            if (angleDiff < 10 && !spike.GetComponent<Spike>().isApproaching) {
                spike.GetComponent<Spike>().isApproaching = true;
            }

            if (angleDiff > 20 && spike.GetComponent<Spike>().isApproaching) {
                PlaceSpike(spike, (spike.GetComponent<Spike>().quadrant - 3) % 4);
            }

            if (spike.GetComponent<Spike>().isApproaching) {
                CheckSpikeCollision(spikeAngle);
            }
        }
    }


    void CheckSpikeCollision(float spikeAngle) {
        float totalSpikeHeight = bigCircleRadius + spikeHeight / 2;
        float radians = spikeAngle * Mathf.Deg2Rad;
        Vector2 spikeTop = new Vector2(totalSpikeHeight * Mathf.Cos(radians), totalSpikeHeight * Mathf.Sin(radians));
        float minDistance = smallCircleRadius;
        float distance = Vector2.Distance(spikeTop, player.transform.position);
        if (distance < minDistance) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
