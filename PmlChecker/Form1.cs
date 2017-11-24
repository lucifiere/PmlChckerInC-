using System;
using System.Windows.Forms;
using PmlChecker.PmlChecker;

namespace PmlChecker
{
    public partial class MainForm : Form
    {
        private Surface _surface = new Surface();

        public MainForm()
        {
            InitializeComponent();
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        // 快捷图标：打开文件
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Surface.OpenFile();
            Surface.SetCodeFileList(codeFileList);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
        }

        // 菜单选项：打开文件
        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Surface.OpenFile();
            Surface.SetCodeFileList(codeFileList);
        }

        private void codeFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}