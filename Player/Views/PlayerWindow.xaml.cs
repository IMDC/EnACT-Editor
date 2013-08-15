﻿using System;
using System.Windows;
using Microsoft.Win32;
using Player.View_Models;

namespace Player.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var playerViewModel = new PlayerViewModel();
            DataContext = playerViewModel;

            //Set up ViewModel Event handlers
            playerViewModel.PlayRequested  += (sender, args) => Player.Play();
            playerViewModel.PauseRequested += (sender, args) => Player.Pause();
            playerViewModel.StopRequested  += (sender, args) => Player.Stop();
            playerViewModel.SpeedRatioChangeRequested += (sender, args) =>
            {
//                Player.SpeedRatio = args.SpeedRatio;
//                Player.Play();
            };
        }
    }
}
