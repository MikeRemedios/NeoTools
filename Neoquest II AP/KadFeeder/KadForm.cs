using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace KadFeeder
{
    public partial class KadForm : Form
    {
        NeoFunctions.Account acc;
        private delegate void LOG(string msg);
        Delegate logDel;
        bool running = false;
        int maxPrice;

        public KadForm(NeoFunctions.Account acc)
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(KadForm_FormClosed);
            this.acc = acc;
            logDel = new LOG(Log);
        }

        private void KadForm_Load(object sender, EventArgs e)
        {

        }

        void KadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f = Application.OpenForms["frmLogin"];
            if (f == null)
                Application.Exit();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Regex itemRegex = new Regex(@"You should give it<br><strong>([^<]*)<\/strong>");
            string data = acc.Get("/games/kadoatery/index.phtml");
            while (running)
            {
                
                IEnumerable<string> items = Regex.Matches(data, itemRegex.ToString())
                    .OfType<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Distinct();

                foreach (string s in items)
                {
                    System.Threading.Thread.Sleep(1000);
                    char[] c = s.ToCharArray();
                    for(int i = 0; i < c.Length; ++i)
                    {
                        if (c[i] == ' ')
                        {
                            c[i] = '+';
                        }
                    }

                    string item = "Faerie+Bori+Plushie";
                    string post = string.Format("type=process_wizard&feedset=0&shopwizard={0}&table=shop&criteria=exact&min_price=0&max_price={1}", item, maxPrice);
                    data = acc.Post("/market.phtml", post, "http://www.neopets.com/market.phtml?type=wizard");
                    Regex wiz = new Regex(@"(\/browseshop.phtml?owner=[a-z0-9_]{3,20}&buy_obj_info_id=([0-9]{1,7})&buy_cost_neopoints=[0-9]{1,5})");
                    Match wizLink = Regex.Match(data, wiz.ToString());
                    if (wizLink.Success)
                    {
                        this.Invoke(logDel, "SUCCESS");
                        this.Invoke(logDel, wizLink.Groups[1].Value);
                        data = acc.Get(wizLink.Groups[1].Value, "http://www.neopets.com/market.phtml");
                        string buyLink = string.Format("<A href=(\"buy_item.phtml?lower=0&owner=a-z0-9_]{3,20}&obj_info_id={0}&g=1&xhs=574298q4&old_price=[0-9]{1,5}&feat={0},[0-9]{1,5},[0-9]{1-6}&_ref_ck=[a-z0-9]{32})", wizLink.Groups[2].Value);

                        acc.Get(Regex.Match(data, buyLink).Groups[1].Value, wizLink.Groups[1].Value);
                    }
                }
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            running = true;
            if (!Int32.TryParse(txtMaxPrice.Text, out maxPrice))
                maxPrice = 99999;
            backgroundWorker1.RunWorkerAsync();
        }

        void Log(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " - " + msg;
            //System.IO.File.AppendAllText(".\\Log - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + ".txt", msg + Environment.NewLine);
            txtLog.Text += msg + "\n";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            running = false;
            backgroundWorker1.CancelAsync();
            
        }
    }
}
