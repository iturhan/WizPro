// CharacterCreationManager.cs dosyas�n�n g�ncellenmi� hali

using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour {
    private CharacterData characterInProgress;
    private List<SpellSO> selectedSpells = new List<SpellSO>();

    // Bu, profesyon ad�m�n�n en az bir kez tamamlan�p tamamlanmad���n� tutar.
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

    // --- YEN� EKLENEN VEYA G�NCELLENEN METOTLAR ---

    // Ad�m 1'e (Profesyon) git
    public void GoToProfessionStep() {
        ShowPanel(professionPanel);
    }

    // Ad�m 2'ye (B�y�ler) git
    public void GoToSpellsStep() {
        hasProfessionBeenSet = true;
        ShowPanel(spellsPanel);
    }

    // Ad�m 3'e (Yetenekler) git
    public void GoToSkillsStep() {
        ShowPanel(skillsPanel);
    }

    // Ad�m 4'e (Sonu�) git
    public void GoToFinalizationStep() {
        ShowPanel(finalizationPanel);
    }

    // Bu metot zaten harika, oldu�u gibi kal�yor
    private void ShowPanel(GameObject panelToShow) {
        professionPanel.SetActive(panelToShow == professionPanel);
        spellsPanel.SetActive(panelToShow == spellsPanel);
        // Hen�z eklemediyseniz bu sat�rlar� ekleyin
        skillsPanel.SetActive(panelToShow == skillsPanel);
        finalizationPanel.SetActive(panelToShow == finalizationPanel);
    }
}