using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    public void Initialize(Vector3 dir, float spd)
    {
        direction = dir;
        speed = spd;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
