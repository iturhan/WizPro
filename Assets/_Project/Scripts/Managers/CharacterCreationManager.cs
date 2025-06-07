// _Project/Scripts/Managers/CharacterCreationManager.cs
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour {
    // Bu, t�m ad�mlar boyunca olu�turulan karakter verisini tutar.
    private CharacterData characterInProgress;
    private List<SpellSO> selectedSpells = new List<SpellSO>();

    [Header("UI Panels")]
    public GameObject professionPanel; // Ad�m 1
    public GameObject spellsPanel;     // Ad�m 2
    public GameObject skillsPanel;     // Ad�m 3
    public GameObject finalizationPanel; // Ad�m 4

    void Start() {
        // Ba�lang��ta sadece ilk paneli aktif et
        ShowPanel(professionPanel);
        // Yeni bir karakter olu�turma s�reci ba�lat
        characterInProgress = new CharacterData();

    }

    public CharacterData GetCurrentCharacterData() {
        return characterInProgress;
    }

    // Profesyonel ekran�ndan �a�r�lacak
    public void UpdateCharacterProfession(RaceSO race, Sex sex, ClassSO cClass) {
        // Se�imler de�i�ti�inde yeni bir karakter verisi olu�turuyoruz.
        // �sim ve portre daha sonra eklenecek.
        characterInProgress = new CharacterData("Unnamed", sex, race, cClass);
    }

    // Y�kleme/�lerleme Mant���
    public void GoToSpellsStep() {
        ShowPanel(spellsPanel);
    }

    // Di�er ad�mlara ge�i� metotlar� (GoToSkillsStep, GoToFinalizeStep) buraya eklenecek.

    private void ShowPanel(GameObject panelToShow) {
        professionPanel.SetActive(panelToShow == professionPanel);
        spellsPanel.SetActive(panelToShow == spellsPanel);
        skillsPanel.SetActive(panelToShow == skillsPanel);
        finalizationPanel.SetActive(panelToShow == finalizationPanel);
    }
    public void SetCharacterSpells(List<SpellSO> spells) {
        selectedSpells = new List<SpellSO>(spells);
        Debug.Log($"Character now has {selectedSpells.Count} spells.");
    }

}
