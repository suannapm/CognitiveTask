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

  //helper function that tells us if we are at the end of the trial or not
  bool endOfTrial(int trial, int index)
  {
    bool ans = false; //default value

    // NOTE: don't hardcode the length of each trial. Instead, you can access
    // the length of each array using the Length property. For example:
    // if (index == flowersOnly.Length)
    if(trial == 0 || trial == 2 || trial == 4)
    {
      if(index == 3){ans = true;}
    }
    if(trial == 1 || trial == 3)
    {
      if(index == 11){ans = true;}
    }
    if(trial == 5)
    {
      if(index == 32){ans = true;}
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

    // display the next trial in the series
    // if we are at the end of a trail, do something special
    if(endOfTrial(currentTrial, answersIndex))
    {
      if(currentTrial == 5) //
      {
        //end of game
      }
      else
      {
        currentTrial = currentTrial + 1; //updates to next trial 
        answersIndex = -1; //so when we add 1 it goes to zero
      }
    }
    answersIndex = answersIndex + 1;
    coroutineVars temp;
    temp.trial = currentTrial;
    temp.index = answersIndex;
    StartCoroutine("trialTiming", temp);
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

  public struct coroutineVars
  {
    public int trial;
    public int index;
  }

  //helper function for if there is no input (right or left arrow keys)
  void MoveOn()
  {
    // display the next trial in the series
    // if we are at the end of a trail, do something special
    if(endOfTrial(currentTrial, answersIndex))
    {
      if(currentTrial == 5) //
      {
        //end of game
      }
      else
      {
        currentTrial = currentTrial + 1; //updates to next trial 
        answersIndex = -1; //so when we add 1 it goes to zero
      }
    }
    answersIndex = answersIndex + 1;
    coroutineVars temp;
    temp.trial = currentTrial;
    temp.index = answersIndex;
    StartCoroutine("trialTiming", temp);
  }

  //coroutine used for timing in trials
  IEnumerator trialTiming(coroutineVars vars)
  {
    crossHair.SetActive(true);
    heartRight.SetActive(false);
    heartLeft.SetActive(false);
    flowerRight.SetActive(false);
    flowerLeft.SetActive(false);
  
    yield return new WaitForSeconds(.5f); //in seconds
    DisplayTrial(vars.trial, vars.index);
    yield return new WaitForSeconds(1.5f); //in seconds
    MoveOn();
    yield return null;
    

  }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(rightKey))
        {
          parseInput(direction.RIGHT);
          Debug.Log("rightkey was pressed");
        }
        if(Input.GetKeyDown(leftKey))
        {
          parseInput(direction.LEFT);
        }
    }
}
