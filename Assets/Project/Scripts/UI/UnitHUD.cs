using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour
{
    public Image portraitImage;
    public Slider hpBar;
    public Slider npBar;

    public void InitHUD(int maxHP, bool isPlayer, Sprite portrait = null)
    {
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        if (isPlayer)
        {
            if (portraitImage != null && portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            
            if (npBar != null)
            {
                npBar.gameObject.SetActive(true);
                npBar.maxValue = 100f;
                npBar.value = 0f;
            }
        }
        else
        {
            if (portraitImage != null) portraitImage.gameObject.SetActive(false);
            if (npBar != null) npBar.gameObject.SetActive(false);
        }
    }

    public void UpdateHP(int currentHP)
    {
        hpBar.value = currentHP;
    }

    public void UpdateNP(float currentNP)
    {
        if (npBar != null) npBar.value = currentNP;
    }
}