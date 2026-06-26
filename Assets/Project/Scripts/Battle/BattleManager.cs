using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, DRAW_CARDS, PLAYER_SELECT, PLAYER_EXECUTE, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    [Header("Data")]
    public PlayerUnitData playerData;
    public List<EnemyUnitData> enemyDatas; 
    
    [Header("Scene Characters")]
    public BattleUnit playerUnit;
    public List<BattleUnit> activeEnemies; 

    [Header("UI Instantiation")]
    public GameObject playerHUDPrefab;
    public Transform playerHUDContainer;
    public GameObject enemyHUDPrefab;
    public Transform enemyHUDContainer;

    private List<CardType> currentHand = new List<CardType>();
    private List<CardType> selectedCards = new List<CardType>();

    public void StartBattle()
    {
        BattleUIManager.Instance.ToggleCanvas(true);
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        GameObject pUI = Instantiate(playerHUDPrefab, playerHUDContainer);
        UnitHUD pHUD = pUI.GetComponent<UnitHUD>();
        pHUD.InitHUD(playerData.maxHP, true, playerData.unitPortrait); 
        playerUnit.Setup(playerData.maxHP, pHUD);

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (i < enemyDatas.Count)
            {
                GameObject eUI = Instantiate(enemyHUDPrefab, enemyHUDContainer);
                UnitHUD eHUD = eUI.GetComponent<UnitHUD>();
                
                eHUD.InitHUD(enemyDatas[i].maxHP, false);
                
                activeEnemies[i].gameObject.SetActive(true);
                activeEnemies[i].Setup(enemyDatas[i].maxHP, eHUD);
            }
        }

        yield return new WaitForSeconds(1f);
        DrawCards();
    }

    private void DrawCards()
    {
        state = BattleState.DRAW_CARDS;
        currentHand.Clear();
        selectedCards.Clear();

        List<CardType> deckPool = new List<CardType>(playerData.deck);

        for (int i = 0; i < deckPool.Count; i++)
        {
            CardType temp = deckPool[i];
            int randomIndex = Random.Range(i, deckPool.Count);
            deckPool[i] = deckPool[randomIndex];
            deckPool[randomIndex] = temp;
        }

        currentHand = deckPool;
        BattleUIManager.Instance.ShowHand(currentHand, this);
        state = BattleState.PLAYER_SELECT;
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
        foreach (CardType card in selectedCards)
        {
            BattleUnit target = GetAliveEnemy();
            if (target == null) break; 

            int damage = playerData.baseAttack;
            float npAmount = playerData.npGain;

            if (card == CardType.BUSTER) { damage = Mathf.RoundToInt(damage * 1.5f); npAmount *= 0.5f; }
            else if (card == CardType.ARTS) { damage = Mathf.RoundToInt(damage * 1.0f); npAmount *= 2.0f; }
            else if (card == CardType.QUICK) { damage = Mathf.RoundToInt(damage * 0.8f); npAmount *= 1.2f; }

            target.TakeDamage(damage);
            playerUnit.AddNP(npAmount);

            if (CheckWinCondition()) 
            { 
                state = BattleState.WON; 
                break; 
            }
            yield return new WaitForSeconds(0.8f);
        }

        if (state == BattleState.WON) EndBattle();
        else StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i].currentHP > 0)
            {
                playerUnit.TakeDamage(enemyDatas[i].baseAttack);
                
                if (playerUnit.currentHP <= 0) 
                {
                    state = BattleState.LOST;
                    break;
                }
                yield return new WaitForSeconds(0.8f);
            }
        }

        if (state == BattleState.LOST) EndBattle();
        else DrawCards();
    }

    private BattleUnit GetAliveEnemy()
    {
        foreach (BattleUnit enemy in activeEnemies)
        {
            if (enemy.currentHP > 0) return enemy;
        }
        return null;
    }

    private bool CheckWinCondition()
    {
        foreach (BattleUnit enemy in activeEnemies)
        {
            if (enemy.currentHP > 0) return false;
        }
        return true;
    }

    private void EndBattle()
    {
        BattleUIManager.Instance.ToggleCanvas(false);
    }
}