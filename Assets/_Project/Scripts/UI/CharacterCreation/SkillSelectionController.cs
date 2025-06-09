// SkillSelectionController.cs (Son ve En Doðru Sürüm)
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SkillSelectionController : MonoBehaviour {
    [Header("Asset References")]
    public GameAssetHolderSO assetHolder;

    [Header("UI References")]
    public GameObject skillDisplayPrefab;
    public Transform skillsContainer;
    public TextMeshProUGUI pointsToSpendText;
    public GameObject nextButton;
    public GameObject backButton;

    // Dahili Deðiþkenler
    private CharacterCreationManager manager;
    private CharacterData data;
    private int pointsToSpend;
    private List<SkillDisplayUI> skillDisplays = new List<SkillDisplayUI>();
    private Dictionary<SkillSO, int> initialSkillValues = new Dictionary<SkillSO, int>();

    [Header("Category UI")]
    public GameObject categoryHeaderPrefab;

    void Awake() { manager = FindAnyObjectByType<CharacterCreationManager>(); }

    void OnEnable() {
        data = manager.GetCurrentCharacterData();
        if (skillDisplays.Count == 0) { PopulateSkillList(); }

        ApplyInitialClassSkills();
        CalculateAvailablePoints();
        UpdateAllUI();
    }

    // Bu metot, sýnýfýn verdiði baþlangýç yeteneklerini karaktere iþler.
    private void ApplyInitialClassSkills() {
        initialSkillValues.Clear();
        // Önce tüm yeteneklerin baþlangýç deðerini 0 olarak kabul et.
        foreach (SkillSO skill in assetHolder.allSkills) {
            initialSkillValues[skill] = 0;
        }

        // Sonra, sýnýfýn verdiði baþlangýç puanlarýný üzerine yaz.
        if (data.characterClass.startingSkillPoints != null) {
            foreach (var allocation in data.characterClass.startingSkillPoints) {
                initialSkillValues[allocation.skill] = allocation.points;
            }
        }
    }

    // Bu metot, harcanacak toplam puaný hesaplar.
    private void CalculateAvailablePoints() {
        // Þimdilik toplam harcanacak puan konsepti yerine doðrudan atama var.
        // Bu yüzden bu metodu basitleþtiriyoruz.
        // Gelecekte buraya bir "bonus puan" mantýðý eklenebilir.
        pointsToSpend = 0; // Bu örnekte bonus puan yok, sadece baþlangýç atamasý var.
    }

    private void PopulateSkillList() {
        // 1. Tüm yetenekleri kategorilerine göre grupla.
        // LINQ'in 'GroupBy' özelliði burada harikalar yaratýyor.
        var groupedSkills = assetHolder.allSkills
                                       .OrderBy(skill => skill.category.ToString()) // Önce kategori adýna göre sýrala
                                       .GroupBy(skill => skill.category);       // Sonra kategoriye göre grupla

        // 2. Her bir kategori grubu için UI elemanlarý oluþtur.
        foreach (var skillGroup in groupedSkills) {
            // a. Kategori Baþlýðýný Ekle
            GameObject headerGO = Instantiate(categoryHeaderPrefab, skillsContainer);
            TextMeshProUGUI headerText = headerGO.GetComponent<TextMeshProUGUI>();
            headerText.text = skillGroup.Key.ToString(); // Grup anahtarý (kategori adý) baþlýk metnidir.

            // b. Bu kategoriye ait yetenekleri ekle
            foreach (SkillSO skill in skillGroup) {
                // Karakterin yetenek sözlüðüne bu yeteneði ekle (eðer yoksa)
                if (!data.skills.ContainsKey(skill)) {
                    data.skills.Add(skill, GetInitialSkillValue(skill));
                }

                // Yetenek satýrý UI'ýný oluþtur ve kur.
                GameObject skillGO = Instantiate(skillDisplayPrefab, skillsContainer);
                SkillDisplayUI skillUI = skillGO.GetComponent<SkillDisplayUI>();
                skillUI.Setup(skill, this);
                skillDisplays.Add(skillUI);
            }
        }
    }

    // OnSkillIncrease ve Decrease metotlarý bonus puan mantýðýna göre güncellendi.
    public void OnSkillIncrease(SkillSO skill) { /* Þimdilik bonus puan yok */ }
    public void OnSkillDecrease(SkillSO skill) { /* Þimdilik bonus puan yok */ }

    public void OnPointChanged() {
        UpdateAllUI();
    }

    private void UpdateAllUI() {
        if (data == null) return;
        pointsToSpendText.text = $"Bonus Puan: {pointsToSpend}";

        foreach (var display in skillDisplays) {
            // Deðeri doðrudan baþlangýç atamasýndan al.
            int currentSkillValue = initialSkillValues.ContainsKey(display.skill) ? initialSkillValues[display.skill] : 0;
            data.skills[display.skill] = currentSkillValue; // Veriyi güncelle
            display.skillValueText.text = currentSkillValue.ToString();

            // Puan daðýtýmý olmadýðý için butonlarý pasif yap.
            display.increaseButton.interactable = false;
            display.decreaseButton.interactable = false;
        }

        // Puan daðýtýmý olmadýðý için Next butonu hep aktif.
        nextButton.SetActive(true);
    }

    private int GetInitialSkillValue(SkillSO skill) {
        // Bu metot, bir yeteneðin baþlangýç deðerini hesaplar.
        // Gelecekte burayý daha karmaþýk hale getirebilirsiniz.
        // Örneðin: Bir ýrkýn belirli bir yeteneðe bonusu varsa,
        // o bonusu burada döndürebilirsiniz.

        // Þimdilik, tüm yeteneklerin baþlangýç deðeri 0'dýr.
        int value = 0;

        // Örnek gelecek kullaným:
        // if (data.characterRace.racialSkillBonuses.ContainsKey(skill)) {
        //     value += data.characterRace.racialSkillBonuses[skill];
        // }

        return value;
    }

    public void OnNextButtonClicked() { manager.GoToSpellsStep(); }
    public void OnBackButtonClicked() { manager.GoToProfessionStep(); }
}