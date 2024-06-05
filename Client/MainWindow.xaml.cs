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
using System.Windows.Media.Animation;
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

       static IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");

        static IPAddress serverIp = iPHostEntry.AddressList[0];
        //static IPAddress serverIp =IPAddress.Parse("192.168.137.212");

          static  client client = new(serverIp);
        public MainWindow()
        {
            //Adding problem names to the list
            InitializeComponent();
          
            Language.Items.Add("java");
            Language.Items.Add("py");
            Language.Items.Add("cpp");
            Submit.Background = new SolidColorBrush(Colors.Transparent);

            Submit.MouseEnter += Submit_MouseEnter;
            Submit.MouseLeave += Submit_MouseLeave;




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
        private void Submit_MouseEnter(object sender, MouseEventArgs e)
        {
            // Create a ColorAnimation to change the background color to gray
            var colorAnimation = new ColorAnimation
            {
                To = Colors.Gray,
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };

            // Apply the animation to the background brush
            var brush = (SolidColorBrush)Submit.Background;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private void Submit_MouseLeave(object sender, MouseEventArgs e)
        {
            // Create a ColorAnimation to change the background color back to transparent
            var colorAnimation = new ColorAnimation
            {
                To = Colors.Transparent,
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };

            // Apply the animation to the background brush
            var brush = (SolidColorBrush)Submit.Background;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }









        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Submit.IsEnabled = false;
            
            if (Language.SelectedItem == null)
            {
                Verdict.Text = "Please select a language";
                Verdict.Foreground = Brushes.White;
            }
            else
            {
                Verdict.Text = "";
                await Task.Delay(300);
                var selectedItem = (TextBlock)ProblemListBox.SelectedItem;
                int id = int.Parse(selectedItem.Text.Split(' ')[0]);
                Solution gg = new Solution(id, Language.SelectedItem.ToString(), Code.Text);
                String verdict = client.SubmitSolution(gg);
                if (verdict.StartsWith("Acc")) Verdict.Foreground = Brushes.Green;
                else Verdict.Foreground = Brushes.Red;
                Verdict.Text = verdict;
            }
            await Task.Delay(2000);
            Submit.IsEnabled = true;
        }
        private void Code_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    // Insert a tab character at the current caret position
                    int caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, "\t");
                    textBox.CaretIndex = caretIndex + 1;

                    // Prevent the tab key from moving the focus to the next control
                    e.Handled = true;
                }
            }
        }

        private void ProblemListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            
            ListBox listBox = sender as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
            {
                Verdict.Text = "";
                // Get the selected item
                var selectedItem = (TextBlock)ProblemListBox.SelectedItem;
                int id = int.Parse(selectedItem.Text.Split(' ')[0]);
                 Problem problem=client.GetProblem(id);
                input.Visibility = Visibility.Visible;
                output.Visibility = Visibility.Visible;
                Ex.Visibility = Visibility.Visible;
                Code.Visibility = Visibility.Visible;
                Language.Visibility = Visibility.Visible;
                Submit.Visibility = Visibility.Visible;
                ProblemRatingBorder.Visibility = Visibility.Visible;
               

                ProblemName.Text =""+ problem.Id + ". " + problem.Name;
                ProblemRating.Text = problem.Rating+"";
                ProblemStatment.Text = problem.Statement;
                ProblemOutput.Text = problem.OutputFormat;
                PrblemInput.Text =problem.InputFormat+"";
                Example.Text = "Input:\n"+problem.ExampleInput+"\n\n"+"Output:\n"+problem.ExampleOutput+"\n";


            }
        }

      
    }
    }


