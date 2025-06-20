using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class CmnBakeCommand : MonoBehaviour {

#if UNITY_EDITOR

//////EditCommand///////////////////////////////////////

    /// <summary>
    /// 子要素を連番でリネーム
    /// </summary>
	[MenuItem("Custom/make/numbering")]
	public static void numberingChild(){
		EditorWindow window = EditorWindow.GetWindow(typeof(numberingWindow));
		window.Show ();
	}
    /// <summary>
    /// テキスト一括入力
    /// </summary>
	[MenuItem("Custom/make/copyListTexts")]
	public static void copyListTexts(){
		EditorWindow window = EditorWindow.GetWindow(typeof(copyListTextWindow));
		window.Show();
	}
    /// <summary>
    /// 画像一括設定
    /// </summary>
	[MenuItem("Custom/make/copyListImages")]
	public static void copyListImages(){
		EditorWindow window = EditorWindow.GetWindow(typeof(copyListImageWindow));
		window.Show();
	}
    /// <summary>
    /// テキスト一括入力（日本語）
    /// </summary>
	[MenuItem("Custom/make/copyListStaticJp")]
	public static void copyListTextJp(){
		EditorWindow window = EditorWindow.GetWindow(typeof(copyListTextJpWindow));
		window.Show();
	}
    /// <summary>
    /// テキスト一括入力（英語）
    /// </summary>
	[MenuItem("Custom/make/copyListStaticEn")]
	public static void copyListTextEn(){
		EditorWindow window = EditorWindow.GetWindow(typeof(copyListTextEnWindow));
		window.Show();
	}
    /// <summary>
    /// アニメーション開始・終了イベント挿入
    /// </summary>
	[MenuItem("Custom/make/insertAnimeEventInitEnd")]
	public static void insertAnimeEventInitEnd(){
		EditorWindow window = EditorWindow.GetWindow(typeof(AnimeEventInsert));
		window.Show();
	}

	/// <summary>
	/// CSV作成
	/// </summary>
	[MenuItem("Custom/make/makeCsvValue")]
	public static void makeCsvValue() {
		EditorWindow window = EditorWindow.GetWindow(typeof(makeCsvValueWindow),false,"CSV作成");
		window.Show();
	}

	//UtilToolLib.writeText("savelog/" + name + ".txt", data);


	////// Window///////////////////////////////////////
	public class numberingWindow : EditorWindow{

		[SerializeField]
		public Transform ParentBase;

		void OnGUI(){
			
			GUILayout.Space (20f);
				
			ParentBase = EditorGUILayout.ObjectField ("目標", ParentBase, typeof(Transform), true) as Transform;

			if( GUILayout.Button( "実行", GUILayout.Height(50f) ) )
			{
				for (int i = 0; i < ParentBase.childCount; i++) {
					ParentBase.GetChild (i).name = (i + 1).ToString ();
				}
			}
		}
	}
	public class copyListTextWindow : copyListWindow{
		void OnGUI(){
			inputProc ("リストテキスト名", "入力テキスト", callback);
		}
		public void callback ( Transform tra, string val){
			tra.GetComponent<Text> ().text = val;
		}
	}
	public class copyListTextJpWindow : copyListWindow{
		void OnGUI(){
			inputProc ("リストテキスト名(Jp)", "入力テキスト(Jp)", callback);
		}
		public void callback ( Transform tra, string val){
			tra.GetComponent<LanguageStaticTextMng> ().JpText = val;
		}
	}
	public class copyListTextEnWindow : copyListWindow{
		void OnGUI(){
			inputProc ("リストテキスト名(En)", "入力テキスト(En)", callback);
		}
		public void callback ( Transform tra, string val){
			tra.GetComponent<LanguageStaticTextMng> ().EngText = val;
		}
	}

	public class copyListImageWindow : copyListWindow{

		public string ImageDire = "";

		void OnGUI()
		{
			GUILayout.Label("画像フォルダ");
			ImageDire = EditorGUILayout.TextField (ImageDire);
			inputProc ("リスト画像名", "画像パス", callback);
		}
		public void callback ( Transform tra, string val){
			val = ImageDire + val;
			Sprite sp = Resources.Load<Sprite> (val);
			tra.GetComponent<Image> ().sprite = sp;
		}
	}

	public class copyListWindow : EditorWindow{

		public string ListName = "";
		public string ListTargetName = "";
		public string OutputListName = "";
		public string InputText = "";

		public Vector2 Scrollbar = Vector2.zero;

		protected void inputProc( string target, string source, System.Action<Transform,string> callback ){

			GUILayout.Space (20f);

			GUILayout.Label("親リスト名");
			ListName = EditorGUILayout.TextField (ListName);

			GUILayout.Label(target);
			ListTargetName = EditorGUILayout.TextField (ListTargetName);

			GUILayout.Label("名前変更");
			OutputListName = EditorGUILayout.TextField (OutputListName);

			GUILayout.Label(source);

			Scrollbar = EditorGUILayout.BeginScrollView( Scrollbar,GUI.skin.box );
			{
				InputText = EditorGUILayout.TextArea (InputText);
			}
			EditorGUILayout.EndScrollView();

			GUILayout.Space (10f);
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));
			GUILayout.Space (10f);

			if( GUILayout.Button( "実行", GUILayout.Height(50f) ) )
			{
				Transform pare = GameObject.Find (ListName).transform;

				string base_txt = InputText.Replace ("\r\n", "\n");
				string[] lines = base_txt.Split ('\n');

				for (int i = 0; i < pare.childCount; i++) {
					if (i < lines.Length) {
						Transform list_item = pare.GetChild (i).Find (ListTargetName);
						if (list_item) {
							callback (list_item, lines [i]);
						}
						if( !OutputListName.Equals(string.Empty)){
							pare.GetChild (i).name = OutputListName + "_" + i;
						}
					}
				}
			}
		}
	}

	public class AnimeEventInsert : EditorWindow{

		public string ListName = "";
		
		void OnGUI(){

			GUILayout.Space (20f);

			GUILayout.Label("親リスト名");
			ListName = EditorGUILayout.TextField (ListName);

			GUILayout.Space (10f);

			if( GUILayout.Button( "実行", GUILayout.Height(50f) ) )
			{
				Transform pare = GameObject.Find (ListName).transform;

				for (int i = 0; i < pare.childCount; i++) {

					Transform obj = pare.GetChild (i);

					Animator[] animators = obj.GetComponentsInChildren<Animator> ();

					foreach( Animator anim in animators ){

						RuntimeAnimatorController ac = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
						Debug.Log (ac.animationClips.Length);

						foreach (AnimationClip clip in ac.animationClips) {
							AnimationEvent init = new AnimationEvent ();
							init.functionName = "InitAnimeEvent";
							init.time = 0;
							init.stringParameter = clip.name;

							init.messageOptions = SendMessageOptions.DontRequireReceiver;
							AnimationUtility.SetAnimationEvents (clip, new AnimationEvent[]{ init });

							Debug.Log (clip.name);
						}
						anim.SendMessage( "InitAnimeEvent" );

					}
				}
			}

		}
	}

	public class makeCsvValueWindow : EditorWindow
	{
		public string LineNum;
		public string[] MinValues;
		public string[] MaxValues;

		void OnGUI() {

			GUILayout.Label("行数");
			LineNum = EditorGUILayout.TextField(LineNum);

			ScriptableObject target = this;

			SerializedObject so = new SerializedObject(target);

			SerializedProperty min = so.FindProperty("MinValues");
			EditorGUILayout.PropertyField(min, true);

			SerializedProperty max = so.FindProperty("MaxValues");
			EditorGUILayout.PropertyField(max, true);

			SerializedProperty countup = so.FindProperty("CountUp");
			EditorGUILayout.PropertyField(max, true);

			so.ApplyModifiedProperties();

			if (GUILayout.Button("実行", GUILayout.Height(50f))) {

				//if( min.arraySize == max.arraySize && countup.arraySize == max.arraySize) {
				//	int line = int.Parse(LineNum);
				//	for(int h = 0; h < line; h++) {
				//		for (int i = 0; i < max.arraySize; i++) {

				//		}
				//	}
    //            }

				//UtilToolLib.writeText("savelog/" + name + ".txt", data);
			}
		}
	}

#endif
}

