using UnityEngine;
using System.Collections;

[System.Serializable]
public class StatusMast 
{

    public const int MAX_STATUS = 9999;

	public enum TYPE
	{
		STR,
		CON,
		AGI,
		LIF,
		LUK,
		MAG,
		MEN,
		HP,
		MP,
		ALL
	}
	
	public const int MAX_HP_CULC = 5;
	public const int MAX_MP_CULC = 5;
	public int[] Param;

	public static readonly string[] TYPE_JP = new string[(int)TYPE.ALL]{
		"強靭",
		"耐久",
		"敏捷",
		"生命",
		"幸運",
		"魔力",
		"精神",
		"HP",
		"MP",
	};

	public int Str{ get{ return Param[(int)TYPE.STR]; } set{ Param[(int)TYPE.STR] = value; } }
	public int Con{ get{ return Param[(int)TYPE.CON]; } set{ Param[(int)TYPE.CON] = value; } }
	public int Agi{ get{ return Param[(int)TYPE.AGI]; } set{ Param[(int)TYPE.AGI] = value; } }
	public int Lif{ get{ return Param[(int)TYPE.LIF]; } set{ Param[(int)TYPE.LIF] = value; } }
	public int Luk{ get{ return Param[(int)TYPE.LUK]; } set{ Param[(int)TYPE.LUK] = value; } }
	public int Mag{ get{ return Param[(int)TYPE.MAG]; } set{ Param[(int)TYPE.MAG] = value; } }
	public int Men{ get{ return Param[(int)TYPE.MEN]; } set{ Param[(int)TYPE.MEN] = value; } }
	public int Hp{ get{ return Param[(int)TYPE.HP]; } set{ Param[(int)TYPE.HP] = value; } }
	public int Mp{ get{ return Param[(int)TYPE.MP]; } set{ Param[(int)TYPE.MP] = value; } }

	public int MaxHp{ get{ return Lif * MAX_HP_CULC; } }
	public int MaxMp{ get { return Men * MAX_MP_CULC; } }

	public StatusMast(){
		Param = new int[(int)TYPE.ALL];
	}
	
	public int getParam( TYPE type ){
		return Param[(int)type];
	}

	public int damage( int value, TYPE type = TYPE.HP, int guard = 0 ){

		int dam = value - guard;

		if ( dam > 0 ) {
			setVal ( -dam, 0, type );
			return dam;
		} else {
			return 0;
		}
	}

	public int heal(int value)
    {
		return heal(value, MaxHp, TYPE.HP);
    }

	public int heal( int value, int max, TYPE type){
		if (value > 0) {
            int before = Hp;
			setVal (value, max, type);
			return Param [(int)type] - before;
		}else {
			return 0;
		}
	}

	private int setVal( int value, int limit, TYPE type ){
		Param [(int)type] += value;
		if ( ( value > 0 && Param [(int)type] > limit )
		    || ( value < 0 && Param [(int)type] < limit ) ) {
			Param [(int)type] = limit;
		}
		return Param [(int)type];
	}

	public int luckJudge(){
		return Random.Range (0, Luk);
	}

	public int[] ParamCopy(){
		int[] cop = new int[Param.Length] ;
		Param.CopyTo (cop,0);
		return cop;
	}

//	public void load( IList list ){
//		for (int i = 0; i < list.Count; i++) {
//			Param [i] = (int)(long)list [i];
//		}
//	}
	
}
