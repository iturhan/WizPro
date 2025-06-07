// Yardýmcý bir sýnýf. Her bir stat satýrý (STR, INT vb.) için bir prefab yapýp bu scripti ekleyebilirsiniz.
// _Project/Scripts/UI/CharacterCreation/StatDisplayUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplayUI : MonoBehaviour {
    public StatType statType;
    public TextMeshProUGUI statNameText;
    public TextMeshProUGUI statValueText;
    public Button increaseButton;
    public Button decreaseButton;

    // Bu metot, ana kontrolcüden butonlara listener eklemek için kullanýlacak
    public void Setup(ProfessionScreenController controller) {

        increaseButton.onClick.RemoveAllListeners(); // ÖNEMLÝ: Panel tekrar aktif olduðunda ayný olayýn tekrar eklenmesini önler.
        decreaseButton.onClick.RemoveAllListeners(); // ÖNEMLÝ: Bu satýrlarý eklemek iyi bir pratiktir.

        // Her bir `Setup` çaðrýsý için `statType`'ýn o anki deðerini
        // yeni bir yerel deðiþkene kopyalýyoruz.
        StatType capturedStatType = statType;

        // Þimdi lambda ifadeleri, her biri kendi kopyalanmýþ deðerine sahip olan
        // bu yerel deðiþkeni kullanacak.
        increaseButton.onClick.AddListener(() => controller.OnStatIncrease(capturedStatType));
        decreaseButton.onClick.AddListener(() => controller.OnStatDecrease(capturedStatType));
    }
}
