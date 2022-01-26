//Code by Eele Roet
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// <para>Shows the player the mission and waits untill the start button is pressed</para>
/// <para>shows the main and subgoals on screen, show the time the player has, plays feedback sounds</para>
/// </summary>
public class MissionObjectiveScript : MonoBehaviour
{
    private GameManager manager;

    //all the text elements in the scene to change for the new missions
    [SerializeField] private TMP_Text mainGoalText;
    [SerializeField] private RectTransform subGoalTextBox;
    [SerializeField] private Transform subGoalLayoutGroup;
    [SerializeField] private GameObject subGoalTextPrefab;
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private AudioSource source;//audio source for playing feedback sounds
    [SerializeField] private AudioClip popUpSound;//sound that plays when the objective pops up
    [SerializeField] private AudioClip startSound;//sound that plays when the start button is pressed

    [SerializeField] private MissionTimerScript timerScript;//reference to timerscript to start the missiontimer on start button press

    private Goal tempGoal;//temporary storage for the new goal

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("HUD").GetComponent<GameManager>();
    }

    public void DisplayNewMission(Goal newGoal)
    {
        tempGoal = newGoal;
        //stop the gametime until the player starts the mission
        Time.timeScale = 0f;

        //feedback sound when the mission pops up
        source.clip = popUpSound;
        source.Play();

        //enable the mission onjective in the hud
        transform.GetChild(0).gameObject.SetActive(true);
        //set all the text in the HUD to the goal text in newGoal 
        mainGoalText.text = newGoal.GetText();
        timeText.text = newGoal.GetTime().ToString();

        //deletes all text currently in the layout group, then makes new ones with the prefab
        StartCoroutine(DeleteAllLayoutText(newGoal));

    }
    private IEnumerator DeleteAllLayoutText(Goal GoalForMakeNewLayoutTexts)
    {
        //delete the text in the layoutgroup
        foreach (RectTransform transform in subGoalLayoutGroup.GetComponentInChildren<RectTransform>())
        {
            if (transform.parent == subGoalLayoutGroup && transform.name != "progressbar")
            {
                Destroy(transform.gameObject);
            }
        }
        yield return null;

        //initialize as many subtextobjects as there are subgoals in the new maingoal
        MakeNewLayoutTexts(GoalForMakeNewLayoutTexts);
    }

    private void MakeNewLayoutTexts(Goal mainGoal)
    {
        foreach (Goal goal in mainGoal.GetSubGoals())
        {
            GameObject newGoal = Instantiate(subGoalTextPrefab, subGoalLayoutGroup);
            //set text to subgoaltext
            newGoal.GetComponent<TMP_Text>().text = goal.GetText();
        }
        subGoalTextBox.sizeDelta = new Vector2(subGoalTextBox.sizeDelta.x, 0f + ((  mainGoal.GetSubGoals().Length) * 60));

    }

    public void StartMissionOnClick()
    {
        //feedback sound when the mission starts
        source.clip = startSound;
        source.Play();

        Time.timeScale = 1;
        transform.GetChild(0).gameObject.SetActive(false);
        manager.NextMainGoal();

        //start missionTimer
        timerScript.StartTimer(tempGoal.GetTime());
    }
}
