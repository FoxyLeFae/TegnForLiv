using UnityEngine;


    public class PanelManager : MonoBehaviour
    {
        public GameObject[] panels;

        private void Start()
        {
         
            ShowPanel(0);
        }

        public void ShowPanel(int index)
        {
            Debug.Log("ShowPanel blev kaldt: " + index);

            if (index < 0 || index >= panels.Length)
            {
                Debug.LogWarning("Ukendt panel-indeks: " + index);
                return;
            }

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] != null)
                {
                    panels[i].SetActive(i == index);
                }
            }
        }
    }
