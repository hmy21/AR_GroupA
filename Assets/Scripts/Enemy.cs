using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int Health;
    [SerializeField] int MaxHealth;

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
