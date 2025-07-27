using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.CabinPropertiesUserControls
{
    /// <summary>
    /// Interaction logic for CabinCalculationsTable.xaml
    /// </summary>
    public partial class CabinCalculationsTable : UserControl
    {
        //Define a Routed Event Here and Connect it with a .Net Event Below
        public static readonly RoutedEvent StartStoryboardEvent = 
            EventManager.RegisterRoutedEvent(
                "StartStoryboard", 
                RoutingStrategy.Tunnel, 
                typeof(RoutedEventHandler), 
                typeof(CabinCalculationsTable));
        private DependencyPropertyDescriptor? dpd;
        public CabinCalculationsTable()
        {
            InitializeComponent();
        }

        //This event bubbles to the defined routed event
        public event RoutedEventHandler StartStoryboard
        {
            add { AddHandler(StartStoryboardEvent, value); }
            remove { RemoveHandler(StartStoryboardEvent, value); }
        }
        /// <summary>
        /// Raise the StartStoryboard Event
        /// </summary>
        protected virtual void RaiseStartStoryboard()
        {
            RaiseEvent(new RoutedEventArgs(CabinCalculationsTable.StartStoryboardEvent,this));
        }

        /// <summary>
        /// Adds a Handler to listen when the Value of the Items Source Property of the GlassesItemsControl Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ItemsControl));
            dpd.AddValueChanged(this.GlassesItemsControl, Glasses_Changed);
        }

        /// <summary>
        /// Starts the Storyboard whenever the Glasses Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Glasses_Changed(object? sender, EventArgs e)
        {
            RaiseStartStoryboard();
        }

        /// <summary>
        /// Removes the Glasses Changed Handler when the User Control is Unloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            dpd?.RemoveValueChanged(this.GlassesItemsControl, Glasses_Changed);
        }
    }
}
