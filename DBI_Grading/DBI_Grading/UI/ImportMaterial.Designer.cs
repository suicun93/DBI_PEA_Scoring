using System.ComponentModel;
using System.Windows.Forms;

namespace DBI_PEA_Grading.UI
{
    partial class ImportMaterial
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.questionTextBox = new System.Windows.Forms.TextBox();
            this.answerTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ImportPaperSetButton = new System.Windows.Forms.Button();
            this.ImportAnswerButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.statusConnectCheckBox = new System.Windows.Forms.CheckBox();
            this.checkConnectionButton = new System.Windows.Forms.Button();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.initialCatalogTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.GetMarkButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Paper Set";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Answers Folder";
            // 
            // questionTextBox
            // 
            this.questionTextBox.Enabled = false;
            this.questionTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.questionTextBox.Location = new System.Drawing.Point(143, 96);
            this.questionTextBox.Name = "questionTextBox";
            this.questionTextBox.Size = new System.Drawing.Size(163, 20);
            this.questionTextBox.TabIndex = 2;
            // 
            // answerTextBox
            // 
            this.answerTextBox.Enabled = false;
            this.answerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.answerTextBox.Location = new System.Drawing.Point(143, 135);
            this.answerTextBox.Name = "answerTextBox";
            this.answerTextBox.Size = new System.Drawing.Size(163, 20);
            this.answerTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(44, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(325, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "We have to import Paper Set by file and student\'s answers by folder";
            // 
            // ImportPaperSetButton
            // 
            this.ImportPaperSetButton.Location = new System.Drawing.Point(318, 95);
            this.ImportPaperSetButton.Name = "ImportPaperSetButton";
            this.ImportPaperSetButton.Size = new System.Drawing.Size(63, 23);
            this.ImportPaperSetButton.TabIndex = 5;
            this.ImportPaperSetButton.Text = "Import";
            this.ImportPaperSetButton.UseVisualStyleBackColor = true;
            this.ImportPaperSetButton.Click += new System.EventHandler(this.BrowseQuestionButton_Click);
            // 
            // ImportAnswerButton
            // 
            this.ImportAnswerButton.Location = new System.Drawing.Point(318, 134);
            this.ImportAnswerButton.Name = "ImportAnswerButton";
            this.ImportAnswerButton.Size = new System.Drawing.Size(63, 23);
            this.ImportAnswerButton.TabIndex = 6;
            this.ImportAnswerButton.Text = "Import";
            this.ImportAnswerButton.UseVisualStyleBackColor = true;
            this.ImportAnswerButton.Click += new System.EventHandler(this.BrowseAnswerButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label4.Location = new System.Drawing.Point(125, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "Import Material";
            // 
            // statusConnectCheckBox
            // 
            this.statusConnectCheckBox.AutoCheck = false;
            this.statusConnectCheckBox.AutoSize = true;
            this.statusConnectCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusConnectCheckBox.ForeColor = System.Drawing.Color.DarkRed;
            this.statusConnectCheckBox.Location = new System.Drawing.Point(224, 342);
            this.statusConnectCheckBox.Name = "statusConnectCheckBox";
            this.statusConnectCheckBox.Size = new System.Drawing.Size(110, 20);
            this.statusConnectCheckBox.TabIndex = 41;
            this.statusConnectCheckBox.Text = "Disconnected";
            this.statusConnectCheckBox.UseVisualStyleBackColor = true;
            this.statusConnectCheckBox.CheckedChanged += new System.EventHandler(this.StatusConnectCheckBox_CheckedChanged);
            // 
            // checkConnectionButton
            // 
            this.checkConnectionButton.Location = new System.Drawing.Point(87, 340);
            this.checkConnectionButton.Name = "checkConnectionButton";
            this.checkConnectionButton.Size = new System.Drawing.Size(117, 23);
            this.checkConnectionButton.TabIndex = 40;
            this.checkConnectionButton.Text = "Check Connection";
            this.checkConnectionButton.UseVisualStyleBackColor = true;
            this.checkConnectionButton.Click += new System.EventHandler(this.CheckConnectionButton_Click);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTextBox.Location = new System.Drawing.Point(150, 268);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(236, 20);
            this.passwordTextBox.TabIndex = 2;
            this.passwordTextBox.Text = "123456";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(27, 270);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 15);
            this.label9.TabIndex = 36;
            this.label9.Text = "Password(Server)";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTextBox.Location = new System.Drawing.Point(150, 233);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(236, 20);
            this.usernameTextBox.TabIndex = 1;
            this.usernameTextBox.Text = "sa";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(27, 235);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 15);
            this.label8.TabIndex = 34;
            this.label8.Text = "Username(Server)";
            // 
            // initialCatalogTextBox
            // 
            this.initialCatalogTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.initialCatalogTextBox.Location = new System.Drawing.Point(150, 303);
            this.initialCatalogTextBox.Name = "initialCatalogTextBox";
            this.initialCatalogTextBox.Size = new System.Drawing.Size(236, 20);
            this.initialCatalogTextBox.TabIndex = 3;
            this.initialCatalogTextBox.Text = "master";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(27, 305);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 15);
            this.label7.TabIndex = 32;
            this.label7.Text = "Initial Catalog";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverNameTextBox.Location = new System.Drawing.Point(150, 198);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(236, 20);
            this.serverNameTextBox.TabIndex = 0;
            this.serverNameTextBox.Text = "localhost";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(27, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 15);
            this.label6.TabIndex = 30;
            this.label6.Text = "Server Name";
            // 
            // GetMarkButton
            // 
            this.GetMarkButton.AccessibleName = "";
            this.GetMarkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GetMarkButton.Location = new System.Drawing.Point(159, 382);
            this.GetMarkButton.Name = "GetMarkButton";
            this.GetMarkButton.Size = new System.Drawing.Size(95, 31);
            this.GetMarkButton.TabIndex = 26;
            this.GetMarkButton.Text = "START";
            this.GetMarkButton.UseVisualStyleBackColor = true;
            this.GetMarkButton.Click += new System.EventHandler(this.GetMarkButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Location = new System.Drawing.Point(1, 180);
            this.label10.MaximumSize = new System.Drawing.Size(500, 3);
            this.label10.MinimumSize = new System.Drawing.Size(500, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(500, 3);
            this.label10.TabIndex = 42;
            this.label10.Text = "label10";
            // 
            // ImportMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 448);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.statusConnectCheckBox);
            this.Controls.Add(this.checkConnectionButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.initialCatalogTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.GetMarkButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ImportAnswerButton);
            this.Controls.Add(this.ImportPaperSetButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.answerTextBox);
            this.Controls.Add(this.questionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImportMaterial";
            this.Text = "ImportMaterial";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportMaterial_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox questionTextBox;
        private TextBox answerTextBox;
        private Label label3;
        private Button ImportPaperSetButton;
        private Button ImportAnswerButton;
        private Label label4;
        private CheckBox statusConnectCheckBox;
        private Button checkConnectionButton;
        private TextBox passwordTextBox;
        private Label label9;
        private TextBox usernameTextBox;
        private Label label8;
        private TextBox initialCatalogTextBox;
        private Label label7;
        private TextBox serverNameTextBox;
        private Label label6;
        private Button GetMarkButton;
        private Label label10;
    }
}

