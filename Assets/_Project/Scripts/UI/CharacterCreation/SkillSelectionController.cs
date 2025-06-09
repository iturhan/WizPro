// SkillSelectionController.cs (Son ve En Do�ru S�r�m)
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

    // Dahili De�i�kenler
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

    // Bu metot, s�n�f�n verdi�i ba�lang�� yeteneklerini karaktere i�ler.
    private void ApplyInitialClassSkills() {
        initialSkillValues.Clear();
        // �nce t�m yeteneklerin ba�lang�� de�erini 0 olarak kabul et.
        foreach (SkillSO skill in assetHolder.allSkills) {
            initialSkillValues[skill] = 0;
        }

        // Sonra, s�n�f�n verdi�i ba�lang�� puanlar�n� �zerine yaz.
        if (data.characterClass.startingSkillPoints != null) {
            foreach (var allocation in data.characterClass.startingSkillPoints) {
                initialSkillValues[allocation.skill] = allocation.points;
            }
        }
    }

    // Bu metot, harcanacak toplam puan� hesaplar.
    private void CalculateAvailablePoints() {
        // �imdilik toplam harcanacak puan konsepti yerine do�rudan atama var.
        // Bu y�zden bu metodu basitle�tiriyoruz.
        // Gelecekte buraya bir "bonus puan" mant��� eklenebilir.
        pointsToSpend = 0; // Bu �rnekte bonus puan yok, sadece ba�lang�� atamas� var.
    }

    private void PopulateSkillList() {
        // 1. T�m yetenekleri kategorilerine g�re grupla.
        // LINQ'in 'GroupBy' �zelli�i burada harikalar yarat�yor.
        var groupedSkills = assetHolder.allSkills
                                       .OrderBy(skill => skill.category.ToString()) // �nce kategori ad�na g�re s�rala
                                       .GroupBy(skill => skill.category);       // Sonra kategoriye g�re grupla

        // 2. Her bir kategori grubu i�in UI elemanlar� olu�tur.
        foreach (var skillGroup in groupedSkills) {
            // a. Kategori Ba�l���n� Ekle
            GameObject headerGO = Instantiate(categoryHeaderPrefab, skillsContainer);
            TextMeshProUGUI headerText = headerGO.GetComponent<TextMeshProUGUI>();
            headerText.text = skillGroup.Key.ToString(); // Grup anahtar� (kategori ad�) ba�l�k metnidir.

            // b. Bu kategoriye ait yetenekleri ekle
            foreach (SkillSO skill in skillGroup) {
                // Karakterin yetenek s�zl���ne bu yetene�i ekle (e�er yoksa)
                if (!data.skills.ContainsKey(skill)) {
                    data.skills.Add(skill, GetInitialSkillValue(skill));
                }

                // Yetenek sat�r� UI'�n� olu�tur ve kur.
                GameObject skillGO = Instantiate(skillDisplayPrefab, skillsContainer);
                SkillDisplayUI skillUI = skillGO.GetComponent<SkillDisplayUI>();
                skillUI.Setup(skill, this);
                skillDisplays.Add(skillUI);
            }
        }
    }

    // OnSkillIncrease ve Decrease metotlar� bonus puan mant���na g�re g�ncellendi.
    public void OnSkillIncrease(SkillSO skill) { /* �imdilik bonus puan yok */ }
    public void OnSkillDecrease(SkillSO skill) { /* �imdilik bonus puan yok */ }

    public void OnPointChanged() {
        UpdateAllUI();
    }

    private void UpdateAllUI() {
        if (data == null) return;
        pointsToSpendText.text = $"Bonus Puan: {pointsToSpend}";

        foreach (var display in skillDisplays) {
            // De�eri do�rudan ba�lang�� atamas�ndan al.
            int currentSkillValue = initialSkillValues.ContainsKey(display.skill) ? initialSkillValues[display.skill] : 0;
            data.skills[display.skill] = currentSkillValue; // Veriyi g�ncelle
            display.skillValueText.text = currentSkillValue.ToString();

            // Puan da��t�m� olmad��� i�in butonlar� pasif yap.
            display.increaseButton.interactable = false;
            display.decreaseButton.interactable = false;
        }

        // Puan da��t�m� olmad��� i�in Next butonu hep aktif.
        nextButton.SetActive(true);
    }

    private int GetInitialSkillValue(SkillSO skill) {
        // Bu metot, bir yetene�in ba�lang�� de�erini hesaplar.
        // Gelecekte buray� daha karma��k hale getirebilirsiniz.
        // �rne�in: Bir �rk�n belirli bir yetene�e bonusu varsa,
        // o bonusu burada d�nd�rebilirsiniz.

        // �imdilik, t�m yeteneklerin ba�lang�� de�eri 0'd�r.
        int value = 0;

        // �rnek gelecek kullan�m:
        // if (data.characterRace.racialSkillBonuses.ContainsKey(skill)) {
        //     value += data.characterRace.racialSkillBonuses[skill];
        // }

        return value;
    }

    public void OnNextButtonClicked() { manager.GoToSpellsStep(); }
    public void OnBackButtonClicked() { manager.GoToProfessionStep(); }
}