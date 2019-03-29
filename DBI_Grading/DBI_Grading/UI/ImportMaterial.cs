using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DBI_Grading.Common;
using DBI_Grading.Model.Student;
using DBI_Grading.Model.Teacher;
using DBI_Grading.Utils;
using DBI_Grading.Utils.Dao;

namespace DBI_Grading.UI
{
    public partial class ImportMaterial : Form
    {
        private List<Submission> Listsubmissions;

        public ImportMaterial()
        {
            InitializeComponent();
            // Get sql connection information from App.config
            try
            {
                usernameTextBox.Text = ConfigurationManager.AppSettings["username"];
                passwordTextBox.Text = ConfigurationManager.AppSettings["password"];
                serverNameTextBox.Text = ConfigurationManager.AppSettings["serverName"];
                initialCatalogTextBox.Text = ConfigurationManager.AppSettings["initialCatalog"];
                Constant.TimeOutInSecond = int.Parse(ConfigurationManager.AppSettings["timeOutInSecond"]);
            }
            catch (Exception e)
            {
                MessageBox.Show("Load config error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Auto Check connection import DB Question set and Answer of student for debug cho nhanh
            CheckConnectionButton_Click(null, null);
        }

        public string QuestionPath { get; set; }

        //public List<string> DBScriptList { get; set; }
        public string AnswerPath { get; set; }

        private bool IsConnectedToDB => statusConnectCheckBox.Checked;

        private void BrowseQuestionButton_Click(object sender, EventArgs e)
        {
            try
            {
                QuestionPath = FileUtils.GetFileLocation();
                if (string.IsNullOrEmpty(QuestionPath))
                    return;
                questionTextBox.Text = QuestionPath;
                //Set Number of Questions

                // Get QuestionPackage from file
                Constant.PaperSet = JsonUtils.LoadQuestion(QuestionPath) as PaperSet;

                if (Constant.PaperSet == null || Constant.PaperSet.Papers.Count == 0)
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
            if (Constant.PaperSet == null)
            {
                MessageBox.Show("You must import PaperSet first to get number question count!");
                return;
            }
            // Init List submissions
            Listsubmissions = new List<Submission>();
            try
            {
                // Get directory where student's submittion was saved
                AnswerPath = FileUtils.GetFolderLocation();
                if (string.IsNullOrEmpty(AnswerPath))
                    return;
                Application.UseWaitCursor = true;
                Text = "Import Material - Importing";
                ImportAnswerButton.Enabled = false;
                GetMarkButton.Enabled = false;
                var t = new Thread(() => SafeThreadCaller(() => GetAnswers(), ExceptionHandler));
                t.Start();
            }
            catch (Exception ex)
            {
                if (Visible)
                    answerTextBox.Text = "";
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void SafeThreadCaller(Action method, Action<Exception> handler)
        {
            try
            {
                method?.Invoke();
            }
            catch (Exception e)
            {
                handler(e);
            }
        }

        private void ExceptionHandler(Exception e)
        {
            MessageBox.Show(e.Message);
        }

        private void GetAnswers()
        {
            try
            {
                // Get all submission files
                if (string.IsNullOrEmpty(AnswerPath))
                    return;
                var directories = Directory.GetDirectories(AnswerPath);

                // Check Folder is empty or not
                if (!directories.Any())
                    throw new Exception("Folder StudentSolution was not found in " + AnswerPath);

                // Look up StudentSolution
                if (Directory.Exists(AnswerPath + @"\StudentSolution"))
                {
                    var directory = AnswerPath + @"\StudentSolution";
                    // List PaperNo
                    var paperNoPaths = Directory.GetDirectories(directory);
                    // Check bao nhieu de duoc import
                    if (!paperNoPaths.Any())
                        throw new Exception("No PaperNo was found in " + directory);
                    // Update UI
                    answerTextBox.Invoke((MethodInvoker)(() => { answerTextBox.Text = AnswerPath; }));
                    // PaperNo Found
                    foreach (var paperNoPath in paperNoPaths)
                    {
                        var paperNo = new DirectoryInfo(paperNoPath).Name;
                        // Xu ly cac folder Roll Number
                        var rollNumberPaths =
                            Directory.GetDirectories(paperNoPath); // Neu khong co roll number nao thi thoi
                        foreach (var rollNumberPath in rollNumberPaths)
                        {
                            var rollNumber = new DirectoryInfo(rollNumberPath).Name;
                            // Init submission for student to add to list
                            var submission = new Submission
                            {
                                PaperNo = paperNo,
                                StudentID = rollNumber
                            };
                            try
                            {
                                if (!Directory.Exists(rollNumberPath + @"\0"))
                                    throw new Exception("Folder 0 not found with " + rollNumber);

                                var solutionPath = rollNumberPath + @"\0"; // "0" folder
                                // Check tool cua thay co bi thieu Solution.zip khong
                                if (!File.Exists(solutionPath + @"\Solution.zip"))
                                {
                                    throw new Exception("Solution.zip was not found with " + rollNumber);
                                }
                                // Student co nop answers
                                var zipSolutionPath = solutionPath + @"\Solution.zip";
                                var extractPath = solutionPath + @"\extract";
                                // Extract zip
                                if (Directory.Exists(extractPath))
                                    Directory.Delete(extractPath, true);
                                ZipFile.ExtractToDirectory(zipSolutionPath, extractPath);
                                var answerPaths = FileUtils.GetAllSql(extractPath);

                                // Add the answer
                                foreach (var answerPath in answerPaths)
                                    try
                                    {
                                        var fileName =
                                            Path.GetFileNameWithoutExtension(answerPath); // Get q1,q2,...
                                        var questionOrder =
                                            int.Parse(StringUtils.GetNumbers(fileName)) - 1; // remove all non-numeric characters
                                        submission.ListAnswer[questionOrder] = File.ReadAllText(answerPath);
                                        submission.AnswerPaths[questionOrder] = answerPath.Substring(extractPath.Length+1); // substring without /extract
                                    }
                                    catch (Exception)
                                    {
                                        // Skip exception
                                    }
                                // Delete Extract folder
                                Directory.Delete(extractPath, true);
                            }
                            catch (Exception)
                            {
                                // Skip exception
                            }
                            Listsubmissions.Add(submission);
                        }
                    }
                }
                else
                {
                    throw new Exception("StudentSolution not found in " + AnswerPath);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Invoke((MethodInvoker)(() =>
               {
                   Application.UseWaitCursor = false;
                   Text = "Import Material";
                   ImportAnswerButton.Enabled = true;
                   GetMarkButton.Enabled = true;
               }));
            }
        }

        private void GetMarkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Listsubmissions == null || Listsubmissions.Count == 0)
                {
                    MessageBox.Show("Please import students' answers", "Error");
                }
                else if (Constant.PaperSet == null || Constant.PaperSet.Papers.Count == 0)
                {
                    MessageBox.Show("Please import Paper Set", "Error");
                }
                else if (!IsConnectedToDB)
                {
                    MessageBox.Show("Please test connect to Sql Server", "Error");
                }
                else
                {
                    General.PrepareSpCompareDatabase();
                    new Grading(Listsubmissions);
                    Hide();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void CheckConnectionButton_Click(object sender, EventArgs e)
        {
            var username = usernameTextBox.Text;
            var password = passwordTextBox.Text;
            var serverName = serverNameTextBox.Text;
            var initialCatalog = initialCatalogTextBox.Text;
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

        private void ImportMaterial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


    }
}