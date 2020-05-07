using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uim;
    public SkillManager sm;

    public bool debugMode = false;
    public bool forPlayer = true;

    public Animator WeaponAnimator;

    public GameObject Empty;

    [Header("Attachment GameObjects")]
    public GameObject Body;
    public GameObject Stock;
    public GameObject Barrel;
    public GameObject BarrelAlt1;
    public GameObject BarrelExt;
    public GameObject Grip;
    public GameObject GripAlt1;
    public GameObject Magazine;
    public GameObject Sight;

    //Inspector Attachment Indexes
    private int index_barrel = 0;
    private int index_grip = 0;

    [Header("Weapon Parts")]
    public Barrels AttachedBarrel;
    public Grips AttachedGrip;

    int max_rounds_per_mag;
    int currentRoundsInMag;

    public int maxAmmo;
    public int currentAmmo;

    private AudioSource weaponSound;

    [Header("Effects")]

    bool reloading = false;

    [Header("Inspect")]
    private bool inspecting = false;

    Vector3 magazine_resting_pos;

    public GameObject GroundImpactEffect;
    public GameObject EnemyImpactEffect;
    public ParticleSystem MuzzleFlash;
    public GameObject projectile;

    [Header("Weapon Stats")]
    [ShowOnly] public AmmoType AmmoType;
    [ShowOnly] public float Damage;
    [ShowOnly] public int WeaponRange;
    [ShowOnly] public int SightMagnification;
    [ShowOnly] public int MagazineCapacity;
    [ShowOnly] public float Accuracy;
    [ShowOnly] public float FireRate;
    [ShowOnly] public float RecoilReduction;
    [ShowOnly] public float MovementSpeed;
    [ShowOnly] public float ReloadSpeed;
    [ShowOnly] public float NumberOfProjectiles;

    private bool statsComputed = false;
    private float NextTimeToFire = 0;
    private float TimeToAddAmmo = 0;

    void Start()
    {
        if (uim == null)
        {
            uim = GameObject.Find("MainCanvas").GetComponent<UIManager>();
        }
        if (sm == null)
        {
            sm = GameObject.Find("Game Manager").GetComponent<SkillManager>();
        }

        updateAttachments();
        CalculateStats();

        //RisingRecoilFrames = RecoilFrames / 2;
        //ReturningRecoilFrames = RecoilFrames / 2;

        //Reload_LoweringFrames = ReloadFrames / 2;
        //Reload_RisingFrames = ReloadFrames / 2;

        if (debugMode)
        {
            maxAmmo = MagazineCapacity * 1000;
        }
        else
        {
            maxAmmo = MagazineCapacity * 10;
        }
        currentRoundsInMag = MagazineCapacity;
        currentAmmo = maxAmmo;
        transform.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (uim == null)
        {
            uim = GameObject.Find("MainCanvas").GetComponent<UIManager>();
        }

        updateAttachments();
        CalculateStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (forPlayer)
        {
            if (!statsComputed)
            {
                updateAttachments();
                CalculateStats();
            }

            if (NextTimeToFire > Time.time + 1000f)
            {
                NextTimeToFire = 0;
                MuzzleFlash.Play();
            }

            if (WeaponAnimator.GetBool("firing") && !WeaponAnimator.GetCurrentAnimatorStateInfo(0).IsName("fire"))
            {
                WeaponAnimator.SetBool("firing", false);
            }

            //Inspect
            if (Input.GetKeyDown(KeyCode.V))
            {
                Inspect();
            }
            if (!uim.SkillMenuActive)
            {
                if (Input.GetButton("Fire1") && Time.time >= NextTimeToFire)
                {
                    NextTimeToFire = Time.time + FireRate;
                    if (currentRoundsInMag != 0)
                    {
                        FireWeapon();
                    }
                    else
                    {
                        Reload();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R) && !WeaponAnimator.GetBool("reloading"))
            {
                Reload();
                if (WeaponAnimator.GetInteger("Weapon") == 4)
                {
                    SBLightsController sblc = GetComponent<SBLightsController>();
                    sblc.StartReloadTimers();
                }
            }
            else if (WeaponAnimator.GetBool("reloading") && Time.time >= TimeToAddAmmo)
            {
                ChangeMagazineAmmo(0);
                WeaponAnimator.SetBool("reloading", false);
            }
        }
    }

    public void fillAmmo()
    {
        currentAmmo = maxAmmo;
        updateAmmoUI();
    }

    public void updateAmmoUI()
    {
        uim.updateAmmoCounter(currentRoundsInMag, currentAmmo);
    }

    public bool atMaxAmmo()
    {
        return currentAmmo < maxAmmo;
    }

    void ChangeMagazineAmmo(int delta = -1)
    {
        if (delta != 0)
        {
            currentRoundsInMag += delta;
        }
        else
        {
            //currentRoundsInMag = MagazineCapacity;
            int k = currentRoundsInMag + currentAmmo;
            if (k > MagazineCapacity)
            {
                currentRoundsInMag = MagazineCapacity;
                currentAmmo = k - MagazineCapacity;
            }
            else
            {
                currentRoundsInMag = k;
                currentAmmo = 0;
            }
        }
        updateAmmoUI();
    }

    void CalculateStats()
    {
        AmmoType = AmmoType.normal;
        Damage = 0;
        WeaponRange = 0;
        SightMagnification = 0;
        MagazineCapacity = 0;
        Accuracy = 0;
        FireRate = 0;
        ReloadSpeed = 0;
        RecoilReduction = 0;
        MovementSpeed = 0;
        NumberOfProjectiles = 0;


        //Magazine
        Damage += Magazine.GetComponent<Magazine>().Damage;
        MagazineCapacity = Magazine.GetComponent<Magazine>().MagazineCapacity;
        MovementSpeed += Magazine.GetComponent<Magazine>().MoveSpeedPenalty;
        ReloadSpeed = Magazine.GetComponent<Magazine>().ReloadSpeed;
        AmmoType = Magazine.GetComponent<Magazine>().AmmoType;

        //Sight
        SightMagnification = Sight.GetComponent<Sight>().Magnification;
        WeaponRange += Sight.GetComponent<Sight>().WeaponRange;
        MovementSpeed += Sight.GetComponent<Sight>().MoveSpeedPenalty;

        //Body
        FireRate = Body.GetComponent<Body>().fireRate;
        Accuracy += Body.GetComponent<Body>().BaseAccuracy;
        MovementSpeed += Body.GetComponent<Body>().MoveSpeedPenalty;

        //Barrel
        switch (AttachedBarrel)
        {
            case Barrels.LongBarrel:
                Damage += Barrel.GetComponent<Barrel>().Damage;
                WeaponRange += Barrel.GetComponent<Barrel>().WeaponRange;
                Accuracy -= Barrel.GetComponent<Barrel>().AccuracyModifier;
                MovementSpeed += Barrel.GetComponent<Barrel>().MoveSpeedPenalty;
                FireRate -= Barrel.GetComponent<Barrel>().FireRateModifier;
                break;
            case Barrels.ShortBarrel:
                Damage += BarrelAlt1.GetComponent<Barrel>().Damage;
                WeaponRange += BarrelAlt1.GetComponent<Barrel>().WeaponRange;
                Accuracy += BarrelAlt1.GetComponent<Barrel>().AccuracyModifier;
                MovementSpeed += BarrelAlt1.GetComponent<Barrel>().MoveSpeedPenalty;
                FireRate -= BarrelAlt1.GetComponent<Barrel>().FireRateModifier;
                break;
        }

        //Barrel Attachment
        NumberOfProjectiles = BarrelExt.GetComponent<BarrelAttachment>().NumberOfProjectiles;
        if (NumberOfProjectiles == 0) NumberOfProjectiles = 1;

        //Grip
        switch (AttachedGrip)
        {
            case Grips.RoundGrip:
                Accuracy += Grip.GetComponent<Grip>().AccuracyModifier;
                RecoilReduction += Grip.GetComponent<Grip>().RecoilReductionModifier;
                MovementSpeed += Grip.GetComponent<Grip>().MoveSpeedPenalty;
                break;
            case Grips.StraightGrip:
                Accuracy += GripAlt1.GetComponent<Grip>().AccuracyModifier;
                RecoilReduction += GripAlt1.GetComponent<Grip>().RecoilReductionModifier;
                MovementSpeed += GripAlt1.GetComponent<Grip>().MoveSpeedPenalty;
                break;
        }

        //Stock
        RecoilReduction += Stock.GetComponent<Stock>().RecoilReductionModifier;
        MovementSpeed += Stock.GetComponent<Stock>().MoveSpeedPenalty;

        statsComputed = true;
    }

    void Inspect()
    {
        uim.toggleInspectUI();
        WeaponAnimator.SetBool("inspecting", !WeaponAnimator.GetBool("inspecting"));
        if (WeaponAnimator.GetBool("inspecting")) return;
        switch (AttachedBarrel)
        {
            case Barrels.LongBarrel:
                uim.updateBarrekCyclerPOS(Barrel.transform.position);
                break;
            case Barrels.ShortBarrel:
                uim.updateBarrekCyclerPOS(BarrelAlt1.transform.position);
                break;
        }
        switch (AttachedGrip)
        {
            case Grips.RoundGrip:
                uim.updateGripCyclerPOS(Grip.transform.position);
                break;
            case Grips.StraightGrip:
                uim.updateGripCyclerPOS(GripAlt1.transform.position);
                break;
        }
    }

    void FireWeapon()
    {
        if (WeaponAnimator.GetBool("reloading") || WeaponAnimator.GetBool("inspecting")) return;

        MuzzleFlash.Play(); //Play muzzle flash
        weaponSound.Play(); //Play weapon sound
        ChangeMagazineAmmo();
        WeaponAnimator.SetBool("firing", true);

        if (WeaponAnimator.GetInteger("Weapon") == 4)
        {
            SBLightsController sblc = GetComponent<SBLightsController>();
            if (Magazine.activeInHierarchy)
            {
                sblc.Fire2Rnd();
            }
            else
            {
                sblc.Fire4Rnd();
            }
            for (int i = 0; i < NumberOfProjectiles; i++)
            {
                FireRayCast();
            }
        }
        else if (WeaponAnimator.GetInteger("Weapon") == 6)
        {
            RaycastHit hit;

            if (Physics.Raycast(Barrel.transform.position, Camera.main.transform.forward, out hit, WeaponRange))
            {
                GameObject proj_go = Instantiate<GameObject>(projectile);

                proj_go.transform.position = Barrel.transform.position;
                proj_go.GetComponent<HellwailerProjectile>().initialize(hit.point, 0.33f);

                Debug.DrawRay(transform.position, Camera.main.transform.forward, Color.red, 10f);
                if (debugMode)
                {
                    Debug.Log("Hellwailer_Projile Target: " + hit.transform.name);
                    Debug.Log("Hellwailer_Projile Current Pos: " + transform.position.ToString());
                    Debug.Log("Hellwailer_Projile Destination: " + hit.point.ToString());
                }
            }
        }
        else
        {
            FireRayCast();
        }
            
    }

    Vector3 GenerateProjectileAngle()
    {
        if (true)
        {
            Vector3 Rot;
            Vector3 forward = Camera.main.transform.forward;


            Quaternion qRot = Random.rotation;
            float angle = 0.0f;
            switch (WeaponAnimator.GetInteger("Weapon"))
            {
                case 1://Firesleet
                    angle = 0.2f - (Accuracy * 0.1f);
                    break;
                case 2://mauler
                    angle = 1f - (Accuracy * 0.5f);
                    break;
                case 3://grimbrand
                    angle = 1f - (Accuracy * 0.5f);
                    break;
                case 4://Scatterburst
                    angle = 10f - (Accuracy * 10f);
                    break;
                case 5://archtronic
                    angle = 1f - (Accuracy * 0.5f);
                    break;
                case 6://hellwailer
                    angle = 3f - (Accuracy * 3f);
                    break;
                default:
                    angle = 10f - (Accuracy * 10f);
                    break;
            }

            Quaternion qRot2 = Quaternion.RotateTowards(Camera.main.transform.rotation, qRot, angle);

            Rot = qRot2.eulerAngles;

            return Rot;
        }
        else
        {
            return Camera.main.transform.forward;
        }
    }

    void FireRayCast()
        {
        RaycastHit hit;
        GameObject go = Instantiate(Empty); ;
        go.transform.position = Camera.main.transform.position;
        go.transform.eulerAngles = GenerateProjectileAngle();



        if (Physics.Raycast(transform.position, go.transform.forward, out hit, WeaponRange)) //Fire raycast from current weapon position forward until it hits something or reaches the maximum range
        {//if something is hit
            if (debugMode)
            {
                Debug.Log(hit.transform.name);//Log the name of the hit object
                Debug.DrawLine(transform.position, hit.point, Color.red, 2); //Draw a black line in the SCENE view between the current weapon position and the position of the point the raycast hit that will last for 2 seconds
            }

            Enemy tempEnemy = hit.transform.GetComponent<Enemy>();
            GameObject ImpactGO;

            if (tempEnemy != null)// If an enemy is hit
            {
                ImpactGO = Instantiate(EnemyImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                tempEnemy.takeDamage((Damage + sm.damageBoost)/NumberOfProjectiles);
                uim.updateScore(1);
            }
            else // If something other than an enemy if hit
            {
                ImpactGO = Instantiate(GroundImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            Destroy(ImpactGO, 2); //Destroys impact effect after 2 seconds
        }
        else if (debugMode) //if nothing is hit
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * WeaponRange, Color.black, 2);
        }
        Destroy(go);

    }

    void Reload()
    {
        if (WeaponAnimator.GetBool("inspecting")) return;

        if (currentRoundsInMag < MagazineCapacity)
        {
            TimeToAddAmmo = Time.time + ReloadSpeed;
            WeaponAnimator.SetBool("reloading", true);
        }
    }

    public void updateAttachments()
    {
        Vector3 barrel_ext_position = Vector3.zero;
        if (AttachedBarrel == Barrels.LongBarrel)
        {
            barrel_ext_position = Barrel.GetComponent<Barrel>().Barrel_Ext_Pos;
            weaponSound = Barrel.GetComponent<Barrel>().weaponSound;
            Barrel.SetActive(true);
            BarrelAlt1.SetActive(false);
            if(uim != null)
            {
                if (uim.BarrelCycler != null)
                {
                    if (uim.BarrelCycler.activeInHierarchy)
                    {
                        uim.updateBarrekCyclerPOS(Barrel.transform.position);
                    }
                }
            }
        }
        else if (AttachedBarrel == Barrels.ShortBarrel)
        {
            barrel_ext_position = BarrelAlt1.GetComponent<Barrel>().Barrel_Ext_Pos;
            weaponSound = BarrelAlt1.GetComponent<Barrel>().weaponSound;
            BarrelAlt1.SetActive(true);
            Barrel.SetActive(false);
            if (uim.BarrelCycler != null)
            {
                if (uim.BarrelCycler.activeInHierarchy)
                {
                    uim.updateBarrekCyclerPOS(BarrelAlt1.transform.position);
                }
            }
        }

        if (BarrelExt.activeInHierarchy)
        {
            BarrelExt.transform.localPosition = barrel_ext_position;
        }

        if (AttachedGrip == Grips.RoundGrip)
        {
            Grip.SetActive(true);
            GripAlt1.SetActive(false);
            if (uim.BarrelCycler != null)
            {
                if (uim.GripCycler.activeInHierarchy)
                {
                    uim.updateGripCyclerPOS(Grip.transform.position);
                }
            }
        }
        else if (AttachedGrip == Grips.StraightGrip)
        {
            GripAlt1.SetActive(true);
            Grip.SetActive(false);
            if (uim.BarrelCycler != null)
            {
                if (uim.GripCycler.activeInHierarchy)
                {
                    uim.updateGripCyclerPOS(GripAlt1.transform.position);
                }
            }
        }
    }
    public void CycleBarrelsForward()
    {
        if (index_barrel == 1)//If Max, cycle to beginning
        {
            index_barrel = 0;
        }
        else
        {
            index_barrel++;
        }

        switch (index_barrel)
        {
            case 0:
                AttachedBarrel = Barrels.LongBarrel;
                updateAttachments();
                break;
            case 1:
                AttachedBarrel = Barrels.ShortBarrel;
                updateAttachments();
                break;
        }
    }
    public void CycleGripsForward()
    {
        if (index_grip == 1)//If Max, cycle to beginning
        {
            index_grip = 0;
        }
        else
        {
            index_grip++;
        }

        switch (index_grip)
        {
            case 0:
                AttachedGrip = Grips.RoundGrip;
                updateAttachments();
                break;
            case 1:
                AttachedGrip = Grips.StraightGrip;
                updateAttachments();
                break;
        }
    }
}


public enum Barrels
{
    LongBarrel,
    ShortBarrel
}

public enum Grips
{
    RoundGrip,
    StraightGrip
}

