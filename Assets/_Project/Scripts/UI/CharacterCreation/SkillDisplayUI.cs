// SkillDisplayUI.cs (REV�ZE ED�LD�)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplayUI : MonoBehaviour {
    // Art�k bir SkillType de�il, bir SkillSO tutuyor.
    public SkillSO skill { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillValueText;
    public Button increaseButton;
    public Button decreaseButton;

    private SkillSelectionController controller;

    // Setup metodu art�k bir SkillSO al�yor.
    public void Setup(SkillSO targetSkill, SkillSelectionController ownerController) {
        this.skill = targetSkill;
        this.controller = ownerController;
        skillNameText.text = skill.skillName; // �smi SO'dan al�yor.

        increaseButton.onClick.RemoveAllListeners();
        decreaseButton.onClick.RemoveAllListeners();

        // Butonlar art�k bir enum de�il, SkillSO referans� g�nderiyor.
        increaseButton.onClick.AddListener(() => controller.OnSkillIncrease(skill));
        decreaseButton.onClick.AddListener(() => controller.OnSkillDecrease(skill));
    }
}