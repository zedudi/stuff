using System;
using System.Windows;
using System.Windows.Controls;

namespace railcamsw.view
{
    public abstract partial class NumericUpDown : UserControl
    {
        abstract protected void Increase(object sender, RoutedEventArgs e);
        abstract protected void Decrease(object sender, RoutedEventArgs e);
        public static DependencyProperty TextProperty;
    }
    public partial class NumericUpDown <T> : NumericUpDown where T : struct
    {
        System.Func<T, T> Inc, Dec;

        public static new DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(T), typeof(NumericUpDown<T>), 
            new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = true });
        public NumericUpDown(Func<T, T> inc, Func<T, T> dec)
        {
            InitializeComponent();
            Inc = inc;
            Dec = dec;
        }

        public T Text
        {
            get { return (T)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        override protected void Increase(object sender, RoutedEventArgs e)
        {
            Text = Inc(Text);
        }
        override protected void Decrease(object sender, RoutedEventArgs e)
        {
            Text = Dec(Text);
        }
    }
    public class NumericUpDownInt : NumericUpDown<int>
    {
        private const int unit = 1;
        public NumericUpDownInt() : base((x) => ( x + unit), (y)=> (y - unit)) { }
    }
    public class NumericUpDownFloat : NumericUpDown<float>
    {
        private static float unit = 0.1f;
        public NumericUpDownFloat() : base((x) => (x + unit), (y) => (y - unit)) { }
    }
}
