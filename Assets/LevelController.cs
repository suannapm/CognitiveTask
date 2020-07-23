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

  [System.Serializable]
  public class HeartsAndFlowersData //the data we want to save 
  {
    public float H_RT = 0.0F; //mean reaction time response for heartsOnly block
    public float F_RT = 0.0F;
    public float M_RT = 0.0F;
    public float H_ACC = 0.0F; //accuracy for heartsOnly block
    public float F_ACC = 0.0F;
    public float M_ACC= 0.0F;
    public float H_TotalCorrect = 0.0F;
    public float F_TotalCorrect = 0.0F;
    public float M_TotalCorrect = 0.0F;
    public float M_H_TotalCorrect = 0.0F; //how many heart test trials were correct in mixed block
    public float M_F_TotalCorrect = 0.0F; //how many flower test trials were correct in mixed block
    public int H_TotalErrors = 0;
    public int F_TotalErrors = 0;
    public int M_TotalErrors = 0;
    public float M_H_ACC = 0.0F; //accuracy in the mixed block for the heart trials
    public float M_F_ACC = 0.0F; //accuracy in the mixed block for the flower trials
    public float M_H_TrialsTotal = 0.0F; //number of heart trials in mixed block
    public float M_F_TrialsTotal = 0.0F; //number of flower trials in mixed block
  }

  public HeartsAndFlowersData dataToSave; //variable that we will manipulate to save

  float correctAnswersHearts = 0.0F; //to keep track on if the answers were correct or not for only the actual trials
  float correctAnswersFlowers = 0.0F;
  float correctAnswersMixed = 0.0F;
  float RTHearts = 0.0F; //add all the RTs together to divide at the end of the game
  float RTFlowers = 0.0F;
  float RTMixed = 0.0F;
  int heartsMissed = 0; //counter for errors
  int flowersMissed = 0;
  int mixedMissed = 0;
  float correctAnswersMixed_H = 0.0F; //to keep track of the heart trials in mixed block
  float correctAnswersMixed_F = 0.0F; //to keep track of the flower trials in mixed block
  int mixedMissed_H = 0; //track of incorrect heart trials in mixed block
  int mixedMissed_F = 0; //track of incorrect flower trials in mixed block
  float mixed_H_TrialsTotal = 0.0F;
  float mixed_F_TrialsTotal = 0.0F;

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
      //update correct answers trackers
      if(practiceMode == false && answerArray == heartsOnly) //in test heart trials
      {
        correctAnswersHearts = correctAnswersHearts + 1;
      }
      if(practiceMode == false && answerArray == flowersOnly)
      {
        correctAnswersFlowers = correctAnswersFlowers + 1;
      }
      if(practiceMode == false && answerArray == mixed)
      {
        correctAnswersMixed = correctAnswersMixed + 1;
        if(answerArray[answersIndex].correctStimulus == stimulus.HEARTS)
        {
          correctAnswersMixed_H = correctAnswersMixed_H + 1;
          mixed_H_TrialsTotal = mixed_H_TrialsTotal + 1;
        }
        else
        {
          correctAnswersMixed_F = correctAnswersMixed_F + 1;
          mixed_F_TrialsTotal = mixed_F_TrialsTotal + 1;
        } //correctStimulus == FLOWERS
      }
    }
    else //incorrect
    {
      Debug.Log("wrong answer");
      if(practiceMode == false && answerArray == heartsOnly){heartsMissed = heartsMissed + 1;}
      if(practiceMode == false && answerArray == flowersOnly){flowersMissed = flowersMissed + 1;}
      if(practiceMode == false && answerArray == mixed)
      {
        //still update how many heart and flower trials are in the mixed block
        if(answerArray[answersIndex].correctStimulus == stimulus.HEARTS)
        {
          mixed_H_TrialsTotal = mixed_H_TrialsTotal + 1;
          mixedMissed_H = mixedMissed_H + 1;
        }
        else //correctStimulus == FLOWERS
        {
          mixed_F_TrialsTotal = mixed_F_TrialsTotal + 1;
          mixedMissed_F = mixedMissed_F + 1;
        }
        mixedMissed = mixedMissed + 1;
      }
      //if in practice mode we want to give feedback
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
          Debug.Log("rightkey was pressed");
          if(currentTrial == 1) //test heart trials
          {
            RTHearts = RTHearts + (waitTimeBetweenTrials - timer);
            //keep track of all the RTs
          }
          if(currentTrial == 3) //test flower trials
          {
            RTFlowers = RTFlowers + (waitTimeBetweenTrials - timer);
          }
          if(currentTrial == 5) //test mixed trials
          {
            RTMixed = RTMixed + (waitTimeBetweenTrials - timer);
          }
          parseInput(direction.RIGHT);
          yield break;
        }
        if(Input.GetKeyDown(leftKey))
        {
          Debug.Log("leftkey was pressed");
          if(currentTrial == 1) //test heart trials
          {
            RTHearts = RTHearts + (waitTimeBetweenTrials - timer);
            //keep track of all the RTs
          }
          if(currentTrial == 3) //test flower trials
          {
            RTFlowers = RTFlowers + (waitTimeBetweenTrials - timer);
          }
          if(currentTrial == 5) //test mixed trials
          {
            RTMixed = RTMixed + (waitTimeBetweenTrials - timer);
          }
          parseInput(direction.LEFT);
          yield break;
        }
      timer -= Time.deltaTime;
      yield return null;
    }
    //keep track of how many were missed
    if(timer <= 0 && practiceMode == false)
    {
      if(currentTrial == 1){heartsMissed = heartsMissed + 1;}
      if(currentTrial == 3){flowersMissed = flowersMissed + 1;}
      if(currentTrial == 5)
      {
        mixedMissed = mixedMissed + 1;
        if(mixed[answersIndex].correctStimulus == stimulus.HEARTS){mixedMissed_H = mixedMissed_H + 1;}
        else{mixedMissed_F = mixedMissed_F + 1;} //correctStimulus == FLOWERS
      }
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
    crossHair.SetActive(false);
    yield return new WaitForSeconds(crossHairWaitTime); 
    //assuming you want the same time for a blank screen as the crosshair
    StartCoroutine("DoTrial");
  }

  // Moves on to the next trial
  void MoveOn()
  {
    // display the next trial in the series
    // if we are at the end of a trial, do something special
    if(endOfTrial(currentTrial, answersIndex))
    {
      if(currentTrial == 5) //
      {
        //end of game
        //calculate correct accuracy
        dataToSave.H_ACC = correctAnswersHearts/heartsOnly.Length;
        dataToSave.F_ACC = correctAnswersFlowers/flowersOnly.Length;
        dataToSave.M_ACC = correctAnswersMixed/mixed.Length;
        dataToSave.M_H_ACC = correctAnswersMixed_H/mixed_H_TrialsTotal;
        dataToSave.M_F_ACC = correctAnswersMixed_F/mixed_F_TrialsTotal;

        //update TrialsTotal for mixed block
        dataToSave.M_H_TrialsTotal = mixed_H_TrialsTotal;
        dataToSave.M_F_TrialsTotal = mixed_F_TrialsTotal;

        //caluclate mean RTs
        dataToSave.H_RT = RTHearts / (heartsOnly.Length - heartsMissed);
        dataToSave.F_RT = RTFlowers / (flowersOnly.Length - flowersMissed);
        dataToSave.M_RT = RTMixed / (mixed.Length - mixedMissed);

        //how many were correct in each block
        dataToSave.H_TotalCorrect = correctAnswersHearts;
        dataToSave.F_TotalCorrect = correctAnswersFlowers;
        dataToSave.M_TotalCorrect = correctAnswersMixed;
        dataToSave.M_H_TotalCorrect = correctAnswersMixed_H;
        dataToSave.M_F_TotalCorrect = correctAnswersMixed_F;

        //Errors in each block
        dataToSave.H_TotalErrors = heartsMissed;
        dataToSave.F_TotalErrors = flowersMissed;
        dataToSave.M_TotalErrors = mixedMissed;

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
