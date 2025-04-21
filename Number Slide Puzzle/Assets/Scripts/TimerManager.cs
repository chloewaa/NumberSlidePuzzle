//Chloe Walsh
//TimerManager
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public int second, minutes;
    [SerializeField] private TextMeshProUGUI timerText; 

    void Start() {
        AddToSecond();
    }

    private void AddToSecond() {
        second++;
        if(second > 59) {
            minutes++;
            second = 0;
        }
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, second); 
        Invoke(nameof(AddToSecond), time: 1);
    }
    
    public void StopTimer() {
        //Stop the timer and hide the text
        CancelInvoke(nameof(AddToSecond)); 
        timerText.gameObject.SetActive(false); 
    }
}
