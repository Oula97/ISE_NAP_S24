namespace RealTimeConferenceClient
{
    partial class Start
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Start));
            buttonMove = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // buttonMove
            // 
            buttonMove.Location = new Point(529, 70);
            buttonMove.Name = "buttonMove";
            buttonMove.Size = new Size(144, 29);
            buttonMove.TabIndex = 0;
            buttonMove.Text = "MainDashboard";
            buttonMove.UseVisualStyleBackColor = true;
            buttonMove.Click += buttonMove_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe Script", 13.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.Location = new Point(539, 31);
            label1.Name = "label1";
            label1.Size = new Size(129, 36);
            label1.TabIndex = 2;
            label1.Text = "Welcome ";
            // 
            // Start
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.hero_scaled;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(buttonMove);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Start";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Start";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonMove;
        private Label label1;
    }
}