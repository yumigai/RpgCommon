using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPlatesMng : PartyPlatesMng
{

    private const float SIZE_ADJ = 0.5f;

    //[SerializeField]
    //public Vector2[] Positions;

    //[SerializeField]
    //public GameObject[] EnemyPrefabs;

    [SerializeField]
    public ZigzagLayoutGroup Layout;

    public new static EnemyPlatesMng Singleton;

    override public List<UnitStatusTran> Units { get { return SaveMng.Quest.Enemys; } }

    private void Awake() {
        if ((typeof(EnemyPlatesMng) == this.GetType())) {
            Singleton = this;
        }
    }

    public new void initPlate() {
        Members.ForEach(it => Destroy(it.gameObject));
        Members.Clear();

        int size_index = 0;
        int index = 0;

        foreach (var u in Units) {
            size_index = u.getEnemyMast().Size - 1; //sizeは1から開始
            CharaPlateMng plate = CreatePlate(UnitPrefab.gameObject);

            //if (index < EnemyPrefabs.Length && EnemyPrefabs[index] != null) {
            //    plate = CreatePlate(EnemyPrefabs[index]);
            //} else {
            //    plate = CreatePlate(EnemyPrefabs[0]);
            //}
            plate.setUnit(u);
            plate.CharaImg.sprite = u.getImage(GameConst.Path.IMG_ENEMY);
            Vector2 size = plate.CharaImg.sprite.rect.size;
            Vector2 base_size = plate.CharaImg.transform.parent.GetComponent<RectTransform>().sizeDelta;

            if (size.x > size.y) {
                plate.CharaImg.rectTransform.sizeDelta = new Vector2(base_size.x, base_size.y * (size.y / size.x));
            } else {
                plate.CharaImg.rectTransform.sizeDelta = new Vector2(base_size.x * (size.x / size.y), base_size.y);
            }
            plate.CharaImg.rectTransform.localPosition = new Vector3(0, 0);

            if (plate.ShadowImg != null) {
                plate.ShadowImg.sprite = plate.CharaImg.sprite;
                plate.ShadowImg.rectTransform.sizeDelta = new Vector2(plate.CharaImg.rectTransform.sizeDelta.x, plate.ShadowImg.rectTransform.sizeDelta.y);
            }

            plate.Index = index;
            index++;

            plate.gameObject.SetActive(true);
            Members.Add(plate);
        }

        UnitPrefab.gameObject.SetActive(false);

        Layout.Arrange();
    }

}
