using UnityEngine;

public class ValueChangeTEST : MonoBehaviour
{
    [SerializeField] chartsTEST charts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charts.newValue = 810;
        for(int i = 0; i < 5; i++)
        {
            charts.AddData(810);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
