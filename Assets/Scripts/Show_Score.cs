using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Show_Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText; // Reference to the coin text

    // Reference to the SwipeControls script
    private SwipeControls swipeControls;
    private CoinManager coinManager; // Reference to the CoinManager script

    private void Start()
    {
        // Find the SwipeControls script in the scene
        swipeControls = FindObjectOfType<SwipeControls>();

        // Find the CoinManager script in the scene
        coinManager = FindObjectOfType<CoinManager>();
    }

    private void Update()
    {
        if (swipeControls != null)
        {
            // Update the score text with the score from SwipeControls
            scoreText.text = swipeControls.score.ToString();
        }
        else
        {
            Debug.LogWarning("SwipeControls script not found in the scene.");
        }

        if (coinManager != null)
        {
            // Update the coin text with the total coins from CoinManager
            coinText.text =  coinManager.totalCoins.ToString();
        }
        else
        {
            Debug.LogWarning("CoinManager script not found in the scene.");
        }
    }
}
