using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FreeImageAnimeMng : MonoBehaviour {

    public enum END_TO
    {
        LOOP,
        STOP,
    }

    [SerializeField]
    public END_TO EndTo;

	[SerializeField]
	public Image AnimeBoard;

	[SerializeField]
	public float Speed;

	[SerializeField]
	public Sprite[] Sprites;

	[SerializeField]
	public bool IsActive = true;

	private int Index;
	private float AnimeTime;

	void Awake(){
		if (AnimeBoard == null) {
			AnimeBoard = GetComponent<Image> ();
		}
	}
	void Start(){
		Index = 0;
		AnimeTime = 0f;
	}
		
	void FixedUpdate(){
		if (Speed > 0f && Sprites.Length > 0 && IsActive ) {
			AnimeTime += Time.fixedDeltaTime;
			if (AnimeTime >= Speed) {
				AnimeTime = 0f;
				Index = Index < Sprites.Length - 1 ? Index + 1 : 0;
                if (EndTo == END_TO.LOOP)
                {
                    AnimeBoard.sprite = Sprites[Index];
                }
                else{
                    this.gameObject.SetActive(false);
                }
			}
		}
	}

}
