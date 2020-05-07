using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldEnemy : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uim;
    public GameManager gm;
    public SkillManager sm;

    public GameObject Player;

    public Material normal;
    public Material Death;
    public SkinnedMeshRenderer MR;
    private Animator animator;

    bool isHurt = false;
    bool grounded;
    bool AlreadyDead = false;
    public int hurtDuration = 5;
    public int attackDuration = 20;
    public int attackFrames;
    public int hurtFrames;
    public bool isWalking = false;
    public bool isDissolving = false;
    public float _Time = 0f;
    public int DissolveTime;

    public float Health = 10f;

    public float Speed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        _Time = 0f;
        Player = GameObject.Find("Character");
        hurtFrames = hurtDuration;
        
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(attackFrames > 0)
        {
            attackFrames--;
        }
        else if(attackFrames == 0)
        {
            StopAttacking();
        }
        if (isHurt)
        {
            hurtFrames--;
            if (hurtFrames == 0)
            {
                MR.material = normal;
                hurtFrames = hurtDuration;
                isHurt = false;
            }
        }
        if (isDissolving)
        {
            _Time += Time.deltaTime * DissolveTime;
            if (_Time >= 8)
            {
                _Time = 0f;
                isDissolving = false;
                gm.RemoveEnemy(transform.gameObject);
            }
            Death.SetFloat("Time", _Time);
        }
        transform.position = Vector3.MoveTowards(transform.position,Player.transform.position, Speed * Time.deltaTime);

        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, 1 << 10);
        transform.position = new Vector3(transform.position.x,hit.point.y, transform.position.z);


        transform.LookAt(Player.transform.position,transform.up);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);


        animator.SetBool("isWalking", true);
        isWalking = true;
    }

    public void takeDamage(float damageToTake = 1)
    {
        Health -= damageToTake;
        if (GameManager.DebugMode)
        {
            Debug.Log(transform.name + " Took " + damageToTake + " damage, Remaing Health: " + Health);
        }
        if (Health <= 0 && !AlreadyDead)
        {
            AlreadyDead = true;
            MR.material = Death;
            isDissolving = true;
            //GameManager.RemoveEnemy(transform.gameObject);
            uim.updateScore(5);
            sm.grantXP(1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.layer == 11)
        {
            Destroy(go);
            takeDamage();
        }
    }


    public void Attacking()
    {
        attackFrames = attackDuration;
        animator.SetBool("attacked", true);

        //yield WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void StopAttacking()
    {
        animator.SetBool("attacked", false);
    }
    /**
     *  Checks if the character is grounded or not
     */
    private bool isGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, 0.5f * 2, 1 << 10))
        {
            if (GameManager.DebugMode)
            {
                Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.red);
            }
            return true;
        }
        else
        {
            if (GameManager.DebugMode)
            {
                Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.blue);
            }
            return false;
        }

    }
}
