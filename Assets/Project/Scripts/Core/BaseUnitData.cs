using UnityEngine;

public enum CardType { BUSTER, ARTS, QUICK, ULTIMATE }

public class BaseUnitData : ScriptableObject
{
    public string unitName;
    public int maxHP;
    public int baseAttack;
}