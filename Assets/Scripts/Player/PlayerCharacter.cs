#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#endregion

public class PlayerCharacter : MonoBehaviour, IDamageable
{
    #region Declarations

    [Header("Movement Settings")]
    [SerializeField]
    private float _movementSpeed;
    private bool isMovementDisabled = false;

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    private float _playerHp;

    [Header("My References")]
    private Rigidbody _rigidBody;

    #endregion

    #region MonoBehaviour




    public void DisableMovement(float duration)
    {
        isMovementDisabled = true;
        Invoke(nameof(EnableMovement), duration);
    }

    private void EnableMovement()
    {
        isMovementDisabled = false;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PlayerInput();
        MoveInDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));

        if (isMovementDisabled)
            return;

        // Example movement logic
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        transform.Translate(direction * 5f * Time.deltaTime, Space.World);
    }

    #endregion


    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.Mouse0)) { }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Interact() { }

    private void Jump()
    {
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    private void MoveInDirection(Vector2 direction)
    {
        Vector3 finalVelocity =
            (direction.x * transform.right + direction.y * transform.forward).normalized
            * _movementSpeed;

        finalVelocity.y = _rigidBody.velocity.y;
        _rigidBody.velocity = finalVelocity;
    }

    protected void TakeDamage(int damage) { }

    public void TakeDamage(float damage)
    {
        _playerHp -= damage;

        print(_playerHp);
    }

    public void attack(float damage) { }
}
