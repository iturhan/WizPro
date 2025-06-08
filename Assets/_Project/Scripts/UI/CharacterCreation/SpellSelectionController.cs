using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SpellSelectionController : MonoBehaviour {
    [Header("Dependencies")]
    [Tooltip("Scene'deki CharacterCreationManager'a atanmal�d�r.")]
    public CharacterCreationManager manager;
    [Tooltip("Proje Asset'lerindeki GameAssetHolderSO'ya atanmal�d�r.")]
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

    // Dahili de�i�kenler
    private CharacterData data;
    private List<SpellSO> selectedSpells = new List<SpellSO>();
    private List<SpellSlotUI> availableSpellSlots = new List<SpellSlotUI>();

    void OnEnable() {
        if (manager == null || assetHolder == null) {
            Debug.LogError("SpellSelectionController: Manager veya AssetHolderSO atanmam��!");
            return;
        }
        data = manager.GetCurrentCharacterData();
        PopulateSpellList();
    }

    private void PopulateSpellList() {
        ClearAllLists();

        if (data == null || data.characterClass == null) { /*...*/ return; }

        // ---- CASUSLARI EKLEYEL�M ----
        Debug.Log("===== B�Y� F�LTRELEME BA�LADI =====");
        Debug.Log("Se�ilen S�n�f: " + data.characterClass.name);

        List<SpellSchoolType> learnableBooks = data.characterClass.learnableSpellSchools;
        Debug.Log("Bu s�n�f�n ��renebilece�i okul say�s�: " + learnableBooks.Count);
        foreach (var book in learnableBooks) { Debug.Log("- " + book); } // Hangi okullar� bildi�ini yazd�r

        Debug.Log("GameAssetHolder'daki toplam b�y� say�s�: " + assetHolder.allSpells.Count);
        // ----------------------------

        if (learnableBooks == null || learnableBooks.Count == 0) { /*...*/ return; }

        int foundSpells = 0; // Bulunan b�y�leri sayal�m
        foreach (SpellSO spell in assetHolder.allSpells) {
            bool canLearn = spell.availableInSpellbooks.Any(book => learnableBooks.Contains(book));

            // ---- SONU�LARI KONTROL EDEL�M ----
            Debug.Log($"B�y�: {spell.spellName} | ��renilebilir mi?: {canLearn}");

            if (canLearn) {
                foundSpells++; // Sayac� art�r
                SpellSlotUI spellSlot = Instantiate(spellSlotPrefab, availableSpellsContainer);
                spellSlot.Setup(spell, this);
                availableSpellSlots.Add(spellSlot);
            }
        }

        Debug.Log($"Filtreleme bitti. Toplam {foundSpells} adet uygun b�y� bulundu.");
        Debug.Log("===================================");

        UpdateUI();
    }

    // Kodun geri kalan� ayn� kalabilir, yukar�daki gibi olmal�...

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
        manager.SetCharacterSpells(selectedSpells); // Se�ilen b�y�leri kaydet
        Debug.Log("Spells confirmed! Proceeding to Skills step.");
        manager.GoToSkillsStep(); // Manager'dan bir sonraki ad�ma gitmesini iste
    }

    public void OnBackButton() {
        // Manager'dan bir �nceki ad�ma gitmesini iste
        manager.GoToProfessionStep();
    }
}