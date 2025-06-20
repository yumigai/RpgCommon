using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveMng : MonoBehaviour {

	public enum CAMERAS{
		DEFAULT,
		BIRD,
		PLAYER,
		ENEMY,
		ROLL,
		ALL
	}

	public enum MOVE_TYPE
	{
		STOP,
		ROTATE,
		ROTATE_LOOP,
		ROUTE,
		ROUTE_LOOP,

	}

	[SerializeField]
	public Transform LookTarget;

	[SerializeField]
	public Transform ThisCamera;

	[SerializeField]
	public Transform[] MoveTargets;

//	[SerializeField]
//	public float[] MoveTimes;

	[SerializeField]
	public MOVE_TYPE MoveType;

	[SerializeField]
	public float MoveSpeed;

	[SerializeField]
	public bool IsAnimation = false;

	[SerializeField]
	public float RotateRange = 10f;

	[SerializeField]
	public float RoatateHigh = 3f;

	[SerializeField]
	public Transform[] CameraPosis;

	[SerializeField]
	public Transform CameraPosiBase;

	[SerializeField]
	public float StopRotateValue = 180f;

	private float RotateValue = 0f;

//	private Vector3 RouteSpeed;
//
//	private int MoveIndex = 0;

	public static CameraMoveMng Singleton;

	void Awake(){
		if (ThisCamera == null) {
			ThisCamera = this.transform;
		}
		if (CameraPosis.Length == 0) {
			CameraPosis = new Transform[CameraPosiBase.childCount];
			for (int i = 0; i < CameraPosiBase.childCount; i++) {
				CameraPosis [i] = CameraPosiBase.GetChild (i);
			}
		}
		Singleton = this;
	}
		

	void FixedUpdate(){
		switch (MoveType) {
		case MOVE_TYPE.ROTATE:
		case MOVE_TYPE.ROTATE_LOOP:
			positoningToActor (MoveSpeed);
			break;
		case MOVE_TYPE.ROUTE:
		case MOVE_TYPE.ROUTE_LOOP:
//			moveRoute ();
			break;
		}
		if (IsAnimation) {
			ThisCamera.LookAt (LookTarget);
		}
	}

	public void positoningToActor( float speed ){

		if (IsAnimation) {
			Vector3 posi = ThisCamera.position;
			ThisCamera.position = posi;
			float mv_angle = speed * Time.deltaTime;
			RotateValue += mv_angle;
			ThisCamera.RotateAround (LookTarget.position, LookTarget.up, mv_angle);
			if (RotateValue >= StopRotateValue) {
				IsAnimation = false;
			}
		}

	}

	public void setRouteMove(){
		for( int i = 0; i < MoveTargets.Length; i++ ){

				Vector3[] moves = new Vector3[ MoveTargets.Length ];
				for (int j = 0; j < moves.Length; j++) {
					moves [j] = MoveTargets [j].position;
				}

			iTween.MoveTo( ThisCamera.gameObject, iTween.Hash( "path",moves, "time", MoveSpeed, "oncomplete","finishAnime" ));
			IsAnimation = true;
		}
	}

	public void setRotate( MOVE_TYPE type = MOVE_TYPE.ROTATE ){
		MoveType = type;
		RotateValue = 0f;
		Vector3 posi = ThisCamera.position;
		posi.z = RotateRange;
		ThisCamera.position = new Vector3( posi.x, RoatateHigh, RotateRange);
		IsAnimation = true;
	}



	public void finishAnime(){
		IsAnimation = false;
	}


	public void setCamera( CAMERAS cam ){
		ThisCamera.transform.position = CameraPosis [(int)cam].position;
		ThisCamera.transform.localRotation = CameraPosis [(int)cam].localRotation;
		if (cam == CAMERAS.ROLL) {
			setRotate ();
		}
	}

	public void rotateFinish(){
		ThisCamera.RotateAround (LookTarget.position, LookTarget.up, StopRotateValue - RotateValue );
		ThisCamera.LookAt (LookTarget);
		IsAnimation = false;
	}

}
