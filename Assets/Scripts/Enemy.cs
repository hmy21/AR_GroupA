using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int Health;
    [SerializeField] int MaxHealth;
    [SerializeField] float movingSpeed;
    [SerializeField] GameObject BulletPrefabs;
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject TestAim;
    [SerializeField] GameObject shotPlace;

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
        GameManager.Instance.getScore(1);
        Destroy(this);
    }

    
    public void Shot()
    {
        Vector3 aim;

        if (TestAim != null)
        {
            aim = TestAim.transform.position;
        }else{
            aim = GameManager.Instance.cameraPosition;
        }
        Transform gun = shotPlace.transform;
        
        Vector3 direction = (aim - gun.position).normalized;
        GameObject bullet = Instantiate(BulletPrefabs, gun.position,quaternion.identity);
        bullet.transform.LookAt(aim,Vector3.up);
        bullet.transform.Rotate(0, 90, 0);
        bullet.AddComponent<BulletMovement>().Initialize(direction, bulletSpeed);

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
