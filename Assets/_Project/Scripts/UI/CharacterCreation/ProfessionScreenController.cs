using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ProfessionScreenController : MonoBehaviour {
    [Header("Asset References")]
    public GameAssetHolderSO assetHolder;

    [Header("UI Elements")]
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown genderDropdown;
    public TMP_Dropdown classDropdown;
    public TextMeshProUGUI bonusPointsText;
    public List<StatDisplayUI> statDisplays;
    public GameObject nextButton;

    [Header("UI Style")]
    public Color defaultStatColor = Color.white;
    public Color modifiedStatColor = new Color(0.5f, 0.7f, 1f); // Mavi tonu
    public Color requirementNotMetColor = Color.red;

    // Dahili De�i�kenler
    private CharacterCreationManager manager;
    private CharacterData data;
    private int bonusPoints;
    private Dictionary<StatType, int> pointsAddedToStat = new Dictionary<StatType, int>();
    private bool isFirstTimeSetupDone = false;

    void Awake() {
        manager = FindAnyObjectByType<CharacterCreationManager>();
    }

    // Panel her aktif oldu�unda �al���r
    void OnEnable() {
        // Her a��l��ta g�ncel karakter verisini al
        data = manager.GetCurrentCharacterData();

        // UI elemanlar� (dropdown doldurma, buton listener'lar�) sadece bir kez kurulmal�.
        if (!isFirstTimeSetupDone) {
            SetupUIForFirstTime();
            isFirstTimeSetupDone = true;
        }

        // Dropdown'lar�, karakterin mevcut verisine g�re do�ru de�ere ayarla.
        // Bu, "Geri" butonuna bas�ld���nda se�imlerin korunmas�n� sa�lar.
        UpdateDropdownsFromData();

        // Aray�z� en g�ncel haliyle g�ster.
        UpdateAllUI();
    }

    // Sadece bir kez �al��acak kurulum metodu.
    private void SetupUIForFirstTime() {
        PopulateDropdownsWithPlaceholders();

        foreach (var display in statDisplays) {
            if (display != null) display.Setup(this);
        }

        raceDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
        genderDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
        classDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
    }

    // Bir dropdown de�eri de�i�ti�inde...
    public void OnDropdownValueChanged() {
        if (raceDropdown.value == 0 || classDropdown.value == 0 || genderDropdown.value == 0) {
            nextButton.SetActive(false);
            return;
        }

        RaceSO selectedRace = assetHolder.allRaces[raceDropdown.value - 1];
        ClassSO selectedClass = assetHolder.allClasses[classDropdown.value - 1];
        Sex selectedGender = (Sex)(genderDropdown.value - 1);

        manager.UpdateCharacterProfession(selectedRace, selectedGender, selectedClass);
        data = manager.GetCurrentCharacterData();

        // E�er bu ilk ge�erli se�imse, bonus puanlar� ver.
        if (!manager.hasProfessionBeenSet) {
            ResetAndRerollBonuses();
        }
        else { // De�ilse (sadece �rk/s�n�f de�i�tiyse), bonuslar� koru.
            RecalculateStatsAndKeepBonuses();
        }
    }

    // Bonuslar� koruyarak statlar� yeniden hesaplar.
    private void RecalculateStatsAndKeepBonuses() {
        data.RecalculateBaseStats();
        foreach (var entry in pointsAddedToStat) {
            data.SetBaseStat(entry.Key, data.GetBaseStat(entry.Key) + entry.Value);
        }
        UpdateAllUI();
    }

    // �lk kurulumda �al���r, hem statlar� hem bonuslar� s�f�rlar.
    private void ResetAndRerollBonuses() {
        data.RecalculateBaseStats();
        pointsAddedToStat.Clear();
        bonusPoints = Random.Range(6, 16);
        UpdateAllUI();
    }

    // Reroll butonuna bas�ld���nda kullan�l�r.
    public void OnRerollButtonClicked() {
        // G�venlik kontrol� hala �nemli!
        if (data == null || data.race == null) {
            Debug.LogWarning("Cannot reroll. Please select a race, class, and gender first.");
            return;
        }

        // Art�k do�ru "hard reset" metodunu �a��r�yoruz.
        ResetAndRerollBonuses();
    }

    public void OnStatIncrease(StatType stat) {
        if (bonusPoints > 0 && data.GetBaseStat(stat) < 18) {
            bonusPoints--;
            data.SetBaseStat(stat, data.GetBaseStat(stat) + 1);
            if (!pointsAddedToStat.ContainsKey(stat)) pointsAddedToStat[stat] = 0;
            pointsAddedToStat[stat]++;
            UpdateAllUI();
        }
    }

    public void OnStatDecrease(StatType stat) {
        if (pointsAddedToStat.ContainsKey(stat) && pointsAddedToStat[stat] > 0) {
            bonusPoints++;
            data.SetBaseStat(stat, data.GetBaseStat(stat) - 1);
            pointsAddedToStat[stat]--;
            if (pointsAddedToStat[stat] == 0) pointsAddedToStat.Remove(stat);
            UpdateAllUI();
        }
    }

    public void OnNextButtonClicked() { manager.GoToSpellsStep(); }

    private void UpdateAllUI() {
        if (data == null || data.race == null) {
            nextButton.SetActive(false);
            return;
        }

        bonusPointsText.text = bonusPoints.ToString();
        bool allRequirementsMet = true;

        foreach (var display in statDisplays) {
            int currentStatValue = data.GetBaseStat(display.statType);
            display.statValueText.text = currentStatValue.ToString();

            // 1. Gereksinimi her zamanki gibi bul.
            StatRequirement requirement = data.characterClass.statRequirements.FirstOrDefault(r => r.stat == display.statType);

            // 2. YEN� ve DO�RU MANTIK: Ger�ek bir gereksinim bulunup bulunmad���n� kontrol et.
            // Varsay�lan/bo� bir struct'�n 'minValue' de�eri 0 olur. Ger�ek bir gereksinimin
            // de�eri her zaman 0'dan b�y�k olmal�d�r. Bu en g�venli kontrold�r.
            bool requirementExists = requirement.minValue > 0;

            // ESK� HATALI KOD: if (requirement.stat != StatType.Strength && ...)

            // 3. Yeni ve do�ru ko�ulu kullan.
            if (requirementExists && currentStatValue < requirement.minValue) {
                display.statValueText.color = requirementNotMetColor;
                allRequirementsMet = false;
            }
            else if (pointsAddedToStat.ContainsKey(display.statType) && pointsAddedToStat[display.statType] > 0) {
                display.statValueText.color = modifiedStatColor;
            }
            else {
                display.statValueText.color = defaultStatColor;
            }

            display.increaseButton.interactable = bonusPoints > 0 && currentStatValue < 18;
            display.decreaseButton.interactable = pointsAddedToStat.ContainsKey(display.statType) && pointsAddedToStat[display.statType] > 0;
        }

        bool areAllDropdownsSelected = raceDropdown.value > 0 && classDropdown.value > 0 && genderDropdown.value > 0;
        nextButton.SetActive(areAllDropdownsSelected && bonusPoints == 0 && allRequirementsMet);
    }

    // --- YARDIMCI METOTLAR ---
    private void PopulateDropdownsWithPlaceholders() {
        raceDropdown.ClearOptions();
        classDropdown.ClearOptions();
        genderDropdown.ClearOptions();

        raceDropdown.options.Add(new TMP_Dropdown.OptionData("--- Irk Se�iniz ---"));
        classDropdown.options.Add(new TMP_Dropdown.OptionData("--- S�n�f Se�iniz ---"));
        genderDropdown.options.Add(new TMP_Dropdown.OptionData("--- Cinsiyet Se�iniz ---"));

        raceDropdown.AddOptions(assetHolder.allRaces.Select(r => r.raceName).ToList());
        classDropdown.AddOptions(assetHolder.allClasses.Select(c => c.className).ToList());
        genderDropdown.AddOptions(System.Enum.GetNames(typeof(Sex)).ToList());
    }

    private void UpdateDropdownsFromData() {
        if (data == null || data.race == null || data.characterClass == null) {
            raceDropdown.value = 0; classDropdown.value = 0; genderDropdown.value = 0;
            return;
        }

        raceDropdown.value = assetHolder.allRaces.IndexOf(data.race) + 1;
        classDropdown.value = assetHolder.allClasses.IndexOf(data.characterClass) + 1;
        genderDropdown.value = (int)data.sex + 1;

        raceDropdown.RefreshShownValue();
        classDropdown.RefreshShownValue();
        genderDropdown.RefreshShownValue();
    }
}