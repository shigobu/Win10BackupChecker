using System;
using System.Windows.Forms;

namespace バックアップはできます
{
    public partial class SelectForm : Form
    {
        public SelectForm()
        {
            InitializeComponent();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialogクラスのインスタンスを作成
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbd.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = false;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                PathTextBox.Text = fbd.SelectedPath;
            }
            fbd.Dispose();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            PathTextBox.Text = PathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(PathTextBox.Text))
            {
                MessageBox.Show("場所が不正です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Hide();
            ResultForm resultForm = new ResultForm();
            resultForm.Show(this);
            ProgressBar progressBar = new ProgressBar(this, resultForm);
            progressBar.ShowDialog();
        }
    }
}
