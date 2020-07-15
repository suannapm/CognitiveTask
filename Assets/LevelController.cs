using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : UnitySingleton<LevelController>
{
  public KeyCode leftKey; 
  public KeyCode rightKey;
  bool practiceMode = true; //when we are in the hearts and flowers only trials

  //enum is a type of class that represents constants
  public enum direction //right and left arrow keys
  {
    RIGHT,
    LEFT
  };
  public enum stimulus
  {
    HEARTS,
    FLOWERS
  };


  [System.Serializable]
  public class trialAnswers //the correct answers to the questions asked 
  {
    public direction correctDirection;
    public stimulus correctStimulus;
  }

  //create different arrays for the correct answers
  public trialAnswers[] heartsOnlyPractice; //len = 4, trial 0
  public trialAnswers[] flowersOnlyPractice; //len = 4, trial 2
  public trialAnswers[] mixedPractice; //len = 4, trial 4
  public trialAnswers[] heartsOnly; //len = 12, trial 1
  public trialAnswers[] flowersOnly; //len = 12, trial 3
  public trialAnswers[] mixed; //len = 33, trial 5

  int answersIndex = 0; //current index in trialAnswers
  int currentTrial = 0; //which set of trialAnswers we are on (ex: heartsOnly)

  public GameObject flowerLeft;
  public GameObject heartLeft;
  public GameObject heartRight;
  public GameObject flowerRight;
  public GameObject crossHair;
  public float waitTimeBetweenTrials;
  public float crossHairWaitTime;

  //helper function that tells us if we are at the end of the trial or not
  bool endOfTrial(int trial, int index)
  {
    bool ans = false; //default value

    if(trial == 0 || trial == 2 || trial == 4) // practice trials
    // Assuming that all of the practice trials are the same length
    {
      if(index == heartsOnlyPractice.Length - 1){ans = true;}
    }
    // Assuming that heart and flower trials are the same length
    if(trial == 1 || trial == 3) // hearts only, flowers only
    {
      if(index == heartsOnly.Length - 1){ans = true;}
    }
    if(trial == 5)
    {
      if(index == mixed.Length - 1){ans = true;}
    }
    return ans;
  }

  void parseInput(direction answer) //function tells us what to do with the player's answers
  {
    trialAnswers[] answerArray = heartsOnly;
    switch(currentTrial) //what to do depending on which trial we are in
    {
      case 0:
        answerArray = heartsOnlyPractice;
        practiceMode = true;
        break;
      case 1:
        answerArray = heartsOnly;
        practiceMode = false;
        break;
      case 2:
        answerArray = flowersOnlyPractice;
        practiceMode = true;
        break;
      case 3:
        answerArray = flowersOnly;
        practiceMode = false;
        break;
      case 4:
        answerArray = mixedPractice;
        practiceMode = true;
        break;
      case 5:
        answerArray = mixed;
        practiceMode = false;
        break;
    }
    if(answer == answerArray[answersIndex].correctDirection) //if we have the correct answer
    {
      Debug.Log("correct answer");
    }
    else //incorrect
    {
      Debug.Log("wrong answer");
    }

    MoveOn();
  }

  IEnumerator DoTrial()
  {
    Debug.Log("Answers index: " + answersIndex);
    Debug.Log("Current trial: " + currentTrial);
    DisplayTrial(currentTrial, answersIndex);

    // listen for user input
    float timer = waitTimeBetweenTrials;
    while (timer > 0)
    {
      if(Input.GetKeyDown(rightKey))
        {
          parseInput(direction.RIGHT);
          Debug.Log("rightkey was pressed");
          MoveOn();
          yield break;
        }
        if(Input.GetKeyDown(leftKey))
        {
          parseInput(direction.LEFT);
          MoveOn();
          yield break;
        }
      timer -= Time.deltaTime;
      yield return null;
    }

    // Move on if the user does not press anything
    MoveOn();
  yield return null;

  }

  IEnumerator WaitInBetweenTrials()
  {
    crossHair.SetActive(true);
    heartRight.SetActive(false);
    heartLeft.SetActive(false);
    flowerRight.SetActive(false);
    flowerLeft.SetActive(false);
    yield return new WaitForSeconds(crossHairWaitTime);
    StartCoroutine("DoTrial");
  }

  // Moves on to the next trial
  void MoveOn()
  {
    // display the next trial in the series
    // if we are at the end of a trail, do something special
    if(endOfTrial(currentTrial, answersIndex))
    {
      if(currentTrial == 5) //
      {
        //end of game
        Debug.Log("End of game :)");
        return;
      }
      else
      {
        currentTrial = currentTrial + 1; //updates to next trial 
        answersIndex = -1; //so when we add 1 it goes to zero
      }
    }
    answersIndex = answersIndex + 1;
    StartCoroutine("WaitInBetweenTrials");
  }

  void DisplayTrial(int trial, int index)
  {
    if(trial == 0 || trial == 1) //heartsOnly
    {
      if(heartsOnly[index].correctDirection == direction.RIGHT)
      {
        heartRight.SetActive(true);
        heartLeft.SetActive(false);
        flowerRight.SetActive(false);
        flowerLeft.SetActive(false);
      }
      else //correctDirection == LEFT
      {
        heartRight.SetActive(false);
        heartLeft.SetActive(true);
        flowerRight.SetActive(false);
        flowerLeft.SetActive(false);
      }
    }

    if(trial == 2 || trial == 3) //flowersOnly
    {
      if(flowersOnly[index].correctDirection == direction.RIGHT)
      {
        heartRight.SetActive(false);
        heartLeft.SetActive(false);
        flowerRight.SetActive(false);
        flowerLeft.SetActive(true);
      }
      else //correctDirection == LEFT
      {
        heartRight.SetActive(false);
        heartLeft.SetActive(false);
        flowerRight.SetActive(true);
        flowerLeft.SetActive(false);
      }
    }

    if(trial == 4 || trial == 5) //mixed
    {
      Debug.Log(index);
      if(mixed[index].correctStimulus == stimulus.HEARTS)
      {
        if(mixed[index].correctDirection == direction.RIGHT)
        {
          heartRight.SetActive(true);
          heartLeft.SetActive(false);
          flowerRight.SetActive(false);
          flowerLeft.SetActive(false);
        }
        else //correctDirection == LEFT
        {
          heartRight.SetActive(false);
          heartLeft.SetActive(true);
          flowerRight.SetActive(false);
          flowerLeft.SetActive(false);
        }
      }
      else //correctStimulus == FLOWERS
      {
        if(mixed[index].correctDirection == direction.RIGHT)
        {
          heartRight.SetActive(false);
          heartLeft.SetActive(false);
          flowerRight.SetActive(false);
          flowerLeft.SetActive(true);
        }
        else //correctDirection == LEFT
        {
          heartRight.SetActive(false);
          heartLeft.SetActive(false);
          flowerRight.SetActive(true);
          flowerLeft.SetActive(false);
        }
      }
    }
  }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DoTrial");
    }
}
