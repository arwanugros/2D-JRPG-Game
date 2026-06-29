using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHUD : MonoBehaviour
{
    public Slider hpSlider;
    public Slider npSlider;
    public Image portraitImage;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI npText;
    public TextMeshProUGUI nameText;

    private int maxHP;

    public void InitHUD(int maxHp, bool showNP, string unitName, Sprite portrait = null)
    {
        this.maxHP = maxHp;
        
        hpSlider.maxValue = maxHp;
        hpSlider.value = maxHp;
        if (hpText != null) hpText.text = maxHp.ToString();

        if (nameText != null) nameText.text = unitName;

        if (npSlider != null)
        {
            npSlider.maxValue = 100f;
            npSlider.value = 0f;
            npSlider.gameObject.SetActive(showNP);
        }
        
        if (npText != null)
        {
            npText.text = "0%";
            npText.gameObject.SetActive(showNP);
        }

        if (portraitImage != null && portrait != null)
        {
            portraitImage.sprite = portrait;
        }
    }

    public void UpdateHP(int currentHP)
    {
        hpSlider.value = currentHP;
        if (hpText != null) hpText.text = currentHP.ToString();
    }

    public void UpdateNP(float currentNP)
    {
        if (npSlider != null) npSlider.value = currentNP;
        if (npText != null) npText.text = Mathf.RoundToInt(currentNP) + "%";
    }
}