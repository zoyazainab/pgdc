using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public int totalCoins = 0;
    public AudioClip coinSound; // Sound to play when collecting a coin
    private AudioSource audioSource;

    void Start()
    {
        UpdateCoinText();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            // Increment the total coin count
            totalCoins++;

            // Update the TextMeshPro text
            UpdateCoinText();

            // Play coin sound
           
           audioSource.PlayOneShot(coinSound);
            

            // Destroy the coin GameObject
            Destroy(other.gameObject);
        }
    }

    void UpdateCoinText()
    {
        // Update the TextMeshPro text with the total coin count
        coinText.text = totalCoins.ToString();
    }
}
