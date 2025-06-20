using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class ParamCmn {

	public const string COMMENT_PATTERN = "//";
	public const string DEFAULT_2ND_PURGE = ":";

	//public enum TYPES{
		//		NUMBER,
		//		DECIMAL,
		//		STRING,
		//		ARRAY_DECIMAL,
		//		ARRAY_STRING,
		//	}

		public enum LINE{
			CSV_HEAD,
			COMMENT,
			VALUES,
		}

//	protected static void loadCsv( string path, int read_start, System.Action<int> setSize, System.Action<int,string[]> action ){
//
//		string txt = UtilToolLib.readResourceText (path);
//		string[] line = UtilToolLib.purgeStringLine (txt);
//
//		int length = line.Length -read_start;
//
//		setSize (length);
//
//		for( int i = 0; i < length; i++ ){
//			int line_index = i+ read_start;
//			string[] pars = line [line_index].Split (',');
//			action (i, pars);
//		}
//	}





	}
