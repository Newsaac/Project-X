using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private struct LevelSpecs {
        public readonly float bestTime;
        public readonly int enemyCnt;
        public LevelSpecs(float bestTime, int enemyCnt) : this() {
            this.bestTime = bestTime;
            this.enemyCnt = enemyCnt;
        }
    }

    [HideInInspector] public bool gameOver = true;
    public float timer = 0f;
    int enemiesLeft;

    [Header("UI")]
    [SerializeField] Button startButton;
    [SerializeField] Button restartButton;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] TextMeshProUGUI enemiesLeftText;
    //[SerializeField] TextMeshProUGUI controlGuideText;

    [Space(20)]
    public ControlsSerializable controls;

    private LevelSpecs levelSpecs;

    void Start() {
        levelSpecs = new LevelSpecs(
            PlayerPrefs.HasKey("Best") ? PlayerPrefs.GetFloat("Best") : 0, 
            10
        );
        gameOver = true;
        Time.timeScale = 0;
    }

    void Update() {
        if(!gameOver)
            timer += Time.deltaTime;
        timerText.text = "Time: " + timer.ToString("0.00");
        enemiesLeftText.text = "Enemies: " + enemiesLeft;
    }

    public void StartGame() {
        Time.timeScale = 1;
        
        timer = 0f;
        enemiesLeft = levelSpecs.enemyCnt;
        bestTimeText.text = "Best: " + (levelSpecs.bestTime == 0f ? "-" : levelSpecs.bestTime.ToString("0.00"));

        gameOver = false;
        ToggleMenuState(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GameOver() {
        if(timer < levelSpecs.bestTime && enemiesLeft == 0)
            PlayerPrefs.SetFloat("Best", timer);
        gameOver = true;
        restartButton.gameObject.SetActive(true);
    }

    void ToggleMenuState(bool mode) {
        startButton.gameObject.SetActive(mode);
        //controlGuideText.gameObject.SetActive(mode);
    }

    public void RestartGame() {
        gameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnemyKilled() {
        enemiesLeft--;
        if (enemiesLeft == 0)
            GameOver();
    }
}
