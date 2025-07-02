using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RespawnMng : MonoBehaviour
{

    //public const float RESPAWN_TARGET_WAITE = 0.6f;

    public const float RANGE_DIVISION = 3; //距離の概念の分割（遠・中・近）
    public const float AIR_HIGH = 25f;
    public const float AIR_LOW = 2f;

    public static IEnumerable<Vector3> Posis;

    public static int PosiCount = 0;

    public void Awake() {
        if (this.transform.childCount > 0) {
            init();
        }
    }

    // Use this for initialization
    void init() {

        var new_list = new List<Vector3>();

        for (int i = 0; i < this.transform.childCount; i++) {
            new_list.Add(this.transform.GetChild(i).transform.position);
        }
        Posis = new List<Vector3>(new_list);
        PosiCount = Posis.Count();
    }

    public static Vector3 getMiddleToLongPosiRandom() {

        posiLongSort();
        float max = PosiCount * 2 / RANGE_DIVISION;
        int index = Random.Range(0, (int)max);
        return Posis.ElementAt(index);
    }

    public static Vector3 getNearPosi(Vector3 base_posi) {
        posiShortSort(base_posi);
        return Posis.ElementAt(0);
    }

    public static Vector3 getShortRandom(Vector3 base_posi) {
        posiShortSort(base_posi);
        float max = PosiCount / RANGE_DIVISION;
        int index = Random.Range(0, (int)max);
        return Posis.ElementAt(index);
    }
    public static Vector3[] getShortPosis(Vector3 base_posi) {
        posiShortSort(base_posi);
        float range = PosiCount / RANGE_DIVISION;
        return Posis.Take((int)range).ToArray();
    }

    public static Vector3[] getMiddlePosis() {
        posiLongSort();
        int range = (int)(PosiCount / RANGE_DIVISION);
        return Posis.Skip(range).Take(range).ToArray();
    }

    public static Vector3[] getMiddleToLongPosis() {
        posiLongSort();
        float range = PosiCount / RANGE_DIVISION * 2;
        return Posis.Take((int)range).ToArray();
    }

    private static void posiShortSort(Vector3 base_posi) {
        Posis = Posis.OrderBy(x => Vector3.Distance(x, base_posi));
    }

    private static void posiLongSort(Vector3 base_posi) {
        Posis = Posis.OrderByDescending(x => Vector3.Distance(x, base_posi));
    }

    private static void posiLongSort() {
        Vector3 hero_posi = FieldPlayerMng.hero().transform.position;
        posiLongSort(hero_posi);
    }

    public static Vector3 getRandom() {
        int index = Random.Range(0, PosiCount);
        return Posis.ElementAt(index);
    }

    public static Vector3 getAirRandom() {
        Vector3 air = getAirRandom();
        air.y += AIR_HIGH;
        return air;
    }

}
