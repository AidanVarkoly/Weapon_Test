using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uim;

    public int XP = 0;
    public int SkillPoints = 0;
    public int XPToLevelUP = 5;

    public float fireRateBoost = 0f;
    public int MoveSpeedBoost = 0;
    public int damageBoost = 0;
    public int HealthBoost = 0;


    public void grantXP(int xpToGrant)
    {
        XP += xpToGrant;
        if (XP == XPToLevelUP)
        {
            XP -= 4;
            SkillPoints++;
            //uim.updateSkillPointIndicator(SkillPoints);
        }
    }

    public void grantFireRateBoost()
    {
        if (fireRateBoost == 0f && SkillPoints >= 1)
        {
            fireRateBoost = 0.5f;
            SkillPoints--;
        }
        else
        {
            fireRateBoost = 0f;
        }
    }
    public void grantMoveSpeedBoost()
    {
        if (MoveSpeedBoost == 0 && SkillPoints >= 1)
        {
            MoveSpeedBoost = 2;
            SkillPoints--;
        }
        else
        {
            MoveSpeedBoost = 0;
        }
    }
    public void grantDamageBoost()
    {
        if (damageBoost == 0 && SkillPoints >= 1)
        {
            damageBoost = 2;
            SkillPoints--;
        }
        else
        {
            damageBoost = 0;
        }
    }
    public void grantHealthBoost()
    {
        if (HealthBoost == 0f && SkillPoints >= 1)
        {
            HealthBoost = 5;
            SkillPoints--;
        }
        else
        {
            HealthBoost = 0;
        }
    }
}
