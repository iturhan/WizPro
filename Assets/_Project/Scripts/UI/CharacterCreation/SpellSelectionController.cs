using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SpellSelectionController : MonoBehaviour {
    [Header("Dependencies")]
    [Tooltip("Scene'deki CharacterCreationManager'a atanmalýdýr.")]
    public CharacterCreationManager manager;
    [Tooltip("Proje Asset'lerindeki GameAssetHolderSO'ya atanmalýdýr.")]
    public GameAssetHolderSO assetHolder;

    [Header("UI Containers")]
    public Transform availableSpellsContainer;
    public Transform selectedSpellsContainer;

    [Header("UI Prefabs & Elements")]
    public SpellSlotUI spellSlotPrefab;
    public TextMeshProUGUI spellDescriptionText;
    public TextMeshProUGUI infoText;

    [Header("Control Buttons")]
    public Button nextButton;
    public Button backButton;

    [Header("Configuration")]
    public int maxSpellsToSelect = 3;

    // Dahili deðiþkenler
    private CharacterData data;
    private List<SpellSO> selectedSpells = new List<SpellSO>();
    private List<SpellSlotUI> availableSpellSlots = new List<SpellSlotUI>();

    void OnEnable() {
        if (manager == null || assetHolder == null) {
            Debug.LogError("SpellSelectionController: Manager veya AssetHolderSO atanmamýþ!");
            return;
        }
        data = manager.GetCurrentCharacterData();
        PopulateSpellList();
    }

    private void PopulateSpellList() {
        ClearAllLists();

        if (data == null || data.characterClass == null) { /*...*/ return; }

        // ---- CASUSLARI EKLEYELÝM ----
        Debug.Log("===== BÜYÜ FÝLTRELEME BAÞLADI =====");
        Debug.Log("Seçilen Sýnýf: " + data.characterClass.name);

        List<SpellSchoolType> learnableBooks = data.characterClass.learnableSpellSchools;
        Debug.Log("Bu sýnýfýn öðrenebileceði okul sayýsý: " + learnableBooks.Count);
        foreach (var book in learnableBooks) { Debug.Log("- " + book); } // Hangi okullarý bildiðini yazdýr

        Debug.Log("GameAssetHolder'daki toplam büyü sayýsý: " + assetHolder.allSpells.Count);
        // ----------------------------

        if (learnableBooks == null || learnableBooks.Count == 0) { /*...*/ return; }

        int foundSpells = 0; // Bulunan büyüleri sayalým
        foreach (SpellSO spell in assetHolder.allSpells) {
            bool canLearn = spell.availableInSpellbooks.Any(book => learnableBooks.Contains(book));

            // ---- SONUÇLARI KONTROL EDELÝM ----
            Debug.Log($"Büyü: {spell.spellName} | Öðrenilebilir mi?: {canLearn}");

            if (canLearn) {
                foundSpells++; // Sayacý artýr
                SpellSlotUI spellSlot = Instantiate(spellSlotPrefab, availableSpellsContainer);
                spellSlot.Setup(spell, this);
                availableSpellSlots.Add(spellSlot);
            }
        }

        Debug.Log($"Filtreleme bitti. Toplam {foundSpells} adet uygun büyü bulundu.");
        Debug.Log("===================================");

        UpdateUI();
    }

    // Kodun geri kalaný ayný kalabilir, yukarýdaki gibi olmalý...

    public void SelectSpell(SpellSO spell) {
        if (selectedSpells.Count < maxSpellsToSelect && !selectedSpells.Contains(spell)) {
            selectedSpells.Add(spell);
            UpdateUI();
        }
    }

    public void DeselectSpell(SpellSO spell) {
        if (selectedSpells.Contains(spell)) {
            selectedSpells.Remove(spell);
            UpdateUI();
        }
    }

    public void ShowSpellDescription(SpellSO spell) {
        if (spell != null) {
            spellDescriptionText.text = $"<b>{spell.spellName}</b>\n\n{spell.description}";
        }
        else {
            spellDescriptionText.text = "Select a spell to see its description.";
        }
    }

    private void UpdateUI() {
        foreach (Transform child in selectedSpellsContainer) Destroy(child.gameObject);

        foreach (SpellSO spell in selectedSpells) {
            SpellSlotUI spellSlot = Instantiate(spellSlotPrefab, selectedSpellsContainer);
            spellSlot.Setup(spell, this, true);
        }

        foreach (var slot in availableSpellSlots) {
            slot.SetInteractable(!selectedSpells.Contains(slot.GetSpell()));
        }

        infoText.text = $"Spells Selected: {selectedSpells.Count} / {maxSpellsToSelect}";
        nextButton.interactable = (selectedSpells.Count == maxSpellsToSelect);
    }

    private void ClearAllLists() {
        foreach (Transform child in availableSpellsContainer) Destroy(child.gameObject);
        foreach (Transform child in selectedSpellsContainer) Destroy(child.gameObject);

        selectedSpells.Clear();
        availableSpellSlots.Clear();
        ShowSpellDescription(null);
    }

    public void OnNextButton() {
        manager.SetCharacterSpells(selectedSpells); // Seçilen büyüleri kaydet
        Debug.Log("Spells confirmed! Proceeding to Skills step.");
        manager.GoToSkillsStep(); // Manager'dan bir sonraki adýma gitmesini iste
    }

    public void OnBackButton() {
        // Manager'dan bir önceki adýma gitmesini iste
        manager.GoToProfessionStep();
    }
}