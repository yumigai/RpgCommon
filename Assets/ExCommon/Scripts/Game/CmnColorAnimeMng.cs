using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CmnColorAnimeMng : MonoBehaviour {

	public enum END_TO
	{
		NON,
		REPEAT,
		REVERSE,
	}

	[SerializeField]
	public float[] AnimeTimes;

	[SerializeField]
	public END_TO EndTo;

	[SerializeField]
	public Color[] Colors;

	[SerializeField]
	public bool IsAlphaOnly = true;

	public bool IsPlay = true;

	private float NowTime;

	private int NowIndex;

	private float[] DeltaTimes;

	private Image[] Imgs;
	private Text[] Texts;
	private Outline[] Outlines;

	void Start(){
		Imgs = GetComponentsInChildren<Image> ();
		Texts = GetComponentsInChildren<Text> ();
		Outlines = GetComponentsInChildren<Outline> ();
		NowTime = 0;
	}

	void FixedUpdate()
	{
		if (IsPlay) {
			NowTime += Time.deltaTime;

			float diff = 1f;
			if (AnimeTimes [NowIndex] > 0f) {
				diff = NowTime / AnimeTimes [NowIndex];
			}

			Color col = Color.Lerp (Colors [NowIndex], Colors [NowIndex + 1], diff);
			float alpha = col.a;
			foreach (Image img in Imgs) {
				if (IsAlphaOnly) {
					col = new Color (img.color.r,img.color.g, img.color.b, alpha );
				}
				img.color = col;
			}
			foreach (Text txt in Texts) {
				if (IsAlphaOnly) {
					col = new Color (txt.color.r,txt.color.g, txt.color.b, alpha );
				}
				txt.color = col;
			}
			foreach (Outline ol in Outlines) {
				if (IsAlphaOnly) {
					col = new Color (ol.effectColor.r,ol.effectColor.g, ol.effectColor.b, alpha );
				}
				ol.effectColor = col;
			}

			if (NowTime >= AnimeTimes [NowIndex]) {
				NowIndex++;
				NowTime = 0f;
			}

			if ( NowIndex >= AnimeTimes.Length - 1)
			{
				switch (EndTo)
				{
				case END_TO.REPEAT:
					NowIndex = 0;
					break;
				case END_TO.REVERSE:
					break;
				case END_TO.NON:
					IsPlay = false;
					break;
				}
			}
		}
	}
}
