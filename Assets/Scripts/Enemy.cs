using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    //private Vector3 moveDirection = new Vector3(0,0,0); // 记录当前的移动方向
    //private Vector3 moveDirection = new Vector3(1, 0, 0);
    //moveDirection = new Vector3(1, 0, 0); // 向右移动

    private NavMeshAgent agent;//移动
    private Vector3 navMeshBoundsMin;
    private Vector3 navMeshBoundsMax;
    private Vector3 moveDirection; // 当前的移动方向

    private Animator animator; // 动画

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
        if (other.CompareTag("Bullet"))
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

        //PickRandomDirection();// 射击后立即改变方向
    }
    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        GameManager.OnGameStateChange += OnGameStateChange;

        // 启动敌人行为的协程
        
        animator = GetComponent<Animator>();//获得动画组件

        agent = GetComponent<NavMeshAgent>();
        agent.speed = movingSpeed;
/*
        agent.isStopped = false;

        // 获取当前NavMesh的边界（确保敌人在桌面上）
        UpdateNavMeshBounds();

        // 开始行为循环
        StartCoroutine(EnemyBehaviorLoop());*/
        // 先禁用 NavMeshAgent，避免在 NavMesh 还没生成时报错
        agent.enabled = false;

        // 启动等待 NavMesh 生成的协程
        StartCoroutine(WaitForNavMesh());
    }

    IEnumerator WaitForNavMesh()
    {
        // 等待 AR 识别到平面，并生成 NavMesh
        Debug.Log("find plane");
        yield return new WaitUntil(() => FindObjectOfType<NavMeshSurface>()?.navMeshData != null);
        Debug.Log("finished");

        // 手动强制烘焙一次 NavMesh（保险起见）
        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        surface.BuildNavMesh();
        Debug.Log("NavMeshSurface Build complete");

        // 等待一帧再执行后续逻辑
        yield return null;

        // 获取当前 NavMesh 的边界（确保敌人在桌面上）
        UpdateNavMeshBounds();
        Debug.Log("get NavmesBounds");

        // 启用 NavMeshAgent
        agent.enabled = true;
        agent.isStopped = false;
        Debug.Log("use NavMeshAgent");

        // 开始行为循环
        StartCoroutine(EnemyBehaviorLoop());
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
        // Debug.Log("MoveDirection: " + moveDirection);
        // 敌人按当前方向移动
        //transform.position += moveDirection * movingSpeed * Time.deltaTime;
    }

    /*void PickRandomDirection()
    {
        // 生成一个随机方向（水平移动）
        float angle = UnityEngine.Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
    }*/

    IEnumerator EnemyBehaviorLoop()
    {
        while (Health > 0) // 敌人存活时循环执行
        {
            /*
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
            */

            // 走路动画
            //animator.ResetTrigger("StartShooting");
            //animator.SetTrigger("StartWalking");

            // 选择一个新的随机目标点
            //Vector3 newPosition = GetRandomNavMeshPosition();
            Debug.Log("begin to get new position");
            Vector3 newPosition = GetSafeNavMeshPosition();
            Debug.Log("222222position" + newPosition);
            agent.SetDestination(newPosition);

            // 移动一段时间
            yield return new WaitForSeconds(moveDuration);
           

            // 停止移动＋动画
            agent.isStopped = true;
            //animator.ResetTrigger("StartWalking");
            //animator.SetTrigger("Idle");
            yield return new WaitForSeconds(pauseDuration);

            // 继续移动，开枪动画
            //animator.ResetTrigger("Idle");
            //animator.SetTrigger("StartShooting");

            //进行射击
            Shot();

            agent.isStopped = false;
        }
    }

    // 在桌面范围内选择一个随机位置
    /*Vector3 GetRandomNavMeshPosition()
    {
        Debug.Log("111111Direction: " + transform.position);
        // 生成一个随机点，x 和 z 方向是随机的，y 为 0
        Vector3 randomPoint = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f));
        NavMeshHit hit;
        // 确保该位置在NavMesh上
        if (NavMesh.SamplePosition(randomPoint, out hit, 3f, NavMesh.AllAreas))
        {
            // 将返回的目标位置的 y 坐标设为当前敌人的 y 坐标，确保它不会垂直飞行
            return new Vector3(hit.position.x, transform.position.y, hit.position.z);
        }
    
        // 如果找不到合适的位置，就返回当前敌人的位置
        return transform.position;
    }*/

    void UpdateNavMeshBounds()
    {
        NavMeshHit hitMin, hitMax;
        if (NavMesh.SamplePosition(Vector3.zero, out hitMin, 10f, NavMesh.AllAreas) && NavMesh.SamplePosition(Vector3.one * 10, out hitMax, 10f, NavMesh.AllAreas))
        {
            navMeshBoundsMin = hitMin.position;
            navMeshBoundsMax = hitMax.position;
        }
    }

    Vector3 GetSafeNavMeshPosition()
    {
        for (int i = 0; i < 10; i++) // 允许最多10次尝试
        {
            Vector3 randomPoint = new Vector3(
                UnityEngine.Random.Range(navMeshBoundsMin.x, navMeshBoundsMax.x),
                transform.position.y,
                UnityEngine.Random.Range(navMeshBoundsMin.z, navMeshBoundsMax.z)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.5f, NavMesh.AllAreas))
            {
                return hit.position; // 找到有效位置，返回
            }
        }

        return transform.position; // 如果找不到有效点，就保持当前位置
    }

    private void OnGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GameStart)
        {
            Destroy(gameObject);
        }
    }
}