using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] GameObject loadButton;
        [SerializeField] Transform scrollViewContent;


        void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper == null) return;
            
            foreach(Transform child in scrollViewContent)
            {
                Destroy(child.gameObject);
            }
            foreach(string save in savingWrapper.ListSaves())
            {
                GameObject buttonInstance = Instantiate(loadButton, scrollViewContent);
                TMP_Text textObject = buttonInstance.GetComponentInChildren<TMP_Text>();
                textObject.text = save;
                Button button = buttonInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                   savingWrapper.LoadGame(save);
                });
            }
        }
    }

}