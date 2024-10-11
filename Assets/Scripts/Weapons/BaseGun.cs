#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class BaseGun : MonoBehaviour
{
    #region Declarations

    [Header("Gun Settings")]
    [SerializeField] public float _gunRange;
    [SerializeField] public float _fireRate;
    [SerializeField] public float _impactForce;
    [SerializeField] private GameObject _impactEffect;
    private float _timePassed;
    [SerializeField] public float _kickForce;
    [SerializeField] public float _kickRecoil;

    [SerializeField] public int _ammo;
    [SerializeField] public int _ammoLeft;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        _timePassed = _fireRate;
    }

    #endregion

    public void Fire()
    {

        if(_timePassed < _fireRate)
        {
            _timePassed += Time.deltaTime;
        }
        else
        {
            _timePassed = 0;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _gunRange))
            {
                Debug.Log("We hit something");

                //Add force to hit objects
                hit.rigidbody?.AddForce(-hit.normal * _impactForce, ForceMode.Impulse);

                //Create the impact object
                GameObject impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1.0f);

            }
        }
        if (transform.rotation.x != -90)
        {
            


        }
        
        
    }
}
