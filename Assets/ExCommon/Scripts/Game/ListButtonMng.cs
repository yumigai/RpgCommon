using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 汎用ボタン
/// </summary>
public class ListButtonMng : ListItemMng
{

	[SerializeField]
	public Text BaseText;

	public object Arg {
		get; set;
	}

	public System.Action<int, object> Callback {
		get; set;
	}

	void Awake() {
		if (BaseText == null) {
			BaseText = GetComponentInChildren<Text>();
		}
	}

	public void pushButton() {
		Callback(Index, Arg);
	}

}