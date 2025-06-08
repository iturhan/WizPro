// CharacterCreationManager.cs dosyasýnýn güncellenmiþ hali

using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour {
    private CharacterData characterInProgress;
    private List<SpellSO> selectedSpells = new List<SpellSO>();

    // Bu, profesyon adýmýnýn en az bir kez tamamlanýp tamamlanmadýðýný tutar.
    public bool hasProfessionBeenSet { get; private set; } = false;

    [Header("UI Panels")]
    public GameObject professionPanel;
    public GameObject spellsPanel;
    public GameObject skillsPanel;
    public GameObject finalizationPanel;

    void Start() {
        ShowPanel(professionPanel);
        characterInProgress = new CharacterData();
    }

    public CharacterData GetCurrentCharacterData() {
        return characterInProgress;
    }

    public void UpdateCharacterProfession(RaceSO race, Sex sex, ClassSO cClass) {
        characterInProgress = new CharacterData("Unnamed", sex, race, cClass);
    }

    public void SetCharacterSpells(List<SpellSO> spells) {
        selectedSpells = new List<SpellSO>(spells);
        Debug.Log($"Character now has {selectedSpells.Count} spells.");
    }

    // --- YENÝ EKLENEN VEYA GÜNCELLENEN METOTLAR ---

    // Adým 1'e (Profesyon) git
    public void GoToProfessionStep() {
        ShowPanel(professionPanel);
    }

    // Adým 2'ye (Büyüler) git
    public void GoToSpellsStep() {
        hasProfessionBeenSet = true;
        ShowPanel(spellsPanel);
    }

    // Adým 3'e (Yetenekler) git
    public void GoToSkillsStep() {
        ShowPanel(skillsPanel);
    }

    // Adým 4'e (Sonuç) git
    public void GoToFinalizationStep() {
        ShowPanel(finalizationPanel);
    }

    // Bu metot zaten harika, olduðu gibi kalýyor
    private void ShowPanel(GameObject panelToShow) {
        professionPanel.SetActive(panelToShow == professionPanel);
        spellsPanel.SetActive(panelToShow == spellsPanel);
        // Henüz eklemediyseniz bu satýrlarý ekleyin
        skillsPanel.SetActive(panelToShow == skillsPanel);
        finalizationPanel.SetActive(panelToShow == finalizationPanel);
    }
}