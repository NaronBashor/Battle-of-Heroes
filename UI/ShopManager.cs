using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDescriptionText;
    [SerializeField] private TextMeshProUGUI characterLoreText;
    [SerializeField] private TextMeshProUGUI characterHealthText;
    [SerializeField] private TextMeshProUGUI characterAttackTypeText;
    [SerializeField] private TextMeshProUGUI characterDamageText;
    [SerializeField] private TextMeshProUGUI characterAttackSpeedText;
    [SerializeField] private TextMeshProUGUI characterMoveSpeedText;

    [SerializeField] private List<TextMeshProUGUI> characterPurchaseCostText;

    private List<string> characterNames = new List<string> {
        "Red-haired Paladin",
        "Skeletal Gladiator",
        "Cannonball Trooper",
        "Goblin Bomber",
        "Mighty Minotaur",
        "Dwarven Hammerer",
        "Viking Berserker",
        "Ironclad Knight",
        "Zeus the Stormcaller"
        };

    private void Start()
    {
        PopulateCharacterInfo("Red-haired Paladin");
        InitPurchaseButtons();
    }

    private void InitPurchaseButtons()
    {
        for (int i = 1; i < characterNames.Count; i++) {
            CharacterData character = characterDatabase.GetCharacterByName(characterNames[i]);
            characterPurchaseCostText[i].text = character.purchaseCost.ToString();
        }
    }

    public void PopulateCharacterInfo(string characterName)
    {
        CharacterData character = characterDatabase.GetCharacterByName(characterName);

        characterNameText.text = character.characterName;
        characterDescriptionText.text = character.characterDescription;
        characterLoreText.text = character.characterLore;
        characterHealthText.text = character.health.ToString();
        characterAttackTypeText.text = character.attackType.ToString();
        characterDamageText.text = character.damage.ToString();
        characterAttackSpeedText.text = character.attackCooldown.ToString();
        characterMoveSpeedText.text = character.speed.ToString();
    }
}
