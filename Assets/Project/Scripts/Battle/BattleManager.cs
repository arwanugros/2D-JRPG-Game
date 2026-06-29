using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public enum BattleState { START, DRAW_CARDS, PLAYER_SELECT, PLAYER_EXECUTE, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
    public BattleState state;
    // public Flowchart battleFlowchart;
    public BattleUnit playerUnit;
    public List<BattleUnit> activeEnemies; 
    public GameObject playerHUDPrefab, enemyHUDPrefab;
    public Transform playerHUDContainer, enemyHUDContainer;

    private List<CardType> currentHand = new List<CardType>();
    private List<CardType> selectedCards = new List<CardType>();

    public void StartBattle()
    {
        BattleUIManager.Instance.ToggleCanvas(true);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(SoundManager.Instance.battleBGM);
        }
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        if (playerUnit.unitData is PlayerUnitData pData)
        {
            GameObject pUI = Instantiate(playerHUDPrefab, playerHUDContainer);
            UnitHUD pHUD = pUI.GetComponent<UnitHUD>();
            pHUD.InitHUD(pData.maxHP, true, pData.unitName, pData.unitPortrait); 
            playerUnit.Setup(pData.maxHP, pHUD);
        }

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i].unitData is EnemyUnitData eData)
            {
                GameObject eUI = Instantiate(enemyHUDPrefab, enemyHUDContainer);
                UnitHUD eHUD = eUI.GetComponent<UnitHUD>();
                eHUD.InitHUD(eData.maxHP, true, eData.unitName, eData.unitPortrait);
                activeEnemies[i].gameObject.SetActive(true);
                activeEnemies[i].Setup(eData.maxHP, eHUD);
            }
        }
        yield return new WaitForSeconds(1f);
        DrawCards();
    }

    private void DrawCards()
    {
        if (playerUnit.unitData is PlayerUnitData pData)
        {
            state = BattleState.DRAW_CARDS;
            currentHand.Clear();
            selectedCards.Clear();
            List<CardType> deckPool = new List<CardType>(pData.deck);
            for (int i = 0; i < deckPool.Count; i++)
            {
                CardType temp = deckPool[i];
                int randomIndex = Random.Range(i, deckPool.Count);
                deckPool[i] = deckPool[randomIndex];
                deckPool[randomIndex] = temp;
            }
            currentHand = deckPool;
            if (playerUnit.currentNP >= 100f) currentHand.Add(CardType.ULTIMATE);
            BattleUIManager.Instance.ShowHand(currentHand, this);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.cardDraw);
            state = BattleState.PLAYER_SELECT;
        }
    }

    public void OnCardClicked(int index)
    {
        if (state != BattleState.PLAYER_SELECT) return;
        selectedCards.Add(currentHand[index]);
        BattleUIManager.Instance.DisableCardButton(index);
        if (selectedCards.Count >= 3)
        {
            state = BattleState.PLAYER_EXECUTE;
            BattleUIManager.Instance.HideAllCards();
            StartCoroutine(ExecuteAttacks());
        }
    }

    private IEnumerator ExecuteAttacks()
    {
        if (playerUnit.unitData is PlayerUnitData pData)
        {
            foreach (CardType card in selectedCards)
            {
                BattleUnit target = GetAliveEnemy();
                if (target == null) break; 
                int damage = pData.baseAttack;
                float npAmount = pData.npGain;
                if (card == CardType.BUSTER) { damage = Mathf.RoundToInt(damage * 1.5f); npAmount *= 0.5f; }
                else if (card == CardType.ARTS) { damage = Mathf.RoundToInt(damage * 1.0f); npAmount *= 2.0f; }
                else if (card == CardType.QUICK) { damage = Mathf.RoundToInt(damage * 1.2f); npAmount *= 1.25f; }
                else if (card == CardType.ULTIMATE) { damage = Mathf.RoundToInt(damage * 2.25f); npAmount = 0f; playerUnit.AddNP(-100f); }

                if (card == CardType.ULTIMATE)
                    yield return StartCoroutine(playerUnit.PerformUltimateAttack(target.transform, () => target.TakeDamage(damage)));
                else
                    yield return StartCoroutine(playerUnit.PerformMeleeAttack(target.transform, () => { target.TakeDamage(damage); playerUnit.AddNP(npAmount); }));
                
                yield return new WaitForSeconds(0.2f);
                if (CheckWinCondition()) { state = BattleState.WON; break; }
            }
            if (state == BattleState.WON) EndBattle(); else StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i].currentHP > 0 && activeEnemies[i].unitData is EnemyUnitData eData)
            {
                bool isCrit = Random.Range(0f, 100f) < eData.critChance;
                int damage = isCrit ? Mathf.RoundToInt(eData.baseAttack * 1.5f) : eData.baseAttack;

                yield return StartCoroutine(activeEnemies[i].PerformMeleeAttack(playerUnit.transform, () => playerUnit.TakeDamage(damage, isCrit)));
                if (playerUnit.currentHP <= 0) { state = BattleState.LOST; break; }
                yield return new WaitForSeconds(0.2f);
            }
        }
        if (state == BattleState.LOST) EndBattle(); else DrawCards();
    }

    private BattleUnit GetAliveEnemy()
    {
        foreach (BattleUnit enemy in activeEnemies) 
            if (enemy.currentHP > 0 && enemy.unitData is EnemyUnitData) return enemy;
        return null;
    }

    private bool CheckWinCondition()
    {
        foreach (BattleUnit enemy in activeEnemies) 
            if (enemy.currentHP > 0 && enemy.unitData is EnemyUnitData) return false;
        return true;
    }

    private void EndBattle()
    {
        BattleUIManager.Instance.HideAllCards();
        if (state == BattleState.WON)
        {
            BattleUIManager.Instance.ShowEndScreen(true);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopBGM(0.5f);
                SoundManager.Instance.PlaySFX(SoundManager.Instance.winJingle);
            }
            PlayerMovement pm = playerUnit.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = true;
            foreach (BattleUnit enemy in activeEnemies)
            {
                AutoAnimator aa = enemy.GetComponent<AutoAnimator>();
                if (aa != null) aa.enabled = true;
            }
            // if (battleFlowchart != null) battleFlowchart.SendFungusMessage("BattleFinished");
        }
        else if (state == BattleState.LOST)
        {
            BattleUIManager.Instance.ShowEndScreen(false);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopBGM(0.5f);
                SoundManager.Instance.PlaySFX(SoundManager.Instance.loseJingle);
            }
        }
    }
}