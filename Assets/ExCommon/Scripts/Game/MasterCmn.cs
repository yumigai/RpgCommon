using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public class MasterCmn : ParamCmn
{

	public const char PURGE_SEPARATOR = ',';

	public static IEnumerable<T> Load2<T>(string path = "") where T : new() {
		if (string.IsNullOrEmpty(path)) {
			path = CmnConst.Path.TXT + typeof(T).Name;
		}

		string[][] line = UtilToolLib.loadTsv(path);
		if (line == null) {
			var lang = SaveMng.IsJp ? CmnConst.Lang.Jp.ToString() : CmnConst.Lang.En.ToString();
			path = CmnConst.Path.TXT + lang + "/" + typeof(T).Name;
			line = UtilToolLib.loadTsv(path);
		}

		int length = line.Length - (int)LINE.VALUES;

		T[] list = new T[length];

		for (int i = (int)LINE.VALUES, index = 0; i < line.Length; i++, index++) {
			list[index] = new T();
			load<T>(line[(int)LINE.CSV_HEAD], line[i], list[index]);
		}

		return list.Select(s => s).AsEnumerable<T>();
	}

	public static T[] load<T>(string path = "") where T : new() {

		if (string.IsNullOrEmpty(path)) {
			path = CmnConst.Path.TXT + typeof(T).Name;
		}

		string[][] line = UtilToolLib.loadTsv(path);
		if (line == null) {
			var lang = SaveMng.IsJp ? CmnConst.Lang.Jp.ToString() : CmnConst.Lang.En.ToString();
			path = CmnConst.Path.TXT + lang + "/" + typeof(T).Name;
			line = UtilToolLib.loadTsv(path);
		}

		int length = line.Length - (int)LINE.VALUES;

		T[] list = new T[length];

		for (int i = (int)LINE.VALUES, index = 0; i < line.Length; i++, index++) {
			list[index] = new T();
			load<T>(line[(int)LINE.CSV_HEAD], line[i], list[index]);
		}

		return list;

	}

	public static void load<T>(string[] tags, string[] values, T obj) {

		System.Type tp = typeof(T);//this.GetType ();

		FieldInfo[] fl = tp.GetFields
			(BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.Instance | BindingFlags.FlattenHierarchy);

		for (int i = 0; i < fl.Length; i++) {

			int index = Array.IndexOf(tags, fl[i].Name);

			Type ft = fl[i].FieldType;

			object val = null;

			try {
				if (ft.IsEnum) {
					val = Enum.Parse(ft, values[index]);
				} else if (ft.IsArray) {

					System.Type et = ft.GetElementType();

					int[] indexs = UtilToolLib.getAllIndex<string>(tags, fl[i].Name);

					if (et == typeof(System.Int32)) {
						val = (object)setArrayValue<int>(values, indexs);
					} else if (et == typeof(System.Int64)) {
						val = (object)setArrayValue<long>(values, indexs);
					} else if (et == typeof(System.Single)) {
						val = (object)setArrayValue<float>(values, indexs);
					} else if (et == typeof(System.Double)) {
						val = (object)setArrayValue<double>(values, indexs);
					} else if (et == typeof(System.Boolean)) {
						val = (object)setArrayValue<bool>(values, indexs);
					} else if (et == typeof(System.String)) {
						val = (object)setArrayValue<string>(values, indexs);
						foreach (string vs in (string[])val) {
							vs.Replace("\\n", "\n");
						}
					} else if (et.IsEnum) {
						int num;
						string[] purge = values[indexs[0]].Split(PURGE_SEPARATOR);
						if (int.TryParse(purge[0], out num)) {
							//最初の値が数値の場合、それに準拠
							val = (object)setArrayValue<int>(values, indexs);
						} else {
							//最初の値が文字列の場合、それに準拠
							Array arr = Array.CreateInstance(et, purge.Length);
							var enums = (string[])setArrayValue<string>(values, indexs);
							for (int e_index = 0; e_index < enums.Length; e_index++) {
								arr.SetValue((int)Enum.Parse(et, enums[e_index]), e_index);
							}
							val = (object)arr;
						}

					} else {
						val = values[index];
					}

				} else {
					val = System.Convert.ChangeType(values[index], ft);
					if (ft == typeof(string)) {
						val = ((string)val).Replace("\\n", "\n");
					}
				}
			} catch (Exception) {
				if (ft == typeof(System.Int32)
					|| ft == typeof(System.Int64)
					|| ft == typeof(System.Single)
					|| ft == typeof(System.Double)) {
					val = 0;
				} else if (ft == typeof(System.Boolean)) {
					val = false;
				} else {
					val = null;
				}
			}

			if (val != null) {
				fl[i].SetValue(obj, val);
			}

		}
	}

	/// <summary>
	/// 配列パラメータ
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="arr"></param>
	/// <param name="values"></param>
	/// <param name="indexs"></param>
	/// <returns></returns>
	private static T[] setArrayValue<T>(string[] values, int[] indexs) {

		List<T> arr = new List<T>();

		for (int i = 0; i < indexs.Length; i++) {

			string[] purge = values[indexs[i]].Split(PURGE_SEPARATOR);

			for (int j = 0; j < purge.Length; j++) {
				try {
					T ad = (T)Convert.ChangeType(purge[j], typeof(T));
					arr.Add(ad);
				} catch (Exception) { }
			}
		}

		return arr.ToArray();
	}

	//private static object[] setArrayValue(string[] values, int[] indexs, Type ft) {

	//	List<object> arr = new List<object>();

	//	for (int i = 0; i < indexs.Length; i++) {

	//		string val = values[indexs[i]];

	//		string[] purge = val.Split(';');

	//           for (int j = 0; j < purge.Length; j++) {
	//               try {
	//				object ad;
	//				if (ft.IsEnum) {
	//					ad = Enum.Parse(ft, purge[j]);

	//				} else {
	//					ad = Convert.ChangeType(purge[j], ft);
	//				}
	//				arr.Add(ad);

	//			} catch (Exception) { }
	//           }
	//       }

	//	return arr.ToArray();
	//}


}