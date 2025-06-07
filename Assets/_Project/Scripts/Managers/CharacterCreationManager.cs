// _Project/Scripts/Managers/CharacterCreationManager.cs
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour {
    // Bu, tüm adýmlar boyunca oluþturulan karakter verisini tutar.
    private CharacterData characterInProgress;
    private List<SpellSO> selectedSpells = new List<SpellSO>();

    [Header("UI Panels")]
    public GameObject professionPanel; // Adým 1
    public GameObject spellsPanel;     // Adým 2
    public GameObject skillsPanel;     // Adým 3
    public GameObject finalizationPanel; // Adým 4

    void Start() {
        // Baþlangýçta sadece ilk paneli aktif et
        ShowPanel(professionPanel);
        // Yeni bir karakter oluþturma süreci baþlat
        characterInProgress = new CharacterData();

    }

    public CharacterData GetCurrentCharacterData() {
        return characterInProgress;
    }

    // Profesyonel ekranýndan çaðrýlacak
    public void UpdateCharacterProfession(RaceSO race, Sex sex, ClassSO cClass) {
        // Seçimler deðiþtiðinde yeni bir karakter verisi oluþturuyoruz.
        // Ýsim ve portre daha sonra eklenecek.
        characterInProgress = new CharacterData("Unnamed", sex, race, cClass);
    }

    // Yükleme/Ýlerleme Mantýðý
    public void GoToSpellsStep() {
        ShowPanel(spellsPanel);
    }

    // Diðer adýmlara geçiþ metotlarý (GoToSkillsStep, GoToFinalizeStep) buraya eklenecek.

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
