// _Project/Scripts/UI/CharacterCreation/ProfessionScreenController.cs
using UnityEngine;
using TMPro; // TextMeshPro i�in
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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

    bool isFirstSetup = true; // Sadece bir kez Reroll yapmak i�in bayrak

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
    public GameObject spellSelectionPanel;

    void Awake() {
        manager = FindAnyObjectByType<CharacterCreationManager>();
    }

    void OnEnable() {
        // Manager'a sor: Profesyon ad�m�n� daha �nce tamamlam�� m�yd�k?
        if (manager.hasProfessionBeenSet) {
            // CEVAP EVET �SE (Geri d�n�yoruz):
            // Ekran� s�f�rlama! Sadece mevcut verilerle aray�z� g�ncelle.
            // UpdateAllUI() metodu, karakterin o anki statlar�n�, bonus puanlar�n�
            // ve renkleri ekrana do�ru bir �ekilde yans�tacakt�r.
            Debug.Log("Profesyon Ekran�na geri d�n�ld�. Mevcut durum geri y�kleniyor.");
            UpdateAllUI();
        }
        else {
            // CEVAP HAYIR �SE (�lk kez geliyoruz):
            // Her �eyi s�f�rdan ba�lat.
            Debug.Log("Profesyon Ekran� ilk kez ba�lat�l�yor.");
            PopulateDropdowns();
            // Stat sat�rlar�n�n butonlar�n� kur. Bu i�lem bir kere yap�lmal�.
            foreach (var display in statDisplays) {
                if (display != null) {
                    display.Setup(this);
                }
            }
            SetupInitialState(); // Bu, RerollAttributes'� �a��rarak statlar� belirler.
        }
    }

    private void PopulateDropdowns() {
        // --- Irk Dropdown'� ---
        raceDropdown.ClearOptions();
        // 'Dropdown.OptionData' yerine 'TMPro.TMP_Dropdown.OptionData' kullan�yoruz
        raceDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("--- Irk Se�iniz ---"));
        foreach (var race in assetHolder.allRaces) {
            raceDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(race.raceName));
        }
        raceDropdown.RefreshShownValue();

        // --- S�n�f Dropdown'� ---
        classDropdown.ClearOptions();
        // 'Dropdown.OptionData' yerine 'TMPro.TMP_Dropdown.OptionData' kullan�yoruz
        classDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("--- S�n�f Se�iniz ---"));
        foreach (var c in assetHolder.allClasses) {
            classDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(c.className));
        }
        classDropdown.RefreshShownValue();

        // --- Cinsiyet Dropdown'� ---
        genderDropdown.ClearOptions();
        // 'Dropdown.OptionData' yerine 'TMPro.TMP_Dropdown.OptionData' kullan�yoruz
        genderDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("--- Cinsiyet Se�iniz ---"));
        foreach (var sex in System.Enum.GetNames(typeof(Sex))) {
            genderDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(sex));
        }
        genderDropdown.RefreshShownValue();

        // Dropdown'lar de�i�ti�inde �a�r�lacak metotlar� ata
        raceDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        genderDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
        classDropdown.onValueChanged.AddListener(delegate { OnProfessionChanged(); });
    }

    private void SetupInitialState() {
        // Ba�lang��ta t�m dropdown'lar se�ili de�ildir, bu y�zden
        // karakter verisini olu�turmak i�in varsay�lan bir kombinasyon kullanabiliriz
        // veya ilk se�imin yap�lmas�n� bekleyebiliriz.
        // Ama en �nemlisi, ilk bonus puan zar�n� burada at�yoruz.

        // Not: Bu metodun �a�r�ld��� OnEnable'da, data'n�n null OLMADI�INDAN emin olmal�y�z.
        // En temiz yol, ilk ge�erli se�im yap�ld���nda Reroll'u tetiklemektir.
        // Ancak basitlik ad�na, OnEnable -> SetupInitialState ak���n� koruyal�m.

        // E�er data hen�z yoksa, varsay�lan bir karakter olu�tur.
        if (data == null) {
            OnProfessionChanged(); // Bu, ilk veri setini olu�turur.
        }

        // Ard�ndan, ilk bonus puan zar�n� at.
        RerollAttributes();
    }

    // Irk, Cinsiyet veya S�n�f de�i�ti�inde her �eyi s�f�rlar
    public void OnProfessionChanged() {
        // �ncelikle t�m dropdown'lar�n se�ili olup olmad���n� kontrol et
        if (raceDropdown.value == 0 || classDropdown.value == 0 || genderDropdown.value == 0) {
            UpdateAllUI();
            return;
        }

        // ... (Karakter verisini olu�turma kodlar�)
        RaceSO selectedRace = assetHolder.allRaces[raceDropdown.value - 1];
        ClassSO selectedClass = assetHolder.allClasses[classDropdown.value - 1];
        Sex selectedGender = (Sex)(genderDropdown.value - 1);
        manager.UpdateCharacterProfession(selectedRace, selectedGender, selectedClass);
        data = manager.GetCurrentCharacterData();

        // --- YEN� MANTIK ---
        // E�er bu, ge�erli bir se�imin yap�ld��� �LK seferse,
        // bonus puanlar i�in zar at.
        if (isFirstSetup) {
            RerollAttributes();
            isFirstSetup = false; // Bayra�� indir, bir daha bu blo�a girilmesin.
        }
        else // Sonraki t�m se�imlerde ise, sadece UI'� s�f�rla.
        {
            ResetUIWithoutReroll();
        }
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
        if (data == null) {
            // Ba�lang��ta dropdown'lar se�ili olmad��� i�in "�leri" butonunu pasif yapal�m.
            nextButton.SetActive(false);
            return;
        }

        bonusPointsText.text = bonusPoints.ToString();
        bool allRequirementsMet = true; // Bu de�i�keni gereksinim kontrol� i�in tutuyoruz

        foreach (var display in statDisplays) {
            // 1. Karakterin o anki verisinden do�ru stat de�erini al.
            int currentStatValue = data.GetBaseStat(display.statType);

            // 2. EN �NEML� D�ZELTME:
            // Sadece stat DE�ER�N� g�steren metin alan�n� ('statValueText') g�ncelle.
            // Stat �SM�N� ('statNameText') g�steren alana DOKUNMA.
            display.statValueText.text = currentStatValue.ToString();

            // 3. Renk ve buton mant���n� �al��t�r (�nceki ad�mdaki gibi).
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

        // "�leri" butonunun durumunu g�ncelle
        bool areAllDropdownsSelected = raceDropdown.value > 0 && classDropdown.value > 0 && genderDropdown.value > 0;
        nextButton.SetActive(areAllDropdownsSelected && bonusPoints == 0 && allRequirementsMet);
    }
        // Bonus puan havuzunu de�i�tirmeden, sadece da��t�lm�� puanlar� s�f�rlar ve UI'� g�nceller.
    private void ResetUIWithoutReroll() {
        // 1. �NCEL�KLE, DA�ITILMI� BONUS PUANLARI GER� AL
        foreach (var stat in pointsAddedToStat.Keys.ToList()) {
            int pointsToRemove = pointsAddedToStat[stat];
            data.SetBaseStat(stat, data.GetBaseStat(stat) - pointsToRemove);
        }

        // 2. PUAN KAYITLARINI TAMAMEN TEM�ZLE
        pointsAddedToStat.Clear();

        // 3. ARAY�Z� G�NCELLE
        UpdateAllUI();
    }



}
