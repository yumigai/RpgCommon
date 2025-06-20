using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomMessageTextMng : MonoBehaviour {

	[SerializeField]
	public Text TargetTxt;

	[SerializeField][Multiline]
	public string[] MessageListsJp;

    [SerializeField]
    [Multiline]
    public string[] MessageListsEn;

    void Awake(){
		setTextComponent ();
	}

	// Use this for initialization
	protected void Start () {

		showMessage ();

	}

	protected void setTextComponent(){
		if (TargetTxt == null) {
			TargetTxt = this.GetComponent<Text> ();
		}
	}

	public void showMessage(){

        string[] message = LanguageStaticTextMng.IsJp ? MessageListsJp : MessageListsEn;

        if (message.Length > 0 ){
			int index = Random.Range (0, message.Length);
			TargetTxt.text = message[index];
		}
	}
}
