// _Project/Scripts/Data/ScriptableObjects/StatusEffectSO.cs
using UnityEngine;
using System.Collections.Generic;
using static RaceSO;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "WizardryPro/Character/Status Effect")]
public class StatusEffectSO : ScriptableObject {
    [Header("Basic Info")]
    public string effectName; // Örn: Afraid, Asleep, OK, Stoned
    [TextArea(3, 10)]
    public string description; // Kullanýcýya gösterilecek açýklama
    [TextArea(3, 10)]
    public string mechanicsDescription; // "Each Round" efektlerini açýklayan metin
    public ConditionSeverity severity;
    public bool isPositiveEffect; // Buff mý, Debuff/Illness mý?

    [Header("Cure Methods")]
    public List<string> cureMethods; // Örn: "Resting", "Cure Lesser Condition", "Resurrection"
                                     // Daha sonra enum'a çevrilebilir veya CureItemSO/SpellSO referanslarý olabilir.

    [Header("Effects (Bu kýsým programatik olarak uygulanacak)")]
    // Bu efektlerin nasýl uygulanacaðý karmaþýk olacak ve özel bir sistem gerektirecek.
    // Þimdilik sadece örnek bazý boolean'lar ve deðerler tutalým.
    public bool preventsAction = false;
    public float damageMultiplierTaken = 1f; // Aldýðý hasar çarpaný (Asleep için 2x)
    public int acOverride = -1; // -1 ise AC'yi deðiþtirmez, yoksa bu deðere sabitler (Asleep için 15)
    public int missChanceIncrease = 0; // Afraid için +25
    public int evasionACModifier = 0; // Afraid için -4
                                      // public StatType statToDrainOnApplication; // Dead için VIT
                                      // public int drainAmount;

    // Olasýlýksal efektler için (programatik olarak yönetilecek)
    // public float chanceToCower = 0f; // Afraid için 0.20
    // public float chanceToRunBase = 0f; // Afraid için 0.20, Personality'e baðlý

    // Bu SO, bir efektin *þablonunu* tanýmlar.
    // Karakter üzerinde aktif olan efektler için ayrý bir runtime class'ý gerekebilir.

    [Header("Resistance Modifiers (Applied by this effect)")] // YENÝ EKLENDÝ
    public List<ResistanceModifier> resistanceModifiers; // Bu efekt aktifken eklenecek/çýkarýlacak dirençler

}
