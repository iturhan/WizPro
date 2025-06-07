// Yard�mc� bir s�n�f. Her bir stat sat�r� (STR, INT vb.) i�in bir prefab yap�p bu scripti ekleyebilirsiniz.
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

    // Bu metot, ana kontrolc�den butonlara listener eklemek i�in kullan�lacak
    public void Setup(ProfessionScreenController controller) {

        increaseButton.onClick.RemoveAllListeners(); // �NEML�: Panel tekrar aktif oldu�unda ayn� olay�n tekrar eklenmesini �nler.
        decreaseButton.onClick.RemoveAllListeners(); // �NEML�: Bu sat�rlar� eklemek iyi bir pratiktir.

        // Her bir `Setup` �a�r�s� i�in `statType`'�n o anki de�erini
        // yeni bir yerel de�i�kene kopyal�yoruz.
        StatType capturedStatType = statType;

        // �imdi lambda ifadeleri, her biri kendi kopyalanm�� de�erine sahip olan
        // bu yerel de�i�keni kullanacak.
        increaseButton.onClick.AddListener(() => controller.OnStatIncrease(capturedStatType));
        decreaseButton.onClick.AddListener(() => controller.OnStatDecrease(capturedStatType));
    }
}
