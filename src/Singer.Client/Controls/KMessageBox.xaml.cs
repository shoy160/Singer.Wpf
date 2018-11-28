using System;
using System.Windows;

namespace Singer.Client.Controls
{
    /// <summary> KMessageBox.xaml 的交互逻辑 </summary>
    public partial class KMessageBox
    {
        public KMessageBox(double width = 0, double heigth = 0)
        {
            InitializeComponent();
            if (width > 0)
                Width = width;
            if (heigth > 0)
                Height = heigth;
        }

        private void AddButton(string text, string className, Thickness margin,
            Action handler = null, bool isFocus = false)
        {
            var btn = new KButton
            {
                Content = text,
                Margin = margin,
                FontSize = 16,
                Width = 156,
                Height = 40
            };
            if (!string.IsNullOrWhiteSpace(className))
                btn.SetResourceReference(StyleProperty, className);
            btn.Click += (o, args) =>
            {
                handler?.Invoke();
                Close();
            };
            BtnControls.Children.Add(btn);
            if (isFocus)
                btn.Focus();
        }

        public static void Alert(string msg, Window owner = null)
        {
            var dialog = new KMessageBox(320)
            {
                TextTitle = { Text = "提示" },
                TextMessage = { Text = msg },
                MessageIcon = { Visibility = Visibility.Visible },
                BtnControls = { Visibility = Visibility.Collapsed },
                MinHeight = 200
            };
            dialog.Show(owner);
        }

        public static bool Confirm(string msg, string yesBtn = "确定", string noBtn = "取消", Window owner = null)
        {
            var dialog = new KMessageBox(600)
            {
                TextTitle = { Text = "消息提醒" },
                TextMessage = { Text = msg },
                MinHeight = 300
            };
            var result = false;
            dialog.AddButton(yesBtn, "Btn-Default", new Thickness(0), () => { result = true; }, true);
            dialog.AddButton(noBtn, "K-Btn", new Thickness(15, 0, 0, 0), () => { result = false; });
            dialog.Show(owner);
            return result;
        }

        public static bool Prop(ref string word, Window owner = null)
        {
            var dialog = new KMessageBox(600)
            {
                TextTitle = { Text = "内容编辑" },
                TextMessage = { Text = word },
                EditPanel = { Visibility = Visibility.Visible },
                EditText = { Text = word },
                MessagePanel = { Visibility = Visibility.Collapsed },
                MinHeight = 300
            };
            var result = false;
            var text = word;

            dialog.AddButton("确认", "Btn-Default", new Thickness(0), () =>
            {
                result = true;
                text = dialog.EditText.Text;
            }, true);
            dialog.AddButton("取消", "K-Btn", new Thickness(15, 0, 0, 0), () => { result = false; });
            dialog.Show(owner);
            word = text;
            return result;
        }
    }
}
