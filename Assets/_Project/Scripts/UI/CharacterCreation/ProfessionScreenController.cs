// _Project/Scripts/UI/CharacterCreation/ProfessionScreenController.cs
using UnityEngine;
using TMPro; // TextMeshPro için
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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

    bool isFirstSetup = true; // Sadece bir kez Reroll yapmak için bayrak

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
    public GameObject spellSelectionPanel;

    void Awake() {
        manager = FindAnyObjectByType<CharacterCreationManager>();
    }

    void OnEnable() {
        // Manager'a sor: Profesyon adýmýný daha önce tamamlamýþ mýydýk?
        if (manager.hasProfessionBeenSet) {
            // CEVAP EVET ÝSE (Geri dönüyoruz):
            // Ekraný sýfýrlama! Sadece mevcut verilerle arayüzü güncelle.
            // UpdateAllUI() metodu, karakterin o anki statlarýný, bonus puanlarýný
            // ve renkleri ekrana doðru bir þekilde yansýtacaktýr.
            Debug.Log("Profesyon Ekranýna geri dönüldü. Mevcut durum geri yükleniyor.");
            UpdateAllUI();
        }
        else {
            // CEVAP HAYIR ÝSE (Ýlk kez geliyoruz):
            // Her þeyi sýfýrdan baþlat.
            Debug.Log("Profesyon Ekraný ilk kez baþlatýlýyor.");
            PopulateDropdowns();
            // Stat satýrlarýnýn butonlarýný kur. Bu iþlem bir kere yapýlmalý.
            foreach (var display in statDisplays) {
                if (display != null) {
                    display.Setup(this);
                }
            }
            SetupInitialState(); // Bu, RerollAttributes'ý çaðýrarak statlarý belirler.
        }
    }

    private void PopulateDropdowns() {
        // --- Irk Dropdown'ý ---
        raceDropdown.ClearOptions();
        // 'Dropdown.OptionData' yerine 'TMPro.TMP_Dropdown.OptionData' kullanýyoruz
        raceDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("--- Irk Seçiniz ---"));
        foreach (var race in assetHolder.allRaces) {
            raceDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(race.raceName));
        }
        raceDropdown.RefreshShownValue();

        // --- Sýnýf Dropdown'ý ---
        classDropdown.ClearOptions();
        // 'Dropdown.OptionData' yerine 'TMPro.TMP_Dropdown.OptionData' kullanýyoruz
        classDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("--- Sýnýf Seçiniz ---"));
        foreach (var c in assetHolder.allClasses) {
            classDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(c.className));
        }
        classDropdown.RefreshShownValue();

        // --- Cinsiyet Dropdown'ý ---
        genderDropdown.ClearOptions();
        // 'Dropdown.OptionData' yerine 'TMPro.TMP_Dropdown.OptionData' kullanýyoruz
        genderDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("--- Cinsiyet Seçiniz ---"));
        foreach (var sex in System.Enum.GetNames(typeof(Sex))) {
            genderDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(sex));
        }
        genderDropdown.RefreshShownValue();

        // Dropdown'lar deðiþtiðinde çaðrýlacak metotlarý ata
        raceDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        genderDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        classDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
    }

    private void SetupInitialState() {
        // Baþlangýçta tüm dropdown'lar seçili deðildir, bu yüzden
        // karakter verisini oluþturmak için varsayýlan bir kombinasyon kullanabiliriz
        // veya ilk seçimin yapýlmasýný bekleyebiliriz.
        // Ama en önemlisi, ilk bonus puan zarýný burada atýyoruz.

        // Not: Bu metodun çaðrýldýðý OnEnable'da, data'nýn null OLMADIÐINDAN emin olmalýyýz.
        // En temiz yol, ilk geçerli seçim yapýldýðýnda Reroll'u tetiklemektir.
        // Ancak basitlik adýna, OnEnable -> SetupInitialState akýþýný koruyalým.

        // Eðer data henüz yoksa, varsayýlan bir karakter oluþtur.
        if (data == null) {
            OnProfessionChanged(); // Bu, ilk veri setini oluþturur.
        }

        // Ardýndan, ilk bonus puan zarýný at.
        RerollAttributes();
    }

    // Irk, Cinsiyet veya Sýnýf deðiþtiðinde her þeyi sýfýrlar
    public void OnProfessionChanged() {
        // Öncelikle tüm dropdown'larýn seçili olup olmadýðýný kontrol et
        if (raceDropdown.value == 0 || classDropdown.value == 0 || genderDropdown.value == 0) {
            UpdateAllUI();
            return;
        }

        // ... (Karakter verisini oluþturma kodlarý)
        RaceSO selectedRace = assetHolder.allRaces[raceDropdown.value - 1];
        ClassSO selectedClass = assetHolder.allClasses[classDropdown.value - 1];
        Sex selectedGender = (Sex)(genderDropdown.value - 1);
        manager.UpdateCharacterProfession(selectedRace, selectedGender, selectedClass);
        data = manager.GetCurrentCharacterData();

        // --- YENÝ MANTIK ---
        // Eðer bu, geçerli bir seçimin yapýldýðý ÝLK seferse,
        // bonus puanlar için zar at.
        if (isFirstSetup) {
            RerollAttributes();
            isFirstSetup = false; // Bayraðý indir, bir daha bu bloða girilmesin.
        }
        else // Sonraki tüm seçimlerde ise, sadece UI'ý sýfýrla.
        {
            ResetUIWithoutReroll();
        }
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
        if (data == null) {
            // Baþlangýçta dropdown'lar seçili olmadýðý için "Ýleri" butonunu pasif yapalým.
            nextButton.SetActive(false);
            return;
        }

        bonusPointsText.text = bonusPoints.ToString();
        bool allRequirementsMet = true; // Bu deðiþkeni gereksinim kontrolü için tutuyoruz

        foreach (var display in statDisplays) {
            // 1. Karakterin o anki verisinden doðru stat deðerini al.
            int currentStatValue = data.GetBaseStat(display.statType);

            // 2. EN ÖNEMLÝ DÜZELTME:
            // Sadece stat DEÐERÝNÝ gösteren metin alanýný ('statValueText') güncelle.
            // Stat ÝSMÝNÝ ('statNameText') gösteren alana DOKUNMA.
            display.statValueText.text = currentStatValue.ToString();

            // 3. Renk ve buton mantýðýný çalýþtýr (önceki adýmdaki gibi).
            StatRequirement? requirement = data.characterClass.GetRequirementForStat(display.statType);
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

        // "Ýleri" butonunun durumunu güncelle
        bool areAllDropdownsSelected = raceDropdown.value > 0 && classDropdown.value > 0 && genderDropdown.value > 0;
        nextButton.SetActive(areAllDropdownsSelected && bonusPoints == 0 && allRequirementsMet);
    }
        // Bonus puan havuzunu deðiþtirmeden, sadece daðýtýlmýþ puanlarý sýfýrlar ve UI'ý günceller.
    private void ResetUIWithoutReroll() {
        // 1. ÖNCELÝKLE, DAÐITILMIÞ BONUS PUANLARI GERÝ AL
        foreach (var stat in pointsAddedToStat.Keys.ToList()) {
            int pointsToRemove = pointsAddedToStat[stat];
            data.SetBaseStat(stat, data.GetBaseStat(stat) - pointsToRemove);
        }

        // 2. PUAN KAYITLARINI TAMAMEN TEMÝZLE
        pointsAddedToStat.Clear();

        // 3. ARAYÜZÜ GÜNCELLE
        UpdateAllUI();
    }



}
