using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private bool importForDebug = false;
        public ImportMaterial()
        {
            InitializeComponent();

            // Auto Check connection import DB QUestion set and Answer of student for debug cho nhanh
            CheckConnectionButton_Click(null, null);
            if (importForDebug)
            {
                BrowseQuestionButton_Click(null, null);
                BrowseAnswerButton_Click(null, null);
            }
        }
        private void BrowseQuestionButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get link to file
                if (!importForDebug)
                    QuestionPath = FileUtils.GetFileLocation();
                else
                    // Bao
                    //QuestionPath = @"E:\OneDrive\000 SWP\Sample\DBI_Exam\03_Sample_for_Testing_New_Phase_(09.03)\01_From_Shuffle\PaperSet.dat";
                    // Duc
                    QuestionPath = @"C:\Users\hoangduc\Desktop\PaperSet.dat";
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
                if (Visible)
                    questionTextBox.Text = "";
            }
        }

        private void BrowseAnswerButton_Click(object sender, EventArgs e)
        {
            // Init List submissions
            Listsubmissions = new List<Submission>();
            try
            {
                // Get directory where student's submittion was saved
                if (!importForDebug)
                    AnswerPath = FileUtils.GetFolderLocation();
                else
                    // Bao
                    //AnswerPath = @"D:\Sys\Desktop\tmp";
                    // Duc
                    AnswerPath = @"C:\Users\hoangduc\Desktop\02_From_Submission";
                Application.UseWaitCursor = true;
                Text = "Import Material - Importing";
                ImportAnswerButton.Enabled = false;
                ThreadPool.QueueUserWorkItem(IDontKnowWhatItIs => GetAnswers(AnswerPath));
            }
            catch (Exception ex)
            {
                if (Visible)
                    answerTextBox.Text = "";
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void GetAnswers(string AnswerPath)
        {
            try
            {
                // Get all submission files
                string[] Directories = Directory.GetDirectories(AnswerPath);

                // Check Folder is empty or not
                if (Directories.Count() == 0)
                    throw new Exception("Folder StudentSolution was not found in " + AnswerPath);

                // Look up StudentSolution
                if (Directory.Exists(AnswerPath + "/StudentSolution"))
                {
                    string directory = AnswerPath + "/StudentSolution";
                    // List PaperNo
                    string[] paperNoPaths = Directory.GetDirectories(directory);
                    // Check bao nhieu de duoc import
                    if (paperNoPaths.Count() == 0)
                        throw new Exception("No PaperNo was found in " + directory);
                    // Update UI
                    answerTextBox.Invoke((MethodInvoker)(() =>
                    {
                        answerTextBox.Text = AnswerPath;
                    }));
                    // PaperNo Found
                    foreach (string paperNoPath in paperNoPaths)
                    {
                        string paperNo = new DirectoryInfo(paperNoPath).Name;
                        // Xu ly cac folder Roll Number
                        string[] rollNumberPaths = Directory.GetDirectories(paperNoPath); // Neu khong co roll number nao thi thoi
                        foreach (string rollNumberPath in rollNumberPaths)
                        {
                            string rollNumber = new DirectoryInfo(rollNumberPath).Name;
                            string[] solutionPaths = Directory.GetDirectories(rollNumberPath);
                            // Init submission for student to add to list
                            Submission submission = new Submission
                            {
                                PaperNo = paperNo,
                                StudentID = rollNumber
                            };
                            try
                            {
                                string solutionPath = solutionPaths[0]; // "0" folder
                                string[] zipFiles = Directory.GetFiles(solutionPath, "*.zip");
                                // Check co nop bai hay khong
                                if (zipFiles.Count() > 0)
                                {
                                    // Student co nop answers
                                    string zipSolutionPath = zipFiles[0];

                                    // Extract zip
                                    if (Directory.Exists(solutionPath + "/extract"))
                                        Directory.Delete(solutionPath + "/extract", true);
                                    ZipFile.ExtractToDirectory(zipSolutionPath, solutionPath + "/extract");
                                    string[] answerPaths = Directory.GetFiles(solutionPath + "/extract", "*.sql");

                                    // Add the answer
                                    foreach (string answerPath in answerPaths)
                                        try
                                        {
                                            string fileName = Path.GetFileNameWithoutExtension(answerPath); // Get q1,q2,...
                                            int questionOrder = int.Parse(fileName.Remove(0, 1)) - 1; // remove "q/Q" letter
                                            submission.ListAnswer[questionOrder] = File.ReadAllText(answerPath);
                                        }
                                        catch (Exception)
                                        {
                                            // skip
                                        }
                                    Directory.Delete(solutionPath + "/extract", true);
                                }
                            }
                            catch (Exception)
                            {
                                // skip
                            }
                            Listsubmissions.Add(submission);
                        }
                    }
                }
                else
                    throw new Exception("StudentSolution not found in " + AnswerPath);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Invoke((MethodInvoker)(() =>
                {
                    Application.UseWaitCursor = false;
                    Text = "Import Material";
                    ImportAnswerButton.Enabled = true;
                }));
            }

        }

        private void GetMarkButton_Click(object sender, EventArgs e)
        {
            if (Listsubmissions == null || Listsubmissions.Count == 0)
                MessageBox.Show("Please import students' answers", "Error");
            else
                if (PaperSet == null || PaperSet.Papers.Count == 0)
                MessageBox.Show("Please import Paper Set", "Error");
            else
                if (!IsConnectedToDB)
                MessageBox.Show("Please test connect to Sql Server", "Error");
            else
                if (General.PrepareSpCompareDatabase())
            {
                var scoring = new Scoring(PaperSet, Listsubmissions);
                Hide();
            }
            else
                MessageBox.Show("DB connection error or Can not create sp_Compare!", "Error");
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

        private void BrowseDatabases_Click(object sender, EventArgs e)
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
            if (IsConnectedToDB)
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
        private bool IsConnectedToDB => statusConnectCheckBox.Checked;

        private void ImportMaterial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
