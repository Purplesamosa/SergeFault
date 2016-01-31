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
	public MonsterAppearance m_Appearance;

	public Slider m_Slider;

	private bool m_SafeZone = true;
	[SerializeField]
	private float m_TotalAnger = 60F;
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

	private IEnumerator VisualTick()
	{
		while(true)
		{
			yield return 0;
			BossFeedbackUpdate();
			if(m_CurrentAnger==m_TotalAnger&&m_VisualAnger>m_TotalAnger*0.99f)
			{
				m_Slider.value = m_TotalAnger;
				m_Appearance.m_Completion=1.0f;
				StopCoroutine("VisualTick");
			}
		}
	}

	private IEnumerator Tick(){
		while(true){
			yield return new WaitForSeconds(m_IncrementRate);
			m_CurrentAnger = Mathf.Min(m_CurrentAnger+m_AngerIncrement,m_TotalAnger);
			if(m_CurrentAnger >= m_TotalAnger)
				GameManager.Instance.LoseGame();
		}
	}

	public void penaltyGodStep(){
		m_CurrentAnger = Mathf.Min ( m_CurrentAnger + m_SizeStep,m_TotalAnger);
		if(m_CurrentAnger >= m_TotalAnger)
			GameManager.Instance.LoseGame();
	}

	public void BossFeedbackUpdate(){
		m_VisualAnger+=(m_CurrentAnger-m_VisualAnger)*m_SmoothSpeed*Time.deltaTime;
		m_Slider.value = m_VisualAnger;
		m_Appearance.m_Completion = m_VisualAnger / m_TotalAnger;//Mathf.Pow(m_VisualAnger/m_TotalAnger,3);
	}

	private IEnumerator SafeZone(){
		while(m_SafeZone && GameManager.Instance.GetGameStart())
			yield return 0;
		StartCoroutine ("Tick");
		StartCoroutine ("VisualTick");
	}

	//MOVE THIS TO GAMEMANAGER!!
	/*
	GameObject GameTitle, StartButton;
	private IEnumerator fadeGameTitle(){
		var temp = Renderer.GetComponent<Material> ().color;
		float _time = 2.0F;
		while(temp.a >0 ){
			temp.a -= Time.deltaTime/_time;
			Renderer.GetComponent<Material>().color = temp;
		}
		yield return 0;
	}
	
	private void StartGameButtonDown(){
		Destroy (StartButton);
		StartCoroutine("fadeGameTitle");
		GameStarted ();
	}
	*/
	//UNTIL HEREEEEE!!

	// Use this for initialization
	void Start () 
	{
		m_Slider.maxValue = m_TotalAnger;
		m_CurrentAnger = 0;
		StartCoroutine ("SafeZone");
	}


}
