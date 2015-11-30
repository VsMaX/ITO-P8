using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ITO_Graph;

namespace ITO_P8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ActionsVm;

            var parsedModel = ParseModel(model);

            Dfs dfs = new Dfs();
        }

        public Dictionary<string, int> ActionToIdDictionary = new Dictionary<string, int>(); 

        private object ParseModel(ActionsVm model)
        {
            var initialStateMatches = ActionRegex.Matches(model.InitialState);
            foreach (var match in initialStateMatches)
            {

                var action = new StripAction();
            }

        }

        public Regex ActionRegex = new Regex(@"\w\([\w,]+\)");
    }

    public class ActionsVm
    {
        public ActionsVm()
        {
            ViewModel = new List<ActionVm>();
        }

        public List<ActionVm> ViewModel { get; set; }
        public string InitialState { get; set; }
        public string Goal { get; set; }
    }

    public class ActionVm
    {
        public string ActionName { get; set; }
        public string Preconditions { get; set; }
        public string Posteffects { get; set; }
    }
}
