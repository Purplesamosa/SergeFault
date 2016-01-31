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
		LoseInvestment_Small,
		LoseInvestment_Big,
		Difamation_Small,
		Difamation_Big,
		Blasfemy_Small,
		Blasfemy_Big,
		Loot_Small,
		Loot_Big,
		MAX
	}
}
