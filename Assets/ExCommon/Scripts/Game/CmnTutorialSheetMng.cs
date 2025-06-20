using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CmnTutorialSheetMng : MonoBehaviour {

	//	public CmnTutorialMng ParentMng{ get; set; }

	public enum END_TO
	{
		KEEP,
		DELETE,
		DELETE_ALL,
		ALL
	}

	public enum START_TO
	{
		WAIT,
		SKIP,
		ALL
	}

	[SerializeField]
	public string MessageText;

	[SerializeField]
	public END_TO EndTo;

	[SerializeField]
	public START_TO StartTo;




}