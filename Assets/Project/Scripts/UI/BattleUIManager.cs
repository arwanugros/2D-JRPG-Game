using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    public GameObject battleCanvas;
    
    [Header("Dynamic Card System")]
    public GameObject cardPrefab;
    public Transform cardContainer;

    private List<CardUI> spawnedCards = new List<CardUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        battleCanvas.SetActive(false);
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
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
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
}