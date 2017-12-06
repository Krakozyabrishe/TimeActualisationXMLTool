using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeActualisationXMLTool
{
    public partial class MainWindow : Window
    {
        public string settedPath;
        public static DateTime settedDateTime;
        public MainWindow()
        {
            settedPath = "none";
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setDateTimeUpDown.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,DateTime.Now.Hour,0,0);            
        }

        private void BTNSetFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                settedPath = folderBrowserDialog.SelectedPath;
        }

        private void BTNStart_Click(object sender, RoutedEventArgs e)
        {
            if (setDateTimeUpDown.Value != null)
            {
                settedDateTime = (DateTime)setDateTimeUpDown.Value;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show($"Please, set the DateTime!", "DateTime error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Program will delete all non-actual files at \'{settedPath}\' at date-time \'{settedDateTime.ToString()}\'",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == System.Windows.Forms.DialogResult.No)
                return;

            if (Directory.Exists(settedPath))
            {
                try
                {
                    string format = "yyMMdd-HHmm"; //don't mix up: MM - month; mm - minutes; hh - a.m./p.m. time; HH - military time
                    CultureInfo provider = CultureInfo.InvariantCulture;     

                    //Create list that contains filtered(deleted) XML files.
                    List<String> xmlFileNameListFiltered = Directory.GetFiles(settedPath, "*.xml").ToList()
                        .Where((s, res) => 
                        (((DateTime.ParseExact(System.IO.Path.GetFileName(s).Substring(0, 11), format, provider).CompareTo(settedDateTime) <= 0)
                        &&(DateTime.ParseExact(System.IO.Path.GetFileName(s).Substring(12, 11), format, provider).CompareTo(settedDateTime) >= 0)
                        ||((DateTime.ParseExact(System.IO.Path.GetFileName(s).Substring(0, 11), format, provider).CompareTo(settedDateTime) >= 0)))) ? false : true)
                         .ToList<string>();

                    //Remove files that are in xmlFileNameListFiltered
                    foreach (string s in xmlFileNameListFiltered)
                        File.Delete(s);

                    System.Windows.Forms.MessageBox.Show($"Removed files:{xmlFileNameListFiltered.Count}", "Non-actual files removed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //Create list that contains existed XML files.
                    List<String> xmlExistedFileNameList = Directory.GetFiles(settedPath, "*.xml").ToList()
                        .Select(s => new string( Path.GetFileName(s).ToCharArray().Reverse().ToArray()))
                        .Select(s => s.Substring(4,s.ToString().IndexOfAny("!_".ToCharArray())-4))
                        .Select(s => new string(s.ToCharArray().Reverse().ToArray()))
                        .ToList<string>();

                    int countSkippedFiles = 0;

                    StreamWriter sw = new StreamWriter(settedPath+"\\ListOfTasks.txt", false);

                    //save task's numbers to TXT file
                    foreach (string s in xmlExistedFileNameList)
                    {
                        if ((s != "") && (s.Count(x => Char.IsDigit(x)) == s.Length))
                            sw.WriteLine("WGSSA-" + s);
                        else
                            countSkippedFiles++;
                    }
                    sw.WriteLine("Skipped files: " + countSkippedFiles.ToString());

                    sw.Close();

                    System.Diagnostics.Process.Start(settedPath + "\\ListOfTasks.txt");
                }
                catch (Exception ex) // lazy exception
                {
                    System.Windows.Forms.MessageBox.Show($"The process failed: {ex.ToString()}", "Actualisation process failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else System.Windows.Forms.MessageBox.Show($"Folder \'{settedPath}\' not found!", "Folder not found",MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
    }
}
