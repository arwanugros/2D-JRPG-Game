using UnityEngine;

public enum CardType { BUSTER, ARTS, QUICK }

public class BaseUnitData : ScriptableObject
{
    public string unitName;
    public int maxHP;
    public int baseAttack;
}