// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Bsw.Wpf.Utilities.Services
{
    public class DisplayMessageBox : IDisplayMessageBox
    {
        public void Display(string messageText,
                            DialogType dialogType)
        {
            MessageBoxImage image;
            string caption;
            switch (dialogType)
            {
                case DialogType.Error:
                    image = MessageBoxImage.Error;
                    caption = "Error";
                    break;
                case DialogType.Information:
                    image = MessageBoxImage.Information;
                    caption = "Information";
                    break;
                case DialogType.Warning:
                    image = MessageBoxImage.Warning;
                    caption = "Warning";
                    break;
                case DialogType.Success:
                    image = MessageBoxImage.Information;
                    caption = "Success";
                    break;
                default:
                    image = MessageBoxImage.Information;
                    caption = "Information";
                    break;
            }
            MessageBox.Show(messageText: messageText,
                            caption: caption,
                            button: MessageBoxButton.OK,
                            icon: image);
        }

        public void DisplayError(string prompt,
                                 Exception exception)
        {
            var messageText = string.Format("{0}, reason: {1}",
                                            prompt,
                                            exception.Message);
            Display(messageText: messageText,
                    dialogType: DialogType.Error);
        }
    }
}