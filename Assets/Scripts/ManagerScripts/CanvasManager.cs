using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas playCanvas;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private PlayerController player;
    [SerializeField] private Image energyImg;
    [SerializeField] private Image healthImg;
    [SerializeField] private Button retryButton;
    [SerializeField] private GameStateManager gameState;

    
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Button campaignButton;
    [SerializeField] private Button survivalButton;

    PlayerStatus playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        playCanvas.gameObject.SetActive(false);
        retryButton.onClick.AddListener(RestartGameClicked);

        menuCanvas.gameObject.SetActive(true);
        campaignButton.onClick.AddListener(CampaignBtnClicked);
        survivalButton.onClick.AddListener(SurvivalClicked);

        scoreText.gameObject.SetActive(true);
        healthImg.gameObject.SetActive(true);
        energyImg.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);

        playerStatus = player.GetComponent<PlayerStatus>();
    }

    void LateUpdate()
    { 
        if (GameStateManager.StartLevel)
        {
            GameStateManager.StartLevel = false;
            levelText.text = "Wave " + (GameStateManager.currentLevelIndex + 1);
            StartCoroutine(ShowText(2f));
        }

        energyImg.fillAmount = playerStatus.GetEnergy() / 100;
        healthImg.fillAmount = player.currentHealth / player.maxHealth;

        if (player.GetIsDead())
        {
            retryButton.gameObject.SetActive(true);
            gameOverText.gameObject.SetActive(true);
        }

        if (SpawnManager.enemyKilled) {
            scoreText.text = "Score: " + player.addAndGetScore(1);
            SpawnManager.enemyKilled = false;
        }
    }

    IEnumerator ShowText(float X)
    {
        levelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(X);
        levelText.gameObject.SetActive(false);
    }

    private void RestartGameClicked()
    {
        gameState.RestartGame();
    }

    private void SurvivalClicked()
    {
        GameStateManager.SetMode(false);
        menuCanvas.gameObject.SetActive(false);
        playCanvas.gameObject.SetActive(true);
    }

    private void CampaignBtnClicked()
    {
        GameStateManager.SetMode(true);
        menuCanvas.gameObject.SetActive(false);
        playCanvas.gameObject.SetActive(true);
    }
}
