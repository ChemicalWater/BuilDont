using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// <para>Script that handles the goals of the player</para>
/// <para>keeps track of the current goal and subgoals, keeps track of progress of these goals,</para>
/// <para>shows instructions to the player on screen, handles losing and winning the game.</para>
/// </summary>
public class GameManager : MonoBehaviour
{
    private static Queue<string> PendingFadingText = new Queue<string>(); //queue of text that will be displayed on screen
    private float timeToDisplayFadingText = 4f;//time to show the text before it fades

    [SerializeField] GameObject GoalProgress; //filling progress bar
    [SerializeField] private FadeTextOnScreen generalInformationOnScreen;//the fading text script
    [SerializeField] private TextMeshProUGUI mainGoalText;//text of the main goal in the HUD
    [SerializeField] private GameObject subGoalTextPrefab;//prefab of the subgoal text
    [SerializeField] private RectTransform subGoalGridLayoutGroup;//layoutgroup where the subgoaltextprefab instances are ordered
    [SerializeField] private TextMeshProUGUI subGoalProgressText;//progress text in the progress bar
    [SerializeField] private Texture completedCheckmark;//the image of the completed checkmark
    [SerializeField] private AudioSource source;//the source for feedback sounds the HUD makes
    [SerializeField] private AudioClip winJingle;//jingle for when the player wins the game
    [SerializeField] private AudioClip subgoalCompleteSound;//sound for when the player completes a goal
    [SerializeField] private AudioClip loseJingle;//jingle for when the player loses the game
    [SerializeField] private RectTransform missionsHUDBox;//scalable container for the on screen goals

    [SerializeField] private MissionObjectiveScript ObjectiveScript;//script that controls the objective pop-up

    //the subgoals the player gets in the first area
    private static Goal[] subGoalsOfMainGoal1 = new Goal[]{new Goal(GoalType.Building, "Destroy 15 buildings", 15),
                                                           new Goal(GoalType.BuildingGold, "Turn a building into gold", 1),
                                                           new Goal(GoalType.CarRubber, "Hit a car with the rubber hammer", 1),
                                                           new Goal(GoalType.CarSmashBuild1Time, "Destroy 3 buildings with cars", 3),
                                                           new Goal(GoalType.Bomb, "Use a bomb on a fence", 1) };

    //the subgoals the player gets in the second area
    private static Goal[] subGoalsOfMainGoal2 = new Goal[]{new Goal(GoalType.PoliceCar, "Destroy 3 police cars", 3),
                                                           new Goal(GoalType.CarSmashBuild5Times, "Destroy 5 buildings with a single car", 1),
                                                           new Goal(GoalType.PoliceBuilding, "Destroy a police station", 1),
                                                           new Goal(GoalType.Ladder, "Find a ladder", 1),
                                                           new Goal(GoalType.UseLadder, "Use the ladder to climb the dam", 1)};

    //the subgoals the player gets in the third area
    private static Goal[] subGoalsOfmainGoal3 = new Goal[]{new Goal(GoalType.FindNPC, "Find a way onto the bridge", 1),
                                                           new Goal(GoalType.Bank, "Withdraw some money at the bank", 1),
                                                           new Goal(GoalType.PayNPC, "Get onto the dock", 1)};

    //the main goals the player gets
    private Goal[] mainGoals = new Goal[] { new Goal(GoalType.Powerstation, "Destroy the powerstation", 1, subGoalsOfMainGoal1, 300), 
                                            new Goal(GoalType.Dam, "Destroy the dam", 1, subGoalsOfMainGoal2, 400), 
                                            new Goal(GoalType.Bridge, "Destroy the Bridge", 1, subGoalsOfmainGoal3, 400)};
    private int currentMainGoal = -1;//index variable for the mainGoals array

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        GoalProgress.GetComponent<Image>().fillAmount = 0;//progress is 0
    }

    // Update is called once per frame
    void Update()
    {
        HandlePendingFadingText();//if text needs to be displayed handle that
    }

    private void HandlePendingFadingText()
    {
        //if there are items in the queue
        if (PendingFadingText.Count > 0)
        {
            //and the fading text is ready to display new text
            if (generalInformationOnScreen.IsReadyForNewText())
            {
                generalInformationOnScreen.DisplayText(PendingFadingText.Dequeue(), timeToDisplayFadingText);//display the text and dequeue the item
            }
        }
    }

    //method for showing text on screen from any other script without a reference to gamemanager
    public static void ShowFadingText(string text)
    {
        PendingFadingText.Enqueue(text);
    }

    public void StartFirstMission()
    {
        ObjectiveScript.DisplayNewMission(mainGoals[currentMainGoal + 1]);//show the objective pop-up for the first mission
    }

    //method called when the mission timer runs out
    public void LoseGame()
    {
        StartCoroutine(LoseSequence());
    }

    //coroutine that plays the lose jingle then goes to the lose scene
    private IEnumerator LoseSequence()
    {
        //play lose jingle
        source.clip = loseJingle;
        source.Play();

        yield return new WaitForSeconds(loseJingle.length);
        //go to lose scene
        SceneManager.LoadScene("LoseScene");
    }

    //method called when the player completes the final main goal
    public void WinGame()
    {
        StartCoroutine(WinSequence());
    }

    //coroutine that plays the win jinle then goes to the win scene
    private IEnumerator WinSequence()
    {
        //play win jingle
        source.clip = winJingle;
        source.Play();

        yield return new WaitForSeconds(winJingle.length);
        //go to win scene
        SceneManager.LoadScene("WinScene");
    }

    //method used to add progress to the player goals when events occur
    //takes a goalType, then checks if the player has a goal with that type, then adds progress and handles completing the goal
    public void AddGoalProgressByType(GoalType type)
    {
        //if the main goal of the player matches the goalType  and   if the goal is completed after the progress is added
        if (mainGoals[currentMainGoal].GetGoalType() == type && mainGoals[currentMainGoal].AddProgressIsCompleted(1))
        {
            ObjectiveScript.DisplayNewMission(mainGoals[currentMainGoal + 1]);//show the pop-up for the next goal
            missionsHUDBox.gameObject.SetActive(false);//don't show the list of goals as the player reads the pop-up
        }

        //if the sub goal of the player matches the goalType   and   if the goal is completed after the progress is added
        if (mainGoals[currentMainGoal].GetCurrentSubGoal().GetGoalType() == type && mainGoals[currentMainGoal].GetCurrentSubGoal().AddProgressIsCompleted(1))
        {
            subGoalGridLayoutGroup.GetChild(mainGoals[currentMainGoal].currentSubGoal).GetChild(0).GetComponent<RawImage>().texture = completedCheckmark;

            //feedback sound when subgoal is completed;
            source.clip = subgoalCompleteSound;
            source.Play();
            //if there is a next subgoal
            if (mainGoals[currentMainGoal].currentSubGoal + 1 < mainGoals[currentMainGoal].GetSubGoals().Length)
            {
                NextSubGoal();//go to the next subgoal
            }
        }
        //update the progress text an bar
        if (mainGoals[currentMainGoal].GetCurrentSubGoal().GetProgress() <= mainGoals[currentMainGoal].GetCurrentSubGoal().GetAmount())
        {
            subGoalProgressText.text = mainGoals[currentMainGoal].GetCurrentSubGoal().GetProgressText();
            GoalProgress.GetComponent<Image>().fillAmount = (mainGoals[currentMainGoal].GetCurrentSubGoal().GetProgress() + 0.000001f) / mainGoals[currentMainGoal].GetCurrentSubGoal().GetAmount();
        }
    }

    //updates the main goal and applies the right subgoals
    public void NextMainGoal()
    {
        missionsHUDBox.gameObject.SetActive(true);//goal checklist is visible again
        currentMainGoal++;//up the index
        mainGoalText.text = mainGoals[currentMainGoal].GetText();//set the text to the new main goal text

        //delete all children of the layoutgroup that aren't the progressbar
        StartCoroutine(DeleteAllLayoutText());

        //resize the goal text box to fit all the goals
        missionsHUDBox.sizeDelta = new Vector2(missionsHUDBox.sizeDelta.x, 150 + ((1 + mainGoals[currentMainGoal].GetSubGoals().Length) * 60));
        
        //initialize the goal progressbar if the current subgoal isnt empty
        if (mainGoals[currentMainGoal].GetCurrentSubGoal().GetGoalType() != GoalType.Empty)
        { 
            GoalProgress.GetComponent<Image>().fillAmount = 0;
            subGoalProgressText.text = mainGoals[currentMainGoal].GetCurrentSubGoal().GetProgressText();
        }
    }

    //deletes all the subgoaltext prefab instances from the layoutgroup
    private IEnumerator DeleteAllLayoutText()
    {
        //delete the text in the layoutgroup
        foreach(RectTransform transform in subGoalGridLayoutGroup.GetComponentInChildren<RectTransform>())
        {
            if(transform.parent == subGoalGridLayoutGroup && transform.name != "progressbar")
            {
                Destroy(transform.gameObject);
            }
        }
        yield return null;

        //initialize as many subtextobjects as there are subgoals in the new maingoal
        MakeNewLayoutTexts();

        //set progressbar to the right sibling index
        GoalProgress.GetComponent<RectTransform>().SetSiblingIndex(mainGoals[currentMainGoal].currentSubGoal + 1);
    }

    //makes new subgoaltext prefab instances in the layoutgroup
    private void MakeNewLayoutTexts()
    {
        foreach(Goal goal in mainGoals[currentMainGoal].GetSubGoals())
        {
            GameObject newGoal = Instantiate(subGoalTextPrefab, subGoalGridLayoutGroup);
            //set text to subgoaltext
            newGoal.GetComponent<TMP_Text>().text = goal.GetText();
        }
    }

    //when a subgoal is completed, give it a checkmark and go to the next subgoal in the checklist,
    //also update the progress
    private void NextSubGoal()
    {
        mainGoals[currentMainGoal].currentSubGoal++;

        //set progressbar to the right sibling index
        GoalProgress.transform.SetSiblingIndex(mainGoals[currentMainGoal].currentSubGoal + 1);

        if (mainGoals[currentMainGoal].GetCurrentSubGoal().GetGoalType() != GoalType.Empty)
        {
            GoalProgress.GetComponent<Image>().fillAmount = 0;
            subGoalProgressText.text = mainGoals[currentMainGoal].GetCurrentSubGoal().GetText();
            PendingFadingText.Enqueue(string.Format("New subgoal: {0}", mainGoals[currentMainGoal].GetCurrentSubGoal().GetText()));
        }
        else
        {
            subGoalProgressText.text = "";
            GoalProgress.GetComponent<Image>().fillAmount = 0;
        }
    }

    private void WinMainGoal()
    {
       StartCoroutine("SwitchToWinScene");
    }

    private IEnumerator SwitchToWinScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("WinScene");
    }
    
}

/// <summary>
/// <para>Class that hold information about goals so it is organised</para>
/// </summary>
public class Goal 
{
    private GoalType type;//type of things the player has to do to complete the goal
    private string text;//text the player sees for the goal
    private int amount;//amount of times the player has to do the goalType action
    private int progress;//amount of times player has done the goalType action
    private float time;//time to complete the goal(only applies to main goals)
    private Goal[] subGoals = new Goal[0];//the subgoals that belong to this goal(only applies to main goals)
    public int currentSubGoal = 0;//subgoal index for subGoals array(only applies to main goals)

    //constructor for creating subgoals
    public Goal(GoalType goaltype, string goalText, int goalAmount)
    {
        type = goaltype;
        text = goalText;
        amount = goalAmount;
    }
    //constructor for creating main goals
    public Goal(GoalType goaltype, string goalText, int goalAmount, Goal[] subGoalArray, float Time)
    {
        type = goaltype;
        text = goalText;
        amount = goalAmount;
        subGoals = subGoalArray;
        time = Time;
    }

    //method for adding progress, return true if the goal is completed after adding the progress
    public bool AddProgressIsCompleted(int amountOfProgress)
    {
        progress += amountOfProgress;
        if(progress >= amount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //getters for the private variables below
    public GoalType GetGoalType()
    {
        return type;
    }

    public string GetText()
    {
        return text;
    }
    public int GetProgress()
    {
        return progress;
    }

    public int GetAmount()
    {
        return amount;
    }

    public string GetProgressText()
    {
        return string.Format("{0} / {1}", progress, amount);
    }

    public float GetTime()
    {
        return time;
    }

    public Goal[] GetSubGoals()
    {
        return subGoals;
    }

    public Goal GetCurrentSubGoal()
    {
        return subGoals[currentSubGoal];
    }
}

/// <summary>
/// enum to give goals a type
/// </summary>
public enum GoalType
{
    Powerstation,   //destroy powerstations
    Dam,            //destroy dams
    Bridge,         //destroy bridges
    Building,       //destroy buildings
    BuildingGold,   //destroy buildings with gold hammer
    BuildingCar,    //destroy buildings with cars
    Bomb,           //use bombs
    Ladder,         //find a ladder
    UseLadder,      //use a ladder
    CarRubber,      //destroy car with rubber hammer
    PoliceCar,      //destroy policecars
    PoliceBuilding, //destroy a police building
    CarSmashBuild1Time,//destroy 1 building with a car
    CarSmashBuild5Times,//destroy 5 buildings with 1 car
    FindNPC,        //find the npc near the bridge
    Bank,           //destroy the bank
    PayNPC,         //pay the npc the money from the bank
    Empty
}


