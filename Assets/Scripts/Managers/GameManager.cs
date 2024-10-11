#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion


public class GameManager : MonoBehaviour
{
	[SerializeField] private bool _hideCursor;

	void Start()
	{
		if (_hideCursor)
        {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}			
	}
}

