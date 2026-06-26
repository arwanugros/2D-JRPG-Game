using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "RPG/Unit/Enemy Data")]
public class EnemyUnitData : BaseUnitData
{
    [Header("Enemy Specific Stats")]
    [Range(0f, 100f)]
    public float critChance;
}