using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Mouse olaylar�n� (hover) yakalamak i�in bu k�t�phane gerekli.

// IPointerEnterHandler ve IPointerExitHandler, mouse'un UI eleman�n�n �zerine gelip gelmedi�ini
// alg�layan aray�zlerdir.
public class SpellSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [Header("UI Elements")]
    [Tooltip("B�y�n�n ikonunu g�sterecek Image bile�eni.")]
    public Image iconImage;
    [Tooltip("B�y�n�n ad�n� g�sterecek TextMeshPro bile�eni.")]
    public TextMeshProUGUI spellNameText;

    // Dahili referanslar
    private SpellSO assignedSpell;
    private SpellSelectionController controller;
    private Button button;

    // Bu slot, 'Se�ilmi� B�y�ler' listesinde mi, yoksa 'Se�ilebilir B�y�ler' listesinde mi?
    private bool isSelectedSlot = false;

    // Script ilk y�klendi�inde �al���r
    void Awake() {
        // Bu objeye ba�l� Button bile�enini bul ve referans�n� al.
        button = GetComponent<Button>();
        if (button == null) {
            Debug.LogError("SpellSlotUI bir Button bile�eni gerektirir!", this);
        }
    }

    /// <summary>
    /// Bu slotu belirli bir b�y� verisiyle kurar.
    /// </summary>
    /// <param name="spell">G�r�nt�lenecek b�y�.</param>
    /// <param name="ownerController">Bu slotu y�neten ana kontrolc�.</param>
    /// <param name="isSelected">Bu slot 'se�ilmi�ler' listesinde mi yer al�yor?</param>
    public void Setup(SpellSO spell, SpellSelectionController ownerController, bool isSelected = false) {
        assignedSpell = spell;
        controller = ownerController;
        isSelectedSlot = isSelected;

        // UI elemanlar�n� b�y� verisine g�re g�ncelle
        iconImage.sprite = assignedSpell.icon;
        spellNameText.text = assignedSpell.spellName;

        // Butonun t�klanma olay�n� ayarla
        button.onClick.RemoveAllListeners(); // �nceki t�m dinleyicileri temizleyerek olas� hatalar� �nle.

        if (isSelectedSlot) {
            // E�er bu 'se�ilmi�ler' listesindeki bir slotsa, t�kland���nda b�y�y� L�STEDEN �IKARIR.
            button.onClick.AddListener(() => controller.DeselectSpell(assignedSpell));
        }
        else {
            // E�er bu 'se�ilebilirler' listesindeki bir slotsa, t�kland���nda b�y�y� L�STEYE EKLER.
            button.onClick.AddListener(() => controller.SelectSpell(assignedSpell));
        }
    }

    // Mouse imleci bu UI eleman�n�n �zerine geldi�inde tetiklenir.
    public void OnPointerEnter(PointerEventData eventData) {
        if (controller != null && assignedSpell != null) {
            // Ana kontrolc�ye bu b�y�n�n a��klamas�n� g�stermesi i�in haber ver.
            controller.ShowSpellDescription(assignedSpell);
        }
    }

    // Mouse imleci bu UI eleman�n�n �zerinden ayr�ld���nda tetiklenir.
    public void OnPointerExit(PointerEventData eventData) {
        if (controller != null) {
            // Ana kontrolc�ye a��klama kutusunu temizlemesi i�in haber ver.
            controller.ShowSpellDescription(null);
        }
    }

    /// <summary>
    /// Bu slotu kontrol eden ana kontrolc�ye, hangi b�y�y� temsil etti�ini s�yler.
    /// </summary>
    public SpellSO GetSpell() {
        return assignedSpell;
    }

    /// <summary>
    /// Bu slotun t�klanabilir olup olmad���n� ayarlar.
    /// </summary>
    public void SetInteractable(bool interactable) {
        if (button != null) {
            button.interactable = interactable;
        }
    }
}
