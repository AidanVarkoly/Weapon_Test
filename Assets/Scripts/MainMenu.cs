using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Animator animator;

    public void Play()
    {
        SceneManager.LoadScene("TestArena");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SettingsMenu()
    {
        animator.SetBool("IsClicked", true);
    }

    public void MainMenuBack()
    {
        animator.SetBool("IsClicked", false);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
}
