using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro; // Wajib ditambahkan untuk TextMeshPro

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    public GameObject battleCanvas;
    
    [Header("Dynamic Card System")]
    public GameObject cardPrefab;
    public Transform cardContainer;
    public Transform ultimateCardContainer;

    [Header("Damage Popups")]
    public GameObject damageTextPrefab;
    public Transform damageTextContainer;

    [Header("End Game UI")]
    public CanvasGroup endScreenPanel;
    public TextMeshProUGUI endScreenText;
    
    private List<CardUI> spawnedCards = new List<CardUI>();
    private Queue<DamageText> damagePool = new Queue<DamageText>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        battleCanvas.SetActive(false);
        
        if (endScreenPanel != null) endScreenPanel.gameObject.SetActive(false);
    }

    public void ToggleCanvas(bool isActive)
    {
        battleCanvas.SetActive(isActive);
    }

    public void ShowHand(List<CardType> hand, BattleManager manager)
    {
        foreach (CardUI card in spawnedCards) Destroy(card.gameObject);
        spawnedCards.Clear();

        for (int i = 0; i < hand.Count; i++)
        {
            Transform targetContainer = (hand[i] == CardType.ULTIMATE) ? ultimateCardContainer : cardContainer;

            GameObject cardObj = Instantiate(cardPrefab, targetContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.InitCard(hand[i], i, manager);
            spawnedCards.Add(cardUI);
        }
    }

    public void DisableCardButton(int index)
    {
        if (index < spawnedCards.Count) spawnedCards[index].SetInteractable(false);
    }

    public void HideAllCards()
    {
        foreach (CardUI card in spawnedCards) card.gameObject.SetActive(false);
    }

    public void SpawnDamageText(int damage, Vector3 position, bool isCrit = false)
    {
        DamageText dmgText = null;

        if (damagePool.Count > 0)
        {
            dmgText = damagePool.Dequeue();
            if (dmgText.gameObject.activeInHierarchy) 
            {
                damagePool.Enqueue(dmgText);
                dmgText = null;
            }
        }

        if (dmgText == null)
        {
            GameObject obj = Instantiate(damageTextPrefab, damageTextContainer);
            dmgText = obj.GetComponent<DamageText>();
        }

        dmgText.Show(damage, position, isCrit);
        damagePool.Enqueue(dmgText);
    }

    // FUNGSI END SCREEN YANG BARU
    public void ShowEndScreen(bool isWin)
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.gameObject.SetActive(true);
            
            if (isWin)
            {
                endScreenText.text = "YOU WIN!";
            }
            else
            {
                endScreenText.text = "YOU LOSE!";
            }

            endScreenPanel.alpha = 0f;
            endScreenPanel.DOFade(1f, 1f); 
        }
    }

    public void RestartBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}