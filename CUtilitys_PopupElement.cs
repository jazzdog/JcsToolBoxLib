using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
//using System.Xaml;

namespace ToolBoxLib
{
    public static partial class CUtil
    {
        private static System.Windows.Controls.Primitives.Popup PopupElement = new System.Windows.Controls.Primitives.Popup();
        public static void ShowPopupElement()
        {
            PopupElement.IsOpen = !PopupElement.IsOpen;
            /*if(PopupElement.IsOpen)
            {
                PopupElement.StaysOpen = false;
            }*/
        }
        public static void SetPopupElementContent(UIElement PlacementTarget, UIElement ContentItem, Rectangle ElementRect, bool StaysOpen = true, System.Windows.Controls.Primitives.PlacementMode PlaceMent = System.Windows.Controls.Primitives.PlacementMode.Absolute)
        {
            PopupElement.PlacementTarget = PlacementTarget;
            PopupElement.Child = null;
            PopupElement.Child = ContentItem;
            PopupElement.Width = ElementRect.Width;
            PopupElement.Height = ElementRect.Height;
            PopupElement.HorizontalOffset = ElementRect.X;
            PopupElement.VerticalOffset = ElementRect.Y;

            PopupElement.StaysOpen = StaysOpen;
            PopupElement.Placement = PlaceMent;

            try
            {
                PlacementTarget.PreviewMouseLeftButtonUp -= PlacementTarget_PreviewMouseLeftButtonUp;
            }
            catch
            {
            }
            PlacementTarget.PreviewMouseLeftButtonUp += PlacementTarget_PreviewMouseLeftButtonUp;
        }

        private static void PlacementTarget_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupElement.IsOpen = false;
        }

    }
}