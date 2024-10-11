#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class PlayerCamera : MonoBehaviour
{
    #region Declarations

    [Header("Camera Movement Settings")]
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _cameraCurrentX = 0;
    private Vector2 _mouseDelta;

    #endregion

    #region MonoBehaviour

    private void LateUpdate()
    {
        UpdateMouseLook();
    }

    #endregion

    void UpdateMouseLook()
    {
        _mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _mouseSensitivity;

        _cameraCurrentX -= _mouseDelta.y;

        _cameraCurrentX = Mathf.Clamp(_cameraCurrentX, -90, 90);

        Camera.main.transform.localEulerAngles = Vector3.right * _cameraCurrentX;
        transform.Rotate(Vector3.up * _mouseDelta.x);
    }
}
