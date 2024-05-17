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
using Modules;



namespace Client
{
   
        public partial class MainWindow : Window
        {
        //  private ProblemController problemController;
        // private TestCaseController testCaseController;
        private List<(int, string)> problemList = new List<(int, string)>
        {
            (1, "Problem 1"),
            (2, "Problem 2"),
            (3, "Problem 3")
        };
        public MainWindow()
        {
            //Adding problem names to the list
            InitializeComponent();
            List<(int, string)> ProblemList = client.RequestProblems(30);
            foreach (var problem in problemList)
            {
                ProblemListBox.Items.Add($"{problem.Item1}: {problem.Item2}");
            }
            Language.Items.Add("JAVA 21");

            Language.Items.Add("Python");
            Language.Items.Add("C++ 17");

        }
         

        private void codeTextBox_TextChanged()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ProblemListBox.Visibility == Visibility.Collapsed)
            {
                ProblemListBox.Visibility = Visibility.Visible;
               // ProblemListIcon.Content = "Hide Problems";
            }
            else
            {
                ProblemListBox.Visibility = Visibility.Collapsed;
              //  ProblemListIcon.Content = "Show Problems";
            }
        }



    

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedItem = ProblemListBox.SelectedItem.ToString();
            int id = int.Parse(selectedItem.Substring(0, 1));
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
                var selectedItem = ProblemListBox.SelectedItem.ToString();
                int id = int.Parse(selectedItem.Substring(0, 1));
                 Problem problem=client.RequestProblem(id); 
                ProblemListBox.Visibility = Visibility.Collapsed;

                ProblemName.Text = problem.Id + ". " + problem.Name;
                ProblemRating.Text = problem.Rating+"";
                ProblemStatment.Text = problem.Statement;
                ProblemOutput.Text = problem.OutputFormat;
                PrblemInput.Text = problem.InputFormat;





            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
    }


