using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterAppearance : MonoBehaviour
{
	[Range(0f, 1f)]
	public float m_Completion;
	public bool m_Update;
	public Image m_Monster;
	public Image m_Eye;
	public Image m_BG;
	public Color m_BGTint;

	public RectTransform m_Cloud_RT;
	public RectTransform m_Cloud_LT;
	public RectTransform m_Cloud_RM;
	public RectTransform m_Cloud_LM;
	public RectTransform m_Cloud_RB;
	public RectTransform m_Cloud_LB;

	public float m_Cloud_RT_Dest;
	public float m_Cloud_LT_Dest;
	public float m_Cloud_RM_Dest;
	public float m_Cloud_LM_Dest;
	public float m_Cloud_RB_Dest;
	public float m_Cloud_LB_Dest;

	private Vector2 m_CRT_Pos;
	private Vector2 m_CLT_Pos;
	private Vector2 m_CRM_Pos;
	private Vector2 m_CLM_Pos;
	private Vector2 m_CRB_Pos;
	private Vector2 m_CLB_Pos;

	private Vector2 m_CRT_Dest;
	private Vector2 m_CLT_Dest;
	private Vector2 m_CRM_Dest;
	private Vector2 m_CLM_Dest;
	private Vector2 m_CRB_Dest;
	private Vector2 m_CLB_Dest;

	private float m_Lerp;

	void Start()
	{
		m_CRT_Pos = m_Cloud_RT.anchoredPosition;
		m_CLT_Pos = m_Cloud_LT.anchoredPosition;
		m_CRM_Pos = m_Cloud_RM.anchoredPosition;
		m_CLM_Pos = m_Cloud_LM.anchoredPosition;
		m_CRB_Pos = m_Cloud_RB.anchoredPosition;
		m_CLB_Pos = m_Cloud_LB.anchoredPosition;

		m_CRT_Dest = m_CRT_Pos;
		m_CLT_Dest = m_CLT_Pos;
		m_CRM_Dest = m_CRM_Pos;
		m_CLM_Dest = m_CLM_Pos;
		m_CRB_Dest = m_CRB_Pos;
		m_CLB_Dest = m_CLB_Pos;

		m_CRT_Dest.x = m_Cloud_RT_Dest;
		m_CLT_Dest.x = m_Cloud_LT_Dest;
		m_CRM_Dest.x = m_Cloud_RM_Dest;
		m_CLM_Dest.x = m_Cloud_LM_Dest;
		m_CRB_Dest.x = m_Cloud_RB_Dest;
		m_CLB_Dest.x = m_Cloud_LB_Dest;
	}

	void Update ()
	{
		if(!m_Update) return;
		m_Eye.enabled = m_Lerp == 1;
		m_Lerp = Mathf.Pow (m_Completion, 2);
		m_Monster.color = Color.Lerp (Color.black, Color.white, m_Lerp);
		m_BG.color = Color.Lerp (Color.white, m_BGTint, m_Lerp);
		m_Cloud_RT.anchoredPosition = Vector2.Lerp (m_CRT_Pos, m_CRT_Dest, m_Lerp);
		m_Cloud_LT.anchoredPosition = Vector2.Lerp (m_CLT_Pos, m_CLT_Dest, m_Lerp);
		m_Cloud_RM.anchoredPosition = Vector2.Lerp (m_CRM_Pos, m_CRM_Dest, m_Lerp);
		m_Cloud_LM.anchoredPosition = Vector2.Lerp (m_CLM_Pos, m_CLM_Dest, m_Lerp);
		m_Cloud_RB.anchoredPosition = Vector2.Lerp (m_CRB_Pos, m_CRB_Dest, m_Lerp);
		m_Cloud_LB.anchoredPosition = Vector2.Lerp (m_CLB_Pos, m_CLB_Dest, m_Lerp);
	}
}
