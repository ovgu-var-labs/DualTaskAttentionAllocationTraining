using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecondaryTaskManager : MonoBehaviour
{
    private char[] possibleChars = new char[] { 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'J', 'K', 'L', 'M', 'O', 'P', 'S', 'T', 'X', 'Y', 'Z' };
    private List<char> openChars;
    private string currentChar = "NaN";
    public string GetCurrentChar() {  return currentChar; }

    //secondary abdominalTask Display
    public TMP_Text secTask;

    private int taskCount;
    public int GetTaskCount() { return taskCount; }
    private int taskSeenCount;
    public int GetTaskSeenCount() { return taskSeenCount; }
    private List<string> seenChars = new List<string>();
    public string GetSeenChars() { return ListToString(seenChars); }
    private List<string> notSeenChars = new List<string>();
    public string GetNotSeenChars() { return ListToString(notSeenChars); }
    private bool currentTaskSeen;
    public bool GetCurrentTaskSeen() { return currentTaskSeen; }

    public bool secondaryTasksLeft() 
    { 
        return openChars.Count > 0;
    }

    public void ResetTask()
    {
        openChars = new List<char>(possibleChars);
        taskCount = 0;
        taskSeenCount = 0;  
        seenChars.Clear();
        notSeenChars.Clear();
        currentTaskSeen = false;
    }

    public void ShowNewTask()
    {
        if (openChars.Count > 0)
        {
            int randIndex = Random.Range(0, openChars.Count);
            currentChar = openChars[randIndex].ToString();
            secTask.text = currentChar;
            openChars.RemoveAt(randIndex);
            taskCount++;
            currentTaskSeen = false;

            notSeenChars.Add(currentChar);
        }
    }

    public void EndTask() { }

    public void TaskSeen()
    {
        taskSeenCount++;
        seenChars.Add(currentChar);
        notSeenChars.Remove(currentChar);
        currentTaskSeen=true;
    }

    private string ListToString(List<string> _list)
    {
        string listAsString = string.Empty;
        foreach (string c in _list)
        {
            listAsString += c + " ";
        }
        return listAsString;
    }
}
