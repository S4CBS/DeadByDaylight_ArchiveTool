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
            labelSQuest = new Label();
            labelKQuest = new Label();
            KQuest = new Label();
            KReward1 = new Label();
            KProgress = new Label();
            KReward2 = new Label();
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
            Queue.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            Queue.ForeColor = Color.Indigo;
            Queue.Location = new Point(12, 304);
            Queue.Name = "Queue";
            Queue.Size = new Size(88, 19);
            Queue.TabIndex = 2;
            Queue.Text = "Очередь: xxx";
            Queue.Click += Queue_Click;
            // 
            // Quest
            // 
            Quest.AutoSize = true;
            Quest.BorderStyle = BorderStyle.FixedSingle;
            Quest.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Quest.ForeColor = Color.Indigo;
            Quest.Location = new Point(113, 111);
            Quest.Name = "Quest";
            Quest.Size = new Size(77, 17);
            Quest.TabIndex = 3;
            Quest.Text = "Испытание:";
            Quest.Click += Quest_Click;
            // 
            // Progress
            // 
            Progress.AutoSize = true;
            Progress.BorderStyle = BorderStyle.FixedSingle;
            Progress.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Progress.ForeColor = Color.Indigo;
            Progress.Location = new Point(113, 165);
            Progress.Name = "Progress";
            Progress.Size = new Size(66, 17);
            Progress.TabIndex = 4;
            Progress.Text = "Прогресс:";
            Progress.Click += Progress_Click;
            // 
            // Reward1
            // 
            Reward1.AutoSize = true;
            Reward1.BorderStyle = BorderStyle.FixedSingle;
            Reward1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Reward1.ForeColor = Color.Indigo;
            Reward1.Location = new Point(114, 212);
            Reward1.Name = "Reward1";
            Reward1.Size = new Size(31, 17);
            Reward1.TabIndex = 5;
            Reward1.Text = "N/A";
            Reward1.Click += Reward1_Click;
            // 
            // Reward2
            // 
            Reward2.AutoSize = true;
            Reward2.BorderStyle = BorderStyle.FixedSingle;
            Reward2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Reward2.ForeColor = Color.Indigo;
            Reward2.Location = new Point(114, 258);
            Reward2.Name = "Reward2";
            Reward2.Size = new Size(31, 17);
            Reward2.TabIndex = 6;
            Reward2.Text = "N/A";
            // 
            // GetQuest
            // 
            GetQuest.Location = new Point(216, 326);
            GetQuest.Name = "GetQuest";
            GetQuest.Size = new Size(153, 26);
            GetQuest.TabIndex = 7;
            GetQuest.Text = "Получить данные";
            GetQuest.UseVisualStyleBackColor = true;
            GetQuest.Click += GetQuest_Click;
            // 
            // PlayerRole
            // 
            PlayerRole.AutoSize = true;
            PlayerRole.BorderStyle = BorderStyle.FixedSingle;
            PlayerRole.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            PlayerRole.ForeColor = Color.Indigo;
            PlayerRole.Location = new Point(12, 335);
            PlayerRole.Name = "PlayerRole";
            PlayerRole.Size = new Size(72, 22);
            PlayerRole.TabIndex = 8;
            PlayerRole.Text = "Роль: xxx";
            // 
            // labelSQuest
            // 
            labelSQuest.AutoSize = true;
            labelSQuest.BorderStyle = BorderStyle.FixedSingle;
            labelSQuest.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            labelSQuest.ForeColor = Color.Indigo;
            labelSQuest.Location = new Point(114, 49);
            labelSQuest.Name = "labelSQuest";
            labelSQuest.Size = new Size(152, 27);
            labelSQuest.TabIndex = 10;
            labelSQuest.Text = "Survivor Quest:";
            labelSQuest.Click += label1_Click;
            // 
            // labelKQuest
            // 
            labelKQuest.AutoSize = true;
            labelKQuest.BorderStyle = BorderStyle.FixedSingle;
            labelKQuest.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            labelKQuest.ForeColor = Color.Indigo;
            labelKQuest.Location = new Point(515, 51);
            labelKQuest.Name = "labelKQuest";
            labelKQuest.Size = new Size(120, 27);
            labelKQuest.TabIndex = 11;
            labelKQuest.Text = "Killer Quest:";
            labelKQuest.Click += label2_Click;
            // 
            // KQuest
            // 
            KQuest.AutoSize = true;
            KQuest.BorderStyle = BorderStyle.FixedSingle;
            KQuest.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            KQuest.ForeColor = Color.Indigo;
            KQuest.Location = new Point(515, 111);
            KQuest.Name = "KQuest";
            KQuest.Size = new Size(77, 17);
            KQuest.TabIndex = 12;
            KQuest.Text = "Испытание:";
            // 
            // KReward1
            // 
            KReward1.AutoSize = true;
            KReward1.BorderStyle = BorderStyle.FixedSingle;
            KReward1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            KReward1.ForeColor = Color.Indigo;
            KReward1.Location = new Point(515, 212);
            KReward1.Name = "KReward1";
            KReward1.Size = new Size(31, 17);
            KReward1.TabIndex = 13;
            KReward1.Text = "N/A";
            // 
            // KProgress
            // 
            KProgress.AutoSize = true;
            KProgress.BorderStyle = BorderStyle.FixedSingle;
            KProgress.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            KProgress.ForeColor = Color.Indigo;
            KProgress.Location = new Point(515, 165);
            KProgress.Name = "KProgress";
            KProgress.Size = new Size(66, 17);
            KProgress.TabIndex = 14;
            KProgress.Text = "Прогресс:";
            // 
            // KReward2
            // 
            KReward2.AutoSize = true;
            KReward2.BorderStyle = BorderStyle.FixedSingle;
            KReward2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            KReward2.ForeColor = Color.Indigo;
            KReward2.Location = new Point(515, 258);
            KReward2.Name = "KReward2";
            KReward2.Size = new Size(31, 17);
            KReward2.TabIndex = 15;
            KReward2.Text = "N/A";
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(984, 361);
            Controls.Add(KReward2);
            Controls.Add(KProgress);
            Controls.Add(KReward1);
            Controls.Add(KQuest);
            Controls.Add(labelKQuest);
            Controls.Add(labelSQuest);
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
            MaximumSize = new Size(1000, 400);
            MinimumSize = new Size(1000, 400);
            Name = "Form";
            Text = "BarasToolba";
            Load += Form_Load;
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
        internal static Label labelSQuest;
        internal static Label labelKQuest;
        internal static Label KQuest;
        internal static Label KReward1;
        internal static Label KProgress;
        internal static Label KReward2;
    }
}
