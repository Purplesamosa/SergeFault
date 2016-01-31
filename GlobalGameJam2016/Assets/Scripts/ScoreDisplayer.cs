using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplayer : MonoBehaviour
{
	public Sprite[] m_Numbers;
	public Image m_Hundreds;
	public Image m_Tens;
	public Image m_Units;

	private int m_ClampedScore;

	public void SetScore(float _score)
	{
		_score=Mathf.Min (_score, 999f);
		m_ClampedScore = Mathf.FloorToInt (_score);
		Debug.Log ("SCORE: " + m_ClampedScore);
		m_Hundreds.sprite = m_Numbers [m_ClampedScore / 100];
		m_Tens.sprite = m_Numbers [(m_ClampedScore % 100)/10];
		m_Units.sprite = m_Numbers [m_ClampedScore % 10];
	}
}
