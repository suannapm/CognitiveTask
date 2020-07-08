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
  public trialAnswers[] heartsOnly; 
  public trialAnswers[] flowersOnly;
  public trialAnswers[] mixed;
  int answersIndex = 0; //current index in trialAnswers
  int currentTrial = 0; //which set of trialAnswers we are on (ex: heartsOnly)

  void parseInput(direction answer) //function tells us what to do with the player's answers
  {
    trialAnswers[] answerArray = heartsOnly;
    switch(currentTrial) //what to do depending on which trial we are in
    {
      case 0:
        answerArray = heartsOnly;
        practiceMode = true;
        break;
      case 1:
        answerArray = flowersOnly;
        practiceMode = true;
        break;
      case 2:
        answerArray = mixed;
        practiceMode = false;
        break;
    }
    if(answer == answerArray[answersIndex].correctDirection) //if we have the correct answer
    {
      Debug.Log("correct answer");
    }
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
