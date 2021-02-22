using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public TMP_Text countTxt;

    public void SetCount(int count) {
        countTxt.text = "x" + count;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
