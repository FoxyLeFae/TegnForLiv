using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject[] panels;

    public void ShowPanel(int index)
    {
        Debug.Log("Knappen blev trykket. Panel: " + index);

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }
}
