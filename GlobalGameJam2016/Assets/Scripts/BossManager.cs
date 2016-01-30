using UnityEngine;
using System.Collections;

public class BossManager: MonoBehaviour {
	#region Singleton
	private static BossManager instance;
	
	public static BossManager Instance
	{
		get
		{
			return instance;
		}
	}
	#endregion

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			DestroyImmediate(this);
		}
	}

	void Update()
	{
		//this is to test the tick system
	}

	private bool m_SafeZone = false;
	private float m_TotalAnger = 60F;
	private float m_AngerIncrement = 1F;
	[SerializeField]
	private float m_IncrementRate = 0.1F;
	[SerializeField]
	private float m_CurrentAnger;

	public void deactivateSafeZone(){
		m_SafeZone = true;
	}

	public void DecreaseAnger(float faith){
		m_CurrentAnger = Mathf.Max (m_CurrentAnger - GameManager.Instance.m_RitualPoints * faith * GameManager.Instance.m_BonusPoints, 0);
	}

	private IEnumerator Tick(){
		while(true){
			yield return new WaitForSeconds(m_IncrementRate);
			m_CurrentAnger += m_AngerIncrement;
			if(m_CurrentAnger >= m_TotalAnger)
				GameManager.Instance.LoseGame();
		}
	}

	public void BossFeedbackUpdate(){

	}

	private IEnumerator SafeZone(){
		while(!m_SafeZone)
			yield return 0;
		StartCoroutine ("Tick");
	}
	// Use this for initialization
	void Start () 
	{
		m_CurrentAnger = 0;
		StartCoroutine ("SafeZone");
	}


}
