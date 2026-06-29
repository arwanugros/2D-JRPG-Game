using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public void Show(int damage, Vector3 startPos, bool isCrit = false)
    {
        gameObject.SetActive(true);
        transform.position = startPos;

        if (isCrit)
        {
            textMesh.text = "CRIT\n" + damage.ToString();
            textMesh.color = Color.yellow;
            textMesh.fontSize = 40f;
        }
        else
        {
            textMesh.text = damage.ToString();
            textMesh.color = Color.white;
            textMesh.fontSize = 36f;
        }

        textMesh.alpha = 1f;

        transform.DOMoveY(startPos.y + 2f, 1f).SetEase(Ease.OutCirc);
        textMesh.DOFade(0f, 1f).OnComplete(() => 
        {
            gameObject.SetActive(false);
        });
    }
}