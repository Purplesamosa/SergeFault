using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeShit : MonoBehaviour {

	public Image[] m_Image;


	public void StartFadeIn()
	{
		GetComponent<Button> ().interactable = false;
		StartCoroutine(InFade());
	}

	private IEnumerator InFade()
	{
		while(m_Image[0].color.a <= 0)
		{
			yield return 0;
			Color _fadeIt = new Color(m_Image[0].color.r,m_Image[0].color.g,m_Image[0].color.b,m_Image[0].color.a-(0.3f*Time.deltaTime));
			for(int i=0;i<m_Image.Length;i++)
			{
				m_Image[i].color = _fadeIt;
			}
		
		}
		for(int i=0;i<m_Image.Length;i++)
		{
			m_Image[i].enabled = false;
		}
	}
}
