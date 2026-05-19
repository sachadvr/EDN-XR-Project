using UnityEngine;
using TMPro;

public class PinCounter : MonoBehaviour
{
    [Header("Pins")]
    public FallingPin[] pins;

    [Header("Score")]
    public int fallenCount = 0;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Récupère toutes les quilles (FallingPin) dans les enfants de BowlingAlley
        pins = GetComponentsInChildren<FallingPin>();
    }

    void Update()
    {
        // Réinitialise le compteur à chaque frame
        fallenCount = 0;

        // Parcourt toutes les quilles
        foreach (FallingPin pin in pins)
        {
            if (pin.isFallen)
            {
                fallenCount++;
            }
        }

        // Met à jour l'affichage du score
        if (scoreText != null)
        {
            scoreText.text = "Score : " + fallenCount.ToString();
        }
    }
}