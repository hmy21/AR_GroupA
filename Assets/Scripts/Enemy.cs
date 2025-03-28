using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int Health = 10;//当前生命值
    [SerializeField] int MaxHealth = 10;//满血量
    [SerializeField] float movingSpeed = 1f;//敌人移动速度
    [SerializeField] float moveDuration = 2f; // 每次移动的持续时间
    [SerializeField] float pauseDuration = 1f; // 射击前的停顿时间
    [SerializeField] GameObject BulletPrefabs;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] GameObject TestAim;
    [SerializeField] GameObject shotPlace;

    private Vector3 moveDirection = new Vector3(0,0,0); // 记录当前的移动方向
    //private Vector3 moveDirection = new Vector3(1, 0, 0);
    //moveDirection = new Vector3(1, 0, 0); // 向右移动

    public void takeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            beKilled();
        }
    }

    private void beKilled()
    {
        GameManager.Instance.winScore(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)//受击
    {
        // Debug.Log("get hit");
        if (other.CompareTag("PlayerBullet"))
        {
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
        transform.LookAt(aim, Vector3.up);
        bullet.transform.LookAt(aim, Vector3.up);
        bullet.transform.Rotate(0, 90, 0);
        bullet.AddComponent<BulletMovement>().Initialize(direction, bulletSpeed);

        PickRandomDirection();// 射击后立即改变方向
    }
    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        GameManager.OnGameStateChange += OnGameStateChange;

        // 启动敌人行为的协程
        StartCoroutine(EnemyBehaviorLoop());
        Debug.Log("Enemy Behavior Coroutine Started");

    }

    public void testFuntion()
    {
        // StartCoroutine(EnemyBehaviorLoop());
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    // Update is called once per frame
    void Update()
    {
         transform.LookAt(GameManager.Instance.cameraPosition, Vector3.up);
        // Debug.Log("MoveDirection: " + moveDirection);
        // 敌人按当前方向移动
        transform.position += moveDirection * movingSpeed * Time.deltaTime;
    }

    void PickRandomDirection()
    {
        // 生成一个随机方向（水平移动）
        float angle = UnityEngine.Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
    }

    IEnumerator EnemyBehaviorLoop()
    {
        while (Health > 0) // 敌人存活时循环执行
        {
            // moveDirection= new Vector3(1,0,0);
            Debug.Log("Enemy is moving");
            //选择一个随机方向并移动
            PickRandomDirection();
            transform.position += moveDirection * movingSpeed * Time.deltaTime;
            yield return new WaitForSeconds(moveDuration); 
            Debug.Log("move Direction:"+moveDirection+"movingSpeed:"+movingSpeed);

            //停止移动，准备射击
            moveDirection = Vector3.zero;
            Debug.Log("Enemy stopped moving, preparing to shoot");
            yield return new WaitForSeconds(pauseDuration);

            //进行射击
            Shot();
            Debug.Log("Enemy shot");
        }
    }

    private void OnGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GameStart)
        {
            Destroy(gameObject);
        }
    }
}