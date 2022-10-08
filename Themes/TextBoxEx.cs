using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextBoxEx
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfNumericUpDown"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfNumericUpDown;assembly=WpfNumericUpDown"
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
    ///     <MyNamespace:TextBoxEx/>
    ///
    /// </summary>
    public class TextBoxExConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    public class TextBoxEx : TextBox
    {
        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));

        }
        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(double), typeof(TextBoxEx), new PropertyMetadata(1.0));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(TextBoxEx), new PropertyMetadata(0));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(TextBoxEx), new PropertyMetadata(0));


        #region CornerRadius
        public CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(TextBoxEx));
        #endregion

        #region IsClearButtonVisible
        public static bool GetIsClearBtnVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsClearBtnVisibleProperty);
        }

        public static void SetIsClearBtnVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsClearBtnVisibleProperty, value);
        }

        public static readonly DependencyProperty IsClearBtnVisibleProperty =
            DependencyProperty.RegisterAttached("IsClearBtnVisible", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(OnTextBoxHookChanged));


        private static void OnTextBoxHookChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            textbox.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(ClearBtnClicked));
            textbox.AddHandler(Button.ClickEvent, new RoutedEventHandler(ClearBtnClicked));
        }

        private static void ClearBtnClicked(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;

            if (button == null || button.Name != "PART_BtnClear")
                return;

            var textbox = sender as TextBox;

            if (textbox == null)
                return;

            textbox.Text = "";
        }

        #endregion

        public static bool GetIsAddBtnVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAddBtnVisibleProperty);
        }

        public static void SetIsAddBtnVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAddBtnVisibleProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAddBtnVisibleProperty =
            DependencyProperty.RegisterAttached("IsAddBtnVisible", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(OnTextChanged));


        public static bool GetIsRemoveBtnVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRemoveBtnVisibleProperty);
        }

        public static void SetIsRemoveBtnVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRemoveBtnVisibleProperty, value);
        }

        public static readonly DependencyProperty IsRemoveBtnVisibleProperty =
            DependencyProperty.RegisterAttached("IsRemoveBtnVisible", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;

            if ((bool)e.NewValue == true)
            {
                textbox.MouseWheel += Textbox_MouseWheel;
            }
            else
            {
                textbox.MouseWheel -= Textbox_MouseWheel;
            }

            textbox.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClicked));
            textbox.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClicked));
        }

        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Textbox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var textboxex = e.Source as TextBoxEx;
            double step = 1;
            int min = 0;
            if (textboxex != null)
            {
                step = textboxex.Step;
                min = textboxex.Minimum;
            }
            var textbox = sender as TextBox;
            double temp;
            bool result = double.TryParse(textbox.Text, out temp);

            if (result == true)
            {
                if (e.Delta > 0)
                {
                    textbox.Text = (temp + step).ToString();

                }
                else
                {
                    textbox.Text = (temp - step).ToString();
                }

                if (double.Parse(textbox.Text) < min)
                    textbox.Text = min.ToString();
                textbox.Select(textbox.Text.Length, 0);//光标设置到文本尾部
            }
        }

        private static void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as RepeatButton;

            if (button == null)
                return;

            var textboxex = e.Source as TextBoxEx;
            double step = 1;
            int min = 0;
            if (textboxex != null)
            {
                step = textboxex.Step;
                min = textboxex.Minimum;
            }

            var textbox = sender as TextBox;

            if (textbox == null)
                return;
            double temp;

            //int Step = step.Step;

            bool result = double.TryParse(textbox.Text, out temp);
            if (result == true)
            {
                if (button.Name == "PART_BtnAdd")
                {
                    textbox.Text = (temp + step).ToString();
                }
                else
                {
                    textbox.Text = (temp - step).ToString();
                }

                if (double.Parse(textbox.Text) < min)
                    textbox.Text = min.ToString();

                textbox.Focus();
                textbox.Select(textbox.Text.Length, 0);
            }

        }
    }
}
