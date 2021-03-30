using Mastermind;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface.Forms
{
    public partial class frmStart : Form
    {

        public frmStart()
        {
            InitializeComponent();
            label3.Text = "Pick your 4 digit secret number with unique digits";
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            frmGame frm = new frmGame();
            frm.Show();
        }


    }
}
