using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	private static AudioManager _instance;
	public static AudioManager Instance
	{
		get
		{
			return _instance;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad ((Object)this);
		_instance = this;
	}
	
	public enum SfxLoop
	{
		Skillbar0 = 0,
		Skillbar1 = 1,
		Skillbar2 = 2,
		SFX1
	};

	public enum SfxNoLoop
	{
		MoneyChargeSound,
		KillVillagerSound
	};

	public AudioSource m_Intro;
	public float m_IntroDuration;
	private bool m_IntroFading;
	public AudioSource m_LoopedSound;
	public AudioSource m_EndSound;
	//THE BACKGROUND MUSIC SOUND
	public AudioSource[] m_Background;
	//NO LOOPS ARRAY MUST HAVE AS LENGTH THE AMOUNT OF MAX SFX YOU WANT TO HAVE AT ONCE
	public AudioClip[] m_SfxsNoLoop;
	public AudioSource[] m_SfxSourcesNoLoop;
	private bool[] m_SfxSlotAvailable;
	private int m_SfxSlotsUsed;
	//LOOPS ARRAY MUST HAVE THE LENGTH THE TOTAL AMOUNT OF LOOPABLE SOUND EFFECTS
	public AudioClip[] m_SfxsLoop;
	public AudioSource[] m_SfxSourcesLoop;
	private bool[] m_FadingOut;
	private int m_FadingSfxs;

	private int m_TargetLayer;
	private int m_CurrentLayer;
	private bool m_BGLayerChanging;

	private float m_MaxVolume = 1f;

	void Start ()
	{
		m_CurrentLayer = 0;

		m_SfxSlotAvailable = new bool[m_SfxSourcesNoLoop.Length];

		for(int i = 0; i < m_SfxSlotAvailable.Length; ++i)
		{
			m_SfxSlotAvailable[i] = true;
		}

		m_SfxSlotsUsed = 0;

		m_FadingOut = new bool[m_SfxsLoop.Length];

		for(int i = 0; i < m_SfxSourcesLoop.Length; ++i)
		{
			m_SfxSourcesLoop[i].clip = m_SfxsLoop[i];
			m_FadingOut[i] = false;
		}

		m_FadingSfxs = 0;

		StartCoroutine ("SfxTick");

		m_BGLayerChanging = false;

		m_LoopedSound.Play ();
		m_LoopedSound.loop = true;
		m_LoopedSound.volume = m_MaxVolume;
		
		m_Intro.Play ();
		m_IntroFading = false;

		m_EndSound.Stop();
	}

	void Update ()
	{
		if(m_FadingSfxs > 0) CheckFades();
		if(m_BGLayerChanging) BGChangeLayer();
		if(m_Intro.isPlaying && !m_IntroFading) CheckForStartBGMLoop();
		if(m_IntroFading) IntroFade();

#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Q)) PlaySfxNoLoop(SfxNoLoop.MoneyChargeSound);

		if(Input.GetKeyDown(KeyCode.W)) PlaySfxLoop(SfxLoop.Skillbar0);
		if(Input.GetKeyUp(KeyCode.W)) StopSfxLoop(SfxLoop.Skillbar0);

		if(Input.GetKeyDown(KeyCode.E)) PlaySfxLoop(SfxLoop.SFX1);
		if(Input.GetKeyUp(KeyCode.E)) StopSfxLoop(SfxLoop.SFX1);

		if(Input.GetKeyDown(KeyCode.KeypadPlus)) GoToLayer(m_CurrentLayer+1);
		if(Input.GetKeyDown(KeyCode.KeypadMinus)) GoToLayer(m_CurrentLayer-1);
#endif
	}

	IEnumerator SfxTick()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			CheckForUnusedSlots();
		}
	}

	#region BGM
	void CheckForStartBGMLoop()
	{
		if(m_Intro.time < m_IntroDuration - 1) return;

		BeginBGMLoop ();
	}

	void BeginBGMLoop()
	{
		for(int i = 0; i < m_Background.Length; ++i)
		{
			m_Background[i].Play();
			m_Background[i].volume = (i > 0 ? 0f : m_MaxVolume);
            m_Background[i].loop = true;
        }

		m_IntroFading = true;
	}

	void IntroFade()
	{
		m_Intro.volume = m_IntroDuration - m_Intro.time;
		if(m_Intro.volume <= 0)
		{
			m_Intro.Stop();
			m_IntroFading = false;
		}
	}

	public int GoToLayer(int _target)
	{
		if(m_BGLayerChanging || _target < 0 || _target >= m_Background.Length) return m_TargetLayer;
		m_TargetLayer = _target;
		m_BGLayerChanging = true;
		return m_TargetLayer;
	}

	void BGChangeLayer()
	{
		if(m_TargetLayer == m_CurrentLayer)
		{
			m_Background[m_CurrentLayer].volume += Time.deltaTime;
			if(m_Background[m_CurrentLayer].volume >= 1)
			{
				m_Background[m_CurrentLayer].volume = 1;
				m_BGLayerChanging = false;
			}

			if(m_CurrentLayer - 1 >= 0)
			{
				if(m_Background[m_CurrentLayer - 1].volume > 0) m_Background[m_CurrentLayer - 1].volume -= Time.deltaTime;
				if(m_Background[m_CurrentLayer - 1].volume < 0) m_Background[m_CurrentLayer - 1].volume = 0;
			}
			if(m_CurrentLayer + 1 < m_Background.Length)
			{
				if(m_Background[m_CurrentLayer + 1].volume > 0) m_Background[m_CurrentLayer - 1].volume -= Time.deltaTime;
				if(m_Background[m_CurrentLayer + 1].volume < 0) m_Background[m_CurrentLayer - 1].volume = 0;
			}

		}
		if(m_TargetLayer > m_CurrentLayer)
		{
			m_Background[m_CurrentLayer].volume -= Time.deltaTime;
			m_Background[m_CurrentLayer+1].volume += Time.deltaTime;

			if(m_Background[m_CurrentLayer].volume < 0) m_Background[m_CurrentLayer].volume = 0;
			if(m_Background[m_CurrentLayer+1].volume >= 1)
			{
				m_Background[m_CurrentLayer+1].volume = 1;
				++m_CurrentLayer;
				if(m_CurrentLayer == m_TargetLayer) m_BGLayerChanging = false;
			}
		}
		else
		{
			m_Background[m_CurrentLayer].volume -= Time.deltaTime;
			m_Background[m_CurrentLayer-1].volume += Time.deltaTime;
			
			if(m_Background[m_CurrentLayer].volume < 0) m_Background[m_CurrentLayer].volume = 0;
			if(m_Background[m_CurrentLayer-1].volume >= 1)
			{
				m_Background[m_CurrentLayer-1].volume = 1;
				--m_CurrentLayer;
                if(m_CurrentLayer == m_TargetLayer) m_BGLayerChanging = false;
            }
		}
	}

	public void EndGame()
	{
		for(int i = 0; i < m_Background.Length; ++i)
		{
			m_Background[i].Stop();
		}

		m_EndSound.Play();
		m_EndSound.loop = false;
	}
	#endregion

	#region SFX_NOLOOP
	public void PlaySfxNoLoop(SfxNoLoop _sfx)
	{
		if(m_SfxSlotsUsed >= m_SfxSourcesNoLoop.Length) return;

		++m_SfxSlotsUsed;

		int _slot = GetUnusedId ();

		m_SfxSourcesNoLoop [_slot].clip = m_SfxsNoLoop [(int)_sfx];
		m_SfxSourcesNoLoop [_slot].Play ();
		m_SfxSourcesNoLoop [_slot].volume = m_MaxVolume;
		m_SfxSourcesNoLoop [_slot].loop = false;
		m_SfxSlotAvailable [_slot] = false;
	}

	void CheckForUnusedSlots()
	{
		for(int i = 0; i < m_SfxSourcesNoLoop.Length; ++i)
		{
			if(!m_SfxSlotAvailable[i] && !m_SfxSourcesNoLoop[i].isPlaying) FreeSfxNoLoopSlot(i);
		}
	}

	void FreeSfxNoLoopSlot(int _idx)
	{
		m_SfxSlotAvailable [_idx] = true;
		--m_SfxSlotsUsed;
	}
	
	int GetUnusedId()
	{
		for(int i = 0; i < m_SfxSourcesNoLoop.Length; ++i)
		{
			if(m_SfxSlotAvailable[i]) return i;
		}

		return 0;
	}
	#endregion

	#region SFX_LOOP
	public void PlaySfxLoop(SfxLoop _sfx)
	{
		if(!m_SfxSourcesLoop[(int)_sfx].isPlaying) m_SfxSourcesLoop[(int)_sfx].Play();
		m_SfxSourcesLoop [(int)_sfx].loop = true;
		m_SfxSourcesLoop [(int)_sfx].volume = m_MaxVolume;
		m_FadingOut [(int)_sfx] = false;
		if(m_FadingSfxs>0) --m_FadingSfxs;
	}

	public void StopSfxLoop(SfxLoop _sfx)
	{
		m_FadingOut [(int)_sfx] = true;
		++m_FadingSfxs;
	}

	void CheckFades()
	{
		for(int i = 0; i < m_FadingOut.Length; ++i)
		{
			if(m_FadingOut[i]) UpdateFadeSfx(i);
		}
	}

	void UpdateFadeSfx(int _idx)
	{
		m_SfxSourcesLoop [_idx].volume -= Time.deltaTime;

		if(m_SfxSourcesLoop [_idx].volume <= 0f)
		{
			m_SfxSourcesLoop[_idx].Stop();
			m_SfxSourcesLoop[_idx].volume = 0f;
			m_FadingOut[_idx] = false;
			--m_FadingSfxs;
		}
	}
	#endregion
}
