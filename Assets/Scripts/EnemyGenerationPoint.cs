using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGenerationPoint : MonoBehaviour
{
    [SerializeField] float GenerateTime;
    [SerializeField] float currentTime;
    [SerializeField] GameObject EnemyPrefabs;

    void Start()
    {
        currentTime = 0;
    }
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= GenerateTime)
        {
            Instantiate(EnemyPrefabs,this.transform.position, Quaternion.identity);
            currentTime = 0;
        }
    }

}
