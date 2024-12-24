using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StockGame
{
    public class ValueChangeTEST : MonoBehaviour
    {
        [SerializeField] chartsTEST[] charts;
        public string[] brands;
        public List<NewsInfo> newsInfo = new List<NewsInfo>();//�j���[�X�ƕω��ʂ̃��X�g
        [SerializeField] int tradingTime;//�������
        bool isReadbrands = false;//������ǂݍ��񂾂�(���L�q�p)
        bool isReadInitialStock = false;
        [SerializeField] int linesCount = 1;//�s��
        public int newsIndex;
        [SerializeField] List<StockInfo> stocks = new List<StockInfo>();
        public int[] initialStock;
        [SerializeField] Text newsMessage;
        public int maxBrands;//�����\���ő吔
        public int updateCount = 0;//OnUpdateNews���Ă΂ꂽ��
        public bool isUpdate = false;//�f�[�^���X�V���邩
        [SerializeField] bool isButtonPushed = false;//Update��FixUpdate�̌Ăяo���̊֌W
        [SerializeField] int waitSeconds;
        [SerializeField] TMP_InputField field;//�����͎b��@�\
        void Awake()
        {
            /*
            for (int i = 0; i < 5; i++)
            {
                charts[0].AddData(810);
                charts[1].AddData(810);
            }*/
            OnSetData(true);//�ʓ|�ɂȂ����̂ŏ�������start�̂�
            StartCoroutine(UpdateStock());
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
            if(isButtonPushed)
            {
                OnUpdateNews();
                isButtonPushed = false;
            }
        }
        private void LateUpdate()
        {
            isUpdate = false;
        }
        public void OnToogleUpdate()
        {
            isButtonPushed = true;
        }
        public void OnChangeValue()
        {
            waitSeconds = int.Parse(field.text);
        }
        IEnumerator UpdateStock()
        {
            while(true)
            {
                isButtonPushed = true;
                yield return new WaitForSeconds(waitSeconds);
            }
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
                    OSSettings.DialogShow("��ذ�ذ�ް�܂��̓p�X���L���Ȓl�ł͂���܂���","�v���I�ȃG���[",MessageBoxIcon.Hand);
                }
                catch(Exception)
                {
                    OSSettings.DialogShow("�X�V����O���t������܂���","setting.txt",MessageBoxIcon.Warning);
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
                            // �t�@�C���̏I�[
                            //MessageBox.Show("line end");
                            break;
                        }

                        // �R�����g�s�E��s���X�L�b�v
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//"))
                        {
                            //MessageBox.Show("�R�����gor��s");
                            continue;
                        }

                        if (line.StartsWith("BrandsName:") && !isReadbrands)
                        {
                            if(!isReadbrands)
                            {
                                // �������̓ǂݎ��
                                //MessageBox.Show("�������ǂݍ��݂�������");
                                line = line.Remove(0, 11); // �uBrandsName: �v���폜
                                string[] tmp_Brands = line.Split(',');
                                brands = new string[tmp_Brands.Length];
                                for (int j = 0; j < tmp_Brands.Length; j++)
                                {
                                    brands[j] = tmp_Brands[j];
                                }
                                isReadbrands = true; // ��x�ǂݎ������t���O�𗧂Ă�
                            }
                            else
                            {
                                MessageBox.Show("�Q�񏑂����ȁH");
                            }
                            continue;
                        }
                        if(line.StartsWith("InitialStock:") && !isReadInitialStock)
                        {
                            try
                            {
                                //MessageBox.Show("�C�j�V�����X�g�b�N��ǂݍ���");
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
                                OSSettings.DialogShow("InitialStock�̒l�̗v�f������܂���B", "setting.txt", MessageBoxIcon.Stop);
                            }
                            catch(Exception)
                            {
                                OSSettings.DialogShow("InitialStock�̏������Ԉ���Ă��邩�AInt32�^�̃I�[�o�[�t���[�ł��B","setting.txt",MessageBoxIcon.Stop);
                            }
                            continue;
                        }
                        if(line.StartsWith("TradingTime:"))
                        {
                            try
                            {
                                line = line.Remove(0, 12);
                                waitSeconds = int.Parse(line);
                            }
                            catch(ArgumentException)
                            {
                                OSSettings.DialogShow("TradingTime:�̌�ɒl����͂��Ă�������", "setting.txt", MessageBoxIcon.Stop);
                            }
                            catch(FormatException)
                            {
                                OSSettings.DialogShow("TradingTime�̏������Ⴂ�܂��BTradingTime�͎��R���ł���K�v������܂��B", "setting.txt", MessageBoxIcon.Error);
                            }
                            catch(OverflowException)
                            {
                                OSSettings.DialogShow("Int32�^�̵��ް�۰�ł��B�v�f:TradingTime", "setting.txt", MessageBoxIcon.Error);
                            }
                            continue;
                        }

                        // �s������
                        //MessageBox.Show("�j���[�X����ǂݍ���vip");
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
                                OSSettings.DialogShow("�v�f�����s�����Ă��܂��B�ꏊ:" + linesCount + "�s", "setting.txt", MessageBoxIcon.Error);
                            }
                            catch(ArgumentNullException)
                            {
                                OSSettings.DialogShow(",(���)�̌�ɂ͗v�f���L�����Ă��������B�ꏊ:" + linesCount + "�s","setting.txt",MessageBoxIcon.Error);
                            }
                            catch (FormatException)
                            {
                                OSSettings.DialogShow("�������Ⴂ�܂��B���p�����ŋL�q���Ă��������B���ѵ��ް�۰���m�F���Ă��������B�ꏊ:" + linesCount + "�s", "setting.txt", MessageBoxIcon.Error);
                            }
                            catch(Exception)
                            {
                                OSSettings.DialogShow("�悭�킩���G���[���o�܂����B�t������ۼު��̧�ق����ďC�����邩�J���҂Ƀo�O�̕񍐂����Ă��������B�J����twitter:@wattzmaro �ꏊ:" + linesCount + "�s", "setting.txt", MessageBoxIcon.Error);
                            }
                        }
                        newsInfo.Add(temp);
                    }
                }
            }

        }
    }

}
