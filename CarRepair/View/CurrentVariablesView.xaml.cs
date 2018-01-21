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

namespace CarRepair.View
{
    /// <summary>
    /// Interaction logic for CurrentVariablesView.xaml
    /// </summary>
    public partial class CurrentVariablesView : UserControl
    {
        public CurrentVariablesView()
        {
            InitializeComponent();
        }

        public ListView Varriables { get { return varriables; } }
    }
}
