using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "RPG/Unit/Player Data")]
public class PlayerUnitData : BaseUnitData
{
    [Header("Player Visuals")]
    public Sprite unitPortrait;
    
    [Header("FGO Player Stats")]
    public CardType[] deck = new CardType[5];
    public float npGain;
}