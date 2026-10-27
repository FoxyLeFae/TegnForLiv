using UnityEngine;


    public class PanelManager : MonoBehaviour
    {
        public GameObject[] panels;

        private void Start()
        {
            // The scene may be saved while any panel is open in the editor.
            // Always begin the app on the first panel so the normal flow works.
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
