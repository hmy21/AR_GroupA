using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int Health =10;
    [SerializeField] int MaxHealth =10;
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
        GameManager.Instance.winScore(1);
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("get hit");
        if(other.CompareTag("Bullet")){
            takeDamage(10);
        }
    }

    public void Shot()
    {
        Vector3 aim;

        if (TestAim != null)
        {
            aim = TestAim.transform.position;
        }
        else
        {
            aim = GameManager.Instance.cameraPosition;
        }
        Transform gun = shotPlace.transform;

        Vector3 direction = (aim - gun.position).normalized;
        GameObject bullet = Instantiate(BulletPrefabs, gun.position, quaternion.identity);
        bullet.transform.LookAt(aim, Vector3.up);
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
