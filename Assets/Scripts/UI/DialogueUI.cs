using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI conversantName;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] TextMeshProUGUI simpleQuestionText;
        [SerializeField] Button nextButton;
        [SerializeField] Transform choiceRoot;
        [SerializeField] Transform choiceUIRoot;
        [SerializeField] GameObject choiceButtonPrefab;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Button quitButton;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(() => playerConversant.Next()); //lambda one liners don't require the {}
            quitButton.onClick.AddListener(() => playerConversant.Quit());
            UpdateUI();
        }

        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsDialogueActive());
            if (!playerConversant.IsDialogueActive())
            {
                return;
            }
            conversantName.text = playerConversant.GetCurrentConversantName();
            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceUIRoot.gameObject.SetActive(playerConversant.IsChoosing());
            if (playerConversant.IsChoosing())
            {
                UpdateSimplifiedQuestion();
                UpdateChoiceButtons();
            }
            else
            {
                AIText.text = playerConversant.GetText();           
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void UpdateChoiceButtons()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }
            foreach (DialogueNode choiceNode in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choiceButtonPrefab, choiceRoot);
                TextMeshProUGUI textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choiceNode.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choiceNode);
                });
            }
        }

        private void UpdateSimplifiedQuestion()
        {
            simpleQuestionText.text = playerConversant.GetSimplifiedQuestion();
        }

    }

}