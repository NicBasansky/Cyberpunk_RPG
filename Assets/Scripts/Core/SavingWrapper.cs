using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using RPG.SceneManagement;
using UnityEngine.SceneManagement;
using System;

namespace RPG.Core
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string currentSaveKey = "currentSaveKey";
        [SerializeField] float fadeToClearSeconds = 0.75f;
        [SerializeField] float fadeToBlackSeconds = 2f;
        int firstSceneIndexToLoad = 1;
        int menuSceneIndexToLoad = 0;

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

        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(currentSaveKey)) return;
            if (!GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave())) return;
            StartCoroutine(LoadLastScene());
        }

        public void NewGame(string saveFile)
        {
            if (string.IsNullOrEmpty(saveFile)) return;
            SetCurrentSave(saveFile);
            StartCoroutine(LoadFirstScene());
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }

        public void LoadMainMenu()
        {
            StartCoroutine(LoadMainMenuCo());
        }

        IEnumerator LoadMainMenuCo()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeToBlack(fadeToBlackSeconds);
            yield return SceneManager.LoadSceneAsync(menuSceneIndexToLoad);
            yield return fader.FadeToClear(fadeToClearSeconds);
            
        }

        private void SetCurrentSave(string saveFile)
        { 
            PlayerPrefs.SetString(currentSaveKey, saveFile);
        }

        private string GetCurrentSave()
        {
            return PlayerPrefs.GetString(currentSaveKey);
        }

        IEnumerator LoadFirstScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeToBlack(fadeToBlackSeconds);
            yield return SceneManager.LoadSceneAsync(firstSceneIndexToLoad, LoadSceneMode.Single);
            yield return fader.FadeToClear(fadeToClearSeconds);
        }

        IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeToBlack(fadeToBlackSeconds);
            //fader.FadeToBlackImmediate();

            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());
            yield return fader.FadeToClear(fadeToClearSeconds);
        }

        

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
            
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave());
            
        }

        private void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }

    }

}