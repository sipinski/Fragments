using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fragments
{
    public partial class mainForm : Form
    {
        MediaPlayer player; 

        public mainForm()
        {
            InitializeComponent();
            Cursor.Hide();
            player = new MediaPlayer(panel1.Handle);
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                // close program on escape
                case Keys.Escape:
                    this.Close();
                    break;

                // Any other keypress triggers the associated action
                default:
                    string keypress = e.KeyData.ToString();
                    if (keypress.Length <= 2)
                    {
#if DEBUG
                        Console.WriteLine("Next action: " + keypress.Substring(keypress.Length - 1, 1));
#endif
                        player.nextAction = keypress.Substring(keypress.Length - 1, 1);
                    }
                    break;
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }         
        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            player.Dispose();
        }
    }
}