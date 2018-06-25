﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace バックアップはできます
{
    public partial class ProgressBar : Form
    {
        SelectForm SelectForm;
        ResultForm ResultForm;
        public ProgressBar(SelectForm selectForm, ResultForm resultForm)
        {
            InitializeComponent();
            SelectForm = selectForm;
            ResultForm = resultForm;
            //バックグラウンド処理実行
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            //GUI更新
            bgWorker.ReportProgress(0, new FlagAndMaxvalue(WoekFlag.DirectoryEnumeration));
            //ディレクトリ一覧
            List<string> directorys = new List<string>();
            //はじめのディレクトリ追加
            directorys.Add(SelectForm.PathTextBox.Text);
            //サブディレクトリ追加
            if (SelectForm.SabFolderCheckBox.Checked)
            {
                foreach (var item in Directory.GetDirectories(SelectForm.PathTextBox.Text, "*", SearchOption.AllDirectories))
                {
                    //キャンセルされたか調べる
                    if (bgWorker.CancellationPending)
                    {
                        //キャンセルされたとき
                        e.Cancel = true;
                        return;
                    }
                    directorys.Add(item);
                }
            }

            //ファイルの数カウント
            int fileCount = 0;
            foreach (var item in directorys)
            {
                //キャンセルされたか調べる
                if (bgWorker.CancellationPending)
                {
                    //キャンセルされたとき
                    e.Cancel = true;
                    return;
                }
                var files = Directory.EnumerateFiles(item);
                fileCount += files.Count();
                bgWorker.ReportProgress(fileCount, new FlagAndMaxvalue(WoekFlag.Counting));
            }

            //ファイルの比較
            //ディレクトリにあるファイルを比較
            foreach (var directory in directorys)
            {
                //キャンセルされたか調べる
                if (bgWorker.CancellationPending)
                {
                    //キャンセルされたとき
                    e.Cancel = true;
                    return;
                }
                List<string> files = ComparisonFileName(bgWorker, directory, fileCount);
                if (files.Count != 0)
                {
                    bgWorker.ReportProgress(0, new FlagAndMaxvalue(WoekFlag.FileUpdate, 0, files.ToArray()));
                }
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //キャスト
            FlagAndMaxvalue flagAndMaxvalue = (FlagAndMaxvalue)e.UserState;
            WoekFlag flag = flagAndMaxvalue.Flag;

            switch (flag)
            {
                case WoekFlag.DirectoryEnumeration:
                    this.label1.Text = "ディレクトリ列挙中・・・";
                    this.Text = this.label1.Text;
                    break;
                case WoekFlag.Counting:
                    this.label1.Text = "見つかったファイル数：" + e.ProgressPercentage.ToString();
                    this.Text = this.label1.Text;
                    break;
                case WoekFlag.Comparison:
                    this.label1.Text = "比較中 " + e.ProgressPercentage.ToString() + @"/" + flagAndMaxvalue.Maxvalue.ToString();
                    this.Text = this.label1.Text;
                    this.label1.Update();
                    this.progressBar1.Maximum = flagAndMaxvalue.Maxvalue;
                    this.progressBar1.Value = e.ProgressPercentage;
                    this.progressBar1.Style = ProgressBarStyle.Blocks;
                    break;
                case WoekFlag.FileUpdate:
                    ResultForm.listBox1.Items.AddRange(flagAndMaxvalue.FileNames);
                    break;
                default:
                    break;
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //エラーが発生したとき
                MessageBox.Show("エラーが発生しました。\n" + e.Error.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                //キャンセルされたとき
                MessageBox.Show("キャンセルされました。");
            }
            else
            {
                //正常に終了したとき
                MessageBox.Show("完了しました。");
            }
            this.Close();                
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ButtonCancel.Enabled = false;
            //キャンセルする
            backgroundWorker1.CancelAsync();
        }

        int fileCountCounter = 0;

        /// <summary>
        /// 指定された、ディレクトリ内のファイルを比較し、同じ名前のファイルがあったファイルを返します。
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        /// <returns>同じ名前ファイル名一覧</returns>
        private List<string> ComparisonFileName(BackgroundWorker bgWorker, string directory, int fileCount)
        {
            //同じ名前があったときの格納場所
            List<string> sameNameFiles = new List<string>();
            //ディレクトリにあるファイル一覧
            string[] files = Directory.GetFiles(directory, "*");
            //同じ名前があるか確認
            for (int i = 0; i < files.Length - 1; i++)
            {
                for (int j = i + 1; j < files.Length; j++)
                {
                    CompareInfo ci = CultureInfo.CurrentCulture.CompareInfo;
                    if (ci.Compare(files[i], files[j], CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType) == 0)
                    {
                        sameNameFiles.Add(files[i]);
                    }
                }
            }
            fileCountCounter += files.Length;
            bgWorker.ReportProgress(fileCountCounter, new FlagAndMaxvalue(WoekFlag.Comparison, fileCount));

            return sameNameFiles;
        }
    }

    /// <summary>
    /// GUIを変更させるときの、実行中作業を表すフラグ
    /// </summary>
    enum WoekFlag
    {
        /// <summary>
        /// ディレクトリ列挙中
        /// </summary>
        DirectoryEnumeration,
        /// <summary>
        /// ファイル数カウント中
        /// </summary>
        Counting,
        /// <summary>
        /// ファイル比較中
        /// </summary>
        Comparison,
        /// <summary>
        /// ファイルをリストへ追加
        /// </summary>
        FileUpdate
    }

    /// <summary>
    /// 実行中フラグと最大値を表します。
    /// </summary>
    class FlagAndMaxvalue
    {
        /// <summary>
        /// フラグを指定して、オブジェクトを初期化します。
        /// </summary>
        /// <param name="flag">実行中フラグ</param>
        public FlagAndMaxvalue(WoekFlag flag)
        {
            Flag = flag;
            Maxvalue = 0;
        }

        /// <summary>
        /// フラグと最大値を指定して、オブジェクトを初期化します。
        /// </summary>
        /// <param name="flag">実行中フラグ</param>
        /// <param name="maxvalue">最大値</param>
        public FlagAndMaxvalue(WoekFlag flag, int maxvalue)
        {
            Flag = flag;
            Maxvalue = maxvalue;
        }

        /// <summary>
        /// フラグと最大値、ファイル名を指定して、オブジェクトを初期化します。
        /// </summary>
        /// <param name="flag">実行中フラグ</param>
        /// <param name="maxvalue">最大値</param>
        /// <param name="fileName">ファイル名</param>
        public FlagAndMaxvalue(WoekFlag flag, int maxvalue, string[] fileNames) : this(flag, maxvalue)
        {
            FileNames = fileNames;
        }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public WoekFlag Flag
        {
            get;
            set;
        }

        /// <summary>
        /// 最大値
        /// </summary>
        public int Maxvalue
        {
            get;
            set;
        }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string[] FileNames
        {
            get;
            set;
        }
    }
}
