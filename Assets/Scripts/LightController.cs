using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Color color;
    public Light lights;
    public static bool LockDown = false;

    public void Start()
    {
        lights = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if(LockDown)
        {
            ChangeColor();
        }
    }

    public void ChangeColor()
    {
        lights.color = Color.red;
    }
}
