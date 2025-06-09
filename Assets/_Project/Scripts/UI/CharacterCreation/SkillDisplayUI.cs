// SkillDisplayUI.cs (REVÝZE EDÝLDÝ)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplayUI : MonoBehaviour {
    // Artýk bir SkillType deðil, bir SkillSO tutuyor.
    public SkillSO skill { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillValueText;
    public Button increaseButton;
    public Button decreaseButton;

    private SkillSelectionController controller;

    // Setup metodu artýk bir SkillSO alýyor.
    public void Setup(SkillSO targetSkill, SkillSelectionController ownerController) {
        this.skill = targetSkill;
        this.controller = ownerController;
        skillNameText.text = skill.skillName; // Ýsmi SO'dan alýyor.

        increaseButton.onClick.RemoveAllListeners();
        decreaseButton.onClick.RemoveAllListeners();

        // Butonlar artýk bir enum deðil, SkillSO referansý gönderiyor.
        increaseButton.onClick.AddListener(() => controller.OnSkillIncrease(skill));
        decreaseButton.onClick.AddListener(() => controller.OnSkillDecrease(skill));
    }
}