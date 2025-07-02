using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RuneTran
{
    public int Id;
    public int MasterId;
    public int Value;

    public RuneTran(RuneMast mast, int value ) {
        MasterId = mast.Id;
        Value = value;
    }

    public RuneTran(int id, int value) {
        MasterId = id;
        Value = value;
    }

    public RuneMast Mst {
        get {
            return RuneMast.List.FirstOrDefault(it => it.Id == MasterId);
        }
    }

    public string getName() {
        string name = Mst.Name;
        if (Mst.Lv == 0) {
            return name;
        } else {
            return string.Format("{0}({1})", name, Mst.Lv);
        }
    }

    public string getInfo() {

        string txt = Mst.UniqueInfo;

        if (txt.Length == 0) {

            txt = RuneMast.INFO_TEMPLATE[(int)Mst.Kind];

            if (Mst.IsElement()) {
                txt = string.Format(txt, new string[] { CommonProcess.getElementWord(Mst.Element), Value.ToString() });
            } else {
                txt = string.Format(txt, Value);
            }

            //switch (Mst.Kind) {
            //    case RuneMast.KIND.ELEMENT_ATTACK:
            //    case RuneMast.KIND.ELEMENT_GUARD:
            //        txt = string.Format(txt, new string[] { CommonProcess.getElementWord(Mst.Element), Value.ToString() });
            //        break;
            //    default:
            //        txt = string.Format(txt, Value);
            //        break;
            //}
        }
        return txt;

    }

    public static RuneTran getRandomRune(int lv) {  

        float judge = Random.Range(0f, 100f);
        if( judge > GameConst.BASE_GET_RUNE_PERCENT) {
            return null;
        }

        var rare = UtilToolLib.getRateRandom(GameConst.RARITY_PERCENT);

        RuneMast[] target = System.Array.FindAll<RuneMast>(RuneMast.List, r => r.Lv <= lv && rare >= r.Rarity );

        if (target.Length == 0) {
            return null;
        }

        int get_index = Random.Range(0, target.Length);
        var mst = target[get_index];

        //最大値をランダム値に含めるため、+1する
        var value = Random.Range(mst.RandValueMin, mst.RandValueMax + 1); 


        //rune.EffectTurn = Random.Range( rune.RandEffectTurnMin, rune.RandEffectTurnMax );

        return new RuneTran(mst, value);
    }
}
