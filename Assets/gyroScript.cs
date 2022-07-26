using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyroScript : MonoBehaviour
{
    private void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            transform.rotation = GyroToUnity(Input.gyro.attitude);
        }
    }
    private Quaternion GyroToUnity(Quaternion q)
    {
        //return new Quaternion(q.x, q.y, -q.z, -q.w
        return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
    }
}
