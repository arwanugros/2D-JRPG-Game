using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    private BattleUnit parentUnit;

    private void Awake()
    {
        parentUnit = GetComponentInParent<BattleUnit>();
    }

    public void TriggerHit()
    {
        if (parentUnit != null) parentUnit.OnAnimationHitTrigger();
    }

    public void TriggerComplete()
    {
        if (parentUnit != null) parentUnit.OnAnimationCompleteTrigger();
    }
}