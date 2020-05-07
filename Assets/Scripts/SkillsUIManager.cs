using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUIManager : MonoBehaviour
{
    SkillManager sm;

    [Header("Panels")]
    public GameObject SkillsPanel;
    public Button Skill1Button;
    public Button Skill2Button;
    public Button Skill3Button;
    public Button Skill4Button;

    [Header("Text Objects")]
    public Text SkillPointIndicator;

    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("Game Manager").GetComponent<SkillManager>();
        SkillPointIndicator.text = "" + sm.SkillPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
