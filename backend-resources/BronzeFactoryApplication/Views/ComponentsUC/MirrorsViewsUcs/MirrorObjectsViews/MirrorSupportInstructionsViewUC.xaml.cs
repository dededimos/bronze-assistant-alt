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

namespace BronzeFactoryApplication.Views.ComponentsUC.MirrorsViewsUcs.MirrorObjectsViews
{
    /// <summary>
    /// Interaction logic for MirrorSupportInstructionsViewUC.xaml
    /// </summary>
    public partial class MirrorSupportInstructionsViewUC : UserControl
    {
        public MirrorSupportInstructionsViewUC()
        {
            InitializeComponent();
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MirrorSupportInstructionsViewUC), new PropertyMetadata(defaultValue:string.Empty));




    }
}
