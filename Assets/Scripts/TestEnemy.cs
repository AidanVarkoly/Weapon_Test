using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : MonoBehaviour
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

    public Transform destination;
    NavMeshAgent navMeshAgent;

    void Awake()
    {
        if (uim == null)
        {
            uim = GameObject.Find("MainCanvas").GetComponent<UIManager>();
        }
        if (gm == null)
        {
            gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }
        if (sm == null)
        {
            sm = GameObject.Find("Game Manager").GetComponent<SkillManager>();
        }
    }

    void Start()
    {
        _Time = 0f;
        Player = GameObject.Find("Character");
        hurtFrames = hurtDuration;

        animator = GetComponent<Animator>();

        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent componenet is not attached to " + gameObject.name);
        }
        else
        {
            SetDestination();
        }

        destination = Player.transform;
    }

    void Update()
    {
        if (attackFrames > 0)
        {
            attackFrames--;
        }
        else if (attackFrames == 0)
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
        SetDestination();
        animator.SetBool("isWalking", true);
        isWalking = true;
    }

    private void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

    public void takeDamage(float damageToTake = 1)
    {
        Health -= damageToTake;
        if (GameManager.DebugMode)
        {
            Debug.Log(transform.name + " Took " + damageToTake + " damage, Remaing Health: " + Health);
        }
        if (Health <= 0)
        {
            MR.material = Death;
            isDissolving = true;
            //GameManager.RemoveEnemy(transform.gameObject);
            uim.updateScore(5);
            sm.grantXP(1);
        }
    }

    public void Attacking()
    {
        attackFrames = attackDuration;
        Debug.Log("attacked");
        animator.SetBool("attacked", true);

        //yield WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void StopAttacking()
    {
        animator.SetBool("attacked", false);
    }

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
