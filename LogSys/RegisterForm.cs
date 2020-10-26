using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogSys
{
    public partial class RegisterForm : Form
    {
        private LoginClient client;
        public RegisterForm(LoginClient client)
        {
            InitializeComponent();
            this.client = client;
            btnRegister.Click += btnRegister_Click;
        }

        void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Passwords do not match!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            client.Register(txtUsername.Text, txtPassword.Text, ProcessComplete);
        }

        private void ProcessComplete(Headers header, ErrorCodes error)
        {
            if (InvokeRequired)
            {
                Invoke(new ProcessCompleteDel(ProcessComplete), header, error);
                return;
            }

            if (header != Headers.Register)
                return;

            if (error == ErrorCodes.Success)
            {
                MessageBox.Show("Registration successful! You may now login.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }
            else if (error == ErrorCodes.Exists)
            {
                MessageBox.Show("Username already exists. Please choose another.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("An unknown error has ocurred. Please try again later.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
