using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public GameObject Enemy;
    public Vector3 temp;
    


    // Update is called once per frame
    void Update()
    {
        temp = this.gameObject.transform.position;
        if(Input.GetKeyDown(KeyCode.I))
        {
            temp.z += 2;
            Instantiate<GameObject>(Enemy, temp, Quaternion.identity);
        }
    }
}
