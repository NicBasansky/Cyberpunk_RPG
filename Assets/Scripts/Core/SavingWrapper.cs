using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using RPG.SceneManagement;

namespace RPG.Core
{
    public class SavingWrapper : MonoBehaviour
    {

        const string defaultSaveFile = "Save";
        [SerializeField] float initialFadeOutSeconds = 0.75f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeToBlackImmediate();
            yield return fader.FadeToClear(initialFadeOutSeconds);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        private void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }



    }

}