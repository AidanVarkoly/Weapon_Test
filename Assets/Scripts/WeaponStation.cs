using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentWeapon
{
    Grimbrand,
    Mauler,
    Firesleet,
    Archtronic,
    Hellwailer,
    Scatterburst
}

public class WeaponStation : MonoBehaviour
{
    public WeaponInfo[] possibleWeapons;
    public GameObject currentWeapon;
    public GameObject container;
    public CurrentWeapon Weapon;
    public Vector3 WeaponRotation;

    [Header("Debug Settings")]
    public bool debugMode = false;
    public float MasterCost;

    // Start is called before the first frame update
    void Start()
    {
        setNewWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentWeapon.activeInHierarchy) currentWeapon.SetActive(true);
    }

    //private void OnValidate()
    //{
    //    setNewWeapon();
    //}

    void setNewWeapon()
    {
        switch (Weapon)
        {
            case CurrentWeapon.Archtronic:
                setupWeapon(4);
                currentWeapon.name = "Archtronic_Modular";
                break;
            case CurrentWeapon.Firesleet:
                setupWeapon(0);
                currentWeapon.name = "FireSleet_Modular";
                break;
            case CurrentWeapon.Grimbrand:
                setupWeapon(1);
                currentWeapon.name = "Grimbrand_Modular";
                break;
            case CurrentWeapon.Hellwailer:
                setupWeapon(5);
                currentWeapon.name = "Hellwailer_Modular";
                break;
            case CurrentWeapon.Mauler:
                setupWeapon(2);
                currentWeapon.name = "Mauler_Modular";
                break;
            case CurrentWeapon.Scatterburst:
                setupWeapon(3);
                currentWeapon.name = "ScatterBurst_Modular";
                break;
        }
    }

    void setupWeapon(int weaponID)
    {
        Destroy(currentWeapon);
        currentWeapon = Instantiate<GameObject>(possibleWeapons[weaponID].WeaponGameObject);
        currentWeapon.transform.SetParent(this.transform);
        currentWeapon.transform.localPosition = possibleWeapons[weaponID].WeaponPostion;
        currentWeapon.transform.eulerAngles = WeaponRotation;

        container.transform.localScale = possibleWeapons[weaponID].ContainerScale;
    }

    public float GetCost()
    {
        if (debugMode)
        {
            return MasterCost;
        }
        else
        {
            switch (Weapon)
            {
                case CurrentWeapon.Archtronic:
                    return possibleWeapons[4].Cost;

                case CurrentWeapon.Firesleet:
                    return possibleWeapons[0].Cost;

                case CurrentWeapon.Grimbrand:
                    return possibleWeapons[1].Cost;

                case CurrentWeapon.Hellwailer:
                    return possibleWeapons[5].Cost;

                case CurrentWeapon.Mauler:
                    return possibleWeapons[2].Cost;

                case CurrentWeapon.Scatterburst:
                    return possibleWeapons[3].Cost;

                default:
                    return MasterCost;
            }
        }
    }

    public string GetPurchaseText()
    {
        float cost = GetCost();
        string name;
        switch (Weapon)
        {
            case CurrentWeapon.Archtronic:
                name = "Archtronic";
                break;

            case CurrentWeapon.Firesleet:
                name = "Firesleet";
                break;

            case CurrentWeapon.Grimbrand:
                name = "Grimbrand";
                break;

            case CurrentWeapon.Hellwailer:
                name = "Hellwailer";
                break;

            case CurrentWeapon.Mauler:
                name = "Mauler";
                break;

            case CurrentWeapon.Scatterburst:
                name = "Scatterburst";
                break;

            default:
                name = "?";
                break;
        }
        return "Press F to purchase " + name + " for " + cost + " pts";
    }

    public string GetBuyAmmoText()
    {
        float cost = GetCost();

        return "Press F refill ammo for " + cost/2 + " pts";
    }
}

[System.Serializable]
public struct WeaponInfo
{
    public string Name;
    public GameObject WeaponGameObject;
    public float Cost;
    public Vector3 WeaponPostion;
    public Vector3 ContainerScale;
}
