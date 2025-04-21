using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public int second, minutes;
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the UI text element

    void Start() {
        AddToSecond();
    }

    private void AddToSecond() {
        second++;
        if(second > 59) {
            minutes++;
            second = 0;
        }
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, second); // Format the time as MM:SS
        Invoke(nameof(AddToSecond), time: 1);
    }
    
    public void StopTimer() {
        CancelInvoke(nameof(AddToSecond)); // Stop the timer when the game is finished
        timerText.gameObject.SetActive(false); // Hide the timer text
    }
}
