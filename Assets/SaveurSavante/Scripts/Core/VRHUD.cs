using UnityEngine;

namespace SaveurSavante.Core
{
    public class VRHUD : MonoBehaviour
    {
        private Transform cam;
        [Tooltip("Position relative à la caméra (X=droite/gauche, Y=Haut/Bas, Z=Avance)")]
        public Vector3 offset = new Vector3(0f, -0.2f, 0.8f); 

        private void Start()
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (cam != null)
            {
                // Position devant la caméra, légèrement en bas
                transform.position = Vector3.Lerp(transform.position, cam.position + cam.TransformDirection(offset), Time.deltaTime * 5f);
                
                // Rotation pour toujours être face à l'utilisateur
                Vector3 lookDirection = transform.position - cam.position;
                if (lookDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
                }
            }
        }
    }
}
