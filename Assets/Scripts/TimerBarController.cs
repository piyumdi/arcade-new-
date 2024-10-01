
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Required for scene management

public class TimerBarController : MonoBehaviour
{
    Image Bar;
    public float maxTime = 30f;
    private float timeleft;
    private bool isTimerRunning = false; // Flag to check if the timer is running

    public GameObject playButton; // Reference to the Play Button

    
    void Start()
    {
        Time.timeScale = 1; // Ensure the time scale is reset to normal when the scene starts
        Bar = GetComponent<Image>();
        timeleft = maxTime;
        Bar.fillAmount = 1; // Initialize the timer bar to full
        playButton.SetActive(false); // Hide the button initially
    }


    void Update()
    {
        
        if (isTimerRunning && timeleft > 0)
        {
            timeleft -= Time.deltaTime;
            Bar.fillAmount = timeleft / maxTime;
        }
        else if (timeleft <= 0)
        {
            Time.timeScale = 0; // Pause the game when the timer runs out
            ShowPlayButton(); // Call to show the play button
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true; // Set the timer running flag to true
        timeleft = maxTime; // Reset timer when called
        Bar.fillAmount = 1; // Reset the bar fill
        playButton.SetActive(false); // Hide the play button when the timer starts
    }

    public void StopTimer()
    {
        isTimerRunning = false; // Set the timer running flag to false
    }

    private void ShowPlayButton()
    {
        playButton.SetActive(true); // Show the play button
        StopTimer(); // Ensure the timer is stopped when showing the button
    }


    public void RestartGame()
    {
        Time.timeScale = 1; // Reset time scale to normal
        SceneManager.LoadScene("SimplePoly City - Low Poly Assets_Demo Scene"); // Reload scene
    }



}


