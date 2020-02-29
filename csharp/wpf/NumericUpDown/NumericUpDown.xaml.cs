using System;
using System.Windows;
using System.Windows.Controls;

namespace view.NumericUpDown
{
    /// <summary>
    /// NumericUpDown is a gap-filling control for wpf applications.
    /// Non-generic base class, as a workaround for wpf restictions, defining the callable properties from xaml.
    /// </summary>
    public abstract partial class NumericUpDown : UserControl
    {
        abstract protected void Increase(object sender, RoutedEventArgs e);
        abstract protected void Decrease(object sender, RoutedEventArgs e);
    }
    /// <summary>
    /// Generic base class defining the operation of the controls.
    /// </summary>
    /// <typeparam name="T">Type of the content</typeparam>
    public class NumericUpDown <T> : NumericUpDown where T : struct
    {
        System.Func<T, T> Inc, Dec;

        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(T), typeof(NumericUpDown<T>), 
            new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = true });
        /// <summary>
        /// Constructor with lambdas for increase and decrease functions
        /// </summary>
        /// <param name="inc">lambda which invoked when the plus button pressed</param>
        /// <param name="dec">lambda which invoked when the minus button pressed</param>
        public NumericUpDown(Func<T, T> inc, Func<T, T> dec)
        {
            InitializeComponent();
            Inc = inc;
            Dec = dec;
        }
        /// <summary>
        /// Visible property
        /// </summary>
        public T Text
        {
            get { return (T)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// Increase button pressed event handler.
        /// </summary>
        /// <param name="_">Not used</param>
        /// <param name="__">Not used</param>
        override protected void Increase(object _, RoutedEventArgs __)
        {
            Text = Inc(Text);
        }
        /// <summary>
        /// Increase button pressed event handler.
        /// </summary>
        /// <param name="_">Not used</param>
        /// <param name="__">Not used</param>
        override protected void Decrease(object _, RoutedEventArgs __)
        {
            Text = Dec(Text);
        }
    }
    /// <summary>
    /// Example subclass handling int.
    /// </summary>
    public class NumericUpDownInt : NumericUpDown<int>
    {
        private const int unit = 1;
        public NumericUpDownInt() : base((x) => ( x + unit), (y)=> (y - unit)) { }
    }
    /// <summary>
    /// Example subclass handling float.
    /// </summary>
    public class NumericUpDownFloat : NumericUpDown<float>
    {
        private static float unit = 0.1f;
        public NumericUpDownFloat() : base((x) => (x + unit), (y) => (y - unit)) { }
    }
}
