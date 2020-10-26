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
    public partial class LoginForm : Form
    {
        private Form mainForm;
        private LoginClient client;
        public LoginForm(Form form)
        {
            InitializeComponent();
            mainForm = form;
            client = new LoginClient();

            btnLogin.Click += btnLogin_Click;
            btnRegister.Click += btnRegister_Click;
        }

        void btnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm regForm = new RegisterForm(client))
            {
                regForm.ShowDialog();
            }
        }

        void btnLogin_Click(object sender, EventArgs e)
        {
            client.Login(txtUsername.Text, txtPassword.Text, ProcessComplete);
        }

        private void ProcessComplete(Headers header, ErrorCodes error)
        {
            if (InvokeRequired)
            {
                Invoke(new ProcessCompleteDel(ProcessComplete), header, error);
                return;
            }

            if (header != Headers.Login)
                return;

            if (error == ErrorCodes.Success)
            {
                MessageBox.Show(string.Format("Welcome, {0}", txtUsername.Text), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                mainForm.Show();
                Hide();
            }
            else
            {
                string msg = "";

                switch (error)
                {
                    case ErrorCodes.InvalidLogin:
                        msg = "The username\\password combo does not match records.";
                        break;
                    case ErrorCodes.Error:
                        msg = "An unknown error has ocurred. Please try again later";
                        break;
                }

                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
