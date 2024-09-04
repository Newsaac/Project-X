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
    [HideInInspector] public int ammoCnt = 0;
    float timer = 0f;
    int enemiesLeft;

    [Header("Game Vars")]
    [SerializeField] int maxPlayerHp;
    [SerializeField] int playerHp;

    [Header("UI")]
    [SerializeField] Button startButton;
    [SerializeField] Button restartButton;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] TextMeshProUGUI enemiesLeftText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Slider hpSlider;
    [SerializeField] Image crosshair;
    //[SerializeField] TextMeshProUGUI controlGuideText;

    [Space(20)]
    public ControlsSerializable controls;

    private LevelSpecs levelSpecs;

    void Start() {
        playerHp = maxPlayerHp;
        hpSlider.value = ((float)playerHp) / maxPlayerHp;
        enemiesLeftText.text = "Enemies: " + enemiesLeft;

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
        ammoText.text = "Ammo: " + ammoCnt;
        timerText.text = "Time: " + timer.ToString("0.00");
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

    public void RestartGame() {
        gameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        if(timer < levelSpecs.bestTime && enemiesLeft == 0)
            PlayerPrefs.SetFloat("Best", timer);
        gameOver = true;
        crosshair.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        restartButton.gameObject.SetActive(true);
    }

    void ToggleMenuState(bool mode) {
        startButton.gameObject.SetActive(mode);
        crosshair.gameObject.SetActive(!mode);
        //controlGuideText.gameObject.SetActive(mode);
    }

    public void EnemyKilled() {
        enemiesLeft--;
        enemiesLeftText.text = "Enemies: " + enemiesLeft;
        if (enemiesLeft == 0)
            GameOver();
    }

    public void DamagePlayer(int amount) {
        playerHp -= amount;
        hpSlider.value = ((float)playerHp) / maxPlayerHp;
        if (playerHp <= 0)
            GameOver();
    }
}
