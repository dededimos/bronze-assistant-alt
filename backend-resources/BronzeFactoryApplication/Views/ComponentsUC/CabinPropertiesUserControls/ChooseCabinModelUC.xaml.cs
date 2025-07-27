using System;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.CabinPropertiesUserControls
{
    /// <summary>
    /// Interaction logic for ChooseCabinModelUC.xaml
    /// </summary>
    public partial class ChooseCabinModelUC : UserControl
    {
        public ChooseCabinModelUC()
        {
            InitializeComponent();
        }

        private void CodeSecondaryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Up)
            {
                Keyboard.Focus(this.CodePrimaryTextBox);
                e.Handled = true;
            }
            else if (e.Key is Key.Down)
            {
                Keyboard.Focus(this.CodeTertiaryTextBox);
                e.Handled = true;
            }
        }
        private void CodePrimaryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Down)
            {
                Keyboard.Focus(this.CodeSecondaryTextBox);
                e.Handled = true;
            }
        }
        private void CodeTertiaryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Up)
            {
                Keyboard.Focus(this.CodeSecondaryTextBox);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Sets Focus to the Primary TextBox on Loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodePrimaryTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.CodePrimaryTextBox.Focus();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ChooseCabinModelViewModel vmOld)
            {
                vmOld.PropertyChanged -= OnShouldGetFocusChanged;
            }
            if (e.NewValue is ChooseCabinModelViewModel vmNew)
            {
                vmNew.PropertyChanged += OnShouldGetFocusChanged;
            }
        }

        private void OnShouldGetFocusChanged(object? sender, PropertyChangedEventArgs e)
        {
            //When there is a property change in ChooseCabinViewModel that informs this should get Focus
            //Then focus the PrimaryCode TextBox and Set afterwards the Should get Focus to False;
            if (e.PropertyName is nameof(ChooseCabinModelViewModel.ShouldGetFocus))
            {
                ChooseCabinModelViewModel? vm = this.DataContext as ChooseCabinModelViewModel;
                if (vm is not null && vm.ShouldGetFocus)
                {
                    this.CodePrimaryTextBox.Focus();
                    vm.ShouldGetFocus = false;
                }
                
                
            }
        }
    }
}
