using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CmnUtilCommand : MonoBehaviour {

#if UNITY_EDITOR

	/// <summary>
	/// Captures the screenshot. スクリーンショット
	/// </summary>
	[MenuItem( "Custom/Util/CaptureScreenshot #F12" )]
	private static void CaptureScreenshot() {
		var filename = "capture_" + System.DateTime.Now.ToString( "yyyyMMdd-HHmmss" ) + ".png";
        ScreenCapture.CaptureScreenshot( filename );
		var assembly = typeof( UnityEditor.EditorWindow ).Assembly;
		var type = assembly.GetType( "UnityEditor.GameView" );
		var gameview = EditorWindow.GetWindow( type );
		gameview.Repaint();
		Debug.Log( "ScreenShot: " + filename );
	}
		
	public static string ImportUnitCsv = "Text/UnitStatusTranCsv";

	//[MenuItem("Custom/Util/UnitDataToJson")]
	//private static void UnitDataToJson(){

	//	SaveMng.UnitData = new SaveMng.UnitWrap();
	//	UnitStatusTran[] units = MasterCmn.load<UnitStatusTran>(ImportUnitCsv);
	//	for (int i = 0; i < units.Length; i++) {
	//		units [i].setLevel ();
	//	}
	//	SaveMng.Units = new List<UnitStatusTran>(units);
	//	string json = JsonUtility.ToJson(SaveMng.UnitData);
	//	System.Type tp = typeof(SaveMng.UnitWrap);
	//	string path = "Resources/" + CmnConst.Path.TXT + tp.Name + ".txt";
	//	UtilToolLib.writeText(path, json);

	//}
		
#endif

}
