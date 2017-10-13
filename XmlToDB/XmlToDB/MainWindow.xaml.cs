using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using XmlToDB.Models;

namespace XmlToDB
{
    public partial class MainWindow : Window
    {
        public new DataSync DataContext
        {
            get { return base.DataContext as DataSync; }
            set { this.DataContext = value; }
        }

        public MainWindow()
        {
            //устанавливаем свойство DataDirectory так чтобы оно всегда указывало в корень проекта
            //AppDomain.CurrentDomain.SetData("DataDirectory",
            //    string.Concat(Environment.CurrentDirectory.Split('\\').TakeWhile(n => !n.Equals("bin", StringComparison.InvariantCultureIgnoreCase)).Select(n => n + @"\")));

            //DataDirectory устанавливается в месте вызова приложения
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);
            InitializeComponent();
        }

        private void PathFinder_Click(object sender, RoutedEventArgs e)
        {
            DataContext.Open();
        }

        private void LoadXML_Click(object sender, RoutedEventArgs e)
        {
            DataContext.Deserialize();
        }

        private void SerializeData_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectPath();
            DataContext.Serialize();  
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            DataContext.Sync();
        }
    }
}
