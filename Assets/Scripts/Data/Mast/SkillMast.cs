using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SkillMast : PowerMast {
	
	//public enum USE_TIMING{
	//	BATTLE,
	//	FIELD,
	//	DUAL,
	//}

	//public enum SIDE{
	//	FRIEND,
	//	TARGET,
	//	DUAL,
	//}

	//public enum TARGET{
	//	ONE,
	//	ANYTHING,
	//	RANDOM,
	//}
    
    public enum TYPE {
        NORMAL,
        SPECIAL,
    }

    public string Tag;

	

	//public USE_TIMING UseTiming;

	public float PowMin;

	public float PowMax;

	public int Cost; //消費MP

	//public int CastTime;

	//public int HitTime;
	
	public StatusMast.TYPE BaseParam;

    public TYPE Type;

    public string Info;

    public float AvrPow{ get { return (PowMax + PowMin) / 2f; } }

	public static SkillMast[] List;

	public static void load(){
		List = load<SkillMast> ();
	}

}
