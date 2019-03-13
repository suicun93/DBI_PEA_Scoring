using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Model.Teacher;
using DBI_PEA_Scoring.Utils;
using DBI_PEA_Scoring.Utils.Dao;

namespace DBI_PEA_Scoring.UI
{
    public partial class ImportMaterial : Form
    {
        public string QuestionPath { get; set; }
        //public List<string> DBScriptList { get; set; }
        public string AnswerPath { get; set; }
        private PaperSet PaperSet;
        private List<Submission> Listsubmissions;
        //private bool importedDB = false;
        public ImportMaterial()
        {
            InitializeComponent();
            // Auto Check connection import DB QUestion set and Answer of student for debug cho nhanh
            try
            {
                CheckConnectionButton_Click(null, null);
            }
            catch (Exception) { }
            
            try
            {
                BrowseQuestionButton_Click(null, null);
            }
            catch (Exception) { }
        }
        private void BrowseQuestionButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get link to file
                //QuestionPath = FileUtils.GetFileLocation();
                // Bao
                QuestionPath = @"E:\OneDrive\000 SWP\Sample\DBI_Exam\03_Sample_for_Testing_New_Phase_(09.03)\01_From_Shuffle\PaperSet.dat";
                // Duc
                //  = @"C:\Users\hoangduc\Desktop\PaperSet.dat";
                questionTextBox.Text = QuestionPath;
                // Get QuestionPackage from file
                PaperSet = null;
                PaperSet = JsonUtils.LoadQuestion(QuestionPath) as PaperSet;
                Constant.DBScriptList = PaperSet.DBScriptList;
                if (PaperSet == null || PaperSet.Papers.Count == 0)
                    throw new Exception("No question was found!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                questionTextBox.Text = "";
            }
        }

        private void BrowseAnswerButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get directory where student's submittion was saved
                //AnswerPath = FileUtils.GetFolderLocation();
                // Bao
                AnswerPath = @"D:\Sys\Desktop\tmp";
                // Duc
                //AnswerPath = @"C:\Users\hoangduc\Desktop\02_From_Submission";
                answerTextBox.Text = AnswerPath;
                // Get all submission files
                string[] submissionFiles = Directory.GetFiles(AnswerPath, "*.dat");
                if (submissionFiles.Count() == 0)
                    throw new Exception("No submittion was found");
                Loadsubmission(submissionFiles);
            }
            catch (Exception ex)
            {
                answerTextBox.Text = "";
                // Setup for status bar
                statusImportAnswerProgressBar.Maximum = 1;
                statusImportAnswerProgressBar.Value = 0;
                statusImportAnswerLabel.Text = "Imported " + "0/0";
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Loadsubmission(string[] files)
        {
            // Init List submissions
            Listsubmissions = new List<Submission>();
            // Setup for status bar
            int submissionCount = files.Count();
            statusImportAnswerProgressBar.Maximum = submissionCount;
            statusImportAnswerProgressBar.Step = 1;
            statusImportAnswerProgressBar.Value = 0;
            int count = 0;
            statusImportAnswerLabel.Text = "Imported " + count + "/" + submissionCount;
            // Setup for decrypt answer of student
            SecureJsonSerializer<Submission> secureJsonSerializer = new SecureJsonSerializer<Submission>();
            foreach (string file in files)
            {
                try
                {
                    Submission submission = secureJsonSerializer.Load(file);
                    Listsubmissions.Add(submission);
                    // Change value status bar
                    count++;
                    statusImportAnswerLabel.Text = "Imported " + count + "/" + submissionCount;
                    statusImportAnswerLabel.Refresh();
                    statusImportAnswerProgressBar.Invoke(new MethodInvoker(() => statusImportAnswerProgressBar.Value = count));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            secureJsonSerializer = null;
            // After loading successfully, diable browse button because jsonSerialize is error if load again.
            ImportAnswerButton.Enabled = false;
            //MessageBox.Show("Loaded :" + Listsubmissions.Count + " submissions.");
        }

        private void GetMarkButton_Click(object sender, EventArgs e)
        {
            if (Listsubmissions == null || Listsubmissions.Count == 0)
                MessageBox.Show("Please import students' answers", "Error");
            else
                if (PaperSet.Papers == null || PaperSet.Papers.Count == 0)
                MessageBox.Show("Please import Paper Set", "Error");
            else
                if (!isConnectedToDB())
                MessageBox.Show("Please test connect to Sql Server", "Error");
            //else
            //    if (!importedDB)
            //    MessageBox.Show("Enter Database!", "Error");
            else
            {
                if (General.PrepareSpCompareDatabase())
                {
                    var scoring = new Scoring(PaperSet, Listsubmissions);
                    Hide();
                }
                else
                {
                    MessageBox.Show("Database connection error!", "Error");
                }

            }
        }

        private void CheckConnectionButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            string serverName = serverNameTextBox.Text;
            string initialCatalog = initialCatalogTextBox.Text;
            try
            {
                if (General.CheckConnection(serverName, username, password, initialCatalog))
                    statusConnectCheckBox.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not connect, check again.\n" + ex.Message, "Error");
            }
        }

        private void browseDatabases_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    // Get directory where student's submittion was saved
            //    DBPath = FileUtils.GetFolderLocation();
            //    dbPathTextBox.Text = DBPath;
            //    // Get all submission files
            //    string[] DBScriptFiles = Directory.GetFiles(DBPath, "*.sql");
            //    if (DBScriptFiles.Count() == 0)
            //        throw new Exception("No DB Script was found");
            //    else
            //        // Get SQL Script files path successfully -> process to generate DB
            //        LoadDBScript(DBScriptFiles);
            //}
            //catch (Exception ex)
            //{
            //    dbPathTextBox.Text = "";
            //    statusImportDatabaseProgressBar.Maximum = 0;
            //    statusImportDatabaseProgressBar.Step = 1;
            //    statusImportDatabaseProgressBar.Value = 0;
            //    MessageBox.Show("Imported DB Failed\n" + ex.Message, "Error");
            //}
        }

        //private void LoadDBScript(string[] dbScriptFiles)
        //{
        //    // reset listDB
        //    Constant.listDB = null;
        //    int dbCount = dbScriptFiles.Count();
        //    statusImportDatabaseProgressBar.Maximum = dbCount;
        //    statusImportDatabaseProgressBar.Step = 1;
        //    statusImportDatabaseProgressBar.Value = 0;
        //    int count = 0;
        //    // Run and save information to local
        //    foreach (string scriptFile in dbScriptFiles)
        //    {
        //        try
        //        {
        //            // Run file to generate database
        //            RunFile(scriptFile);
        //        }
        //        catch (Exception)
        //        {
        //            //MessageBox.Show(e.Message + " at file " + Path.GetFileName(scriptFile), "Error");
        //        }
        //        // Get DB names and paths into constant
        //        General.SavePathDB();
        //        // Success -> change UI
        //        count++;
        //        statusImportDatabaseProgressBar.Value = count;

        //    }
        //    // If no DBs were imported -> throw exception
        //    if (Constant.listDB == null)
        //        throw new Exception("No DB scripts run");
        //    // If Db was imported
        //    importedDB = true;
        //    browseDatabasesButton.Enabled = false;
        //    // Report to user
        //    string report = "Imported \n";
        //    foreach (Database dbName in Constant.listDB)
        //        report += "\t" + dbName.SourceDbName + "\n";
        //    MessageBox.Show(report, "Success");
        //}

        //private void RunFile(string url)
        //{
        //    FileInfo file = new FileInfo(url);
        //    string script = file.OpenText().ReadToEnd();
        //    General.ExecuteSingleQuery(script);
        //}

        private void StatusConnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnectedToDB())
            {
                usernameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
                serverNameTextBox.Enabled = false;
                initialCatalogTextBox.Enabled = false;
                statusConnectCheckBox.ForeColor = Color.Green;
                statusConnectCheckBox.Text = "Sql Connected";
                checkConnectionButton.Enabled = false;
            }
        }
        private bool isConnectedToDB()
        {
            return statusConnectCheckBox.Checked;
        }

        private void ImportMaterial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
