using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	#region PARTICLES_CONTROL
	public ParticleSystem m_GodsWrathFx;
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


	public LightningBolt[] m_LightingBolt;
	public MonsterAppearance m_MonsterAppearance;
	public int m_Steps=12;
	private int m_LastStep=0;

	public Image m_Slider;
	
	private bool m_SafeZone = true;
	private float m_TotalAnger = 80F;
	[SerializeField]
	private float m_AngerIncrement = 1;
	[SerializeField]
	private float m_IncrementRate = 1;
	[SerializeField]
	private float m_CurrentAnger;

	[SerializeField]
	private float m_SizeStep;

	public float m_VisualAnger=0;
	private float m_SmoothSpeed = 1;


	public void deactivateSafeZone(){
		m_SafeZone = false;
	}
	
	public void DecreaseAnger(float faith){
		m_CurrentAnger = Mathf.Max (m_CurrentAnger - GameManager.Instance.m_RitualPoints * faith * GameManager.Instance.m_BonusPoints, 0);
	}
	
	private IEnumerator Tick(){
		while(true){
			yield return new WaitForSeconds(m_IncrementRate);
			m_CurrentAnger = Mathf.Min(m_CurrentAnger+m_AngerIncrement,m_TotalAnger);
			BossFeedbackUpdate();
			if(m_CurrentAnger >= m_TotalAnger)
				GameManager.Instance.LoseGame();
		}
	}

	public void penaltyGodStep(float _value){
		m_CurrentAnger = Mathf.Min (m_CurrentAnger + _value, m_TotalAnger);
		m_GodsWrathFx.Play (true);
		if (m_CurrentAnger >= m_TotalAnger)
			GameManager.Instance.LoseGame ();
	}
	
	public void BossFeedbackUpdate(){
		m_Slider.fillAmount = m_CurrentAnger/m_TotalAnger;
		//get current step
		int curStep = Mathf.FloorToInt((m_CurrentAnger / m_TotalAnger)*m_Steps);
		if (curStep != m_LastStep) 
		{
			if(curStep>m_LastStep)
			{
				for(int i=0;i<Random.Range(1,m_LightingBolt.Length);i++)
				{
					m_LightingBolt[i].GenerateBolt();
				}
				if(curStep==(m_Steps-1))
					AudioManager.Instance.PlaySfxNoLoop(AudioManager.SfxNoLoop.GOD_STEP_04);
				else
				{
					if(curStep==m_Steps)
						AudioManager.Instance.PlaySfxNoLoop(AudioManager.SfxNoLoop.GOD_GAMEOVER);
					else
						AudioManager.Instance.PlaySfxNoLoop(AudioManager.SfxNoLoop.GOD_STEP_01);
				}

				AudioManager.Instance.GoToLayer(curStep);
			}
			m_MonsterAppearance.MoveToValue(Mathf.Pow((float)curStep/(float)m_Steps,2));

			m_LastStep=curStep;
		}
	}
	
	private IEnumerator SafeZone(){
		while(m_SafeZone && GameManager.Instance.GetGameStart())
			yield return 0;
		StartCoroutine ("Tick");
	}

	public GameObject StartButton,GameTitle;
	private IEnumerator fadeGameTitle(){
		var temp = GameTitle.GetComponent<Image>().color;
		float _time = 2.0F;
		while(temp.a >0 ){
			temp.a -= Time.deltaTime/_time;
			GameTitle.GetComponent<Image>().color = temp;
			yield return 0;
		}
		Destroy (GameTitle);
	}


	// Use this for initialization
	public void FakeStart () 
	{
		m_CurrentAnger = 0;
		StartCoroutine ("SafeZone");
		deactivateSafeZone ();
	}
	
	
}