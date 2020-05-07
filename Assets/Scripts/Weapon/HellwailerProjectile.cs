using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellwailerProjectile : MonoBehaviour
{
    public GameObject explosion;

    bool initisilized = false;
    bool collided = false;
    bool stopped = false;
    Vector3 targetPosition;
    float ProjSpeed;

    // Update is called once per frame
    void Update()
    {
        if (initisilized & !stopped)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.33f);
            if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
            {
                Debug.Log("Stopped");
                stopped = true;
            }
        }
        else if (stopped)
        {
            GameObject tempGO = Instantiate<GameObject>(explosion);
            tempGO.transform.position = transform.position;
            Destroy(tempGO, 0.5f);


            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            foreach(Collider col in colliders)
            {
                if(col.gameObject.layer == 9)
                {
                    col.gameObject.GetComponent<Enemy>().takeDamage(5);
                }
            }
            Destroy(transform.gameObject);
        }
    }

    public void initialize(Vector3 targetPos,float speed)
    {
        initisilized = true;

        targetPosition = targetPos;

        ProjSpeed = speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;

        if (go.layer == 9)
        {
            stopped = true;
        }
    }
}
