using UnityEngine;
using System;

namespace SaveurSavante.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Chapitres complétés")]
        public bool egypteComplete = false;
        public bool vikingsComplete = false;
        public bool titanicComplete = false;
        public bool gandhiComplete = false;

        [Header("Position retour Hub")]
        public Vector3 hubPosition = new Vector3(0, 1.5f, 0);

        public event Action OnChapterCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CompleteChapter(string chapterName)
        {
            switch (chapterName.ToLower())
            {
                case "egypte":
                    egypteComplete = true;
                    break;
                case "vikings":
                    vikingsComplete = true;
                    break;
                case "titanic":
                    titanicComplete = true;
                    break;
                case "gandhi":
                    gandhiComplete = true;
                    break;
            }

            OnChapterCompleted?.Invoke();

            // Vérifier si tous les chapitres sont complétés
            if (AllChaptersComplete())
            {
                Debug.Log("🎉 Félicitations ! Vous avez complété tous les chapitres de Saveur Savante !");
            }
        }

        public bool AllChaptersComplete()
        {
            return egypteComplete && vikingsComplete && titanicComplete && gandhiComplete;
        }

        public int GetCompletedChaptersCount()
        {
            int count = 0;
            if (egypteComplete) count++;
            if (vikingsComplete) count++;
            if (titanicComplete) count++;
            if (gandhiComplete) count++;
            return count;
        }
    }
}
