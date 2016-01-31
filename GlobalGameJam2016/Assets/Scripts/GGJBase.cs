using UnityEngine;
using System.Collections;

public class GGJBase : MonoBehaviour {
	
	public enum MinionType
	{
		SonOfARitch=0,
		Believer=1,
		Martir=2,
		RARE=3,
		MAX=4
	}

	public enum Penalty
	{
		LoseInvestment_Small=0,
		Difamation_Small=1,
		Blasfemy_Small=2,
		Loot_Small=3,

		LoseInvestment_Big=4,
		Difamation_Big=5,
		Blasfemy_Big=6,
		Loot_Big=7,

		MAX
	}
}
