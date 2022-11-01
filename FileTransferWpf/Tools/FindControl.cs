using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileTransferWpf.Tools
{
    internal static class FindControl
    {
        //[return: NotNull]
        public static T FindByName<T>(this StackPanel panel, string name) where T : FrameworkElement
        {
            
            foreach(FrameworkElement c in panel.Children)
            {
                if (c.Name == name)
                    return (T)c;
            }
            throw new ArgumentException("null pointer");
        }


    }
}
