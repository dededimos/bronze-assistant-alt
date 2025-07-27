using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BronzeFactoryApplication.Helpers
{
    //UNFINISHED!
    public static class WindowsExtensions
    {
        public enum PositionWindowToParent
        {
            Center = 0,
            RightOut = 1,
            RightIn = 2,
            LeftOut = 3,
            LeftIn = 4,
            TopOut = 5,
            TopIn = 6,
            BottomOut = 7,
            BottomIn = 8,
        }
        public enum PositionWindowTo
        {
            Center,
            LeftCenter,
            RightCenter,
            TopCenter,
            BottomCenter,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public static void PositionWindow(this Window window, PositionWindowTo preferedPosition = PositionWindowTo.Center, double margin = 20)
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window),$"Could Not Position Window");
            }
            else
            {
                switch (preferedPosition)
                {
                    case PositionWindowTo.LeftCenter:
                        window.Left = margin;
                        window.Top = (screenHeight - window.Height) / 2 ;
                        break;
                    case PositionWindowTo.RightCenter:
                        break;
                    case PositionWindowTo.TopCenter:
                        break;
                    case PositionWindowTo.BottomCenter:
                        break;
                    case PositionWindowTo.TopLeft:
                        break;
                    case PositionWindowTo.TopRight:
                        break;
                    case PositionWindowTo.BottomLeft:
                        break;
                    case PositionWindowTo.BottomRight:
                        break;
                    case PositionWindowTo.Center:
                    default:
                        break;
                }
            }
        }

        public static void PositionWindowToOwner(this Window window ,PositionWindowToParent preferedPosition = PositionWindowToParent.Center )
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            //Only Position Owner
            if (window.Owner is null)
            {
                if (screenHeight <= window.Height || screenWidth <= window.Width)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.Top = (screenHeight - window.Height) / 2;
                    window.Left = (screenWidth - window.Width) / 2;
                }
            }
            else
            {
                
                switch (preferedPosition)
                {
                    case PositionWindowToParent.RightOut:
                        break;
                    case PositionWindowToParent.RightIn:
                        break;
                    case PositionWindowToParent.LeftOut:
                        break;
                    case PositionWindowToParent.LeftIn:
                        break;
                    case PositionWindowToParent.TopOut:
                        break;
                    case PositionWindowToParent.TopIn:
                        break;
                    case PositionWindowToParent.BottomOut:
                        break;
                    case PositionWindowToParent.BottomIn:
                        break;
                    case PositionWindowToParent.Center:
                    default:
                        window.Left = window.Owner.Left + (window.Owner.Width - window.Width) / 2;
                        window.Top = window.Owner.Top + (window.Owner.Height - window.Height) / 2;
                        break;
                }
            }
        }

    }
}
