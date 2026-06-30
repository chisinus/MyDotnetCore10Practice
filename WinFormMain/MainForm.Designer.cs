namespace WinFormMain
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            cbUserActions = new ComboBox();
            richTextBox1 = new RichTextBox();
            label2 = new Label();
            btnDynamoDBGo = new Button();
            cbUsers = new ComboBox();
            btnSQSSubmit = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 31);
            label1.Name = "label1";
            label1.Size = new Size(30, 15);
            label1.TabIndex = 0;
            label1.Text = "User";
            // 
            // cbUserActions
            // 
            cbUserActions.FormattingEnabled = true;
            cbUserActions.Items.AddRange(new object[] { "Add", "Update", "Delete", "Search" });
            cbUserActions.Location = new Point(92, 27);
            cbUserActions.Name = "cbUserActions";
            cbUserActions.Size = new Size(174, 23);
            cbUserActions.TabIndex = 2;
            cbUserActions.Text = "Add";
            cbUserActions.SelectedIndexChanged += cbUserActions_SelectedIndexChanged;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(19, 291);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(1189, 445);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(19, 273);
            label2.Name = "label2";
            label2.Size = new Size(47, 15);
            label2.TabIndex = 4;
            label2.Text = "Results";
            // 
            // btnDynamoDBGo
            // 
            btnDynamoDBGo.Location = new Point(578, 27);
            btnDynamoDBGo.Name = "btnDynamoDBGo";
            btnDynamoDBGo.Size = new Size(197, 23);
            btnDynamoDBGo.TabIndex = 5;
            btnDynamoDBGo.Text = "DynamoDB Submit";
            btnDynamoDBGo.UseVisualStyleBackColor = true;
            btnDynamoDBGo.Click += btnDynamoDBGo_Click;
            // 
            // cbUsers
            // 
            cbUsers.FormattingEnabled = true;
            cbUsers.Location = new Point(272, 27);
            cbUsers.Name = "cbUsers";
            cbUsers.Size = new Size(300, 23);
            cbUsers.TabIndex = 6;
            // 
            // btnSQSSubmit
            // 
            btnSQSSubmit.Location = new Point(781, 27);
            btnSQSSubmit.Name = "btnSQSSubmit";
            btnSQSSubmit.Size = new Size(197, 23);
            btnSQSSubmit.TabIndex = 7;
            btnSQSSubmit.Text = "SQS Submit";
            btnSQSSubmit.UseVisualStyleBackColor = true;
            btnSQSSubmit.Click += btnSQSSubmit_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1220, 748);
            Controls.Add(btnSQSSubmit);
            Controls.Add(cbUsers);
            Controls.Add(btnDynamoDBGo);
            Controls.Add(label2);
            Controls.Add(richTextBox1);
            Controls.Add(cbUserActions);
            Controls.Add(label1);
            Name = "MainForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cbUserActions;
        private RichTextBox richTextBox1;
        private Label label2;
        private Button btnDynamoDBGo;
        private ComboBox cbUsers;
        private Button btnSQSSubmit;
    }
}
