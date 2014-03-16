using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace GameManager
{
    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public RatingControl()
        {
            InitializeComponent();
        }

        private void DeselectAll()
        {
            S1.IsChecked = false;
            S2.IsChecked = false;
            S3.IsChecked = false;
            S4.IsChecked = false;
            S5.IsChecked = false;
        }

        public static DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(int),
    typeof(RatingControl), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RatingControl control = d as RatingControl;
            control.Value = (int)e.NewValue;
        }

        public int Value 
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
                SelectButtons(value);
            }
        }

        private int ButtonToNumber(ToggleButton button)
        {
            switch (button.Name)
            {
                case "S1":
                    return 1;
                case "S2":
                    return 2;
                case "S3":
                    return 3;
                case "S4":
                    return 4;
                case "S5":
                    return 5;
                default:
                    return 0;
            }
        }
        private void SelectButtons(int val)
        {
            DeselectAll();
            switch (val)
            {
                case 1:
                    S1.IsChecked = true;
                    break;
                case 2:
                    S1.IsChecked = true;
                    S2.IsChecked = true;
                    break;
                case 3:
                    S1.IsChecked = true;
                    S2.IsChecked = true;
                    S3.IsChecked = true;

                    break;
                case 4:
                    S1.IsChecked = true;
                    S2.IsChecked = true;
                    S3.IsChecked = true;
                    S4.IsChecked = true;
                    break;
                case 5:
                    S1.IsChecked = true;
                    S2.IsChecked = true;
                    S3.IsChecked = true;
                    S4.IsChecked = true;
                    S5.IsChecked = true;
                    break;
            }
        }

        private void RatingButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;

            Value = ButtonToNumber(button);
        }

        private void MouseEnterEventCallback(object sender, MouseEventArgs e)
        {
            SelectButtons(ButtonToNumber(sender as ToggleButton));
        }

        private void MouseLeaveEventCallback(object sender, MouseEventArgs e)
        {
            SelectButtons(Value);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    Value = 1;
                    break;
                case Key.D2:
                    Value = 2;
                    break;
                case Key.D3:
                    Value = 3;
                    break;
                case Key.D4:
                    Value = 4;
                    break;
                case Key.D5:
                    Value = 5;
                    break;
                case Key.Delete:
                case Key.D0:
                    Value = 0;
                    break;
                case Key.Left:
                    Value--;
                    Value = Value.Clamp(0, 5);
                    break;
                case Key.Right:
                    Value++;
                    Value = Value.Clamp(0, 5);
                    break;
            }
        }

    }
}
