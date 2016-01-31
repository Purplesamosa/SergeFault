using UnityEngine;
using System.Collections;

public class aeiou : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{

	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A)) Application.LoadLevel (Application.loadedLevel);
	}
}
