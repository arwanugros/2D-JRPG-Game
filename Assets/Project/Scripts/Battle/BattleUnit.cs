using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public int currentHP { get; private set; }
    public float currentNP { get; private set; }
    
    private int maxHP;
    private UnitHUD myHUD;

    public void Setup(int hp, UnitHUD hud)
    {
        maxHP = hp;
        currentHP = maxHP;
        currentNP = 0f;
        myHUD = hud;
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        myHUD.UpdateHP(currentHP);
    }

    public void AddNP(float amount)
    {
        currentNP = Mathf.Clamp(currentNP + amount, 0f, 100f);
        myHUD.UpdateNP(currentNP);
    }
}