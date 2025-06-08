using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Mouse olaylarýný (hover) yakalamak için bu kütüphane gerekli.

// IPointerEnterHandler ve IPointerExitHandler, mouse'un UI elemanýnýn üzerine gelip gelmediðini
// algýlayan arayüzlerdir.
public class SpellSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [Header("UI Elements")]
    [Tooltip("Büyünün ikonunu gösterecek Image bileþeni.")]
    public Image iconImage;
    [Tooltip("Büyünün adýný gösterecek TextMeshPro bileþeni.")]
    public TextMeshProUGUI spellNameText;

    // Dahili referanslar
    private SpellSO assignedSpell;
    private SpellSelectionController controller;
    private Button button;

    // Bu slot, 'Seçilmiþ Büyüler' listesinde mi, yoksa 'Seçilebilir Büyüler' listesinde mi?
    private bool isSelectedSlot = false;

    // Script ilk yüklendiðinde çalýþýr
    void Awake() {
        // Bu objeye baðlý Button bileþenini bul ve referansýný al.
        button = GetComponent<Button>();
        if (button == null) {
            Debug.LogError("SpellSlotUI bir Button bileþeni gerektirir!", this);
        }
    }

    /// <summary>
    /// Bu slotu belirli bir büyü verisiyle kurar.
    /// </summary>
    /// <param name="spell">Görüntülenecek büyü.</param>
    /// <param name="ownerController">Bu slotu yöneten ana kontrolcü.</param>
    /// <param name="isSelected">Bu slot 'seçilmiþler' listesinde mi yer alýyor?</param>
    public void Setup(SpellSO spell, SpellSelectionController ownerController, bool isSelected = false) {
        assignedSpell = spell;
        controller = ownerController;
        isSelectedSlot = isSelected;

        // UI elemanlarýný büyü verisine göre güncelle
        iconImage.sprite = assignedSpell.icon;
        spellNameText.text = assignedSpell.spellName;

        // Butonun týklanma olayýný ayarla
        button.onClick.RemoveAllListeners(); // Önceki tüm dinleyicileri temizleyerek olasý hatalarý önle.

        if (isSelectedSlot) {
            // Eðer bu 'seçilmiþler' listesindeki bir slotsa, týklandýðýnda büyüyü LÝSTEDEN ÇIKARIR.
            button.onClick.AddListener(() => controller.DeselectSpell(assignedSpell));
        }
        else {
            // Eðer bu 'seçilebilirler' listesindeki bir slotsa, týklandýðýnda büyüyü LÝSTEYE EKLER.
            button.onClick.AddListener(() => controller.SelectSpell(assignedSpell));
        }
    }

    // Mouse imleci bu UI elemanýnýn üzerine geldiðinde tetiklenir.
    public void OnPointerEnter(PointerEventData eventData) {
        if (controller != null && assignedSpell != null) {
            // Ana kontrolcüye bu büyünün açýklamasýný göstermesi için haber ver.
            controller.ShowSpellDescription(assignedSpell);
        }
    }

    // Mouse imleci bu UI elemanýnýn üzerinden ayrýldýðýnda tetiklenir.
    public void OnPointerExit(PointerEventData eventData) {
        if (controller != null) {
            // Ana kontrolcüye açýklama kutusunu temizlemesi için haber ver.
            controller.ShowSpellDescription(null);
        }
    }

    /// <summary>
    /// Bu slotu kontrol eden ana kontrolcüye, hangi büyüyü temsil ettiðini söyler.
    /// </summary>
    public SpellSO GetSpell() {
        return assignedSpell;
    }

    /// <summary>
    /// Bu slotun týklanabilir olup olmadýðýný ayarlar.
    /// </summary>
    public void SetInteractable(bool interactable) {
        if (button != null) {
            button.interactable = interactable;
        }
    }
}
