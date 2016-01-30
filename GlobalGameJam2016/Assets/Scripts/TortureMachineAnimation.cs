using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TortureMachineAnimation : MonoBehaviour
{
	public RectTransform[] m_DoorImages;
	public float m_DoorDistanceToCover;
	[Range(0f, 1f)]
	public float m_DoorCompletion;
	public bool m_UpdateDoor;
	private float m_DoorSteps;
	private Vector2 m_DoorDestination;
	// Use this for initialization
	void Start ()
	{
		m_DoorSteps = 1f / (float)(m_DoorImages.Length-1);
		m_DoorDestination.x = 0;
		m_DoorDestination.y = m_DoorDistanceToCover;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_UpdateDoor) UpdateDoor();
	}

	void UpdateDoor()
	{
		for(int i = 0; i < (m_DoorImages.Length - 1); ++i)
		{
			m_DoorImages[i].anchoredPosition = Vector2.Lerp(Vector2.zero, m_DoorDestination, Mathf.Min(i*m_DoorSteps, m_DoorCompletion));
		}
		m_DoorImages[m_DoorImages.Length-1].anchoredPosition = Vector2.Lerp(Vector2.zero, m_DoorDestination, m_DoorCompletion);
	}
}
