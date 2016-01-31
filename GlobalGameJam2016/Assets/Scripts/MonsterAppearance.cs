using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterAppearance : MonoBehaviour
{

	public Image[] m_Cloud;
	public Color m_CloudTint;

	[Range(0f, 1f)]
	public float m_Completion;
	public bool m_Update;
	public Image m_Monster;
	public Image m_Eye;
	public Image[] m_BG;
	public Color m_BGTint;

	public Color m_InitialColor;

	public RectTransform m_Cloud_RT;
	public RectTransform m_Cloud_LT;
	public RectTransform m_Cloud_RM;
	public RectTransform m_Cloud_LM;
	public RectTransform m_Cloud_RB;
	public RectTransform m_Cloud_LB;
	public RectTransform m_Cloud_T;

	public float m_Cloud_RT_Dest;
	public float m_Cloud_LT_Dest;
	public float m_Cloud_RM_Dest;
	public float m_Cloud_LM_Dest;
	public float m_Cloud_RB_Dest;
	public float m_Cloud_LB_Dest;
	public float m_Cloud_T_Dest;

	private Vector2 m_CRT_Pos;
	private Vector2 m_CLT_Pos;
	private Vector2 m_CRM_Pos;
	private Vector2 m_CLM_Pos;
	private Vector2 m_CRB_Pos;
	private Vector2 m_CLB_Pos;
	private Vector2 m_CT_Pos;

	private Vector2 m_CRT_Dest;
	private Vector2 m_CLT_Dest;
	private Vector2 m_CRM_Dest;
	private Vector2 m_CLM_Dest;
	private Vector2 m_CRB_Dest;
	private Vector2 m_CLB_Dest;
	private Vector2 m_CT_Dest;

	private float m_Lerp;

	#region SMOOTH_VARAIBLES
	private float m_CurVal=0;
	private float m_SpeedSmooth=10;
	private float m_TargetVal=0;
	#endregion

	void Start()
	{
		m_CRT_Pos = m_Cloud_RT.anchoredPosition;
		m_CLT_Pos = m_Cloud_LT.anchoredPosition;
		m_CRM_Pos = m_Cloud_RM.anchoredPosition;
		m_CLM_Pos = m_Cloud_LM.anchoredPosition;
		m_CRB_Pos = m_Cloud_RB.anchoredPosition;
		m_CLB_Pos = m_Cloud_LB.anchoredPosition;
		m_CT_Pos = m_Cloud_T.anchoredPosition;

		m_CRT_Dest = m_CRT_Pos;
		m_CLT_Dest = m_CLT_Pos;
		m_CRM_Dest = m_CRM_Pos;
		m_CLM_Dest = m_CLM_Pos;
		m_CRB_Dest = m_CRB_Pos;
		m_CLB_Dest = m_CLB_Pos;
		m_CT_Dest = m_CT_Pos;

		m_CRT_Dest.x = m_Cloud_RT_Dest;
		m_CLT_Dest.x = m_Cloud_LT_Dest;
		m_CRM_Dest.x = m_Cloud_RM_Dest;
		m_CLM_Dest.x = m_Cloud_LM_Dest;
		m_CRB_Dest.x = m_Cloud_RB_Dest;
		m_CLB_Dest.x = m_Cloud_LB_Dest;
		m_CT_Dest.y = m_Cloud_T_Dest;
	}

	void Update ()
	{
		if(!m_Update) return;
		m_Eye.enabled = m_Lerp > 0.99f;
		//get smooth value first
		m_CurVal += (m_TargetVal - m_CurVal) * m_SpeedSmooth * Time.deltaTime;

		m_Lerp = Mathf.Pow (m_CurVal, 2);
		m_Monster.color = Color.Lerp (Color.black, Color.white, m_Lerp);
		for(int i=0;i<m_BG.Length;i++)
			m_BG[i].color = Color.Lerp (Color.white, m_BGTint, m_Lerp);
		m_Cloud_RT.anchoredPosition = Vector2.Lerp (m_CRT_Pos, m_CRT_Dest, m_Lerp);
		m_Cloud_LT.anchoredPosition = Vector2.Lerp (m_CLT_Pos, m_CLT_Dest, m_Lerp);
		m_Cloud_RM.anchoredPosition = Vector2.Lerp (m_CRM_Pos, m_CRM_Dest, m_Lerp);
		m_Cloud_LM.anchoredPosition = Vector2.Lerp (m_CLM_Pos, m_CLM_Dest, m_Lerp);
		m_Cloud_RB.anchoredPosition = Vector2.Lerp (m_CRB_Pos, m_CRB_Dest, m_Lerp);
		m_Cloud_LB.anchoredPosition = Vector2.Lerp (m_CLB_Pos, m_CLB_Dest, m_Lerp);
		m_Cloud_T.anchoredPosition = Vector2.Lerp (m_CT_Pos, m_CT_Dest, m_Lerp);
		for (int i=0; i<m_Cloud.Length; i++)
			m_Cloud [i].color = Color.Lerp (m_InitialColor, m_CloudTint,m_Lerp);

	}

	public void MoveToValue(float _val)
	{
		m_TargetVal = _val;
	}
}
