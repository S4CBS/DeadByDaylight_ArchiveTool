namespace BarasToolba
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            Start = new Button();
            Stop = new Button();
            Queue = new Label();
            Quest = new Label();
            Progress = new Label();
            Reward1 = new Label();
            Reward2 = new Label();
            GetQuest = new Button();
            PlayerRole = new Label();
            SuspendLayout();
            // 
            // Start
            // 
            Start.Location = new Point(22, 22);
            Start.Name = "Start";
            Start.Size = new Size(75, 42);
            Start.TabIndex = 0;
            Start.Text = "Start";
            Start.UseVisualStyleBackColor = true;
            Start.Click += Start_Click;
            // 
            // Stop
            // 
            Stop.Location = new Point(22, 70);
            Stop.Name = "Stop";
            Stop.Size = new Size(75, 42);
            Stop.TabIndex = 1;
            Stop.Text = "Stop";
            Stop.UseVisualStyleBackColor = true;
            Stop.Click += Stop_Click;
            // 
            // Queue
            // 
            Queue.AutoSize = true;
            Queue.BorderStyle = BorderStyle.FixedSingle;
            Queue.ForeColor = Color.Indigo;
            Queue.Location = new Point(22, 138);
            Queue.Name = "Queue";
            Queue.Size = new Size(77, 17);
            Queue.TabIndex = 2;
            Queue.Text = "Очередь: xxx";
            Queue.Click += Queue_Click;
            // 
            // Quest
            // 
            Quest.AutoSize = true;
            Quest.BorderStyle = BorderStyle.FixedSingle;
            Quest.ForeColor = Color.Indigo;
            Quest.Location = new Point(170, 29);
            Quest.Name = "Quest";
            Quest.Size = new Size(74, 17);
            Quest.TabIndex = 3;
            Quest.Text = "Испытание:";
            // 
            // Progress
            // 
            Progress.AutoSize = true;
            Progress.BorderStyle = BorderStyle.FixedSingle;
            Progress.ForeColor = Color.Indigo;
            Progress.Location = new Point(170, 70);
            Progress.Name = "Progress";
            Progress.Size = new Size(65, 17);
            Progress.TabIndex = 4;
            Progress.Text = "Прогресс:";
            // 
            // Reward1
            // 
            Reward1.AutoSize = true;
            Reward1.BorderStyle = BorderStyle.FixedSingle;
            Reward1.ForeColor = Color.Indigo;
            Reward1.Location = new Point(170, 112);
            Reward1.Name = "Reward1";
            Reward1.Size = new Size(31, 17);
            Reward1.TabIndex = 5;
            Reward1.Text = "N/A";
            // 
            // Reward2
            // 
            Reward2.AutoSize = true;
            Reward2.BorderStyle = BorderStyle.FixedSingle;
            Reward2.ForeColor = Color.Indigo;
            Reward2.Location = new Point(170, 152);
            Reward2.Name = "Reward2";
            Reward2.Size = new Size(31, 17);
            Reward2.TabIndex = 6;
            Reward2.Text = "N/A";
            // 
            // GetQuest
            // 
            GetQuest.Location = new Point(170, 188);
            GetQuest.Name = "GetQuest";
            GetQuest.Size = new Size(153, 31);
            GetQuest.TabIndex = 7;
            GetQuest.Text = "Получить данные";
            GetQuest.UseVisualStyleBackColor = true;
            GetQuest.Click += GetQuest_Click;
            // 
            // PlayerRole
            // 
            PlayerRole.AutoSize = true;
            PlayerRole.BorderStyle = BorderStyle.FixedSingle;
            PlayerRole.ForeColor = Color.Indigo;
            PlayerRole.Location = new Point(22, 179);
            PlayerRole.Name = "PlayerRole";
            PlayerRole.Size = new Size(57, 17);
            PlayerRole.TabIndex = 8;
            PlayerRole.Text = "Роль: xxx";
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(584, 231);
            Controls.Add(PlayerRole);
            Controls.Add(GetQuest);
            Controls.Add(Reward2);
            Controls.Add(Reward1);
            Controls.Add(Progress);
            Controls.Add(Quest);
            Controls.Add(Queue);
            Controls.Add(Stop);
            Controls.Add(Start);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(600, 270);
            MinimumSize = new Size(600, 270);
            Name = "Form";
            Text = "BarasToolba";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Start;
        private Button Stop;
        private Button GetQuest;
        internal static Label PlayerRole;
        internal static Label Queue;
        internal static Label Quest;
        internal static Label Progress;
        internal static Label Reward1;
        internal static Label Reward2;
    }
}
