using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class CmnSaveProc
{

	public class SaveClass
	{
		public void save(int slot = -1) {
			save(this.GetType().Name, slot);
		}
		public void save(string class_name, int slot) {
			string json = JsonUtility.ToJson(this);
			class_name = className(class_name, slot);
			PlayerPrefs.SetString(class_name, json);
		}

		public void reset(int slot = -1) {
			string class_name = className(this.GetType().Name,slot);
			PlayerPrefs.DeleteKey(class_name);
			//PlayerPrefs.DeleteKey(this.GetType().Name);
		}

		//loadは本体そのものなので、CmnSaveProcのstaticメソッドで行う
	}

	public class GameConfig : SaveClass
	{
		public enum LANG
		{
			JP,
			ENG,
			ALL
		}

		public int SelectLang;
		public bool Standby; //言語選択完了状態
		public int[] TutorialSequence;
		public bool TutorialFinish = false;
		public string PlayerName;

		public GameConfig() {
			SelectLang = (int)LANG.JP;
			TutorialSequence = new int[0];
		}
	}

	/// <summary>
	/// 日本語か
	/// </summary>
	public static bool IsJp {
		get {
			return Conf.SelectLang == (int)GameConfig.LANG.JP;
		}
	}


	///// <summary>
	///// キーコンフィグ
	///// </summary>
	//public class KeyConfigTran : SaveClass {

	//    public string[] Buttons = new string[(int)CmnConfig.GamePadButton.All];
	//    public string[] KeyBoards = new string[(int)CmnConfig.GamePadButton.All];

	//    public bool GetKeyDown(CmnConfig.GamePadButton button) {

	//        if (Input.GetKeyDown(Buttons[(int)button])) {
	//            return true;
	//        }
	//        if (Input.GetKeyDown(KeyBoards[(int)button])) {
	//            return true;
	//        }
	//        return false;
	//    }
	//}

	public enum KEY
	{
		TIME,
		//		CONFIG,
		ALL
	}

	public static GameConfig Conf = new GameConfig();
	public static KeyConfigTran Key = new KeyConfigTran();
	public static System.DateTime Timer { private set; get; }

	public static void saveNowTime() {
		System.DateTime now = System.DateTime.Now;
		saveTime(now);
	}

	// 時刻の保存
	public static void saveTime(System.DateTime time) {
		PlayerPrefs.SetString(KEY.TIME.ToString(), time.ToBinary().ToString());
		Timer = time;
	}

	// 時刻の読み出し
	public static System.DateTime loadTime() {
		string datetimeString = PlayerPrefs.GetString(KEY.TIME.ToString());
		if (datetimeString.Length == 0) {
			Timer = System.DateTime.Now;
		} else {
			Timer = System.DateTime.FromBinary(System.Convert.ToInt64(datetimeString));
		}
		return Timer;
	}

	public static void saveConfig() {
		string json = JsonUtility.ToJson(Conf);
		PlayerPrefs.SetString(typeof(GameConfig).Name, json);
	}
	public static void loadConfig() {
		string json = PlayerPrefs.GetString(typeof(GameConfig).Name);
		if (json == null || json == string.Empty) {
			Conf = new GameConfig();
		} else {
			Conf = JsonUtility.FromJson<GameConfig>(json);
		}
	}

	public static T[] addArray<T>(T[] arr, T val) {
		T[] next = new T[arr.Length + 1];
		System.Array.Copy(arr, next, arr.Length);
		next[arr.Length] = val;
		return next;
	}

	public static void loadAll() {
		loadTime();
		loadConfig();
		Key = load<KeyConfigTran>();
	}

	public static void resetSlotAll(int slot = -1 ) {
		PlayerPrefs.DeleteKey(KEY.TIME.ToString());
		Conf.reset();
	}



	//public static float[] load( string key ){
	//	string str = PlayerPrefs.GetString ( key );
	//	if (str == string.Empty) {
	//		return null;
	//	} else {
	//		float[] val = UtilToolLib.purgeFloat (str);
	//		return val;
	//	}
	//}

	//public static void save( string key, float[] val ){
	//	string status = UtilToolLib.convertFloat (val);
	//	PlayerPrefs.SetString (key, status);
	//	PlayerPrefs.Save ();
	//}

	//public static string[] loadStr( string key ){
	//	string str = PlayerPrefs.GetString ( key );
	//	if (str == string.Empty) {
	//		return null;
	//	} else {
	//		string[] val = str.Split(',');
	//		return val;
	//	}
	//}

	//public static void saveStr( string key, string[] val ){
	//	string csv = string.Join(",", val);
	//	PlayerPrefs.SetString (key, csv );
	//	PlayerPrefs.Save ();
	//}

	public static void resetSave() {
		PlayerPrefs.DeleteAll();
	}

	//public static void reset<T>(int slot = -1) {
	//	string class_name = className<T>(slot);
	//	PlayerPrefs.DeleteKey(class_name);
	//}

	/// <summary>
	/// セーブデータロード
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="init">初期化するか</param>
	/// <returns></returns>
	public static T load<T>(bool init = false, int slot = -1) where T : new() {

		string class_name = className<T>(slot);
		return load<T>(init, class_name);
	}

	public static T load<T>(bool init, string class_name) where T : new() {

		string json = PlayerPrefs.GetString(class_name);

		if (json == null || json == string.Empty) {

			if (init) {
				return newInstance<T>();
			} else {
				return new T();
			}
		}
		return JsonUtility.FromJson<T>(json);
	}

	public static T newInstance<T>() where T : new() {

		T itm = new T();

		System.Type tp = typeof(T);

		FieldInfo[] fl = tp.GetFields
			(BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.Instance | BindingFlags.DeclaredOnly);
		for (int i = 0; i < fl.Length; i++) {
			object val = null;
			if (fl[i].FieldType.IsArray) {
				val = newArray(fl[i].FieldType);
			} else if (fl[i].FieldType.IsGenericType) {
				val = newGenelic(fl[i].FieldType);
			}

			if (val != null) {
				fl[i].SetValue(itm, val);
			}
		}


		return itm;
	}

	public static string className<T>(int slot = -1) {
		System.Type class_type = typeof(T);
		return className(class_type.Name, slot);
	}

	public static string className(string class_name, int slot) {
		if (slot >= 0) {
			class_name += "_" + slot.ToString();
		}
		return class_name;
	}

	public static object newArray(System.Type ft) {

		object val = null;

		System.Type et = ft.GetElementType();

		if (et == typeof(System.Int32) || et.IsEnum) {
			val = new int[0];
		} else if (et == typeof(System.Int64)) {
			val = new long[0];
		} else if (et == typeof(System.Single)) {
			val = new float[0];
		} else if (et == typeof(System.Double)) {
			val = new double[0];
		} else if (et == typeof(System.Boolean)) {
			val = new bool[0];
		} else if (et == typeof(System.String)) {
			val = new string[0];
		} else {
			val = null;
		}

		return val;
	}

	public static object newGenelic(System.Type ft) {

		object val = null;

		System.Type et = ft.GetGenericArguments().Single();

		if (et == typeof(System.Int32) || et.IsEnum) {
			val = new List<int>();
		} else if (et == typeof(System.Int64)) {
			val = new List<long>();
		} else if (et == typeof(System.Single)) {
			val = new List<float>();
		} else if (et == typeof(System.Double)) {
			val = new List<double>();
		} else if (et == typeof(System.Boolean)) {
			val = new List<bool>();
		} else if (et == typeof(System.String)) {
			val = new List<string>();
		} else {
			val = null;
		}

		return val;
	}

	/// <summary>
	/// データ破損時修復
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="data"></param>
	public static void repairBrokenData<T>(ref T data) where T : new() {
		if (data == null) {
			data = new T();
		}
	}

	/// <summary>
	/// セーブデータが空か
	/// </summary>
	/// <returns></returns>
	public static bool isEmpty(int slot = -1) {
		//string class_name = className<GameConfig>(slot);
		//string json = PlayerPrefs.GetString(class_name);

		if (slot >= 0) {
			string class_name = className<GameConfig>(slot);
			return !PlayerPrefs.HasKey(class_name);
		} else if (Conf == null || !SaveMng.Status.IsFinishOpening) {
			return true;
		}
		return false;
	}

	//public static bool isAllEmpty() {

	//	PlayerPrefs.HasKey

	//	string class_name = className<GameConfig>(slot);
	//	string json = PlayerPrefs.GetString(class_name);

	//	return false;
	//}

	/// <summary>
	/// 設定してある設定キーの取得
	/// </summary>
	/// <param name="deal"></param>
	/// <returns></returns>
	//public static string getKeyStr(CmnConfig.GamePadButton deal ) {
	//	string key = deal.ToString();
	//	if (Key != null) {
	//		if (CmnBaseProcessMng.IsGamePad) {
	//			key = Key.Buttons[(int)deal];
	//		} else {
	//			key = Key.KeyBoards[(int)deal];
	//		}
	//	}
		
	//	return key;
	//}
}