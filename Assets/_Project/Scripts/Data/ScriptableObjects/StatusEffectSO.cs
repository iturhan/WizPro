// _Project/Scripts/Data/ScriptableObjects/StatusEffectSO.cs
using UnityEngine;
using System.Collections.Generic;
using static RaceSO;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "WizardryPro/Character/Status Effect")]
public class StatusEffectSO : ScriptableObject {
    [Header("Basic Info")]
    public string effectName; // �rn: Afraid, Asleep, OK, Stoned
    [TextArea(3, 10)]
    public string description; // Kullan�c�ya g�sterilecek a��klama
    [TextArea(3, 10)]
    public string mechanicsDescription; // "Each Round" efektlerini a��klayan metin
    public ConditionSeverity severity;
    public bool isPositiveEffect; // Buff m�, Debuff/Illness m�?

    [Header("Cure Methods")]
    public List<string> cureMethods; // �rn: "Resting", "Cure Lesser Condition", "Resurrection"
                                     // Daha sonra enum'a �evrilebilir veya CureItemSO/SpellSO referanslar� olabilir.

    [Header("Effects (Bu k�s�m programatik olarak uygulanacak)")]
    // Bu efektlerin nas�l uygulanaca�� karma��k olacak ve �zel bir sistem gerektirecek.
    // �imdilik sadece �rnek baz� boolean'lar ve de�erler tutal�m.
    public bool preventsAction = false;
    public float damageMultiplierTaken = 1f; // Ald��� hasar �arpan� (Asleep i�in 2x)
    public int acOverride = -1; // -1 ise AC'yi de�i�tirmez, yoksa bu de�ere sabitler (Asleep i�in 15)
    public int missChanceIncrease = 0; // Afraid i�in +25
    public int evasionACModifier = 0; // Afraid i�in -4
                                      // public StatType statToDrainOnApplication; // Dead i�in VIT
                                      // public int drainAmount;

    // Olas�l�ksal efektler i�in (programatik olarak y�netilecek)
    // public float chanceToCower = 0f; // Afraid i�in 0.20
    // public float chanceToRunBase = 0f; // Afraid i�in 0.20, Personality'e ba�l�

    // Bu SO, bir efektin *�ablonunu* tan�mlar.
    // Karakter �zerinde aktif olan efektler i�in ayr� bir runtime class'� gerekebilir.

    [Header("Resistance Modifiers (Applied by this effect)")] // YEN� EKLEND�
    public List<ResistanceModifier> resistanceModifiers; // Bu efekt aktifken eklenecek/��kar�lacak diren�ler

}
