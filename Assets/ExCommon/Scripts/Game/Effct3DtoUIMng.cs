using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effct3DtoUIMng : MonoBehaviour
{
	[SerializeField,Header("円動作の大きさ")]
	private float MoveCircleR = 30f;

	[SerializeField, Header("円動作の速度")]
	private float MoveCircleSpeed = 5f;

	[SerializeField, Header("円動作の開始角度")]
	private float MoveCircleInitAngle = 270f;

	[SerializeField,Header("目標への移動回数、小さいほど速い")]
	private int MoveTargetLimit = 15;
	

	private float MoveAngle;
	private int MoveTargetCount;
	private Vector3 StartPoint;
	private Vector3 TargetPoint;
	private Vector2 MoveSpeed;

	public System.Action Callback;

	public static Effct3DtoUIMng Singleton;

    private void Awake() {
		Singleton = this;
    }

    private void OnEnable() {
		StartPoint = new Vector3(0f,0f);
		this.transform.position = StartPoint;
		TargetPoint = new Vector3(1300f,1300f);
		MoveAngle = MoveCircleInitAngle;
	}

    private void init() {
		this.transform.position = StartPoint;
		MoveAngle = MoveCircleInitAngle;
	}

    void FixedUpdate() {

        if (MoveAngle < 360f) {

            MoveAngle += MoveCircleSpeed;
            Vector3 posi = getCurclePosition(MoveCircleR, MoveAngle);
            posi.y += MoveCircleR;
            this.transform.position = new Vector3(StartPoint.x + posi.x, StartPoint.y + posi.y, transform.position.z);

        } else {

            if (MoveTargetCount == 0) {
				MoveSpeed = new Vector3((this.transform.position.x - TargetPoint.x) / MoveTargetLimit,
                                           (this.transform.position.y - TargetPoint.y) / MoveTargetLimit, transform.position.z);
            }

            MoveTargetCount++;
            if (MoveTargetCount < MoveTargetLimit) {
                this.transform.position = new Vector3(this.transform.position.x - MoveSpeed.x, this.transform.position.y - MoveSpeed.y, transform.position.z);
            } else {
                Callback?.Invoke();
                this.gameObject.SetActive(false);
            }
        }

    }


	public void setTarget(Vector3 start, Vector3 target, Camera camera1, Camera camera2) {
		var s = UtilToolLib.changeScreenPosi(camera1, start);
		StartPoint = s;
		this.transform.position = s;
		TargetPoint = target;
	}

	public Vector2 getCurclePosition(float r, float angle) {
		float radian = angle / 180f * Mathf.PI;
		float x = r * Mathf.Cos(radian);
		float y = r * Mathf.Sin(radian);
		return new Vector2(-x, y);
	}

}
