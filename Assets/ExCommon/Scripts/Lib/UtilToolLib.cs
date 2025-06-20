using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class UtilToolLib : MonoBehaviour
{

    public static void addSaveInt(string key, int num) {
        int upd = PlayerPrefs.GetInt(key);
        upd += num;
        PlayerPrefs.SetInt(key, upd);
        PlayerPrefs.Save();
    }


    public const int TICK_SECOND = 10000000;
    public const string DATE_TIME_STR_FORMAT = "yyyy/MM/dd HH:mm";
    public const string TIME_STR_FORMAT = "HH:mm";
    public static readonly char[] ALPHABET_INDEX = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

    public static uint getUnixTime(DateTime time) {

        DateTime init_time = new DateTime(1970, 1, 1);
        uint ut = (uint)(int)(time.Second - init_time.Second);
        return ut;
    }

    public static uint getUnixTimeDiff(DateTime time1, DateTime time2) {
        uint unix1 = getUnixTime(time1);
        uint unix2 = getUnixTime(time2);
        return unix1 - unix2;
    }

    /// <summary>
    /// 対象となるIndexをランダムで返す
    /// </summary>
    /// <param name="rates"></param>
    /// <param name="min"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public static int getRateRandom(int[] rates, int min = 0, int def = -1) {
        int max = rates.Sum();
        return getRateRandom(min, max, rates, def);
    }

    public static int getRateRandom(float[] rates, int min = 0, int def = -1) {
        float max = rates.Sum();
        float judge = UnityEngine.Random.Range(min, max);
        return getRateIndex(judge, rates, def);
    }

    public static int getRateRandom(int min, int max, int[] rates, int def = -1) {
        int judge = UnityEngine.Random.Range(min, max);
        return getRateIndex(judge, rates, def);
    }

    public static List<T> objectToList<T>(object obj) {
        if (obj == null) {
            return null;
        }
        List<T> ret = new List<T>();
        IList ilis = obj as IList;
        foreach (T val in ilis) {
            ret.Add(val);
        }
        return ret;
    }

    public static int[] getIntList(object obj) {
        IList list = obj as IList;
        int[] ret = new int[list.Count];
        for (int i = 0; i < ret.Length; i++) {
            ret[i] = (int)(long)list[i];
        }
        return ret;
    }

    public static List<T2> objectToList<T1, T2>(object obj) {
        List<T1> lis1 = objectToList<T1>(obj);
        List<T2> lis2 = new List<T2>();
        foreach (T1 v1 in lis1) {
            T2 v2 = (T2)Convert.ChangeType(v1, typeof(T2));
            lis2.Add(v2);
        }
        return lis2;
    }

    public static int[] purgeNumber(string str) {
        return purgeNumber<int>(str);
    }

    public static float[] purgeFloat(string str) {
        return purgeNumber<float>(str);
    }

    private static T[] purgeNumber<T>(string str) {

        string[] purge_strs = str.Split(',');

        object[] arr = new object[purge_strs.Length];

        for (int i = 0; i < purge_strs.Length; i++) {

            if (typeof(T) == typeof(int)) {
                arr[i] = int.Parse(purge_strs[i]);
            }
            if (typeof(T) == typeof(long)) {
                arr[i] = long.Parse(purge_strs[i]);
            }
            if (typeof(T) == typeof(float)) {
                arr[i] = float.Parse(purge_strs[i]);
            }
            if (typeof(T) == typeof(double)) {
                arr[i] = double.Parse(purge_strs[i]);
            }

        }

        T[] rtn_value = (T[])arr.Cast<T>().ToArray<T>();

        return rtn_value;
    }

    public static int getRateIndex(int judge, int[] rates, int def = -1) {

        int rate = 0;

        for (int i = 0; i < rates.Length; i++) {
            rate += rates[i];
            if (judge < rate) {
                return i;
            }
        }

        return def;
    }

    public static int getRateIndex(float judge, float[] rates, int def = -1) {

        float rate = 0;

        for (int i = 0; i < rates.Length; i++) {
            rate += rates[i];
            if (judge < rate) {
                return i;
            }
        }

        return def;
    }

    public static void writeText(string file_name, string txt) {
        string path = Application.dataPath + "/" + file_name;
        File.WriteAllText(path, txt);
    }
    public static string readText(string file_name) {
        string path = Application.dataPath + "/" + file_name;
        string txt = File.ReadAllText(path);
        return txt;
    }

    public static string loadText(string path) {

        var res = Resources.Load(path);

        if (res == null) {
            return "";
        }

        TextAsset txt = Instantiate(res) as TextAsset;

        return txt.text;
    }

    public static string[][] loadTsv(string path, int init = 0, char sep = '\t', bool empty_skip = true) {
        return loadCsv(path, init, sep, empty_skip);
    }

    public static string[][] loadCsv(string path, int init = 0, char sep = ',', bool empty_skip = true) {

        string txt = loadText(path);
        
        if (string.IsNullOrEmpty(txt)){
            return null;
        }

        string[] line = purgeStringLine(txt);

        if (empty_skip) {
            line = Array.FindAll(line, it => !string.IsNullOrEmpty(it));
        }

        string[][] csv = new string[line.Length - init][];

        for (int i = 0; i < csv.Length; i++) {
            int l_i = i + init;
            csv[i] = line[l_i].Split(sep);
        }
        return csv;

    }

    // public static string readResourceText(string file_name) {

    //     string path = file_name;
    //     string txt = Resources.Load<TextAsset>(path).text;
    //     return txt;
    // }

    public static string[] purgeStringLine(string str) {
        str = str.Replace('\r'.ToString(), "");
        string[] line = str.Split('\n');
        return line;
    }

    public static string convertFloat(float[] param, string sep = ",") {

        return string.Join(sep, param);

        //string[] strs = Array.ConvertAll<float, string>(param,
        //    delegate (float value) {
        //        return value.ToString();
        //    });

        //string csv = string.Join(sep, strs);
        //return csv;
    }



    //public static IList changeToIList<IEnurator>(List<object> list) {
    //    IList ilist = new List<object>();
    //    foreach (object val in list) {
    //        ilist.Add(val);
    //    }
    //    return ilist;
    //}

    public static DateTime stringToTime(string datetimeString) {
        return System.DateTime.FromBinary(System.Convert.ToInt64(datetimeString));
    }
    public static string timeToString(DateTime time) {
        return time.ToBinary().ToString();
    }

    //public static Vector3[] getTargetPositions(Transform pare) {
    //    Vector3[] posis = new Vector3[pare.childCount];
    //    for (int i = 0; i < pare.childCount; i++) {
    //        posis[i] = pare.GetChild(i).transform.position;
    //    }
    //    return posis;
    //}

    //public static Transform[] getTargetTransforms(Transform pare) {
    //    Transform[] tra = new Transform[pare.childCount];
    //    for (int i = 0; i < pare.childCount; i++) {
    //        tra[i] = pare.GetChild(i).transform;
    //    }
    //    return tra;
    //}

    /// <summary>
    /// 配列カウントアップ
    /// Array.FoaEachでよくね？
    /// </summary>
    /// <param name="arr"></param>
	public static void fillCountUp(ref int[] arr) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] = i;
        }
        //Enumerable.Repeatは重いので避けるため
    }

    /// <summary>
    /// 特定の値を除外したランダム値の取得
    /// </summary>
    /// <param name="range"></param>
    /// <param name="exclusion"></param>
    /// <returns></returns>
	public static int randomExclusion(int range, int exclusion) {
        int num = UnityEngine.Random.Range(0, range - 1);
        if (num >= exclusion) {
            num++;
        }
        return num;
    }

    public static double diffDayTime(DateTime time1, DateTime time2) {
        System.TimeSpan diff_proc = time1.Subtract(time2);
        return diff_proc.TotalSeconds;
    }
    public static double diffHourTime(DateTime time1, DateTime time2) {
        System.TimeSpan diff_proc = time1.Subtract(time2);
        return diff_proc.TotalHours;
    }

    public static int[] getAllIndex<T>(T[] tags, T search) {

        int index = 0;
        List<int> indexs = new List<int>();

        while (index >= 0) {
            index = Array.IndexOf(tags, search, index);
            if (index >= 0) {
                indexs.Add(index);
                index++;
            }
        }

        return indexs.ToArray();

    }

    /// <summary>
    /// Linq の Selectの代用（非推奨））
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tags"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public static List<int> getAllIndex<T>(List<T> tags, T search) {

        int index = 0;
        List<int> indexs = new List<int>();

        while (index >= 0) {
            index = tags.IndexOf(search, index);
            if (index >= 0) {
                indexs.Add(index);
                index++;
            }
        }

        return indexs;
    }

    /// <summary>
    /// Nullまたは空を判定
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(Array array) {
        return (array == null || array.Length == 0);
    }

    /// <summary>
    /// 全ボタンON/OFF切替
    /// </summary>
    /// <param name="buttons"></param>
    /// <param name="value"></param>
    public static void AllButtonOnOff(Button[] buttons, bool value) {
        Array.ForEach(buttons, it => { if (it != null) it.interactable = value; });
    }

    /// <summary>
    /// オブジェクトアクティブ変更
    /// </summary>
    /// <param name="objs"></param>
    /// <param name="value"></param>
    public static void AllObjectActive(GameObject[] objs, bool value) {
        Array.ForEach(objs, it => { if (it != null) it.SetActive(value); });
    }
    public static void AllObjectActive(object[] objs, bool value) {
        Array.ForEach(objs, (it => { if (it != null) ((MonoBehaviour)it).gameObject.SetActive(value); }));
    }

    /// <summary>
    /// 中心を指定して回転
    /// </summary>
    /// <param name="obj">回転する物</param>
    /// <param name="center">回転する中心座標</param>
    /// <param name="move">中心座標に合わせて移動する位置</param>
    /// <param name="angle">回転量</param>
    /// <param name="before">親</param>
    public static void rotatePivot(GameObject obj, Vector3 center, Vector3 move, Vector3 angle, Transform before) {
        GameObject pare = new GameObject();
        pare.transform.position = center;
        obj.transform.parent = pare.transform;
        pare.transform.position = move;
        pare.transform.eulerAngles = angle;
        obj.transform.parent = before;
        Destroy(pare);
    }

    /// <summary>
    /// 画像の枠に合わせて、画像の縦横比を合わせてサイズを調整する
    /// </summary>
    public static bool changeImageSizeFrameFit(Image img) {

        Vector2 psize = img.transform.parent.GetComponent<RectTransform>().rect.size;

        if (psize.x <= 0 || psize.y <= 0) {
            //レイアウト要素で調整中の場合
            return false;
        }

        img.SetNativeSize();
        var rect = img.GetComponent<RectTransform>().rect;
        Vector2 size = rect.size;

        if (size.x > psize.x || size.y > psize.y) {
            float ratio = size.y / size.x;

            //一旦幅に合わせる
            size.x = psize.x;
            size.y = size.x * ratio;

            if (size.y > psize.y) {
                size.y = psize.y;
                size.x = size.y / ratio;
            }

            rect.size = size;
            img.GetComponent<RectTransform>().sizeDelta = size;

        }

        return true;
    }

    public static bool judgePer100(int per) {
        int rand = UnityEngine.Random.Range(0, 100);
        return rand < per;
    }

    public static char getAlphabet(int index) {
        index = Mathf.Clamp(index, 0, ALPHABET_INDEX.Length - 1);
        return ALPHABET_INDEX[index];
    }

    public static bool addId(int id, ref List<int> list) {
        if (list.IndexOf(id) < 0) {
            list.Add(id);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 座標変換
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="screen_posi"></param>
    /// <returns></returns>
    public static Vector3 changeScreenPosi(Camera cam, Vector3 screen_posi) {
        Vector3 posi = cam.WorldToScreenPoint(screen_posi);
        return cam.ScreenToWorldPoint(posi);
    }

    public static int ParseInt(string str, int def = 0) {
        int o;
        return int.TryParse(str, out o) ? o : def;
    }
}