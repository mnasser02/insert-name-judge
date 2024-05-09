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
            public MainWindow()
            {
                //problemController = new ProblemController();
                InitializeComponent();
                //testCaseController = new TestCaseController();
                //DataContext = problemController;
                // Call your function to get the list of names
               // String[] names = problemController.GetNames();

                // Populate the ListBox with the names
                //foreach (string name in names)
                //{
                //    ProbList.Items.Add(name);
                //}
                LangDropBox.Items.Add("Java 21");
                LangDropBox.Items.Add("Python");
                LangDropBox.Items.Add("C++ 17");
                LangDropBox.Items.Add("C#");
                Input.Text = "";
                Output.Text = "";
            }


            private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                ListBox listBox = sender as ListBox;

                if (listBox != null && listBox.SelectedItem != null)
                {
                    //string selectedProblemName = listBox.SelectedItem.ToString();
                    //Problem selectedProblem = problemController.GetProblem(selectedProblemName);
                    //// Now you can display the statement of the selected problem
                    //// For example, update a TextBlock with the statement
                    //statementTextBlock.Text = selectedProblem.Statement;
                    //ProbName.Content = selectedProblem.Name;
                    //Input.Text = selectedProblem.Input_Format;
                    //Output.Text = selectedProblem.Output_Format;
                    //TestCase[] gg = testCaseController.GetTestCase(selectedProblem.Id);
                    //input1.Text = gg[0].Input;
                    //input2.Text = gg[1].Input;
                    //output1.Text = gg[0].Output;
                    //output2.Text = gg[1].Output;

                }
            }
        }
    }


