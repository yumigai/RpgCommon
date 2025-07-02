using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyEncountMast : MasterCmn
{

    public int Id;
    public int MapId;
    public int EnemyId;
    public int Percent;
    public int MaxNum;
    public int Lv;

    public static IReadOnlyList<EnemyEncountMast> List;

    public static List<UnitStatusTran> encount(int map_id, int field_size) {

        List<UnitStatusTran> enemys = new List<UnitStatusTran>();

        var encounts = List.Where(it => it.MapId == map_id);
        if (encounts == null || encounts.Count() <= 0) {
            return enemys;
        }

        var enc_per = encounts.Select(it => it.Percent).ToArray();

        for (int i = 0, fill = 0; i < field_size && fill < field_size; i++) {
            int index = UtilToolLib.getRateRandom(enc_per);

            var enc = encounts.ElementAt(index);

            var mst = EnemyMast.List.First(it => it.Id == enc.EnemyId);

            int lv = enc.Lv == 0 ? mst.BaseLv : enc.Lv;

            var tran = EnemyMast.getEnemy(mst, lv);

            var num = Random.Range(1, enc.MaxNum);

            var size = mst.Size;

            for (int j = 0; j < num; j++) {
                fill += size;
                if (fill <= field_size) {
                    enemys.Add(tran);
                }
            }
        }

        return enemys;
    }

    public static void load() {
        List = load<EnemyEncountMast>();
    }
}
