namespace DBI_PEA_Scoring.UI
{
    partial class ImportMaterial
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.browseQuestionButton = new System.Windows.Forms.Button();
            this.browseAnswerButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.statusImportProgressBar = new System.Windows.Forms.ProgressBar();
            this.statusImportLabel = new System.Windows.Forms.Label();
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
            this.browseDatabases = new System.Windows.Forms.Button();
            this.databaseLinksTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Import Results";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Import Answer Folder";
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
            this.label3.Location = new System.Drawing.Point(34, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(345, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "We have to import question bank by file and student\'s answers by folder";
            // 
            // browseQuestionButton
            // 
            this.browseQuestionButton.Location = new System.Drawing.Point(318, 95);
            this.browseQuestionButton.Name = "browseQuestionButton";
            this.browseQuestionButton.Size = new System.Drawing.Size(63, 23);
            this.browseQuestionButton.TabIndex = 5;
            this.browseQuestionButton.Text = "Browse";
            this.browseQuestionButton.UseVisualStyleBackColor = true;
            this.browseQuestionButton.Click += new System.EventHandler(this.browseQuestionButton_Click);
            // 
            // browseAnswerButton
            // 
            this.browseAnswerButton.Location = new System.Drawing.Point(318, 134);
            this.browseAnswerButton.Name = "browseAnswerButton";
            this.browseAnswerButton.Size = new System.Drawing.Size(63, 23);
            this.browseAnswerButton.TabIndex = 6;
            this.browseAnswerButton.Text = "Browse";
            this.browseAnswerButton.UseVisualStyleBackColor = true;
            this.browseAnswerButton.Click += new System.EventHandler(this.browseAnswerButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label4.Location = new System.Drawing.Point(122, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "Import Material";
            // 
            // statusImportProgressBar
            // 
            this.statusImportProgressBar.Location = new System.Drawing.Point(33, 195);
            this.statusImportProgressBar.Name = "statusImportProgressBar";
            this.statusImportProgressBar.Size = new System.Drawing.Size(347, 23);
            this.statusImportProgressBar.TabIndex = 8;
            // 
            // statusImportLabel
            // 
            this.statusImportLabel.AutoSize = true;
            this.statusImportLabel.Location = new System.Drawing.Point(168, 175);
            this.statusImportLabel.Name = "statusImportLabel";
            this.statusImportLabel.Size = new System.Drawing.Size(76, 13);
            this.statusImportLabel.TabIndex = 10;
            this.statusImportLabel.Text = "Importing (0/0)";
            // 
            // statusConnectCheckBox
            // 
            this.statusConnectCheckBox.AutoSize = true;
            this.statusConnectCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusConnectCheckBox.ForeColor = System.Drawing.Color.DarkRed;
            this.statusConnectCheckBox.Location = new System.Drawing.Point(224, 393);
            this.statusConnectCheckBox.Name = "statusConnectCheckBox";
            this.statusConnectCheckBox.Size = new System.Drawing.Size(110, 20);
            this.statusConnectCheckBox.TabIndex = 41;
            this.statusConnectCheckBox.Text = "Disconnected";
            this.statusConnectCheckBox.UseVisualStyleBackColor = true;
            this.statusConnectCheckBox.CheckedChanged += new System.EventHandler(this.statusConnectCheckBox_CheckedChanged);
            // 
            // checkConnectionButton
            // 
            this.checkConnectionButton.Location = new System.Drawing.Point(87, 391);
            this.checkConnectionButton.Name = "checkConnectionButton";
            this.checkConnectionButton.Size = new System.Drawing.Size(117, 23);
            this.checkConnectionButton.TabIndex = 40;
            this.checkConnectionButton.Text = "Check Connection";
            this.checkConnectionButton.UseVisualStyleBackColor = true;
            this.checkConnectionButton.Click += new System.EventHandler(this.checkConnectionButton_Click);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTextBox.Location = new System.Drawing.Point(150, 317);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(236, 20);
            this.passwordTextBox.TabIndex = 2;
            this.passwordTextBox.Text = "123456";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(27, 319);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 15);
            this.label9.TabIndex = 36;
            this.label9.Text = "Password for DB";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTextBox.Location = new System.Drawing.Point(150, 282);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(236, 20);
            this.usernameTextBox.TabIndex = 1;
            this.usernameTextBox.Text = "sa";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(27, 284);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 15);
            this.label8.TabIndex = 34;
            this.label8.Text = "Username for DB";
            // 
            // initialCatalogTextBox
            // 
            this.initialCatalogTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.initialCatalogTextBox.Location = new System.Drawing.Point(150, 352);
            this.initialCatalogTextBox.Name = "initialCatalogTextBox";
            this.initialCatalogTextBox.Size = new System.Drawing.Size(236, 20);
            this.initialCatalogTextBox.TabIndex = 3;
            this.initialCatalogTextBox.Text = "master";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(27, 354);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 15);
            this.label7.TabIndex = 32;
            this.label7.Text = "Initial catalog";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverNameTextBox.Location = new System.Drawing.Point(150, 247);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(236, 20);
            this.serverNameTextBox.TabIndex = 0;
            this.serverNameTextBox.Text = "localhost";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(27, 248);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 15);
            this.label6.TabIndex = 30;
            this.label6.Text = "Server Name";
            // 
            // browseDatabases
            // 
            this.browseDatabases.Location = new System.Drawing.Point(323, 444);
            this.browseDatabases.Name = "browseDatabases";
            this.browseDatabases.Size = new System.Drawing.Size(63, 23);
            this.browseDatabases.TabIndex = 29;
            this.browseDatabases.Text = "Browse";
            this.browseDatabases.UseVisualStyleBackColor = true;
            this.browseDatabases.Click += new System.EventHandler(this.browseDatabases_Click);
            // 
            // databaseLinksTextBox
            // 
            this.databaseLinksTextBox.Enabled = false;
            this.databaseLinksTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseLinksTextBox.Location = new System.Drawing.Point(149, 446);
            this.databaseLinksTextBox.Name = "databaseLinksTextBox";
            this.databaseLinksTextBox.Size = new System.Drawing.Size(163, 20);
            this.databaseLinksTextBox.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(26, 449);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 15);
            this.label5.TabIndex = 27;
            this.label5.Text = "Import Databases";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(159, 483);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(95, 31);
            this.button3.TabIndex = 26;
            this.button3.Text = "GET MARKS";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.getMarkButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Location = new System.Drawing.Point(1, 232);
            this.label10.MaximumSize = new System.Drawing.Size(500, 3);
            this.label10.MinimumSize = new System.Drawing.Size(500, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(500, 3);
            this.label10.TabIndex = 42;
            this.label10.Text = "label10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Location = new System.Drawing.Point(-47, 429);
            this.label11.MaximumSize = new System.Drawing.Size(500, 3);
            this.label11.MinimumSize = new System.Drawing.Size(500, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(500, 3);
            this.label11.TabIndex = 43;
            this.label11.Text = "label11";
            // 
            // ImportMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 536);
            this.Controls.Add(this.label11);
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
            this.Controls.Add(this.browseDatabases);
            this.Controls.Add(this.databaseLinksTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.statusImportLabel);
            this.Controls.Add(this.statusImportProgressBar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.browseAnswerButton);
            this.Controls.Add(this.browseQuestionButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.answerTextBox);
            this.Controls.Add(this.questionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ImportMaterial";
            this.Text = "ImportMaterial";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportMaterial_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox questionTextBox;
        private System.Windows.Forms.TextBox answerTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button browseQuestionButton;
        private System.Windows.Forms.Button browseAnswerButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar statusImportProgressBar;
        private System.Windows.Forms.Label statusImportLabel;
        private System.Windows.Forms.CheckBox statusConnectCheckBox;
        private System.Windows.Forms.Button checkConnectionButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox initialCatalogTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button browseDatabases;
        private System.Windows.Forms.TextBox databaseLinksTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}

