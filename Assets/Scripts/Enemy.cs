using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int Health;
    [SerializeField] int MaxHealth;
    [SerializeField] float movingSpeed;

    public void takeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            beKilled();
        }
    }

    private void beKilled()
    {
        Destroy(this);
    }

    private void shot()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
