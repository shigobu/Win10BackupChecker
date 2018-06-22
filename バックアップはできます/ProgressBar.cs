using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            bgWorker.ReportProgress(0);
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ResultForm.listBox1.Items.Add("kokekokko");
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //エラーが発生したとき
                MessageBox.Show("エラーが発生しました。\n" + e.Error.Message);
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
            //キャンセルする
            backgroundWorker1.CancelAsync();
        }
    }
}
