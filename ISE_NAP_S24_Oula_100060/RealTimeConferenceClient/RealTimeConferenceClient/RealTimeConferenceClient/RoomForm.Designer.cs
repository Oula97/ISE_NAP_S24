namespace RealTimeConferenceClient
{
    partial class RoomForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoomForm));
            panel1 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            buttonstopvideo = new Button();
            panelVideo = new Panel();
            btnSendVideo = new Button();
            listBoxMessages = new ListBox();
            buttonSend = new Button();
            textBoxMessage = new TextBox();
            lblMemberCount = new Label();
            lblRoomName = new Label();
            btnLeaveRoom = new Button();
            textBoxConnectionStatus = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackgroundImage = Properties.Resources.hero_scaled;
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(flowLayoutPanel1);
            panel1.Controls.Add(buttonstopvideo);
            panel1.Controls.Add(panelVideo);
            panel1.Location = new Point(342, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(696, 414);
            panel1.TabIndex = 39;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Location = new Point(553, 5);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(140, 371);
            flowLayoutPanel1.TabIndex = 43;
            flowLayoutPanel1.Visible = false;
            // 
            // buttonstopvideo
            // 
            buttonstopvideo.Location = new Point(7, 382);
            buttonstopvideo.Name = "buttonstopvideo";
            buttonstopvideo.Size = new Size(686, 29);
            buttonstopvideo.TabIndex = 42;
            buttonstopvideo.Text = "Stop Video/Audio";
            buttonstopvideo.UseVisualStyleBackColor = true;
            buttonstopvideo.Visible = false;
            buttonstopvideo.Click += buttonstopvideo_Click;
            // 
            // panelVideo
            // 
            panelVideo.Location = new Point(7, 5);
            panelVideo.Name = "panelVideo";
            panelVideo.Size = new Size(540, 371);
            panelVideo.TabIndex = 40;
            panelVideo.Visible = false;
            // 
            // btnSendVideo
            // 
            btnSendVideo.BackColor = Color.Transparent;
            btnSendVideo.BackgroundImageLayout = ImageLayout.Center;
            btnSendVideo.Location = new Point(13, 360);
            btnSendVideo.Name = "btnSendVideo";
            btnSendVideo.Size = new Size(323, 29);
            btnSendVideo.TabIndex = 38;
            btnSendVideo.Text = "Start Video/Audio";
            btnSendVideo.UseVisualStyleBackColor = false;
            btnSendVideo.Click += btnSendVideo_Click;
            // 
            // listBoxMessages
            // 
            listBoxMessages.FormattingEnabled = true;
            listBoxMessages.Location = new Point(13, 54);
            listBoxMessages.Name = "listBoxMessages";
            listBoxMessages.ScrollAlwaysVisible = true;
            listBoxMessages.Size = new Size(323, 204);
            listBoxMessages.TabIndex = 35;
            // 
            // buttonSend
            // 
            buttonSend.Location = new Point(13, 325);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(323, 29);
            buttonSend.TabIndex = 37;
            buttonSend.Text = "Send";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new Point(13, 264);
            textBoxMessage.Multiline = true;
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.Size = new Size(323, 55);
            textBoxMessage.TabIndex = 36;
            // 
            // lblMemberCount
            // 
            lblMemberCount.AutoSize = true;
            lblMemberCount.Location = new Point(247, 15);
            lblMemberCount.Name = "lblMemberCount";
            lblMemberCount.Size = new Size(50, 20);
            lblMemberCount.TabIndex = 34;
            lblMemberCount.Text = "label2";
            // 
            // lblRoomName
            // 
            lblRoomName.AutoSize = true;
            lblRoomName.BackColor = Color.Transparent;
            lblRoomName.Location = new Point(13, 15);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new Size(50, 20);
            lblRoomName.TabIndex = 33;
            lblRoomName.Text = "label1";
            // 
            // btnLeaveRoom
            // 
            btnLeaveRoom.Location = new Point(13, 395);
            btnLeaveRoom.Name = "btnLeaveRoom";
            btnLeaveRoom.Size = new Size(323, 29);
            btnLeaveRoom.TabIndex = 32;
            btnLeaveRoom.Text = "Leave Room";
            btnLeaveRoom.UseVisualStyleBackColor = true;
            btnLeaveRoom.Click += btnLeaveRoom_Click;
            // 
            // textBoxConnectionStatus
            // 
            textBoxConnectionStatus.Location = new Point(13, 430);
            textBoxConnectionStatus.Name = "textBoxConnectionStatus";
            textBoxConnectionStatus.ReadOnly = true;
            textBoxConnectionStatus.Size = new Size(1025, 27);
            textBoxConnectionStatus.TabIndex = 40;
            // 
            // RoomForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1051, 466);
            Controls.Add(panel1);
            Controls.Add(btnSendVideo);
            Controls.Add(listBoxMessages);
            Controls.Add(buttonSend);
            Controls.Add(textBoxMessage);
            Controls.Add(lblMemberCount);
            Controls.Add(lblRoomName);
            Controls.Add(btnLeaveRoom);
            Controls.Add(textBoxConnectionStatus);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "RoomForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Room";
            Load += RoomForm_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button buttonstopvideo;
        private Panel panelVideo;
        private Button btnSendVideo;
        private ListBox listBoxMessages;
        private Button buttonSend;
        private TextBox textBoxMessage;
        private Label lblMemberCount;
        private Label lblRoomName;
        private Button btnLeaveRoom;
        private TextBox textBoxConnectionStatus;
    }
}