using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InputTxtWindowMng : MonoBehaviour {

    [SerializeField]
    public Text TargetTxt;
	
	[System.NonSerialized]
    public System.Action<string> Callback;
    
    public static InputTxtWindowMng Singleton;

    // Use this for initialization
    void Awake () {
    	if(Singleton == null){
			Singleton = this;
		}
	}
	
	public static void Show( GameObject prefab, Transform parent, string init, System.Action<string> action ){
	
		if( Singleton == null ){
			GameObject obj = Instantiate( prefab ) as GameObject;
			obj.transform.parent = parent;
			obj.transform.localScale = prefab.transform.localScale;
            obj.transform.localPosition = prefab.transform.localPosition;
			Singleton = obj.GetComponent<InputTxtWindowMng>();
		}
		Singleton.Callback = action;
		Singleton.TargetTxt.text = init;
	}
	
	public void exec(){
		Callback( TargetTxt.text );
		close();
	}
	
	public void close(){
		Destroy( Singleton.gameObject );
	}
	
	
}
