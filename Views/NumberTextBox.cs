using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetaNet.Views
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MetaNet.Views"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MetaNet.Views;assembly=MetaNet.Views"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:NumberTextBox/>
    ///
    /// </summary>
    public class NumberTextBox : TextBox
    {
        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(NumberTextBox), new PropertyMetadata(1));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(NumberTextBox), new PropertyMetadata(0));

        static NumberTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberTextBox), new FrameworkPropertyMetadata(typeof(NumberTextBox)));
        }

        public NumberTextBox()
        {
            InputMethod.SetIsInputMethodEnabled(this, false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            XamlIcon DeButton = (XamlIcon)this.Template.FindName("DeButton", this);
            XamlIcon InButton = (XamlIcon)this.Template.FindName("InButton", this);

            DeButton.MouseLeftButtonDown += DeButton_MouseLeftButtonDown;
            InButton.MouseLeftButtonDown += InButton_MouseLeftButtonDown;
        }

        private void DeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.Text))
            {
                this.Text = int.Parse(this.Text) - Step < Minimum ? Minimum + "" : int.Parse(this.Text) - Step + "";
                this.SelectionStart = this.Text.Length;
            }
            else
            {
                this.Text = Minimum + "";
                this.SelectionStart = this.Text.Length;
            }
        }

        private void InButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.Text))
            {
                this.Text = int.Parse(this.Text) + Step + "";
                this.SelectionStart = this.Text.Length;
            }
            else
            {
                this.Text = Minimum + "";
                this.SelectionStart = this.Text.Length;
            }
        }
    }
}
