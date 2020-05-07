using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator animator;
    public bool open = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(open)
        {
            animator.SetBool("character_nearby", true);
        }
        if(!open)
        {
            animator.SetBool("character_nearby", false);
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            open = true;
        }
        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            open = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            open = false;
        }
    }
}
