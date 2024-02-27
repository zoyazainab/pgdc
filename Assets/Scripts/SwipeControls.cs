using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Add this line to import the SceneManager namespace
using UnityEngine.UI;
using TMPro;

public class SwipeControls : MonoBehaviour
{
    public float maxMoveDistance = 5f;
    public float forwardSpeed = 5f;
    public float jumpForce = 8f;
    public float swipeDownForce = 10f;
    public float dampingFactor = 0.7f;
    public float scoreIncrementRate = 1f; // Score increment rate per second


    private Vector2 startTouchPosition;
    private bool isGrounded;
    private bool isCollidingWithObstacle = false;
    private bool isJumping = false;
    private Rigidbody rb;
    private Animator playerAnimator;
    public TextMeshProUGUI Score_run;
    private bool shouldIncrementScore = false;
    public GameObject Score_Panel;
    public float time_till_scorepanel = 3;
    public Button retryButton;
    public delegate void ScoreUpdateEvent(int score);
    public static event ScoreUpdateEvent OnScoreUpdate;

    // Variable to check if swipe controls should be active
    private bool isSwipeActive = false;

    public int score = 0; // Score variable
    private Coroutine scoreCoroutine; // Coroutine reference for score incrementation


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        retryButton.onClick.AddListener(RetryGame);
        Score_Panel.SetActive(false);
    }

    void Update()
    {
        if (!isCollidingWithObstacle && isSwipeActive)
        {
            MoveForward();
            CheckGrounded();
            CheckSwipeInput();
        }
    }
    void RetryGame()
    {
        // Restart the game by reloading the current scene
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }
    void CheckSwipeInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    Vector2 swipeDirection = touch.position - startTouchPosition;

                    if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                    {
                        if (swipeDirection.x < 0)
                            MoveLeft();
                        else if (swipeDirection.x > 0)
                            MoveRight();
                    }
                    else
                    {
                        if (swipeDirection.y > 0)
                            Jump();
                        else if (swipeDirection.y < 0)
                            MoveDown();
                    }
                    break;
            }
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void MoveLeft()
    {
        float newPosX = Mathf.Max(transform.position.x - maxMoveDistance, -maxMoveDistance);
        transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
    }

    void MoveRight()
    {
        float newPosX = Mathf.Min(transform.position.x + maxMoveDistance, maxMoveDistance);
        transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
    }

    void Jump()
    {
        if (isGrounded && !isJumping)
        {
            playerAnimator.SetBool("IsJumping", true);
            Debug.Log("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;

            // Call the ResetJumpAnimation after a delay
            StartCoroutine(ResetJumpAnimation(0.5f)); // You can adjust the delay as needed
        }
    }

    IEnumerator ResetJumpAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the IsJumping parameter after the delay
        playerAnimator.SetBool("IsJumping", false);
        isJumping = false;
    }


    void MoveDown()
    {
        playerAnimator.SetBool("IsRolling", true);
        rb.AddForce(Vector3.down * swipeDownForce, ForceMode.Impulse);

        StartCoroutine(ResetRollAnimation(0.5f)); // You can adjust the delay as needed
    }
    IEnumerator ResetRollAnimation(float delays)
    {
        yield return new WaitForSeconds(delays);

        // Reset the IsJumping parameter after the delay
        playerAnimator.SetBool("IsRolling", false);
        //isJumping = false;
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        if (isGrounded)
        {
            isJumping = false;
            Vector3 vel = rb.velocity;
            vel.x *= dampingFactor;
            vel.z *= dampingFactor;
            rb.velocity = vel;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlayObstacleCollisionAnimation();
            playerAnimator.SetBool("IsRunning", false);

          
          
            // Calculate the target position by moving the player back along the z-axis
            Vector3 targetPosition = transform.position - transform.forward * 5f; // Adjust the distance as needed

            // Smoothly move the player to the target position
            StartCoroutine(MovePlayerSmoothly(targetPosition));

            PauseScoreIncrement();
            StartCoroutine(DelayBeforeShowingPanel(time_till_scorepanel)); // 3 seconds delay
        }
    }

    IEnumerator MovePlayerSmoothly(Vector3 targetPosition)
    {
        float duration = 0.5f; // Adjust the duration of the movement
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player reaches the exact target position
        transform.position = targetPosition;
    }

    IEnumerator DelayBeforeShowingPanel(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Display the score panel after the delay
        Score_Panel.SetActive(true);
    }

    void PlayObstacleCollisionAnimation()
    {
        if (playerAnimator != null)
        {
            Debug.Log("Animation Played");
            playerAnimator.SetTrigger("ObstacleCollisionTrigger");

            // Stop further movement by setting speed to zero
            forwardSpeed = 0f;

            // Set the flag for obstacle collision
           // isCollidingWithObstacle = true;
        }
    }

    // Function to set swipe controls active
    public void ActivateSwipeControls()
    {
        isSwipeActive = true;
    }

    // Coroutine to increment the score continuously
    void StartScoreIncrement()
    {
        if (shouldIncrementScore)
        {
            scoreCoroutine = StartCoroutine(IncreaseScore());
        }
    }
    public void SetShouldIncrementScore(bool shouldIncrement)
    {
        shouldIncrementScore = shouldIncrement;
        if (shouldIncrementScore)
        {
            StartScoreIncrement(); // Start the score incrementation coroutine if the flag is true
        }
    }

    IEnumerator IncreaseScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / scoreIncrementRate); // Wait for the specified time
            score++; // Increment the score
            UpdateScoreText(); // Update the score text
        }
    }


    // Function to pause the score incrementation
    void PauseScoreIncrement()
    {
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine); // Stop the coroutine to pause score incrementation
        }
    }

    // Function to update the score text
    void UpdateScoreText()
    {
        if (Score_run != null)
        {
            // Check if the score exceeds 999999, pause the score right there
            if (score > 999999)
            {
                score = 999999;
                PauseScoreIncrement(); // Pause score incrementation when reaching maximum score
            }

            // Update the TextMeshProUGUI object with the current score
            Score_run.text = score.ToString("D6"); // "D6" formats the score as a 6-digit string with leading zeros if necessary

            // Trigger the event with the updated score
            OnScoreUpdate?.Invoke(score);
        }
    }
}