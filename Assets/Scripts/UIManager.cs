using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [Header("Panel Groups")]
    public GameObject SkillsUIPrefab;
    GameObject SkillsUI;


    [Header("Game Play Panels")]
    public GameObject roundInfoPanel;
    public GameObject roundResultPanel;
    public GameObject ScorePanel;
    public GameObject AmmoCounterPanel;
    public GameObject crosshair;

    [Header("Inspect Panels")]
    public GameObject BarrelCycler;
    public GameObject GripCycler;

    [Header("Skills Panels")]
    public GameObject SkillsPanel;
    public Button Skill1Button;
    public Button Skill2Button;
    public Button Skill3Button;
    public Button Skill4Button;

    [Header("Settings Panels")]
    public GameObject SettingsPanel;
    public Button volumeButton;
    public Button exit;

    [Header("Text Objects")]
    public Text ammoCounter;
    public Text roundInfo;
    public Text score;
    public Text health;
    public Text roundResult;
    public Text SkillPointIndicator;
    public Text InteractionText;

    [Header("Other Values")]
    public float scorePts = 0;
    public int HealthPts = 30;
    public bool inspecting = false;
    public bool SkillMenuActive = false;
    public bool SettingsMenuActive = false;
    public AudioMixer audioMixer;
    private bool clicked;

    private void Start()
    {
    //    ammoCounter = GameObject.Find("Ammo Counter Text").GetComponent<Text>();
    //    roundInfo = GameObject.Find("Round Info Text").GetComponent<Text>();
    //    score = GameObject.Find("Score Text").GetComponent<Text>();
    //    health = GameObject.Find("Health Text").GetComponent<Text>();
    //    roundResult = GameObject.Find("Round Result Text").GetComponent<Text>();
    //    SkillPointIndicator = GameObject.Find("Skill Points Indicator").GetComponent<Text>();

        //crosshair = GameObject.Find("Crosshair");
        //ScorePanel = GameObject.Find("Score Panel");
        //HealthPanel = GameObject.Find("Health Panel");
        //AmmoCounterPanel = GameObject.Find("Ammo Counter Panel");


        //Panels that should not be shown unless activated need to start activated to be found, 
        //after which they can be disabled

        //roundInfoPanel = GameObject.Find("Round Info");
        roundInfoPanel.SetActive(false);

        //roundResultPanel = GameObject.Find("Round Result");
        roundResultPanel.SetActive(false);

        //BarrelCycler = GameObject.Find("Barrel Cycler");
        BarrelCycler.SetActive(false);

        //GripCycler = GameObject.Find("Grip Cycler");
        GripCycler.SetActive(false);

        //SkillsPanel = GameObject.Find("Skills Panel");
        //SkillsPanel.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !inspecting)//Open Skils Menu
        {
            toggleGameplayUI();
            toggleSkillsUI();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            updateScore(100f);
        }


        if (Input.GetKey(KeyCode.Escape))
        {
            SettingsPanel.gameObject.SetActive(true);
            SettingsMenuActive = true;

        }

        if (SettingsMenuActive)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SettingsPanel.gameObject.SetActive(false);
                SettingsMenuActive = false;
            }
        }

        if (!clicked)
        {
            volumeButton.onClick.AddListener(() => PlayAudio());
        }
        if (clicked)
        {
            volumeButton.onClick.AddListener(() => StopAudio());
        }
    }



    public static void toggleMouseLock()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    public void updateAmmoCounter(int currentAmmoInMag,int currentAmmo)
    {
        ammoCounter.text = "" + currentAmmoInMag + "/" + currentAmmo;
    }

    public void updateInteractionText(string text)
    {
        InteractionText.text = text;
    }

    public void updateRoundCountdown(float currnetTime)
    {
        if (currnetTime >= 10f)
        {
            roundInfo.text = currnetTime.ToString("00");
        }
        else
        {
            roundInfo.text = currnetTime.ToString("0.0");
        }
    }

    public void updateRoundEnemies(int remainingEnemies)
    {
        roundInfo.text = "" + remainingEnemies;
    }

    public void updateScore(float scoreToAdd)
    {
        scorePts += scoreToAdd;
        score.text = scorePts.ToString("####");
    }

    public void setMaxHealth(int maxHealth)
    {
        HealthPts = maxHealth;
    }

    public void updateHealth(int HealthToAdd)
    {
        HealthPts += HealthToAdd;
        health.text = "" + HealthPts;
    }

    public void updateRoundResult(string result)
    {
        roundResultPanel.SetActive(true);
        roundResult.text = result;
    }

    public void updateSkillPointIndicator(int currentSkillPoints)
    {
        SkillsUI.GetComponent<SkillsUIManager>().SkillPointIndicator.text = "" + currentSkillPoints;
    }

    public void updateBarrekCyclerPOS(Vector3 BarrelPos)
    {
        BarrelCycler.transform.position = Camera.main.WorldToScreenPoint(BarrelPos);
    }

    public void updateGripCyclerPOS(Vector3 GripPos)
    {
        GripCycler.transform.position = Camera.main.WorldToScreenPoint(GripPos);
    }

    public void toggleGameplayUI()
    {
        CharacterController.toggleMouseLock();
        crosshair.SetActive(!crosshair.activeInHierarchy);
        roundInfoPanel.SetActive(!roundInfoPanel.activeInHierarchy);
        ScorePanel.SetActive(!ScorePanel.activeInHierarchy);
        AmmoCounterPanel.SetActive(!AmmoCounterPanel.activeInHierarchy);
    }

    public void toggleInspectUI()
    {
        inspecting = !inspecting;
        toggleGameplayUI();
        BarrelCycler.SetActive(!BarrelCycler.activeInHierarchy);
        GripCycler.SetActive(!GripCycler.activeInHierarchy);
    }

    //public void toggleSkillsUI()
    //{
    //    SkillMenuActive = !SkillMenuActive;
    //    SkillsPanel.SetActive(!SkillsPanel.activeInHierarchy);
    //}

    public void toggleSkillsUI()
    {
        if (!SkillMenuActive)
        {
            SkillsUI = Instantiate<GameObject>(SkillsUIPrefab,transform);
        }
        else
        {
            Destroy(SkillsUI);
        }
        SkillMenuActive = !SkillMenuActive;
    }

    public void PlayAudio()
    {
        audioMixer.SetFloat("volume", 100);
        clicked = true;
    }

    public void StopAudio()
    {
        audioMixer.SetFloat("volume", 0);
        clicked = false;

    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
