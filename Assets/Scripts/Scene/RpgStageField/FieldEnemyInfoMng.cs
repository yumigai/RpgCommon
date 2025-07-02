using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldEnemyInfoMng : MonoBehaviour
{

    [SerializeField]
    public Image Gauge;

    [SerializeField]
    public Text Info;

    [SerializeField]
    public FieldEnemyMng Enemy;

    private RectTransform RecTra;

    private void Awake() {
        RecTra = GetComponent<RectTransform>();
    }

    public void setInit(FieldEnemyMng ene) {
        Enemy = ene;
        Info.text = Enemy.Lv.ToString();
    }

    private void FixedUpdate() {
        if (Enemy == null) {
            Destroy(this.gameObject);
            return;
        }

        Vector2 posi = Camera.main.WorldToScreenPoint(Enemy.HitEffectPoint.position);
        RecTra.position = posi;
    }

}
