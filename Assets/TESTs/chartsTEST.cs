using UnityEngine; 
using XCharts;
using XCharts.Runtime;
using TMPro;
using System;

public class chartsTEST : MonoBehaviour
{
    [SerializeField] LineChart lineChart;
    [SerializeField] bool isChangeValue;
    [SerializeField] bool isChangeElements;
    public bool isAdddata;
    public bool isRepairGraph;
    public double newValue;
    [SerializeField] double[] oldValues;
    [SerializeField] TMP_InputField field;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineChart.RemoveData();
        lineChart.AddSerie<Line>();
        TitleSet("test_saitama");
        var xaxis = lineChart.EnsureChartComponent<XAxis>();
        xaxis.splitNumber = 5;
        var yaxis = lineChart.EnsureChartComponent<YAxis>();
        yaxis.type = Axis.AxisType.Value;
        for (int i = 0;i<5;i++)
        {
            lineChart.AddXAxisData(i + "?");//X軸の要素
        }
        oldValues = new double[4];
        lineChart.AnimationEnable(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAdddata)
        {
            //lineChart.AddData(0, 810);//とりあえず平滑で初期化
            AddData(newValue);
            isAdddata = false;
        }
        if (isChangeValue)
        {
            //for(int i = 0;)
            //lineChart.UpdateData(0,i,)
            lineChart.UpdateData(0, 3, 931);//特定のデータだけを変更
        }
        if (isChangeElements)
        {
            lineChart.AddData(0, 1919);
        }
        if(isRepairGraph)
        {
            for (int i = 0; i < 4; i++)
            {
                oldValues[i] = lineChart.GetData(0, i+1);
            }
            for(int i = 0; i<5 ; i++)
            {
                if (oldValues.Length > i)
                {
                    lineChart.UpdateData(0, i, oldValues[i]);
                }
                else
                {
                    lineChart.UpdateData(0, i, newValue);
                }
            }
            isRepairGraph = false;
        }
    }
    public void AddData(double value)
    {
        lineChart.AddData(0, value);
    }
    void TitleSet(string title)
    {
        lineChart.EnsureChartComponent<Title>().text = title;
        lineChart.SetSize(300, 200);
    }
    public void onClick()
    {
        try
        {
            newValue = double.Parse(field.text);
        }
        catch (FormatException e)
        {
            //GUI.Label(new Rect(10, 10, 200, 40), "書式に不正な箇所があります。内容:" + e.Message);
            Debug.LogError("書式に不正な箇所があります。内容:" + e.Message);
        }
        isRepairGraph = true;
    }
}
