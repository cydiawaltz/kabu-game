using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UIElements;

namespace StockGame
{
    public class ValueChangeTEST : MonoBehaviour
    {
        [SerializeField] chartsTEST charts;
        [SerializeField] string[] brands;
        [SerializeField] List<NewsInfo> stockInfo = new List<NewsInfo>();//ニュースと変化量のリスト
        [SerializeField] Toggle read;
        [SerializeField] int tradingTime;//取引時間
        bool isReadbrands = false;//銘柄を読み込んだか(一回記述用)
        bool isReadInitialStock = false;
        [SerializeField] int linesCount;//行数
        [SerializeField] List<StockInfo> stocks = new List<StockInfo>();
        [SerializeField] int[] initialStock;
        void Start()
        {
            charts.newValue = 810;
            for (int i = 0; i < 5; i++)
            {
                charts.AddData(810);
            }
            OnSetData(true);//面倒になったので初期化はstartのみ
        }

        public void OnSetData(bool isOn)
        {
            if (isOn)
            {
                try
                {
                    string path = OSSettings.GetSettingPath();
                    LoadData(path);
                    stocks = new List<StockInfo>(brands.Length);
                    for(int i = 0;i < brands.Length;i++)
                    {
                        stocks[i].stocks.Add(initialStock[i]);
                        charts.lineChart.UpdateData(i, 0, initialStock[i]);
                    }
                }
                catch(PathNotException)
                {
                    OSSettings.DialogShow("ｽﾄﾘｰﾑﾘｰﾀﾞｰまたはパスが有効な値ではありません","致命的なエラー",MessageBoxIcon.Hand);
                }
                catch(Exception)
                {
                    OSSettings.DialogShow("更新するグラフがありません","setting.txt",MessageBoxIcon.Warning);
                }
            }
        }
        void LoadData(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string header = sr.ReadLine();
                if (header.Contains("SSC Stock Format 1.00"))
                {
                    while (true)
                    {
                        linesCount++;
                        string line = sr.ReadLine();
                        if (line == null)
                        {
                            // ファイルの終端
                            MessageBox.Show("line end");
                            break;
                        }

                        // コメント行・空行をスキップ
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//"))
                        {
                            //MessageBox.Show("コメントor空行");
                            continue;
                        }

                        if (line.StartsWith("BrandsName:") && !isReadbrands)
                        {
                            if(!isReadbrands)
                            {
                                // 銘柄名の読み取り
                                MessageBox.Show("銘柄名読み込みさいたま");
                                line = line.Remove(0, 11); // 「BrandsName: 」を削除
                                string[] tmp_Brands = line.Split(',');
                                brands = new string[tmp_Brands.Length];
                                for (int j = 0; j < tmp_Brands.Length; j++)
                                {
                                    brands[j] = tmp_Brands[j];
                                }
                                isReadbrands = true; // 一度読み取ったらフラグを立てる
                            }
                            else
                            {
                                MessageBox.Show("２回書いたな？");
                            }
                            continue;
                        }
                        if(line.StartsWith("InitialStock:") && !isReadInitialStock)
                        {
                            try
                            {
                                MessageBox.Show("イニシャルストックを読み込み");
                                line = line.Remove(0, 13);
                                string[] tmp_start = line.Split(",");
                                initialStock = new int[tmp_start.Length];
                                for (int j = 0; j < tmp_start.Length; j++)
                                {
                                    initialStock[j] = int.Parse(tmp_start[j]);
                                }
                                isReadInitialStock = true;
                            }
                            catch(ArgumentNullException)
                            {
                                OSSettings.DialogShow("InitialStockの値の要素が足りません。", "setting.txt", MessageBoxIcon.Stop);
                            }
                            catch(Exception)
                            {
                                OSSettings.DialogShow("InitialStockの書式が間違っているか、Int32型のオーバーフローです。","setting.txt",MessageBoxIcon.Stop);
                            }
                            continue;
                        }

                        // 行を処理
                        //MessageBox.Show("ニュース速報読み込みvip");
                        string[] lines = line.Split(',');
                        NewsInfo temp = new NewsInfo();
                        temp.news = lines[0];
                        for (int i = 1; i < brands.Length + 1; i++)
                        {
                            try
                            {
                                int tmp = int.Parse(lines[i]);
                                temp.changeAmount.Add(tmp);
                            }
                            catch(NullReferenceException) 
                            {
#if UNITY_WEBGL
                    GUI.Label(new Rect(10, 10, 200, 20), "書式が不正です。場所:" + linesCount + "行目");
#elif UNITY_STANDALONE_WIN && UNITY_EDITOR
                                MessageBox.Show("NullReferenceException");
#endif
                            }
                            catch(Exception)
                            {
                                MessageBox.Show("書式が不正です。場所:" + linesCount + "行目");
                                continue;
                            }
                        }
                        stockInfo.Add(temp);
                    }
                }
            }

        }
    }

}
