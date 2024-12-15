using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMain : MonoBehaviour, IDamageable
{
    [SerializeField] float _hp;
    public void attack(float damage)
    {
        _hp -= damage;
        if (_hp <= 0) { Destroy(gameObject); }
    }

    public void TakeDamage(float damage)
    {

    }
}
