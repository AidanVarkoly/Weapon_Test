using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uim;
    public Animator arms_anim;

    public int MaxHealth = 30;
    public int Health;
    public float HealthRegenCooldown = 3; //Time before Regen Starts
    public float HealthRegenInterval = 0.75f;
    public float DamageInterval = 0.5f; //Time Between each point of danage
    public float TimeToStartHealing;
    public float TimeToNextHeal;
    public float TimeToNextDamage;
    public bool isHealing = false;
    public bool exchangingWeapons = false;
    public GameObject[] possibleWeapons;
    int activeSlot;
    public GameObject slot1;
    int slot1Index;
    public GameObject slot2;
    int slot2Index;
    public float SwitchInterval = 0.5f;
    public float TimetoSwitch;
    bool changingWeapons = false;
    int NewWeaponIndex = -1;

    void Start()
    {
        Health = MaxHealth;
        uim.setMaxHealth(MaxHealth);
        TimeToStartHealing = 0;

        activeSlot = 1;

        refreshSlotIndexes();
        slot1.GetComponent<Weapon>().WeaponAnimator.SetInteger("Weapon", slot1Index);
        //slot1.SetActive(false);
    }

    void refreshSlotIndexes()
    {
        switch (slot1.name)
        {
            case "FireSleet_Modular":
                slot1Index = 1;
                break;
            case "Mauler_Modular":
                slot1Index = 2;
                break;
            case "Grimbrand_Modular":
                slot1Index = 3;
                break;
            case "ScatterBurst_Modular":
                slot1Index = 4;
                break;
            case "Archtronic_Modular":
                slot1Index = 5;
                break;
            case "Hallwailer_Modular":
                slot1Index = 6;
                break;
            default:
                slot1Index = 0;
                break;
        }

        switch (slot2.name)
        {
            case "FireSleet_Modular":
                slot2Index = 1;
                break;
            case "Mauler_Modular":
                slot2Index = 2;
                break;
            case "Grimbrand_Modular":
                slot2Index = 3;
                break;
            case "ScatterBurst_Modular":
                slot2Index = 4;
                break;
            case "Archtronic_Modular":
                slot2Index = 5;
                break;
            case "Hellwailer_Modular":
                slot2Index = 6;
                break;
            default:
                slot2Index = 0;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!slot1.activeInHierarchy && activeSlot == 1)
        {
            slot1.SetActive(true);
        }

        if (changingWeapons)
        {
            if (Time.time >= TimetoSwitch)
            {
                switch (activeSlot)
                {
                    case 1:
                        slot1.SetActive(true);
                        slot2.SetActive(false);

                        slot1.GetComponent<Weapon>().updateAmmoUI();
                        break;
                    case 2:
                        slot1.SetActive(false);
                        slot2.SetActive(true);

                        slot2.GetComponent<Weapon>().updateAmmoUI();
                        break;
                }
                changingWeapons = false;
            }
        }
        else if (exchangingWeapons)
        {
            if (Time.time >= TimetoSwitch)
            {
                switch (activeSlot)
                {
                    case 1:
                        slot1.SetActive(false);

                        slot1 = possibleWeapons[NewWeaponIndex];
                        refreshSlotIndexes();

                        slot1.SetActive(true);

                        slot1.GetComponent<Weapon>().updateAmmoUI();
                        arms_anim.SetInteger("Weapon", slot1Index);
                        break;
                    case 2:
                        slot2.SetActive(false);

                        slot2 = possibleWeapons[NewWeaponIndex];
                        refreshSlotIndexes();

                        slot2.SetActive(true);

                        slot2.GetComponent<Weapon>().updateAmmoUI();
                        arms_anim.SetInteger("Weapon", slot2Index);
                        break;
                }
                exchangingWeapons = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && activeSlot != 1)
        {
            arms_anim.SetInteger("Weapon", slot1Index);
            TimetoSwitch = Time.time + SwitchInterval;
            activeSlot = 1;
            changingWeapons = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && activeSlot != 2)
        {
            slot2.GetComponent<Weapon>().WeaponAnimator.SetInteger("Weapon", slot2Index);
            TimetoSwitch = Time.time + SwitchInterval;
            activeSlot = 2;
            changingWeapons = true;
        }

        if (Health == MaxHealth && isHealing)
        {
            isHealing = false;
        }
        else
        {
            if(Time.time >= TimeToNextHeal)
            {
                Health++;
                uim.updateHealth(1);
                TimeToNextHeal = Time.time + TimeToNextHeal;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward,out hit,5f)) //Send out a raycast to detect any interactables
        {
            if(hit.transform.name == "WeaponStation")//Check if the object is a weapon station
            {
                WeaponStation tempWS = hit.transform.parent.gameObject.GetComponent<WeaponStation>(); //store the weapon station
                float cost;
                Weapon selectedWeapon;

                if (tempWS.currentWeapon.name == slot1.name || tempWS.currentWeapon.name == slot2.name)
                {//Check if the weapon in the weapon station is already owned by the player
                    uim.updateInteractionText(tempWS.GetBuyAmmoText()); //update the ui to show buy ammo prompt

                    if (Input.GetKeyDown(KeyCode.F)) //check if the player has hit their interact key
                    {
                        cost = tempWS.GetCost() / 2; //compute the ammo cost and set it to the variable

                        if(tempWS.currentWeapon.name == slot1.name)
                        {
                            selectedWeapon = slot1.GetComponent<Weapon>(); //set the selected weapon
                        }
                        else if (tempWS.currentWeapon.name == slot2.name)
                        {
                            selectedWeapon = slot2.GetComponent<Weapon>();
                        }
                        else
                        {
                            selectedWeapon = null; //this should not be possible. Tf it does, I'm not sure how it happened
                        }

                        if(uim.scorePts >= cost && selectedWeapon.atMaxAmmo())
                        {
                            uim.updateScore(-cost);
                            selectedWeapon.fillAmmo();
                        }
                    }
                }
                else
                {
                    uim.updateInteractionText(tempWS.GetPurchaseText()); //update the ui to show buy weapon prompt

                    if (Input.GetKeyDown(KeyCode.F)) //check if the player has hit the interact key
                    {
                        cost = tempWS.GetCost(); //get the weapon cost and set it to the variable

                        if (tempWS.currentWeapon.name == slot1.name)
                        {
                            selectedWeapon = slot1.GetComponent<Weapon>(); //set the selected weapon
                        }
                        else if (tempWS.currentWeapon.name == slot2.name)
                        {
                            selectedWeapon = slot2.GetComponent<Weapon>();
                        }
                        else
                        {
                            selectedWeapon = null;
                        }

                        if (uim.scorePts >= cost && selectedWeapon == null)
                        {
                            for (int i = 0; i < possibleWeapons.Length; i++)
                            {
                                if (possibleWeapons[i].name == tempWS.currentWeapon.name)
                                {
                                    NewWeaponIndex = i;
                                    break;
                                }
                            }

                            switch (activeSlot)
                            {
                                case 1:
                                    arms_anim.SetInteger("Weapon", 0);
                                    TimetoSwitch = Time.time + SwitchInterval;
                                    exchangingWeapons = true;
                                    uim.updateScore(-cost);
                                    break;
                                case 2:
                                    arms_anim.SetInteger("Weapon", 0);
                                    TimetoSwitch = Time.time + SwitchInterval;
                                    exchangingWeapons = true;
                                    uim.updateScore(-cost);
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                if(uim.InteractionText.text != "")
                {
                    uim.updateInteractionText("");
                }
            }
        }
            
        //if (Health < MaxHealth && HealthRegenFrames > 0)
        //{
        //    HealthRegenFrames--;
        //}
        //else if (HealthRegenFrames == 0 && RegenFrames > 0)  
        //{
        //    RegenFrames--;
        //    if (RegenFrames == 0)
        //    {
        //        if (Health == MaxHealth) HealthRegenFrames = HealthRegenCooldown;
        //        RegenFrames = CooldownBetweenRegen;
        //    }
        //}
        //if (DamageFrames > 0)
        //{
        //    DamageFrames--;
        //}
    }

    public void takeDamage()
    {
        if (Time.time >= TimeToNextDamage)
        {
            Health--;
            if (Health == 0)
            {
                uim.updateRoundResult("You Died!");
                return;
            }
            uim.updateHealth(-1);
            TimeToNextDamage = Time.time + DamageInterval;
            if (Health == MaxHealth - 1)
            {
                TimeToStartHealing = Time.time + TimeToStartHealing;
                isHealing = true;
            }
        }
        
    }

    /*private void OnCollisionEnter(Collision collision)
    {

        GameObject go = collision.gameObject;

        if (go.layer == 9)
        {
            go.GetComponent<Enemy>().Attacking();
            takeDamage();
        }
        
        
    }*/
}
