#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class PlayerCharacter : MonoBehaviour
{
    #region Declarations

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;

    [Header("My References")]
    private Rigidbody _rigidBody;
    private BaseGun _equippedGun;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        _equippedGun = GetComponentInChildren<BaseGun>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PlayerInput();
        MoveInDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }

    #endregion

    private void OnTriggerEnter (Collider other)
    {
        ICollactable collectable = other.GetComponent<ICollactable>();
        if (collectable != null)
        {
            collectable.Collect();
        }
    }
    private void PlayerInput()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            FireGun();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Interact()
    {

    }

    private void FireGun()
    {
        _equippedGun?.Fire();
    }

    private void Jump()
    {
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    private void MoveInDirection(Vector2 direction)
    {
        Vector3 finalVelocity = (direction.x * transform.right + direction.y * transform.forward).normalized * _movementSpeed;

        finalVelocity.y = _rigidBody.velocity.y;
        _rigidBody.velocity = finalVelocity;
    }

    protected void TakeDamage(int damage)
    {

    }
    
}
