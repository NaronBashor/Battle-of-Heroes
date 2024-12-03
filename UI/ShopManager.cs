using TMPro;
using UnityEngine;

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

    private void Start()
    {
        PopulateCharacterInfo("Red-haired Paladin");
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
        characterAttackSpeedText.text = character.attackSpeed.ToString();
        characterMoveSpeedText.text = character.speed.ToString();
    }
}
