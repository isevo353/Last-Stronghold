using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text tutorialText;
    public float showTime = 5f;

    void Start()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "Выбери башню в панели магазина → курсор станет башней → кликни по кругу, чтобы поставить.\nПКМ или ESC — отмена.";

            // Ñêðûâàåì ÷åðåç showTime ñåêóíä
            Invoke(nameof(HideTutorial), showTime);
        }
    }

    void HideTutorial()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "";
        }
    }
}
