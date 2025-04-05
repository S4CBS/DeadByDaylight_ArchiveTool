namespace BarasToolba
{
    public partial class Form : System.Windows.Forms.Form
    {
        public static int Prioritet = 0;
        public static int predPrioritet = 0;
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

        }

        private void Form_Load(object sender, EventArgs e)
        {

        }

        private void Reward1_Click(object sender, EventArgs e)
        {

        }

        private void Progress_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Quest_Click(object sender, EventArgs e)
        {

        }

        private void KQuest_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Globals_Session.Game.bhvrSession == null)
            {
                if (Prioritet == 0)
                {
                    Prioritet = 1;
                }
                else
                {
                    Prioritet = 0;
                }
            }
        }

        private void ListTomes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Prioritet == 1)
            {
                if (ListTomes.SelectedItem != null)
                {
                    using (var tw = new StreamWriter(FiddlerCore.TomeQFolderPath, false))
                    {
                        tw.WriteLine(ListTomes.SelectedItem.ToString());
                    }
                }
            }
            else
            {
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
