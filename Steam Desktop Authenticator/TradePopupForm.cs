using SteamAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class TradePopupForm : Form
    {
        private SteamGuardAccount acc;
        private List<Confirmation> confirms = new List<Confirmation>();
        private bool deny2, accept2;

        public TradePopupForm()
        {
            InitializeComponent();
            lblStatus.Text = "";
        }

        public SteamGuardAccount Account
        {
            get { return acc; }
            set { acc = value; lblAccount.Text = acc.AccountName; }
        }

        public Confirmation[] Confirmations
        {
            get { return confirms.ToArray(); }
            set { confirms = new List<Confirmation>(value); }
        }

        private void TradePopupForm_Load(object sender, EventArgs e)
        {
            this.Location = (Point)Size.Subtract(Screen.GetWorkingArea(this).Size, this.Size);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (!accept2)
            {
                // Allow user to confirm first
                lblStatus.Text = "再按接受确认";
                btnAccept.BackColor = Color.FromArgb(128, 255, 128);
                accept2 = true;
            }
            else
            {
                lblStatus.Text = "接受中...";
                acc.AcceptConfirmation(confirms[0]);
                confirms.RemoveAt(0);
                Reset();
            }
        }

        private void btnDeny_Click(object sender, EventArgs e)
        {
            if (!deny2)
            {
                lblStatus.Text = "再次拒绝确认";
                btnDeny.BackColor = Color.FromArgb(255, 255, 128);
                deny2 = true;
            }
            else
            {
                lblStatus.Text = "拒绝中...";
                acc.DenyConfirmation(confirms[0]);
                confirms.RemoveAt(0);
                Reset();
            }
        }

        private void Reset()
        {
            deny2 = false;
            accept2 = false;
            btnAccept.BackColor = Color.FromArgb(192, 255, 192);
            btnDeny.BackColor = Color.FromArgb(255, 255, 192);

            btnAccept.Text = "接受";
            btnDeny.Text = "拒绝";
            lblAccount.Text = "";
            lblStatus.Text = "";

            if (confirms.Count == 0)
            {
                this.Hide();
            }
            else
            {
                //TODO: Re-add confirmation description support to SteamAuth.
                lblDesc.Text = "确认";
            }
        }

        public void Popup()
        {
            Reset();
            this.Show();
        }
    }
}
