using UnityEngine;
using System;
using UnityEngine.UI;

public class ButtonAnimationController : MonoBehaviour
{
    public SwipeControls swipeControls;
    public EnemyController enemyController;
    private Animator _animator;
    public GameObject start_Button;
    public GameObject Pause_Button;
    public GameObject Setting_Button;
    public GameObject Score_Text;
    public GameObject ShopButton;
    public GameObject ShopPanel;
    public GameObject CoinCounter;

    // Define event to notify when score is updated
    public event Action<int> OnScoreUpdated;

    private void Start()
    {
        // Find the SwipeControls script in the scene
        swipeControls = FindObjectOfType<SwipeControls>();
        enemyController = FindObjectOfType<EnemyController>();
        _animator = GetComponent<Animator>();
        Open();
        Pause_Button.SetActive(false);
        Setting_Button.SetActive(true);
        Score_Text.SetActive(false);
        ShopButton.SetActive(true);
        ShopPanel.SetActive(false);
        CoinCounter.SetActive(false);
    }

    private void Update()
    {
        // Check for button click

    }

    private void ActivateSwipeControls()
    {
        if (swipeControls != null)
        {
            swipeControls.ActivateSwipeControls();
        }
    }

    public void start()
    {
        // Call the ActivateSwipeControls function
        ActivateSwipeControls();
        _animator.SetBool("IsRunning", true);
        Close();

        // Enable the enemy functionality when the button is clicked
        EnableEnemy();
        CoinCounter.SetActive(true);

        // Set the flag to increment the score in the SwipeControls script
        swipeControls.SetShouldIncrementScore(true);

    }

    private void EnableEnemy()
    {
        if (enemyController != null)
        {
            enemyController.isEnemyActive = true;
        }
    }

    public void Close()
    {
        start_Button.SetActive(false);
        ShopButton.SetActive(false);
        ShopPanel.SetActive(false);
    }

    public void Open()
    {
        start_Button.SetActive(true);
    }


    public void panelss()
    {
        Pause_Button.SetActive(true);
        Setting_Button.SetActive(false);
        Score_Text.SetActive(true);
    }

    // Method to update the score and trigger the event
    public void UpdateScore(int score)
    {
        if (OnScoreUpdated != null)
        {
            OnScoreUpdated(score);
        }
    }

    public void ShopOpen()
    {
        ShopPanel.SetActive(true);
    }
    public void ShopClose()
    {
        ShopPanel.SetActive(false);
    }

}
