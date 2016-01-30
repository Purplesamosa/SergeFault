using UnityEngine;
using System.Collections;

public class BossManager: Singleton<BossManager> {

	private float m_SafeZone;
	private float m_TotalAnger;
	private float m_AngerIncrement;
	private float m_IncrementRate;

	public void DecreaseAnger(float faith){
		m_TotalAnger = m_TotalAnger - GameManager.Instance.m_RitualPoints * faith * GameManager.Instance.m_BonusPoints;
		if (m_TotalAnger < 0)
			m_TotalAnger = 0;
	}

	private IEnumerator Tick(){
		while(true){
			yield return new WaitForSeconds(m_IncrementRate);
			m_TotalAnger = m_TotalAnger + m_AngerIncrement;
			if(m_TotalAnger >= 1F)
				GameManager.Instance.LoseGame();
		}
	}

	public void BossFeedbackUpdate(){

	}

	private IEnumerator SafeZone(){
		yield return new WaitForSeconds (m_SafeZone);
		StartCoroutine ("Tick");
	}
	// Use this for initialization
	void Start () {
		StartCoroutine ("SafeZone");
	}


}
