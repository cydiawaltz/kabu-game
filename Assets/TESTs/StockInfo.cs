using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StockGame
{
    [Serializable]
    public class NewsInfo
    {
        //ニュースのインデックスにひもづけて管理する
        //news,1,2,3,4...と記述
        //changeValueは、銘柄の順序と紐づけ
        public string news;
        public List<int> changeAmount = new List<int>();
    }
    [Serializable]
    public class StockInfo
    {
        public List<int> stocks = new List<int>();
        public void Update(int newValue,bool isPlus)
        {
            if(isPlus)
            {
                stocks.Add(newValue);
            }
            else
            {
                for (int i = 0; i < stocks.Count; i++)
                {
                    if (i == stocks.Count - 1)
                    {
                        stocks[stocks.Count - 1] = newValue;
                    }
                    else
                    {
                        stocks[i] = stocks[i + 1];
                    }
                }
            }
        }
    }
}