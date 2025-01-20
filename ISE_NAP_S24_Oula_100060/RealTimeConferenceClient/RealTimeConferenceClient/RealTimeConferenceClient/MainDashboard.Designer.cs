namespace RealTimeConferenceClient
{
    partial class MainDashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDashboard));
            btndelete = new Button();
            buttonjoin = new Button();
            label2 = new Label();
            txtRoomName = new TextBox();
            label1 = new Label();
            btnCreateRoom = new Button();
            lstRooms = new ListBox();
            SuspendLayout();
            // 
            // btndelete
            // 
            btndelete.Location = new Point(338, 146);
            btndelete.Name = "btndelete";
            btndelete.Size = new Size(119, 29);
            btndelete.TabIndex = 25;
            btndelete.Text = "Delete Room";
            btndelete.UseVisualStyleBackColor = true;
            btndelete.Click += btndelete_Click;
            // 
            // buttonjoin
            // 
            buttonjoin.Location = new Point(175, 156);
            buttonjoin.Name = "buttonjoin";
            buttonjoin.Size = new Size(98, 50);
            buttonjoin.TabIndex = 24;
            buttonjoin.Text = "Join To Room";
            buttonjoin.UseVisualStyleBackColor = true;
            buttonjoin.Click += buttonjoin_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(351, 35);
            label2.Name = "label2";
            label2.Size = new Size(96, 20);
            label2.TabIndex = 23;
            label2.Text = "Name Room:";
            // 
            // txtRoomName
            // 
            txtRoomName.Location = new Point(338, 65);
            txtRoomName.Name = "txtRoomName";
            txtRoomName.Size = new Size(119, 27);
            txtRoomName.TabIndex = 22;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe Script", 13.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.Location = new Point(176, 13);
            label1.Name = "label1";
            label1.Size = new Size(94, 36);
            label1.TabIndex = 21;
            label1.Text = "Rooms";
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.Location = new Point(338, 110);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(119, 29);
            btnCreateRoom.TabIndex = 20;
            btnCreateRoom.Text = "Create Room";
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // lstRooms
            // 
            lstRooms.FormattingEnabled = true;
            lstRooms.Location = new Point(175, 46);
            lstRooms.Name = "lstRooms";
            lstRooms.ScrollAlwaysVisible = true;
            lstRooms.Size = new Size(98, 104);
            lstRooms.TabIndex = 19;
            // 
            // MainDashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.hero_scaled;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(btndelete);
            Controls.Add(buttonjoin);
            Controls.Add(label2);
            Controls.Add(txtRoomName);
            Controls.Add(label1);
            Controls.Add(btnCreateRoom);
            Controls.Add(lstRooms);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainDashboard";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btndelete;
        private Button buttonjoin;
        private Label label2;
        private TextBox txtRoomName;
        private Label label1;
        private Button btnCreateRoom;
        private ListBox lstRooms;
    }
}