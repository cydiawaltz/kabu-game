using UnityEngine; 
using XCharts;
using XCharts.Runtime;
using TMPro;
using System;
using StockGame;

public class chartsTEST : MonoBehaviour
{
    public LineChart lineChart;
    [SerializeField] bool isChangeValue;
    [SerializeField] bool isChangeElements;
    public bool isAdddata;
    public bool isRepairGraph;
    public double newValue;
    [SerializeField] double[] oldValues;
    [SerializeField] TMP_InputField field;
    [SerializeField] ValueChangeTEST valueChange;
    [SerializeField] int updateTime = 0;//更新した回数
    [SerializeField] TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        lineChart.enabled = true;
    }
    void Start()
    {
        valueChange = GameObject.FindWithTag("setting").GetComponent<ValueChangeTEST>();
        lineChart.RemoveData();
        lineChart.AddSerie<Line>();
        var xaxis = lineChart.EnsureChartComponent<XAxis>();
        xaxis.splitNumber = 5;
        var yaxis = lineChart.EnsureChartComponent<YAxis>();
        yaxis.type = Axis.AxisType.Value;
        for (int i = 0;i<5;i++)
        {
            lineChart.AddXAxisData(i + "?");//X軸の要素
            //lineChart.AddData(0,810);
        }
        oldValues = new double[4];
        lineChart.AnimationEnable(false);
        try
        {
            TitleSet(valueChange.brands[int.Parse(gameObject.name)]);
            lineChart.AddData(0, valueChange.initialStock[int.Parse(gameObject.name)]);
            updateTime++;
        }
        catch
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isAdddata)
        {
            //lineChart.AddData(0, 810);//とりあえず平滑で初期化
            AddData(newValue);
            text.text = "株価:"+newValue.ToString();
            updateTime++;
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
            text.text = "株価:"+newValue.ToString();
            updateTime++;
            isRepairGraph = false;
        }
        if (valueChange.isUpdate)
        {
            newValue = valueChange.newsInfo[valueChange.newsIndex].changeAmount[int.Parse(gameObject.name)];
            if (updateTime < 5)
            {
                isAdddata = true;
            }
            else
            {
                isRepairGraph = true;
            }
        }
    }

    public void AddData(double value)
    {
        lineChart.AddData(0, value);
    }
    void TitleSet(string title)
    {
        lineChart.EnsureChartComponent<Title>().text = title;
        //lineChart.SetSize(300, 200);
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
