using UnityEngine;

public class FallingPin : MonoBehaviour
{
    [Header("Falling Detection Settings")]
    public float fallAngleThreshold = 45f;
    public bool isFallen = false;

    [Header("Optional Celebration Effect")]
    public ParticleSystem celebrationEffect;

    void Update()
    {
        //gameObject.SetActive(!isFallen);
        // Si la quille est déjà tombée, on ne refait pas le test
        if (isFallen)
            return;

        // Calcul de l’angle entre l’axe vertical de la quille et le haut du monde
        float angle = Vector3.Angle(transform.up, Vector3.up);

        // Si l’angle dépasse le seuil, la quille est considérée comme tombée
        if (angle < fallAngleThreshold)
        {
            isFallen = true;

            // Joue l’effet de particules si assigné
            if (celebrationEffect != null)
            {
                celebrationEffect.Play();
            }

            // (Optionnel pour tester visuellement)
            // Désactive la quille lorsqu'elle tombe
            // gameObject.SetActive(false);
        }
    }
}