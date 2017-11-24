using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PmlChecker.PmlChecker
{
    public class Surface
    {
        private static SourceCode _curCode;
        private static List<SourceCode> _curCodeList = new List<SourceCode>();

        public static void OpenFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = @"请选择文件",
                Filter = @"C源码文件(*.c)|*.c"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;
            _curCodeList.Clear();
            _curCodeList.Add(new SourceCode(fileDialog.FileName));
        }

        public static void OpenFolder()
        {
            var dialog = new FolderBrowserDialog {Description = @"请选择C源码所在的文件夹"};
            if (dialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(dialog.SelectedPath)) return;
            var folder = new DirectoryInfo(dialog.SelectedPath);
            _curCodeList.Clear();
            foreach (var file in folder.GetFiles())
            {
                _curCodeList.Add(new SourceCode(file.Name));
            }
        }

        public static void SetCodeFileList(ListView listView)
        {
            if (_curCodeList.Count == 0) return;
            listView.Items.Clear();
            foreach (var sourceCode in _curCodeList)
            {
                var item = new ListViewItem {Text = sourceCode.FileName};
                listView.Items.Add(item);
            }
        }

    }
}