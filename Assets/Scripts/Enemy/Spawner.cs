using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;



public class Spawner : MonoBehaviour
{

    [Header("Spawner Settings")]
    [SerializeField]protected GameObject spawnerParent;
    [SerializeField]protected GameObject[] Enemy;
    [SerializeField]private float spawnTime;
    float spawnTimer;



    Transform[] spawners;
    int childCount;
    int chooseSpawner;
    private int chooseEnemy;


    // Start is called before the first frame update
    void Start ()
    {
        spawnTimer = spawnTime;
        childCount = spawnerParent.transform.childCount;
        spawners = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            spawners[i] = spawnerParent.transform.GetChild(i);
        }

    }

    // Update is called once per frame
    void Update ()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            spawnTimer = spawnTime;
            Spawn();
        }
    }

    void Spawn ()
    {
        chooseSpawner = Random.Range(0, childCount);
        chooseEnemy = Random.Range(0, Enemy.Length);

        GameObject enemy =  Instantiate(Enemy[chooseEnemy], spawners[chooseSpawner]);
        enemy.transform.position =new Vector3 ( spawners[chooseSpawner].transform.position.x+1, spawners[chooseSpawner].transform.position.y, spawners[chooseSpawner].transform.position.z);

    }
}
