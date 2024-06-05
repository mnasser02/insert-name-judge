using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Modules;



namespace Client
{
   
        public partial class MainWindow : Window
        {
        //  private ProblemController problemController;
        // private TestCaseController testCaseController;
      static  IPHostEntry ipHostEntry = Dns.GetHostEntry("localhost");
           static IPAddress serverIp = ipHostEntry.AddressList[0];

          static  ClientApp client = new(serverIp);
        public MainWindow()
        {
            //Adding problem names to the list
            InitializeComponent();
          
            Language.Items.Add("java");
            Language.Items.Add("py");
            Language.Items.Add("cpp");
          
          

            
            List<(int, string)> problems = client.GetProblemsIdsNames();
            foreach (var (id, name) in problems)
            {
                TextBlock textblock = new TextBlock()
                {
                    Text= $"{id} {name}"
                };
               ProblemListBox.Items.Add(textblock);
            }

          

        }
         

    

  



    

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            var selectedItem = (TextBlock)ProblemListBox.SelectedItem;
            int id = int.Parse(selectedItem.Text.Split(' ')[0]);
            Solution gg = new Solution(id, Language.SelectedItem.ToString(), Code.Text);
            String verdict =client.SubmitSolution(gg);
            Verdict.Text = verdict;
        }

        private void ProblemListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            
            ListBox listBox = sender as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
            {
                // Get the selected item
                var selectedItem = (TextBlock)ProblemListBox.SelectedItem;
                int id = int.Parse(selectedItem.Text.Split(' ')[0]);
                 Problem problem=client.GetProblem(id);
                input.Visibility = Visibility.Visible;
                output.Visibility = Visibility.Visible;


                ProblemName.Text =""+ problem.Id + ". " + problem.Name;
                ProblemRating.Text = problem.Rating+"";
                ProblemStatment.Text = problem.Statement;
                ProblemOutput.Text = problem.OutputFormat;
                PrblemInput.Text =problem.InputFormat+"";





            }
        }

      
    }
    }


