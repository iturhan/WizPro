// _Project/Scripts/Data/ScriptableObjects/SpellSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpell", menuName = "WizardryPro/Magic/Spell")]
public class SpellSO : ScriptableObject {
    [Header("Basic Info")]
    public string spellName;
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;
    [Range(1, 7)]
    public int spellBookLevel = 1; // B�y�n�n ��renilebilece�i minimum seviye (kitap seviyesi)

    [Header("School & Realm")]
    // Bu b�y� hangi okullar�n/kitaplar�n par�as�?
    public List<SpellSchoolType> availableInSpellbooks;
    public MagicRealm magicRealm; // B�y�n�n ait oldu�u ana b�y� alan�

    [Header("Casting Mechanics")]
    public SpellTargetingType targetingType = SpellTargetingType.NoTarget;
    public int baseSpellPointCost = 1;
    [Range(1, 8)] // Genellikle 7, baz� b�y�ler i�in 8
    public int maxPowerLevel = 7;
    // Her g�� seviyesi i�in SP maliyeti. E�er bo�sa, baseSpellPointCost * powerLevel kullan�l�r.
    // �rn: [3, 5, 7, 10, 14, 19, 25] -> G�� seviyesi 1 i�in 3 SP, GL 7 i�in 25 SP.
    public List<int> spellPointCostByPowerLevel;

    [Header("Effects & Duration (Basit Metin G�sterimi - Faz 2'de Detaylanacak)")]
    // �rnek: "10/L", "4/L", "N/A"
    public string effectPotencyFormulaText = "N/A";
    // �rnek: "Instant", "1/L", "24 Hours", "5 Minutes/L"
    public string durationFormulaText = "Instant";

    [Header("Requirements & Flags")]
    public bool canCastInCombat = true;
    public bool canCastOutOfCombat = true;
    // Gelecekte: Gerekli e�yalar (�rn: Alchemist i�in powders), Minimum Mastery Skill vb.

    // FAZ 2'DE EKLENECEK DAHA DETAYLI ETK� S�STEM�:
    // public List<SpellEffectEntry> spellEffects;
}

// �leride kullan�lacak yap� (Faz 2)
// [System.Serializable]
// public class SpellEffectEntry
// {
//     public SpellEffectType effectType; // Damage, Heal, ApplyStatus, ModifyStat etc.
//     public float baseValue;
//     public float perCasterLevelValue;
//     public float perPowerLevelMultiplier; // G�� seviyesine g�re etki �arpan�
//     public StatType relatedStat; // Etkisi hangi stattan etkileniyor
//     public StatusEffectSO statusToApply;
//     public string durationFormula; // Bu etkiye �zel s�re
//     public SpellTargetingType specificTargetingOverride; // E�er bu efektin hedeflemesi farkl�ysa
// }

/*
spellPointCostByPowerLevel: E�er bu liste bo� b�rak�l�rsa, basit bir hesaplama (�rn: baseSpellPointCost * se�ilenG��Seviyesi) kullan�labilir. Ama Wizardry genellikle her seviye i�in �zel maliyetler belirler.
effectPotencyFormulaText ve durationFormulaText: �imdilik b�y�lerin etkilerini ve s�relerini metin olarak tutaca��z. Bir sonraki fazda bunlar� daha yap�sal bir "Spell Effect System" ile de�i�tirece�iz.
*/
