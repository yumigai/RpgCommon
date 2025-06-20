using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR_WIN
using UnityEditor;
#endif

public class CmnDebugMng : MonoBehaviour
{

#if UNITY_EDITOR_WIN
	[MenuItem("Custom/debug/deleteSave")]
	public static void deleteSave(){
		CmnSaveProc.resetSave ();
	}

//	[MenuItem("Custom/debug/writeSave")]
//	public static void writeSave(){
//
////		SaveProc.Status.save ();
////
////		string data = PlayerPrefs.GetString( SaveProc.Status.GetType().Name );
////		UtilToolLib.writeText( "savelog/" + SaveProc.Status.ToString() + ".txt", data );
////
////		Debug.Log (data.ToString ());
//	}
//
//	[MenuItem("Custom/debug/readSave")]
//	public static void loasSave(){
//
//		//		SaveProc.GameStatus st =  SaveProc.Status.load<SaveProc.GameStatus> ();
//		//
//		//		st.GetIds = new int[]{ 1, 2 };
//		//
//		//		Debug.Log (st.ToString ());
//
//	}



//	[MenuItem( "Custom/Util/AutoScreenshot" )]
//	private static void AutoScreenShot() {
//		EditorWindow window = EditorWindow.GetWindow(typeof(AutoScreenShotWindow));
//		window.Show();
//	}







//	public class AutoScreenShotWindow : EditorWindow{
//
//		public string DireName = "";
//		public float CaptureTime = 1f;
//		public bool isAct = false;
//		public float NowDelta = 0f;
//
//		void OnGUI(){
//			
//			GUILayout.Space (20f);
//
//			GUILayout.Label("フォルダ");
//			DireName = EditorGUILayout.TextField (DireName);
//
//			GUILayout.Label("時間間隔");
//			CaptureTime = EditorGUILayout.FloatField(CaptureTime);
//
//			GUILayout.Space (10f);
//			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));
//			GUILayout.Space (10f);
//
//			if (isAct) {
//
//				NowDelta += Time.deltaTime;
//
//				if (NowDelta >= CaptureTime) {
//					CaptureScreenshot ();
//					NowDelta = 0f;
//				}
//
//				if (GUILayout.Button ("停止", GUILayout.Height (50f))) {
//					isAct = false;
//				}
//			} else {
//				NowDelta = 0f;
//				if( GUILayout.Button( "開始", GUILayout.Height(50f) ) )	{
//					isAct = true;
//				}
//			}
//		}
//
//	}




#endif
}

