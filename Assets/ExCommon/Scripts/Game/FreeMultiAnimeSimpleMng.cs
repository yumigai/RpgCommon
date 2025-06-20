using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMultiAnimeSimpleMng : MonoBehaviour
{
    public enum END_TO
    {
        LOOP,
        REVERSE,
        STOP,
        BREAK,
    }

    [SerializeField]
    public Vector3 RoleSpeed;

    [SerializeField]
    public Vector3 MoveSpeed;

    [SerializeField]
    public Vector3 ScaleSpeed;

    [SerializeField]
    public float AnimeTime = 1f;

    [SerializeField]
    public END_TO EndTo;

    [SerializeField]
    private bool IsStop = false;

    private float ReverseAdj = 1;

    private float RestTime = 0f;

    private Quaternion InitRole;
    private Vector3 InitPosition;
    private Vector3 InitScale;

    private void Awake() {
        InitRole = this.transform.localRotation;
        InitPosition = this.transform.localPosition;
        InitScale = this.transform.localScale;
        RestTime = AnimeTime;
    }

    void FixedUpdate() {

        if (IsStop || RestTime <= 0f) {
            return;
        }

        RestTime -= Time.fixedDeltaTime;

        Transform t = this.transform;

        t.Rotate(RoleSpeed * ReverseAdj);
        t.localPosition += MoveSpeed * ReverseAdj;
        t.localScale += ScaleSpeed * ReverseAdj;

        if(RestTime <= 0f) {
            switch (EndTo) {
                case END_TO.LOOP:
                RestTime = AnimeTime;
                StatusReset();
                break;
                case END_TO.REVERSE:
                RestTime = AnimeTime;
                ReverseAdj *= -1;
                if (ReverseAdj > 0) {
                    StatusReset();
                }
                break;
                case END_TO.STOP:
                IsStop = true;
                break;
                case END_TO.BREAK:
                    Destroy(this.gameObject);
                break;
            }
        }
    }

    private void StatusReset() {
        this.transform.localRotation = InitRole;
        this.transform.localPosition = InitPosition;
        this.transform.localScale = InitScale;
    }

    public void AnimeStart() {
        IsStop = false;
    }

    public void AnimeStop() {
        IsStop = true;
    }
}
