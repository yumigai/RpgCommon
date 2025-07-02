using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TrapMast : MasterCmn {

//	public const int TRAP_DIFFICULT = 5;

	public const float TRAP_LV_DAMAGE = 0.2f;

	public enum TYPE
	{
		DAMAGE,
		ALERT,

	}


    public int Id;
    public string Name;
    public int Lv;
    public int Percent;
    public TYPE Type;
    public StatusMast.TYPE DamageType;
    public UnitMast.TARGET Target;
    public float Value;

	public static Dictionary<int,TrapMast> Traps{ get; set; }

	public string damageJp{ get{ return StatusMast.TYPE_JP[(int)DamageType]; } }

    public static TrapMast[] List;

	public static TrapMast encountTrap( int trap_lv ){

		TrapMast[] traps = System.Array.FindAll<TrapMast>(List, it => it.Lv <= trap_lv);

		if (traps.Length > 0) {
			int index = Random.Range(0, traps.Length);
			return traps[index];
		}
//        {
//            System.Array.Sort<TrapMast>(trap_entrys, (a, b) => b.Lv - a.Lv);
//            int max_lv = trap_entrys[0].Lv;
//
//            TrapMast[] traps = System.Array.FindAll<TrapMast>(trap_entrys, it => it.Lv == max_lv);
//
//            int index = Random.Range(0, traps.Length);
//            return traps[index];
//
//        }

        return null;
	}

    public static void load()
    {
        List = load<TrapMast>();
    }


}
