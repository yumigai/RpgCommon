using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CmnMenuWindow : MonoBehaviour {

#if UNITY_EDITOR

	public class TextMenu : EditorWindow{

		public string ListName = "";
		public string ListTargetName = "";
		public string OutputListName = "";
		public string InputText = "";

		public Vector2 Scrollbar = Vector2.zero;

		protected void inputProc( string[] labels, ref string[] vals, System.Action callback ){

			GUILayout.Space (20f);

			for( int i = 0; i < labels.Length; i++ ){
				GUILayout.Label(labels[i]);
				vals[i] = EditorGUILayout.TextField (vals[i]);
			}

			GUILayout.Space (10f);
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));
			GUILayout.Space (10f);

			if( GUILayout.Button( "実行", GUILayout.Height(50f) ) )
			{
				callback();
			}
		}
	}

#endif
}
