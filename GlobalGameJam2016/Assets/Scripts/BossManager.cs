using UnityEngine;
using System.Collections;

public class BossManager: Singleton<BossManager> {

	private bool m_SafeZone = false;
	private float m_TotalAnger = 60F;
	private float m_AngerIncrement = 1F;
	private float m_IncrementRate = 0.1F;
	private float m_CurrentAnger;

	public void deactivateSafeZone(){
		m_SafeZone = true;
	}

	public void DecreaseAnger(float faith){
		m_CurrentAnger -= GameManager.Instance.m_RitualPoints * faith * GameManager.Instance.m_BonusPoints;
		if (m_TotalAnger < 0)
			m_TotalAnger = 0;
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
			yield return new WaitForSeconds (m_SafeZone);
		StartCoroutine ("Tick");
	}
	// Use this for initialization
	void Start () {
		m_CurrentAnger = 0;
		StartCoroutine ("SafeZone");
	}


}
