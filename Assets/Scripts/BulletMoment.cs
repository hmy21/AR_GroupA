using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    [SerializeField] float maxExistTime = 10;
    [SerializeField] float currentExistTime = 0;

    void Start()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    public void Initialize(Vector3 dir, float spd)
    {
        direction = dir;
        speed = spd;
    }

    void Update()
    {
        currentExistTime += Time.deltaTime;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        if (currentExistTime >= maxExistTime)
        {
            Destroy(gameObject);
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
