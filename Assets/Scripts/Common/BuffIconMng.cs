using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconMng : MonoBehaviour
{
    [SerializeField]
    private CmnColorAnimeMng ColorAnime;

    [SerializeField]
    private FreeMultiAnimeSimpleMng MoveAnime;

    [SerializeField]
    private Outline[] OutlineColor;

    [SerializeField]
    public Color BuffColor;

    [SerializeField]
    public Color DeBuffColor;

    [SerializeField]
    private Image Icon;

    [SerializeField]
    private Text ValueText;

    [SerializeField]
    private Sprite[] IconImages = new Sprite[(int)BuffTran.TYPE.ALL];

    private void Awake() {
        if (ColorAnime == null) {
            ColorAnime = this.GetComponent<CmnColorAnimeMng>();
        }
        if (MoveAnime == null) {
            MoveAnime = this.GetComponent<FreeMultiAnimeSimpleMng>();
        }
    }

    public void BuffAnime() {
        ChangeColor(BuffColor);
        MoveAnime.Forward();
        Ready();
    }

    public void DeBuffAnime() {
        ChangeColor(DeBuffColor);
        MoveAnime.Reverse();
        Ready();
    }

    private void ChangeColor(Color color) {
        foreach (var outline in OutlineColor) {
            outline.effectColor = color;
        }
    }
    private void Ready() {
        ColorAnime.ResetColor();
        MoveAnime.AnimeStart();
    }

    public void EffectStart(BuffTran.TYPE type, int value) {

        switch (type) {
            case BuffTran.TYPE.HIT:
            case BuffTran.TYPE.SWAY:
            case BuffTran.TYPE.ATK:
            case BuffTran.TYPE.DEF:
            case BuffTran.TYPE.MAG:
            case BuffTran.TYPE.REG: {
                    Icon.sprite = IconImages[(int)type];
                    if (value > 0) {
                        BuffAnime();
                    } else if (value < 0) {
                        DeBuffAnime();
                    }
                }
                this.gameObject.SetActive(true);
                TimeInvokeMng.TimerHide(1f, this.gameObject);
            break;
        }
    }

}
