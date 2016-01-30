using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	static private GameManager instance;
	static public GameManager Instance
	{
		get{
			return instance;
		}
	}

	void Awake()
	{
		instance = this;
	}

	public float GetBarValue()
	{
		return 0.5f;
	}
}
