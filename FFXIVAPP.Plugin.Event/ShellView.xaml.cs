// FFXIVAPP.Plugin.Event
// ShellView.xaml.cs
// 
// Copyright © 2013 ZAM Network LLC

using System.Windows;

namespace FFXIVAPP.Plugin.Event
{
    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView
    {
        public static ShellView View;

        public ShellView()
        {
            InitializeComponent();
            View = this;
        }

        public bool IsRendered { get; set; }

        private void ShellView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsRendered)
            {
                return;
            }
            IsRendered = true;
        }
    }
}
