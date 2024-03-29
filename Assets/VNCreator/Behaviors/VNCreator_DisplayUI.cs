﻿using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VNCreator
{
    public class VNCreator_DisplayUI : DisplayBase
    {
        [Header("Text")]
        public Text characterNameTxt;
        public Text dialogueTxt;
        [Header("Visuals")]
        public Image characterImg;
        public Image backgroundImg;
        [Header("Audio")]
        public AudioSource musicSource;
        public AudioSource soundEffectSource;
        [Header("Buttons")]
        public Button nextBtn;
        public Button previousBtn;
        public Button saveBtn;
        public Button menuButton;
        [Header("Choices")]
        public Button choiceBtn1;
        public Button choiceBtn2;
        public Button choiceBtn3;
        [Header("End")]
        public GameObject endScreen;
        [Header("Main menu")]
        [Scene]
        public string mainMenu;

        void Start()
        {
            nextBtn.onClick.AddListener(delegate { NextNode(0); });
            if(previousBtn != null)
                previousBtn.onClick.AddListener(Previous);
            if(saveBtn != null)
                saveBtn.onClick.AddListener(Save);
            if (menuButton != null)
                menuButton.onClick.AddListener(ExitGame);

            if(choiceBtn1 != null)
                choiceBtn1.onClick.AddListener(delegate { NextNode(0); });
            if(choiceBtn2 != null)
                choiceBtn2.onClick.AddListener(delegate { NextNode(1); });
            if(choiceBtn3 != null)
                choiceBtn3.onClick.AddListener(delegate { NextNode(2); });

            endScreen.SetActive(false);

            StartCoroutine(DisplayCurrentNode());
        }

        protected override void NextNode(int _choiceId)
        {
            if (lastNode)
            {
                endScreen.SetActive(true);
                return;
            }

            base.NextNode(_choiceId);
            StartCoroutine(DisplayCurrentNode());
        }

        IEnumerator DisplayCurrentNode()
        {
            characterNameTxt.text = CurrentDialogueNode.CharacterName;
            if (CurrentDialogueNode.CharacterSprList != null)
            {
                //characterImg.sprite = currentNode.CharacterSpr.LoadAsync().ToCoroutine();
                characterImg.color = Color.white;
            }
            else
            {
                characterImg.color = new Color(1, 1, 1, 0);
            }

            if (CurrentDialogueNode.BackgroundSprList != null)
            {
                //Добавить отображение случайного спрайта
                backgroundImg.sprite = null;
            }

            if (CurrentDialogueNode.Choices <= 1) 
            {
                nextBtn.gameObject.SetActive(true);

                choiceBtn1.gameObject.SetActive(false);
                choiceBtn2.gameObject.SetActive(false);
                choiceBtn3.gameObject.SetActive(false);

                previousBtn.gameObject.SetActive(loadList.Count != 1);
            }
            else
            {
                nextBtn.gameObject.SetActive(false);

                choiceBtn1.gameObject.SetActive(true);
                choiceBtn1.transform.GetChild(0).GetComponent<Text>().text = CurrentDialogueNode.ChoiceOptions[0];

                choiceBtn2.gameObject.SetActive(true);
                choiceBtn2.transform.GetChild(0).GetComponent<Text>().text = CurrentDialogueNode.ChoiceOptions[1];

                if (CurrentDialogueNode.Choices == 3)
                {
                    choiceBtn3.gameObject.SetActive(true);
                    choiceBtn3.transform.GetChild(0).GetComponent<Text>().text = CurrentDialogueNode.ChoiceOptions[2];
                }
                else
                {
                    choiceBtn3.gameObject.SetActive(false);
                }
            }

            if (CurrentDialogueNode.BackgroundMusic != null)
                VNCreator_MusicSource.instance.Play(CurrentDialogueNode.BackgroundMusic);
            if (CurrentDialogueNode.SoundEffect != null)
                VNCreator_SfxSource.instance.Play(CurrentDialogueNode.SoundEffect);

            dialogueTxt.text = string.Empty;
            if (GameOptions.isInstantText)
            {
                dialogueTxt.text = CurrentDialogueNode.DialogueText;
            }
            else
            {
                char[] _chars = CurrentDialogueNode.DialogueText.ToCharArray();
                string fullString = string.Empty;
                for (int i = 0; i < _chars.Length; i++)
                {
                    fullString += _chars[i];
                    dialogueTxt.text = fullString;
                    yield return new WaitForSeconds(0.01f/ GameOptions.readSpeed);
                }
            }
        }

        protected override void Previous()
        {
            base.Previous();
            StartCoroutine(DisplayCurrentNode());
        }

        void ExitGame()
        {
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }
    }
}