using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileTransferWpf.Elements
{
    internal class AddElements
    {
        
        public static StackPanel AddElement(StackPanel panel, RoutedEventHandler @event)
        {

            return Application.Current.Dispatcher.Invoke<StackPanel>(() =>
            {
                StackPanel stackPanel = new StackPanel();
                
                WrapPanel wrapePanel = new WrapPanel();

                Button choose = new Button();
                choose.Content = "选择文件";
                choose.Margin = new Thickness(0, 0, 5, 0);
                choose.Background = Brushes.LightBlue;
                wrapePanel.Children.Add(choose); 
            

                Button sendFile = new Button();
                sendFile.Content = "发送文件";
                sendFile.Margin = new Thickness(0, 0, 0, 0);
                sendFile.Background = Brushes.LightBlue;
                wrapePanel.Children.Add(sendFile);


                //Button reSend = new Button();
                //reSend.Content = "重新发送文件";
                //reSend.Margin = new Thickness(0, 0, 0, 0);
                //reSend.Background = Brushes.LightBlue;
                //wrapePanel.Children.Add(reSend);

                stackPanel.Children.Add(wrapePanel);

                TextBlock textBlock = new TextBlock();
                textBlock.Name = "ShowRecvText";
                
                stackPanel.Children.Add(textBlock);

                TextBox textBox = new TextBox();
                textBox.Name = "Content";
                textBox.Height = 24;
                textBox.AcceptsReturn = true;
                textBox.FontSize = 18;
                textBox.Margin= new Thickness(0, 0, 0, 5);
                stackPanel.Children.Add(textBox);


                Button sendText = new Button();
                sendText.Content = "发送";
                sendText.HorizontalAlignment = HorizontalAlignment.Center;
                sendText.Width = 200;
                sendText.Background= Brushes.LightBlue;
                sendText.Click += @event;
                stackPanel.Children.Add(sendText);

                

               



                ProgressBar progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Name = "ProgressBar";
                //progressBar.
                stackPanel.Children.Add(progressBar);
                panel.Children.Add(stackPanel);
                return stackPanel;

                });

            }



        }


    
}
