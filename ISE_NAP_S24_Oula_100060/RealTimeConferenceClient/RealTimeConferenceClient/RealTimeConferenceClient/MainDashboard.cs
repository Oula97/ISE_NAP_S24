using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealTimeConferenceClient
{
    public partial class MainDashboard : Form
    {
        // قائمة لتخزين أسماء الغرف
        private List<Room> rooms = new List<Room>(); // قائمة الغرف
        public MainDashboard()
        {
            InitializeComponent();
            LoadRoomsFromAppSettings(); // تحميل الغرف عند بدء التطبيق

        }
        private void buttonjoin_Click(object sender, EventArgs e)
        {
            if (lstRooms.SelectedItem is Room selectedRoom)
            {
                if (selectedRoom.MemberCount < selectedRoom.MaxMembers)
                {
                    selectedRoom.MemberCount++;
                    UpdateRoomList();
                    SaveRoomsToAppSettings(); // حفظ الغرف بعد التغيير
                    RoomForm roomForm = new RoomForm(selectedRoom);
                    roomForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show($"Room '{selectedRoom.Name}' is full. Maximum {selectedRoom.MaxMembers} members allowed.", "Room Full");
                }
            }
            else
            {
                MessageBox.Show("Please select a room to join.", "Error");
            }
        }
        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            string roomName = txtRoomName.Text.Trim();

            if (!string.IsNullOrEmpty(roomName))
            {
                if (!rooms.Exists(r => r.Name == roomName))
                {
                    Room newRoom = new Room
                    {
                        Name = roomName,
                        MemberCount = 0
                    };
                    rooms.Add(newRoom);
                    UpdateRoomList();
                    SaveRoomsToAppSettings(); // حفظ الغرف بعد إنشاء غرفة جديدة
                    MessageBox.Show($"Room '{roomName}' created successfully!", "Room Created");
                }
                else
                {
                    MessageBox.Show("Room name already exists. Please choose a different name.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid room name.", "Error");
            }
        }
        private void UpdateRoomList()
        {
            lstRooms.DataSource = null; // تفريغ المصدر
            lstRooms.DataSource = rooms; // تحديث المصدر
        }
        private void SaveRoomsToAppSettings()
        {
            // تحويل قائمة الغرف إلى سلسلة نصية
            var roomNames = rooms.Select(r => r.Name).ToList();
            Properties.Settings.Default.Rooms = string.Join(";", roomNames); // تخزين الغرف كقائمة مفصولة بفاصلة
            Properties.Settings.Default.Save(); // حفظ الإعدادات
        }
        private void LoadRoomsFromAppSettings()
        {
            string savedRooms = Properties.Settings.Default.Rooms;
            if (!string.IsNullOrEmpty(savedRooms))
            {
                var roomNames = savedRooms.Split(';'); // تقسيم القائمة النصية
                foreach (var roomName in roomNames)
                {
                    rooms.Add(new Room { Name = roomName, MemberCount = 0 }); // إضافة الغرف المحفوظة إلى القائمة
                }
                UpdateRoomList(); // تحديث واجهة المستخدم
            }
        }
        private void btndelete_Click(object sender, EventArgs e)
        {
            if (lstRooms.SelectedItem is Room selectedRoom)
            {
                // التحقق من وجود الغرفة في القائمة
                if (rooms.Contains(selectedRoom))
                {
                    // حذف الغرفة من القائمة
                    rooms.Remove(selectedRoom);
                    UpdateRoomList();
                    SaveRoomsToAppSettings(); // حفظ الغرف بعد الحذف
                    MessageBox.Show($"Room '{selectedRoom.Name}' deleted successfully.", "Room Deleted");
                }
            }
            else
            {
                MessageBox.Show("Please select a room to delete.", "Error");
            }
        }
    }

}

