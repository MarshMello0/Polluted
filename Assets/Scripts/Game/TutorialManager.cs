using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject nextButton;

    [SerializeField] private RectTransform textTransform, backPanelTransform;
    
    private List<string> tutorialsTexts = new List<string>();
    private int currentTutorial = 0;
    private int tutorialCount;

    [SerializeField] private GameObject tutorialParent;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private DisplayCityName displayCityName;
    [Header("Typing Delays")] 
    [SerializeField] private float fullStop;
    [SerializeField] private float comma;
    [SerializeField] private float letters;

    private bool forwardPressed, leftPressed, backPressed, rightPressed, jumpPressed, sprintPressed, interactPressed;
    private bool readyForInteract;

    private void Awake()
    {
        string cityName = GameObject.FindWithTag("City").name;
        
        tutorialsTexts.Add(
            string.Format(
                "Welcome to Polluted. The earth has been evacuated in a rush after the amount of pollution caused by humans has reached a dangerous level, however, you were left behind. You have woken up in your local city called {0}, now your goal is to try and survive as long as you can by scavaging of what people left behind.",
                cityName));
        
        tutorialsTexts.Add(
            string.Format("To walk around the city use, {0} {1} {2} {3}.",
                playerController.kForward.ToString(),
                playerController.kLeft.ToString(),
                playerController.kBackward.ToString(),
                playerController.kRight.ToString()));
        
        tutorialsTexts.Add(
            string.Format("Press {0} to jump and {1} to sprint.",
                playerController.kJump.ToString(),
                playerController.kSprint.ToString()));
        tutorialsTexts.Add(
            string.Format("Use {0} to pick up items and place them in your inventory, once they are in your inventory you can click on them to see information about that item.",
                KeyCode.E.ToString()));
        
        tutorialsTexts.Add("Well done, you completed this complex tutorial.");

        tutorialCount = tutorialsTexts.Count - 1;
    }

    private void Start()
    {
        StartCoroutine(DispalyText(tutorialsTexts[0]));
        SetMovement(false);
        SetJumping(false);
    }

    private void LateUpdate()
    {
        MatchSizes();
        CheckConditions();
        
        if (Input.GetKeyDown(KeyCode.Return) && nextButton.activeInHierarchy)
            Next();
        
        if (Input.GetKeyDown(KeyCode.O))
            Skip();
    }

    private void SetMovement(bool state)
    {
        playerController.canMove = state;
    }

    private void SetJumping(bool state)
    {
        playerController.canJump = state;
    }

    private bool CanMove()
    {
        return playerController.canMove;
    }

    private bool CanJump()
    {
        return playerController.canJump;
    }

    private void CheckConditions()
    {
        switch (currentTutorial)
        {
            case 0:
                if (!nextButton.activeInHierarchy)
                    nextButton.SetActive(true);
                break;
            case  1:
                if (Input.GetKeyDown(playerController.kForward) && !forwardPressed && CanMove())
                {
                    string[] keyArray = {playerController.kForward.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + playerController.kForward.ToString()[0] +
                                     "</color>";
                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    text.text = newText;
                    forwardPressed = true;
                }
                else if (Input.GetKeyDown(playerController.kLeft) && !leftPressed && CanMove())
                {
                    string[] keyArray = {playerController.kLeft.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + playerController.kLeft.ToString()[0] +
                                     "</color>";
                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    text.text = newText;
                    leftPressed = true;
                }
                else if (Input.GetKeyDown(playerController.kBackward) && !backPressed && CanMove())
                {
                    string[] keyArray = {playerController.kBackward.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + playerController.kBackward.ToString()[0] +
                                     "</color>";
                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    text.text = newText;
                    backPressed = true;
                } 
                else if (Input.GetKeyDown(playerController.kRight) && !rightPressed && CanMove())
                {
                    string[] keyArray = {playerController.kRight.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + playerController.kRight.ToString()[0] +
                                     "</color>";
                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    text.text = newText;
                    rightPressed = true;
                }

                if (forwardPressed && leftPressed && backPressed && rightPressed && !nextButton.activeInHierarchy)
                {
                    nextButton.SetActive(true);
                }
                break;
            case 2:
                if (Input.GetKeyDown(playerController.kJump) && !jumpPressed && CanJump())
                {
                    string[] keyArray = {playerController.kJump.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + playerController.kJump +
                                     "</color>";

                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    
                    text.text = newText;
                    jumpPressed = true;
                }
                else if (Input.GetKeyDown(playerController.kSprint) && !sprintPressed && CanJump())
                {
                    string[] keyArray = {playerController.kSprint.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + playerController.kSprint +
                                     "</color>";
                    
                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    text.text = newText;
                    sprintPressed = true;
                }

                if (jumpPressed && sprintPressed && !nextButton.activeInHierarchy)
                {
                    nextButton.SetActive(true);
                }
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.E) && !interactPressed && readyForInteract)
                {
                    string[] keyArray = {KeyCode.E.ToString()};
                    string[] split = text.text.Split(keyArray, StringSplitOptions.None);
                    string newText = split[0] + "<color=#008000ff>" + KeyCode.E +
                                     "</color>";
                    
                    for (int i = 1; i < split.Length; i++)
                    {
                        newText += split[i];
                    }
                    text.text = newText;
                    interactPressed = true;
                }
                if (interactPressed && !nextButton.activeInHierarchy)
                {
                    nextButton.SetActive(true);
                }
                break;
            case 4:
                if (!nextButton.activeInHierarchy)
                    nextButton.SetActive(true);
                break;
        }
    }

    private void MatchSizes()
    {
        backPanelTransform.sizeDelta = textTransform.sizeDelta;
    }

    public void Next()
    {
        StopAllCoroutines();
        currentTutorial++;
        if (currentTutorial > tutorialCount)
        {
            tutorialParent.SetActive(false);
            pauseMenuManager.finishedTutorial = true;
            displayCityName.DisplayName();
            enabled = false;
        }
        else
        {
            StartCoroutine(DispalyText(tutorialsTexts[currentTutorial]));   
        }
    }

    public void Skip()
    {
        tutorialParent.SetActive(false);
        SetJumping(true);
        SetMovement(true);
        pauseMenuManager.finishedTutorial = true;
        displayCityName.DisplayName();
        enabled = false;
    }

    private IEnumerator DispalyText(string text)
    {
        nextButton.SetActive(false);
        this.text.text = "";
        foreach (char letter in text)
        {
            while (pauseMenuManager.isPaused)
            {
                yield return null;
            }
            this.text.text += letter;

            switch (letter)
            {
                case '.':
                    yield return new WaitForSecondsRealtime(fullStop);
                    break;
                case ',':
                    yield return new WaitForSecondsRealtime(comma);
                    break;
                default:
                    yield return new WaitForSecondsRealtime(letters);
                    break;
                
            }
        }

        if (currentTutorial == 1)
        {
            SetMovement(true);
        }

        if (currentTutorial == 2)
        {
            SetJumping(true);
        }

        if (currentTutorial == 3)
        {
            readyForInteract = true;
        }
    }
}
