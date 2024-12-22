using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

namespace StockGame
{
    public class ValueChangeTEST : MonoBehaviour
    {
        [SerializeField] chartsTEST[] charts;
        public string[] brands;
        public List<NewsInfo> newsInfo = new List<NewsInfo>();//ニュースと変化量のリスト
        [SerializeField] int tradingTime;//取引時間
        bool isReadbrands = false;//銘柄を読み込んだか(一回記述用)
        bool isReadInitialStock = false;
        [SerializeField] int linesCount = 1;//行数
        public int newsIndex;
        [SerializeField] List<StockInfo> stocks = new List<StockInfo>();
        public int[] initialStock;
        [SerializeField] Text newsMessage;
        public int maxBrands;//銘柄表示最大数
        public int updateCount = 0;//OnUpdateNewsが呼ばれた回数
        public bool isUpdate = false;//データを更新するか
        void Awake()
        {
            /*
            for (int i = 0; i < 5; i++)
            {
                charts[0].AddData(810);
                charts[1].AddData(810);
            }*/
            OnSetData(true);//面倒になったので初期化はstartのみ
        }
        public void OnUpdateNews()
        {
            newsIndex = UnityEngine.Random.Range(0, newsInfo.Count);
            NewsInfo news = newsInfo[newsIndex];
            newsMessage.text = news.news;
            for (int i = 0;i < brands.Length;i++)
            {
                int value = stocks[i].stocks.Last() + news.changeAmount[i];
                if (updateCount == maxBrands)
                {
                    stocks[i].Update(value, false);
                }
                else
                {
                    stocks[i].Update(value,true);
                    updateCount++;
                }
            }
            isUpdate = true;
            /*for(int i = 0;i<5;i++)
            {
                charts[0].lineChart.UpdateData(0, i, stocks[0].stocks[i]);
                charts[1].lineChart.UpdateData(0, i, stocks[0].stocks[i]);
            }*/
        }
        private void FixedUpdate()
        {
            isUpdate = false;
        }
        void OnToogleUpdate(bool isOn)
        {
            OnUpdateNews();
        }

        void OnSetData(bool isOn)
        {
            if (isOn)
            {
                try
                {
                    string path = OSSettings.GetSettingPath();
                    LoadData(path);
                    for(int i = 0;i < brands.Length;i++)
                    {
                        StockInfo stock = new StockInfo();
                        stock.stocks.Add(initialStock[i]);
                        stocks.Add(stock);
                        //charts.lineChart.UpdateData(i, 0, initialStock[i]);
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
                            //MessageBox.Show("line end");
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
                                //MessageBox.Show("銘柄名読み込みさいたま");
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
                                //MessageBox.Show("イニシャルストックを読み込み");
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
                            catch (IndexOutOfRangeException)
                            {
                                OSSettings.DialogShow("要素数が不足しています。場所:" + linesCount + "行", "setting.txt", MessageBoxIcon.Error);
                            }
                            catch(ArgumentNullException)
                            {
                                OSSettings.DialogShow(",(ｺﾝﾏ)の後には要素を記入してください。場所:" + linesCount + "行","setting.txt",MessageBoxIcon.Error);
                            }
                            catch (FormatException)
                            {
                                OSSettings.DialogShow("書式が違います。半角数字で記述してください。ｼｽﾃﾑｵｰﾊﾞｰﾌﾛｰも確認してください。場所:" + linesCount + "行", "setting.txt", MessageBoxIcon.Error);
                            }
                            catch(Exception)
                            {
                                OSSettings.DialogShow("よくわからんエラーが出ました。付属のﾌﾟﾛｼﾞｪｸﾄﾌｧｲﾙを見て修正するか開発者にバグの報告をしてください。開発者twitter:@wattzmaro 場所:" + linesCount + "行", "setting.txt", MessageBoxIcon.Error);
                            }
                        }
                        newsInfo.Add(temp);
                    }
                }
            }

        }
    }

}
