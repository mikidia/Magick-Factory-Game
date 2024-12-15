using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private EnemyDataContainer enemyScriptableObj;
    private EnemyStateMachine stateMachine;
    private float enemyDamage;


    public void attack(float damage)
    {
        stateMachine.ClosestAttackPoint.GetComponent<IDamageable>().TakeDamage(enemyDamage);
    }

    public void TakeDamage(float damage)
    {

    }


    private void Start()
    {

        stateMachine = GetComponent<EnemyStateMachine>();
        enemyScriptableObj = stateMachine.EnemyScriptableObj;
        enemyDamage = enemyScriptableObj.damage;


    }
}
