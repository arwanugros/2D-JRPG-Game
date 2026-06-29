using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Button cardButton;
    public TextMeshProUGUI cardText;
    public Image cardImage;

    private int cardIndex;
    private BattleManager manager;

    public void InitCard(CardType type, int index, BattleManager bManager)
    {
        cardIndex = index;
        manager = bManager;

        cardText.text = type.ToString();

        if (type == CardType.BUSTER) cardImage.color = Color.red;
        else if (type == CardType.ARTS) cardImage.color = Color.blue;
        else if (type == CardType.QUICK) cardImage.color = Color.green;
        else if (type == CardType.ULTIMATE) cardImage.color = Color.yellow;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnClick);
        cardButton.interactable = true;
    }

    private void OnClick()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.uiClick);
        manager.OnCardClicked(cardIndex);
        cardButton.interactable = false;
    }

    public void SetInteractable(bool state)
    {
        cardButton.interactable = state;
    }
}