using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "RPG/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Unit Profile")]
    public string unitName;
    public Sprite unitSprite;

    [Header("Battle Stats")]
    public int maxHP;
    public int baseAttack;
    
    public UnitType type;
}

public enum UnitType
{
    Player,
    Enemy,
    Boss
}