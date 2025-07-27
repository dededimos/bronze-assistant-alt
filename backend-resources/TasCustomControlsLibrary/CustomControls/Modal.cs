﻿using System;
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

namespace TasCustomControlsLibrary.CustomControls
{
    /// <summary>
    /// A Modal Control to be Used inside any other Control (Usually a Window)
    /// </summary>
    public class Modal : ContentControl
    {
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Modal), new PropertyMetadata(false));

        //inherit from content control so to accept template bindings ! like the content control
        static Modal()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(typeof(Modal)));
            BackgroundProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(CreateDefaultBackground()));
        }

        /// <summary>
        /// Creates a Default Dimming Background for the Outer area of the Modal (the area not containing the modal)
        /// </summary>
        /// <returns></returns>
        private static object CreateDefaultBackground()
        {
            return new SolidColorBrush(Colors.Black) { Opacity = 0.5 };
        }
    }
}
