namespace BarasToolba
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            FiddlerCore.Stop();
        }

        public void Start_Click(object sender, EventArgs e)
        {
            FiddlerCore.Start();
        }

        public void Stop_Click(object sender, EventArgs e)
        {
            FiddlerCore.Stop();
        }

        private void Queue_Click(object sender, EventArgs e)
        {

        }

        private void GetQuest_Click(object sender, EventArgs e)
        {
            FiddlerCore.UpdateData();
        }

        private void SpoofingRole_Click(object sender, EventArgs e)
        {
            Program.RoleSpoffer();
        }
    }
}
