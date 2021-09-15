using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health = null;
        [SerializeField] RectTransform healthbar = null;
        [SerializeField] Canvas rootCanvas = null;



        void Update()
        {
            if (Mathf.Approximately(health.GetHealthPointFraction(), 0) ||
                    Mathf.Approximately(health.GetHealthPointFraction(), 1))
            {
                rootCanvas.enabled = false;
            }
            else
            {
                rootCanvas.enabled = true;
                healthbar.localScale = new Vector3(health.GetHealthPointFraction(), 1, 1);
            }


        }

    }

}
