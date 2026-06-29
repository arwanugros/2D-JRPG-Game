using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    public BaseUnitData unitData;

    [Header("Animation & Visuals")]
    public Animator animator; 
    private Vector3 originalPosition;

    public int currentHP { get; private set; }
    public float currentNP { get; private set; }
    
    private int maxHP;
    private UnitHUD myHUD;

    private Action currentHitAction;
    private bool hasHitFired;
    private bool hasAnimationCompleted;

    public void Setup(int hp, UnitHUD hud)
    {
        maxHP = hp;
        currentHP = maxHP;
        currentNP = 0f;
        myHUD = hud;

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;
        AutoAnimator aa = GetComponent<AutoAnimator>();
        if (aa != null) aa.enabled = false;
        if (animator != null) animator.SetBool("IsRunning", false);
    }

    public void TakeDamage(int damage, bool isCrit = false)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        myHUD.UpdateHP(currentHP);
        Vector3 popupPos = transform.position + new Vector3(0, 1f, 0);
        BattleUIManager.Instance.SpawnDamageText(damage, popupPos, isCrit);

        if (currentHP <= 0)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.dieSound);
            if (animator != null) animator.SetTrigger("Die");
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.hitDamage);
            if (animator != null) animator.SetTrigger("Hurt");
            transform.DOShakePosition(0.2f, 0.2f);
        }
    }

    public void AddNP(float amount)
    {
        currentNP = Mathf.Clamp(currentNP + amount, 0f, 100f);
        myHUD.UpdateNP(currentNP);
    }

    public IEnumerator PerformMeleeAttack(Transform target, Action onHit)
    {
        originalPosition = transform.position;
        currentHitAction = onHit;
        hasHitFired = false;
        hasAnimationCompleted = false;

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.dash);
        if (animator != null) animator.SetTrigger("DashForward");
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - (direction * .5f); 
        transform.DOMove(attackPosition, 0.3f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.3f);

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.meleeAttack);
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitUntil(() => hasHitFired); 
        yield return new WaitUntil(() => hasAnimationCompleted);

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.dash);
        if (animator != null) animator.SetTrigger("DashBackward");
        transform.DOMove(originalPosition, 0.3f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(0.3f);
        if (animator != null) animator.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator PerformUltimateAttack(Transform target, Action onHit)
    {
        originalPosition = transform.position;
        currentHitAction = onHit;
        hasHitFired = false;
        hasAnimationCompleted = false;

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.dash);
        if (animator != null) animator.SetTrigger("DashForward");
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - (direction * 1.5f); 
        transform.DOMove(attackPosition, 0.3f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.3f);

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.ultimateAttack);
        if (animator != null) animator.SetTrigger("Ultimate");
        yield return new WaitUntil(() => hasHitFired);
        yield return new WaitUntil(() => hasAnimationCompleted);

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.dash);
        if (animator != null) animator.SetTrigger("DashBackward");
        transform.DOMove(originalPosition, 0.3f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(0.3f);
        if (animator != null) animator.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
    }

    public void OnAnimationHitTrigger() { hasHitFired = true; currentHitAction?.Invoke(); }
    public void OnAnimationCompleteTrigger() { hasAnimationCompleted = true; }
}