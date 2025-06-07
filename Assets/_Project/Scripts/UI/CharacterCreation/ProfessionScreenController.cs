// _Project/Scripts/UI/CharacterCreation/ProfessionScreenController.cs
using UnityEngine;
using TMPro; // TextMeshPro için
using System.Collections.Generic;
using System.Linq;

public class ProfessionScreenController : MonoBehaviour {
    [Header("Asset References")]
    [Tooltip("Tüm Irk ve Sýnýf SO'larýný tutan asset")]
    public GameAssetHolderSO assetHolder;

    [Header("Manager Reference")]
    private CharacterCreationManager manager;

    [Header("UI Elements")]
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown genderDropdown;
    public TMP_Dropdown classDropdown;
    public TextMeshProUGUI bonusPointsText;

    // Stat text'leri için bir dictionary veya public liste kullanabilirsiniz
    public List<StatDisplayUI> statDisplays; // Her bir stat için UI elemanlarýný tutan özel bir component

    [Header("UI Style")] // YENÝ EKLENDÝ
    public Color defaultStatColor = Color.white; // Deðiþtirilmemiþ statlar için varsayýlan renk
    public Color modifiedStatColor = new Color(0.5f, 0.7f, 1f); // Deðiþtirilmiþ statlar için mavi tonu bir renk
    public Color requirementNotMetColor = Color.red; // Gereksinim karþýlanmadýðýnda kullanýlacak renk.

    [Header("Control Buttons")]
    public GameObject nextButton; // Bonus puanlar bitmeden aktif olmamalý

    // Dahili Deðiþkenler
    private CharacterData data;
    private int bonusPoints;
    private Dictionary<StatType, int> pointsAddedToStat = new Dictionary<StatType, int>();
    

    void Awake() {
        manager = FindAnyObjectByType<CharacterCreationManager>();
    }

    void OnEnable() {
        PopulateDropdowns();
        // ---- YENÝ EKLENEN KISIM ----
        // Stat satýrlarýndaki butonlara hangi fonksiyonu çaðýracaklarýný söylüyoruz.
        // Bu, her satýrýn Setup() metodunu çaðýrarak yapýlýr.
        foreach (var display in statDisplays) {
            if (display != null) {
                display.Setup(this); // 'this' diyerek kendisini (ProfessionScreenController'ý) referans olarak verir.
            }
            else {
                Debug.LogError("ProfessionScreenController'daki 'statDisplays' listesinde boþ (null) bir eleman var! Inspector'u kontrol edin.");
            }
        }
        // ---- YENÝ KISMIN SONU ----
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

        // Dropdown'lar deðiþtiðinde çaðrýlacak metotlarý ata
        raceDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        genderDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        classDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
    }

    private void SetupInitialState() {
        // Baþlangýçta dropdown'lardaki ilk seçeneklerle karakteri oluþtur
        OnProfessionChanged();
    }

    // Irk, Cinsiyet veya Sýnýf deðiþtiðinde her þeyi sýfýrlar
    public void OnProfessionChanged() {
        pointsAddedToStat.Clear();

        RaceSO selectedRace = assetHolder.allRaces[raceDropdown.value];
        Sex selectedGender = (Sex)System.Enum.Parse(typeof(Sex), genderDropdown.options[genderDropdown.value].text);
        ClassSO selectedClass = assetHolder.allClasses[classDropdown.value];

        manager.UpdateCharacterProfession(selectedRace, selectedGender, selectedClass);
        data = manager.GetCurrentCharacterData();

        RerollAttributes(); // Otomatik olarak yeni bonus puanlar ver
    }

    // Zar atma ve puanlarý sýfýrlama
    public void RerollAttributes() {

        // 1. ÖNCELÝKLE, DAÐITILMIÞ BONUS PUANLARI GERÝ AL
        // `pointsAddedToStat` dictionary'sinde kayýtlý olan her bir stat için:
        foreach (var stat in pointsAddedToStat.Keys.ToList()) {
            // Bu stata kaç puan eklenmiþti?
            int pointsToRemove = pointsAddedToStat[stat];
            // Karakterin ana verisinden bu puanlarý çýkararak stat'ý eski haline getir.
            data.SetBaseStat(stat, data.GetBaseStat(stat) - pointsToRemove);
        }

        pointsAddedToStat.Clear();

        // Yeni bonus puanlarý ver (Örnek: 6-15 arasý)
        bonusPoints = Random.Range(6, 16);

        UpdateAllUI();
    }

    // Stat artýrma butonuna basýldýðýnda
    public void OnStatIncrease(StatType stat) {
        if (bonusPoints > 0 && data.GetBaseStat(stat) < 18) {
            bonusPoints--;
            data.SetBaseStat(stat, data.GetBaseStat(stat) + 1);

            if (!pointsAddedToStat.ContainsKey(stat)) pointsAddedToStat[stat] = 0;
            pointsAddedToStat[stat]++;

            UpdateAllUI();
        }
    }

    // Stat azaltma butonuna basýldýðýnda
    public void OnStatDecrease(StatType stat) {
        if (pointsAddedToStat.ContainsKey(stat) && pointsAddedToStat[stat] > 0) {
            bonusPoints++;
            data.SetBaseStat(stat, data.GetBaseStat(stat) - 1);
            pointsAddedToStat[stat]--;

            UpdateAllUI();
        }
    }

    // "Next" butonuna basýldýðýnda
    public void OnNextButtonClicked() {
        if (bonusPoints == 0) {
            manager.GoToSpellsStep();
        }
    }

    // Tüm arayüzü güncelleyen ana metot
    private void UpdateAllUI() {
        bool allRequirementsMet = true;
        bonusPointsText.text = bonusPoints.ToString();

        foreach (var display in statDisplays) {
            int currentStatValue = data.GetBaseStat(display.statType);
            display.statValueText.text = currentStatValue.ToString();

            // Deðiþkenin tipi 'StatRequirement?' olmalý
            StatRequirement? requirement = data.characterClass.GetRequirementForStat(display.statType);

            // Null kontrolü 'requirement.Value' ile birlikte kullanýlmalý
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
