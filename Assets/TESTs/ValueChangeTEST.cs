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
        [SerializeField] List<NewsInfo> stockInfo = new List<NewsInfo>();//�j���[�X�ƕω��ʂ̃��X�g
        [SerializeField] Toggle read;
        [SerializeField] int tradingTime;//�������
        bool isReadbrands = false;//������ǂݍ��񂾂�(���L�q�p)
        bool isReadInitialStock = false;
        [SerializeField] int linesCount;//�s��
        [SerializeField] List<StockInfo> stocks = new List<StockInfo>();
        [SerializeField] int[] initialStock;
        void Start()
        {
            charts.newValue = 810;
            for (int i = 0; i < 5; i++)
            {
                charts.AddData(810);
            }
            OnSetData(true);//�ʓ|�ɂȂ����̂ŏ�������start�̂�
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
                            MessageBox.Show("line end");
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
                                MessageBox.Show("�������ǂݍ��݂�������");
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
                                MessageBox.Show("�C�j�V�����X�g�b�N��ǂݍ���");
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
                            catch(NullReferenceException) 
                            {
#if UNITY_WEBGL
                    GUI.Label(new Rect(10, 10, 200, 20), "�������s���ł��B�ꏊ:" + linesCount + "�s��");
#elif UNITY_STANDALONE_WIN && UNITY_EDITOR
                                MessageBox.Show("NullReferenceException");
#endif
                            }
                            catch(Exception)
                            {
                                MessageBox.Show("�������s���ł��B�ꏊ:" + linesCount + "�s��");
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
