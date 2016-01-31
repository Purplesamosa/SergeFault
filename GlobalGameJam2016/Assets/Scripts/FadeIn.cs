using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour {

	private Image m_Image;

	public void StartFadeIn()
	{
		m_Image = GetComponent<Image>();
		StartCoroutine(InFade());
	}

	private IEnumerator InFade()
	{
		while(m_Image.color.a < 1)
		{
			yield return 0;
			Color _fadeIt = new Color(m_Image.color.r,m_Image.color.g,m_Image.color.b,m_Image.color.a+(0.3f*Time.deltaTime));
			m_Image.color = _fadeIt;
		}
	}
}
