using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EnemyEditorMng : MonoBehaviour
{
    [SerializeField]
    private MultiUseScrollMng Scroll;

    [SerializeField]
    private GameObject EnemyInfoPanel;

    [SerializeField]
    private Image Img;

    [SerializeField]
    private Text Id;

    [SerializeField]
    private Text Name;

    [SerializeField]
    private Text Type;

    [SerializeField]
    private Text Weak;

    [SerializeField]
    private Text Regist;

    [SerializeField]
    private Text Status;

    [SerializeField]
    private Text Skill;

    [SerializeField]
    private GameObject WeakList;

    [SerializeField]
    private GameObject RegistList;

    private int ShowIndex = 0;

    private List<InputField> InputNames = new List<InputField>();

    private void Awake() {
        EnemyMast.load();
        closeInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var ene in EnemyMast.List) {
            var item = Scroll.makeListItem();
            item.Id = ene.Id;
            var input = item.GetComponentInChildren<InputField>();
            InputNames.Add(input);
            input.text = ene.Name;
            item.setIcon( GameConst.Path.IMG_ENEMY + ene.Img);
            item.Callback = pushEnemy;
            setElement(ene.Weak, item.transform.Find(WeakList.name));
            setElement(ene.Regist, item.transform.Find(RegistList.name));
            StartCoroutine(waitImageSize(item.Icon));
        }
    }

    IEnumerator waitImageSize(Image img) {
        yield return new WaitForEndOfFrame();
        UtilToolLib.changeImageSizeFrameFit(img);
    }

    public void pushEnemy(MultiUseListMng list) {
        EnemyMast ene = EnemyMast.List.First(it => it.Id == list.Id);
        enemyInfo(ene);
        EnemyInfoPanel.SetActive(true);
    }

    private void enemyInfo(EnemyMast ene) {

        Sprite sp = Resources.Load<Sprite>(GameConst.Path.IMG_ENEMY + ene.Img);
        Img.sprite = sp;

        Id.text = ene.Id.ToString();
        Name.text = ene.Name;
        Type.text = ene.Job.ToString();
        Weak.text = string.Join(", ", ene.Weak);
        Regist.text = string.Join(", ", ene.Regist);
        Status.text = string.Join(",", ene.Potentials);
        //Skill.text = string.Join(", ", ene.SkillTags);
    }

    public void changeInfo( int add ) {

        ShowIndex = Mathf.Clamp(ShowIndex + add, 0, EnemyMast.List.Count());

        //int now = int.Parse(Id.text);
        //var enemy = EnemyMast.List.First(it => it.Id == now);

        //int index = Mathf.Clamp( EnemyMast.List. it => it.Id == now) + add, 0, EnemyMast.List.Length);
        //var index = EnemyMast.List.Select((x, i) => (x, i));
        enemyInfo(EnemyMast.List[ShowIndex]);
    }

    public void closeInfo() {
        EnemyInfoPanel.SetActive(false);
    }

    private void OnDestroy() {
        Debug.Log("auto save : output/enemyNemes.txt");
        string val = "";

        foreach (var input in InputNames) {
            val += input.text + "\n";
        }

        UtilToolLib.writeText("output/enemyNemes.txt", val);
    }

    private void setElement( GameConst.ELEMENT[] element, Transform parent ) {
        for (int i = 0; i < parent.childCount; i++) {
            parent.GetChild(i).gameObject.SetActive(false);
        }

        foreach (var ele in element) {
            if (ele != GameConst.ELEMENT.Non && ele != GameConst.ELEMENT.All) {
                parent.Find(ele.ToString()).gameObject.SetActive(true);
            }
        }
    }
}
