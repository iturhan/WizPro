// _Project/Scripts/UI/CharacterCreation/ProfessionScreenController.cs
using UnityEngine;
using TMPro; // TextMeshPro i�in
using System.Collections.Generic;
using System.Linq;

public class ProfessionScreenController : MonoBehaviour {
    [Header("Asset References")]
    [Tooltip("T�m Irk ve S�n�f SO'lar�n� tutan asset")]
    public GameAssetHolderSO assetHolder;

    [Header("Manager Reference")]
    private CharacterCreationManager manager;

    [Header("UI Elements")]
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown genderDropdown;
    public TMP_Dropdown classDropdown;
    public TextMeshProUGUI bonusPointsText;

    // Stat text'leri i�in bir dictionary veya public liste kullanabilirsiniz
    public List<StatDisplayUI> statDisplays; // Her bir stat i�in UI elemanlar�n� tutan �zel bir component

    [Header("UI Style")] // YEN� EKLEND�
    public Color defaultStatColor = Color.white; // De�i�tirilmemi� statlar i�in varsay�lan renk
    public Color modifiedStatColor = new Color(0.5f, 0.7f, 1f); // De�i�tirilmi� statlar i�in mavi tonu bir renk
    public Color requirementNotMetColor = Color.red; // Gereksinim kar��lanmad���nda kullan�lacak renk.

    [Header("Control Buttons")]
    public GameObject nextButton; // Bonus puanlar bitmeden aktif olmamal�

    // Dahili De�i�kenler
    private CharacterData data;
    private int bonusPoints;
    private Dictionary<StatType, int> pointsAddedToStat = new Dictionary<StatType, int>();
    

    void Awake() {
        manager = FindAnyObjectByType<CharacterCreationManager>();
    }

    void OnEnable() {
        PopulateDropdowns();
        // ---- YEN� EKLENEN KISIM ----
        // Stat sat�rlar�ndaki butonlara hangi fonksiyonu �a��racaklar�n� s�yl�yoruz.
        // Bu, her sat�r�n Setup() metodunu �a��rarak yap�l�r.
        foreach (var display in statDisplays) {
            if (display != null) {
                display.Setup(this); // 'this' diyerek kendisini (ProfessionScreenController'�) referans olarak verir.
            }
            else {
                Debug.LogError("ProfessionScreenController'daki 'statDisplays' listesinde bo� (null) bir eleman var! Inspector'u kontrol edin.");
            }
        }
        // ---- YEN� KISMIN SONU ----
        SetupInitialState();
    }

    private void PopulateDropdowns() {
        // Race
        raceDropdown.ClearOptions();
        raceDropdown.AddOptions(assetHolder.allRaces.Select(r => r.raceName).ToList());

        // Gender (Cinsiyet)
        genderDropdown.ClearOptions();
        genderDropdown.AddOptions(System.Enum.GetNames(typeof(Sex)).Where(s => s != "Unspecified").ToList());

        // Class
        classDropdown.ClearOptions();
        classDropdown.AddOptions(assetHolder.allClasses.Select(c => c.className).ToList());

        // Dropdown'lar de�i�ti�inde �a�r�lacak metotlar� ata
        raceDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        genderDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        classDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
    }

    private void SetupInitialState() {
        // Ba�lang��ta dropdown'lardaki ilk se�eneklerle karakteri olu�tur
        OnProfessionChanged();
    }

    // Irk, Cinsiyet veya S�n�f de�i�ti�inde her �eyi s�f�rlar
    public void OnProfessionChanged() {
        pointsAddedToStat.Clear();

        RaceSO selectedRace = assetHolder.allRaces[raceDropdown.value];
        Sex selectedGender = (Sex)System.Enum.Parse(typeof(Sex), genderDropdown.options[genderDropdown.value].text);
        ClassSO selectedClass = assetHolder.allClasses[classDropdown.value];

        manager.UpdateCharacterProfession(selectedRace, selectedGender, selectedClass);
        data = manager.GetCurrentCharacterData();

        RerollAttributes(); // Otomatik olarak yeni bonus puanlar ver
    }

    // Zar atma ve puanlar� s�f�rlama
    public void RerollAttributes() {

        // 1. �NCEL�KLE, DA�ITILMI� BONUS PUANLARI GER� AL
        // `pointsAddedToStat` dictionary'sinde kay�tl� olan her bir stat i�in:
        foreach (var stat in pointsAddedToStat.Keys.ToList()) {
            // Bu stata ka� puan eklenmi�ti?
            int pointsToRemove = pointsAddedToStat[stat];
            // Karakterin ana verisinden bu puanlar� ��kararak stat'� eski haline getir.
            data.SetBaseStat(stat, data.GetBaseStat(stat) - pointsToRemove);
        }

        pointsAddedToStat.Clear();

        // Yeni bonus puanlar� ver (�rnek: 6-15 aras�)
        bonusPoints = Random.Range(6, 16);

        UpdateAllUI();
    }

    // Stat art�rma butonuna bas�ld���nda
    public void OnStatIncrease(StatType stat) {
        if (bonusPoints > 0 && data.GetBaseStat(stat) < 18) {
            bonusPoints--;
            data.SetBaseStat(stat, data.GetBaseStat(stat) + 1);

            if (!pointsAddedToStat.ContainsKey(stat)) pointsAddedToStat[stat] = 0;
            pointsAddedToStat[stat]++;

            UpdateAllUI();
        }
    }

    // Stat azaltma butonuna bas�ld���nda
    public void OnStatDecrease(StatType stat) {
        if (pointsAddedToStat.ContainsKey(stat) && pointsAddedToStat[stat] > 0) {
            bonusPoints++;
            data.SetBaseStat(stat, data.GetBaseStat(stat) - 1);
            pointsAddedToStat[stat]--;

            UpdateAllUI();
        }
    }

    // "Next" butonuna bas�ld���nda
    public void OnNextButtonClicked() {
        if (bonusPoints == 0) {
            manager.GoToSpellsStep();
        }
    }

    // T�m aray�z� g�ncelleyen ana metot
    private void UpdateAllUI() {
        bool allRequirementsMet = true;
        bonusPointsText.text = bonusPoints.ToString();

        foreach (var display in statDisplays) {
            int currentStatValue = data.GetBaseStat(display.statType);
            display.statValueText.text = currentStatValue.ToString();

            // De�i�kenin tipi 'StatRequirement?' olmal�
            StatRequirement? requirement = data.characterClass.GetRequirementForStat(display.statType);

            // Null kontrol� 'requirement.Value' ile birlikte kullan�lmal�
            if (requirement != null && currentStatValue < requirement.Value.minValue) {
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

        nextButton.SetActive(bonusPoints == 0 && allRequirementsMet);
    }
}
