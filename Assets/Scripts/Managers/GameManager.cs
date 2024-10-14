#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion


public class GameManager : MonoBehaviour
{
	[SerializeField] private bool _hideCursor;
	[SerializeField] private GameObject[] buildings;
	 public string tagToSearch = "Building"; // Тег для поиска объектов
    public GameObject childPrefab;
	void Start()
	{
		        // Find all objects with the specified tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagToSearch);

        // If no prefab is assigned, log an error and exit the method
        if (childPrefab == null)
        {
            Debug.LogError("Child prefab is not assigned!");
            return;
        }

        // Loop through each found object
        foreach (GameObject obj in taggedObjects)
        {
            // Instantiate a copy of the prefab
            GameObject childObject = Instantiate(childPrefab);

            // Set the instantiated object as a child of the found object
            childObject.transform.SetParent(obj.transform);

            // Optionally reset the position and rotation of the child object relative to the parent
            childObject.transform.localPosition = Vector3.zero; // Set local position to center within the parent
            childObject.transform.localRotation = Quaternion.identity; // Set local rotation to default

            Debug.Log($"Added child to: {obj.name}");
        }	
	}
}

