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
            tutorialText.text = "КЛИКАЙ НА КРУГИ\nчтобы поставить башню\n\nСтоимость: 50 монет\n\nДеньги за убийство врагов";

            // Скрываем через showTime секунд
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
