﻿using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Squirrel;
using James.HelperClasses;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        public GeneralUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Uninstalls the application if it was deliverd by using squirrel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UninstallProgram(object sender, RoutedEventArgs e)
        {
            var manager = new UpdateManager(Config.Instance.ReleaseUrl);
            await manager.FullUninstall();
            Environment.Exit(0);
        }

        private void CloseProgram(object sender, RoutedEventArgs e) => Environment.Exit(0);

        /// <summary>
        /// Resets the config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ResetConfig(object sender, RoutedEventArgs e)
        {
            (await MetroDialogHelper.ShowDialog(this, "Reset config", "Are you sure that you want to reset your config? All edits in the config will be permanently removed!"))
            .OnSuccess(() =>
            {
                Config.Instance.ResetConfig();
                DataContext = Config.Instance;
            });
        }

        private void LaunchWelcomeWindow(object sender, RoutedEventArgs e) => new Windows.WelcomeWindow(false).Show();

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => DataContext = Config.Instance;
    }
}