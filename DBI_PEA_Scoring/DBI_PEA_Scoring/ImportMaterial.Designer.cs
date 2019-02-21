namespace DBI_PEA_Scoring
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
            this.getMarkButton = new System.Windows.Forms.Button();
            this.statusImportLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Import Question";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Import Answer Folder";
            // 
            // questionTextBox
            // 
            this.questionTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.questionTextBox.Location = new System.Drawing.Point(168, 96);
            this.questionTextBox.Name = "questionTextBox";
            this.questionTextBox.Size = new System.Drawing.Size(120, 21);
            this.questionTextBox.TabIndex = 2;
            // 
            // answerTextBox
            // 
            this.answerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.answerTextBox.Location = new System.Drawing.Point(168, 135);
            this.answerTextBox.Name = "answerTextBox";
            this.answerTextBox.Size = new System.Drawing.Size(120, 21);
            this.answerTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(345, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "We have to import question bank by file and student\'s answers by folder";
            // 
            // browseQuestionButton
            // 
            this.browseQuestionButton.Location = new System.Drawing.Point(306, 95);
            this.browseQuestionButton.Name = "browseQuestionButton";
            this.browseQuestionButton.Size = new System.Drawing.Size(75, 23);
            this.browseQuestionButton.TabIndex = 5;
            this.browseQuestionButton.Text = "Browse";
            this.browseQuestionButton.UseVisualStyleBackColor = true;
            // 
            // browseAnswerButton
            // 
            this.browseAnswerButton.Location = new System.Drawing.Point(306, 134);
            this.browseAnswerButton.Name = "browseAnswerButton";
            this.browseAnswerButton.Size = new System.Drawing.Size(75, 23);
            this.browseAnswerButton.TabIndex = 6;
            this.browseAnswerButton.Text = "Browse";
            this.browseAnswerButton.UseVisualStyleBackColor = true;
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
            // getMarkButton
            // 
            this.getMarkButton.Location = new System.Drawing.Point(168, 237);
            this.getMarkButton.Name = "getMarkButton";
            this.getMarkButton.Size = new System.Drawing.Size(75, 23);
            this.getMarkButton.TabIndex = 9;
            this.getMarkButton.Text = "Get Marks";
            this.getMarkButton.UseVisualStyleBackColor = true;
            // 
            // statusImportLabel
            // 
            this.statusImportLabel.AutoSize = true;
            this.statusImportLabel.Location = new System.Drawing.Point(167, 175);
            this.statusImportLabel.Name = "statusImportLabel";
            this.statusImportLabel.Size = new System.Drawing.Size(76, 13);
            this.statusImportLabel.TabIndex = 10;
            this.statusImportLabel.Text = "Importing (0/0)";
            // 
            // ImportMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 279);
            this.Controls.Add(this.statusImportLabel);
            this.Controls.Add(this.getMarkButton);
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
            this.Load += new System.EventHandler(this.ImportMaterial_Load);
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
        private System.Windows.Forms.Button getMarkButton;
        private System.Windows.Forms.Label statusImportLabel;
    }
}

