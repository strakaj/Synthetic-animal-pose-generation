using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJitter : MonoBehaviour
{
    public float rotation = 2f;
    Quaternion start_rotation;

    /// <summary>
    /// Get initial rotation.
    /// </summary>
    void Start()
    {
        start_rotation = gameObject.transform.rotation;
    }

    /// <summary>
    /// Randomly rotates object in all axes.
    /// </summary>
    public void applay_transform()
    {
        gameObject.transform.Rotate(new Vector3(Random.Range(-rotation, rotation), Random.Range(-rotation, rotation), Random.Range(-rotation, rotation)));
    }

    /// <summary>
    /// Resets rotation to initial values.
    /// </summary>
    public void reset_transform()
    {
        gameObject.transform.rotation = start_rotation;
    }
}
