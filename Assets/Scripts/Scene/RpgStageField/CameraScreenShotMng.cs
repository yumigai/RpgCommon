using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class CameraScreenShotMng : MonoBehaviour {

    public const string SCREEN_NAME = "screen_shot.png";


    public enum POSI{
		BASE,
		RANDOM,
	}

	[SerializeField]
	public Transform[] CameraTargets;

	[SerializeField]
	public bool OffScreen = false;

	public static CameraScreenShotMng Instance;
	//public static Sprite ShotImage;
	public bool ShotSepia;

	private Vector3 BaseLocalPosition;
	private Quaternion BaseLocalQuater;

	public static Texture2D ScreenShot;// { get; private set; }
	private RenderTexture renderTexture = null;
	private string CapturePath;

	void Awake(){
		Instance = this;
		//ShotImages = new List<Sprite> ();
		BaseLocalPosition = this.transform.localPosition;
		BaseLocalQuater = this.transform.localRotation;
		if (OffScreen) {
			ScreenShot = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
			renderTexture = new RenderTexture (Screen.width, Screen.height, 24);
			GetComponent<Camera> ().targetTexture = renderTexture;
		}
	}

	void Start(){
		this.gameObject.SetActive (false);
	}

	//void OnPostRender(){
	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (OffScreen) {
			TakeOffScreen ();
		} else {
			takeScreen ();
		}
		gameObject.SetActive (false);

	}

	public void ready( POSI posi, string file_name ){
		gameObject.SetActive (true);
		CapturePath = Application.persistentDataPath + "/" + file_name;

		switch( posi ){
		case POSI.BASE:
			basePosition();
			break;
		case POSI.RANDOM:
			randomPosition();
			break;
		}
	}

	protected void TakeOffScreen()
	{
//		StartCoroutine (takeOffScreenProcess ());
		ScreenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ScreenShot.Apply();

		//ShotImage = Sprite.Create(ScreenShot, new Rect(0, 0, ScreenShot.width, ScreenShot.height), new Vector2(0.5f, 0.5f));

		//ShotImages.Add (sprite);
		byte [] png = ScreenShot.EncodeToPNG();
		File.WriteAllBytes(CapturePath, png);


	}

//	private IEnumerator takeOffScreenProcess(){
//		yield return new WaitForEndOfFrame();
//		ScreenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
//		ScreenShot.Apply();
//
//		ShotImage = Sprite.Create(ScreenShot, new Rect(0, 0, ScreenShot.width, ScreenShot.height), new Vector2(0.5f, 0.5f));
//		//ShotImages.Add (sprite);
//		byte [] png = ScreenShot.EncodeToPNG();
//		File.WriteAllBytes(CapturePath, png);
//	}

	protected void takeScreen(){
		
		ScreenCapture.CaptureScreenshot (CapturePath);
	}

	void OnDestroy()
	{
		Destroy(renderTexture);
		Destroy(ScreenShot);
	}


	private void basePosition(){
		transform.localPosition = BaseLocalPosition;
		transform.localRotation = BaseLocalQuater;
	}

	private void randomPosition(){
		if (CameraTargets != null && CameraTargets.Length > 0) {
			int index = Random.Range (0, CameraTargets.Length);
			transform.position = CameraTargets [index].position;
			//transform.localRotation = CameraTargets [index].localRotation;
			transform.LookAt(FieldPlayerMng.hero().HitEffectPoint);
			GameObject hit = getRayHitObject( transform );
			if( hit != null && hit != FieldPlayerMng.hero().gameObject && hit.transform.parent.gameObject == FieldPlayerMng.hero().gameObject ){
				basePosition();
			}
		}
	}

	public GameObject getRayHitObject( Transform tra ){
		Ray ray = new Ray(tra.position, tra.forward);

		RaycastHit hit;

		float maxDistance = 100;

		if (Physics.Raycast(ray, out hit, maxDistance)){
			return hit.collider.gameObject;
		}
		return null;
	}

	//	private void shot( string name ){
	//		Application.CaptureScreenshot (name);
	//		gameObject.SetActive (false);
	//	}
	//
	//	public void baseShot( string name ){
	//		ready ();
	//		transform.localPosition = BaseLocalPosition;
	//		transform.localRotation = BaseLocalQuater;
	//		shot (name);
	//	}
	//
	//	public void randomShot( string name ){
	//		ready ();
	//		if (CameraTargets != null && CameraTargets.Length > 0) {
	//			int index = Random.Range (0, CameraTargets.Length);
	//			transform.localPosition = CameraTargets [index].localPosition;
	//			transform.localRotation = CameraTargets [index].localRotation;
	//		}
	//		shot (name);
	//	}

	public static void callBaseShot( string name = SCREEN_NAME)
    {
		Instance.ready(POSI.BASE, name );
	}
	public static void callRandomShot( string name = SCREEN_NAME)
    {
		Instance.ready(POSI.RANDOM, name );
	}
	public static void callBaseShot( string name, bool is_sepia ){
		Instance.ShotSepia = is_sepia;
		callBaseShot (name);
	}
	public static void callRandomShot( string name, bool is_sepia ){
		Instance.ShotSepia = is_sepia;
		callRandomShot (name);
	}

	public static Texture2D loadTexture( string file_name ){
#if UNITY_EDITOR
        string path = file_name;
#else
        string path =  Application.persistentDataPath + "/" + file_name;
#endif
        Texture2D img = ReadTexture (path, Screen.width, Screen.height);
		return img;
	}

    /// <summary>
    /// スクショを撮る
    /// </summary>
    /// <param name="file_name"></param>
    public static void TakeScreenShot(string file_name = SCREEN_NAME) {
        ScreenCapture.CaptureScreenshot(file_name);
    }

    /// <summary>
    /// スクショ撮影＆画像取得
    /// </summary>
    public static IEnumerator TakeAndGetScreenShot( System.Action<Sprite> callback ) {
        ScreenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        ScreenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ScreenShot.Apply();
        byte[] png = ScreenShot.EncodeToPNG();
        Texture2D tex = changeByteToTexture(png, Screen.width, Screen.height);
        callback(changeTextureToSprite(tex));
    }

    ///// <summary>
    ///// スクショ撮影＆画像取得
    ///// </summary>
    //public static Sprite TakeAndGetScreenShot2() {
    //    ScreenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //    ScreenShot.ReadPixels(new Rect(0, 0, ScreenShot.texelSize.x, ScreenShot.texelSize.y), 0, 0);
    //    ScreenShot.Apply();
    //    byte[] png = ScreenShot.EncodeToPNG();
    //    Texture2D tex = changeByteToTexture(png, (int)ScreenShot.texelSize.x, (int)ScreenShot.texelSize.y);
    //    return changeTextureToSprite(tex);

    //}

    /// <summary>
    /// スクショをロード
    /// </summary>
    /// <param name="file_name"></param>
    /// <returns></returns>
    public static Sprite loadSprite( string file_name = SCREEN_NAME)
    {
		Texture2D tex = loadTexture (file_name);
		return changeTextureToSprite(tex);
	}

    /// <summary>
    /// テクスチャ・スプライト変換
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    private static Sprite changeTextureToSprite(Texture2D tex) {
        if (tex == null) {
            return null;
        }
        Sprite sp = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sp;
    }

	private static byte[] ReadPngFile(string path){
		byte[] values = null;
		try{
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			BinaryReader bin = new BinaryReader(fileStream);
			values = bin.ReadBytes((int)bin.BaseStream.Length);

			bin.Close();
		}catch( System.Exception e ){
		}

		return values;
	}

	private static Texture2D ReadTexture(string path, int width, int height){
		byte[] readBinary = ReadPngFile(path);
		return changeByteToTexture(readBinary,width,height);
	}

    private static Texture2D changeByteToTexture(byte[] bin, int width, int height) {
        if (bin == null) {
            return null;
        }
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bin);

        return texture;
    }

}
