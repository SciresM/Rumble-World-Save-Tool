namespace Pokemon_Rumble_World_Save_Tool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.B_Open_CMP = new System.Windows.Forms.Button();
            this.B_Open_DEC = new System.Windows.Forms.Button();
            this.B_Save_CMP = new System.Windows.Forms.Button();
            this.B_Save_DEC = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NUP_Diamonds = new System.Windows.Forms.NumericUpDown();
            this.NUP_P = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NUP_Rank = new System.Windows.Forms.NumericUpDown();
            this.GB_MonEdit = new System.Windows.Forms.GroupBox();
            this.NUP_ST6 = new System.Windows.Forms.NumericUpDown();
            this.CB_ST6 = new System.Windows.Forms.ComboBox();
            this.NUP_ST5 = new System.Windows.Forms.NumericUpDown();
            this.CB_ST5 = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.NUP_ST4 = new System.Windows.Forms.NumericUpDown();
            this.CB_ST4 = new System.Windows.Forms.ComboBox();
            this.NUP_ST3 = new System.Windows.Forms.NumericUpDown();
            this.CB_ST3 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.NUP_ST2 = new System.Windows.Forms.NumericUpDown();
            this.CB_ST2 = new System.Windows.Forms.ComboBox();
            this.NUP_ST1 = new System.Windows.Forms.NumericUpDown();
            this.CB_ST1 = new System.Windows.Forms.ComboBox();
            this.NUP_Trait = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.CHK_MEvo = new System.Windows.Forms.CheckBox();
            this.PB_SelectedMon = new System.Windows.Forms.PictureBox();
            this.CB_Move2 = new System.Windows.Forms.ComboBox();
            this.CB_Move1 = new System.Windows.Forms.ComboBox();
            this.CB_Trait = new System.Windows.Forms.ComboBox();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CB_MonSelection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Diamonds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Rank)).BeginInit();
            this.GB_MonEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Trait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SelectedMon)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 9);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(285, 20);
            this.textBox1.TabIndex = 19;
            // 
            // B_Open_CMP
            // 
            this.B_Open_CMP.Location = new System.Drawing.Point(12, 35);
            this.B_Open_CMP.Name = "B_Open_CMP";
            this.B_Open_CMP.Size = new System.Drawing.Size(285, 23);
            this.B_Open_CMP.TabIndex = 18;
            this.B_Open_CMP.Text = "Open Compressed 00slot00";
            this.B_Open_CMP.UseVisualStyleBackColor = true;
            this.B_Open_CMP.Click += new System.EventHandler(this.B_Open_CMP_Click);
            // 
            // B_Open_DEC
            // 
            this.B_Open_DEC.Location = new System.Drawing.Point(12, 64);
            this.B_Open_DEC.Name = "B_Open_DEC";
            this.B_Open_DEC.Size = new System.Drawing.Size(285, 23);
            this.B_Open_DEC.TabIndex = 20;
            this.B_Open_DEC.Text = "Open Decompressed 00slot00_dec";
            this.B_Open_DEC.UseVisualStyleBackColor = true;
            this.B_Open_DEC.Click += new System.EventHandler(this.B_Open_DEC_Click);
            // 
            // B_Save_CMP
            // 
            this.B_Save_CMP.Enabled = false;
            this.B_Save_CMP.Location = new System.Drawing.Point(12, 93);
            this.B_Save_CMP.Name = "B_Save_CMP";
            this.B_Save_CMP.Size = new System.Drawing.Size(285, 23);
            this.B_Save_CMP.TabIndex = 21;
            this.B_Save_CMP.Text = "Save Compressed 00slot00";
            this.B_Save_CMP.UseVisualStyleBackColor = true;
            this.B_Save_CMP.Click += new System.EventHandler(this.B_Save_CMP_Click);
            // 
            // B_Save_DEC
            // 
            this.B_Save_DEC.Enabled = false;
            this.B_Save_DEC.Location = new System.Drawing.Point(12, 122);
            this.B_Save_DEC.Name = "B_Save_DEC";
            this.B_Save_DEC.Size = new System.Drawing.Size(285, 23);
            this.B_Save_DEC.TabIndex = 22;
            this.B_Save_DEC.Text = "Save Decompressed 00slot00_dec";
            this.B_Save_DEC.UseVisualStyleBackColor = true;
            this.B_Save_DEC.Click += new System.EventHandler(this.B_Save_DEC_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(298, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Pokediamonds:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(361, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "P: ";
            // 
            // NUP_Diamonds
            // 
            this.NUP_Diamonds.Enabled = false;
            this.NUP_Diamonds.Location = new System.Drawing.Point(384, 9);
            this.NUP_Diamonds.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUP_Diamonds.Name = "NUP_Diamonds";
            this.NUP_Diamonds.Size = new System.Drawing.Size(73, 20);
            this.NUP_Diamonds.TabIndex = 25;
            this.NUP_Diamonds.ValueChanged += new System.EventHandler(this.Update_Data);
            // 
            // NUP_P
            // 
            this.NUP_P.Enabled = false;
            this.NUP_P.Location = new System.Drawing.Point(384, 32);
            this.NUP_P.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUP_P.Name = "NUP_P";
            this.NUP_P.Size = new System.Drawing.Size(73, 20);
            this.NUP_P.TabIndex = 26;
            this.NUP_P.ValueChanged += new System.EventHandler(this.Update_Data);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(342, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Rank: ";
            // 
            // NUP_Rank
            // 
            this.NUP_Rank.Enabled = false;
            this.NUP_Rank.Location = new System.Drawing.Point(384, 56);
            this.NUP_Rank.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUP_Rank.Name = "NUP_Rank";
            this.NUP_Rank.Size = new System.Drawing.Size(73, 20);
            this.NUP_Rank.TabIndex = 28;
            this.NUP_Rank.ValueChanged += new System.EventHandler(this.Update_Data);
            // 
            // GB_MonEdit
            // 
            this.GB_MonEdit.Controls.Add(this.NUP_ST6);
            this.GB_MonEdit.Controls.Add(this.CB_ST6);
            this.GB_MonEdit.Controls.Add(this.NUP_ST5);
            this.GB_MonEdit.Controls.Add(this.CB_ST5);
            this.GB_MonEdit.Controls.Add(this.label13);
            this.GB_MonEdit.Controls.Add(this.label14);
            this.GB_MonEdit.Controls.Add(this.NUP_ST4);
            this.GB_MonEdit.Controls.Add(this.CB_ST4);
            this.GB_MonEdit.Controls.Add(this.NUP_ST3);
            this.GB_MonEdit.Controls.Add(this.CB_ST3);
            this.GB_MonEdit.Controls.Add(this.label11);
            this.GB_MonEdit.Controls.Add(this.label12);
            this.GB_MonEdit.Controls.Add(this.NUP_ST2);
            this.GB_MonEdit.Controls.Add(this.CB_ST2);
            this.GB_MonEdit.Controls.Add(this.NUP_ST1);
            this.GB_MonEdit.Controls.Add(this.CB_ST1);
            this.GB_MonEdit.Controls.Add(this.NUP_Trait);
            this.GB_MonEdit.Controls.Add(this.label10);
            this.GB_MonEdit.Controls.Add(this.label9);
            this.GB_MonEdit.Controls.Add(this.CHK_MEvo);
            this.GB_MonEdit.Controls.Add(this.PB_SelectedMon);
            this.GB_MonEdit.Controls.Add(this.CB_Move2);
            this.GB_MonEdit.Controls.Add(this.CB_Move1);
            this.GB_MonEdit.Controls.Add(this.CB_Trait);
            this.GB_MonEdit.Controls.Add(this.CB_Species);
            this.GB_MonEdit.Controls.Add(this.label8);
            this.GB_MonEdit.Controls.Add(this.label7);
            this.GB_MonEdit.Controls.Add(this.label6);
            this.GB_MonEdit.Controls.Add(this.label5);
            this.GB_MonEdit.Controls.Add(this.CB_MonSelection);
            this.GB_MonEdit.Controls.Add(this.label4);
            this.GB_MonEdit.Enabled = false;
            this.GB_MonEdit.Location = new System.Drawing.Point(10, 150);
            this.GB_MonEdit.MaximumSize = new System.Drawing.Size(450, 220);
            this.GB_MonEdit.MinimumSize = new System.Drawing.Size(450, 130);
            this.GB_MonEdit.Name = "GB_MonEdit";
            this.GB_MonEdit.Size = new System.Drawing.Size(450, 220);
            this.GB_MonEdit.TabIndex = 29;
            this.GB_MonEdit.TabStop = false;
            this.GB_MonEdit.Text = "Toy Editing";
            // 
            // NUP_ST6
            // 
            this.NUP_ST6.Location = new System.Drawing.Point(414, 193);
            this.NUP_ST6.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_ST6.Name = "NUP_ST6";
            this.NUP_ST6.Size = new System.Drawing.Size(26, 20);
            this.NUP_ST6.TabIndex = 30;
            this.NUP_ST6.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_ST6
            // 
            this.CB_ST6.FormattingEnabled = true;
            this.CB_ST6.Location = new System.Drawing.Point(290, 193);
            this.CB_ST6.Name = "CB_ST6";
            this.CB_ST6.Size = new System.Drawing.Size(121, 21);
            this.CB_ST6.TabIndex = 29;
            this.CB_ST6.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // NUP_ST5
            // 
            this.NUP_ST5.Location = new System.Drawing.Point(198, 192);
            this.NUP_ST5.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_ST5.Name = "NUP_ST5";
            this.NUP_ST5.Size = new System.Drawing.Size(26, 20);
            this.NUP_ST5.TabIndex = 28;
            this.NUP_ST5.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_ST5
            // 
            this.CB_ST5.FormattingEnabled = true;
            this.CB_ST5.Location = new System.Drawing.Point(74, 192);
            this.CB_ST5.Name = "CB_ST5";
            this.CB_ST5.Size = new System.Drawing.Size(121, 21);
            this.CB_ST5.TabIndex = 27;
            this.CB_ST5.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(230, 197);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "SubTrait 6:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 197);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "SubTrait 5:";
            // 
            // NUP_ST4
            // 
            this.NUP_ST4.Location = new System.Drawing.Point(414, 167);
            this.NUP_ST4.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_ST4.Name = "NUP_ST4";
            this.NUP_ST4.Size = new System.Drawing.Size(26, 20);
            this.NUP_ST4.TabIndex = 24;
            this.NUP_ST4.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_ST4
            // 
            this.CB_ST4.FormattingEnabled = true;
            this.CB_ST4.Location = new System.Drawing.Point(290, 167);
            this.CB_ST4.Name = "CB_ST4";
            this.CB_ST4.Size = new System.Drawing.Size(121, 21);
            this.CB_ST4.TabIndex = 23;
            this.CB_ST4.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // NUP_ST3
            // 
            this.NUP_ST3.Location = new System.Drawing.Point(198, 166);
            this.NUP_ST3.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_ST3.Name = "NUP_ST3";
            this.NUP_ST3.Size = new System.Drawing.Size(26, 20);
            this.NUP_ST3.TabIndex = 22;
            this.NUP_ST3.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_ST3
            // 
            this.CB_ST3.FormattingEnabled = true;
            this.CB_ST3.Location = new System.Drawing.Point(74, 166);
            this.CB_ST3.Name = "CB_ST3";
            this.CB_ST3.Size = new System.Drawing.Size(121, 21);
            this.CB_ST3.TabIndex = 21;
            this.CB_ST3.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(230, 171);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "SubTrait 4:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 171);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "SubTrait 3:";
            // 
            // NUP_ST2
            // 
            this.NUP_ST2.Location = new System.Drawing.Point(414, 140);
            this.NUP_ST2.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_ST2.Name = "NUP_ST2";
            this.NUP_ST2.Size = new System.Drawing.Size(26, 20);
            this.NUP_ST2.TabIndex = 18;
            this.NUP_ST2.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_ST2
            // 
            this.CB_ST2.FormattingEnabled = true;
            this.CB_ST2.Location = new System.Drawing.Point(290, 140);
            this.CB_ST2.Name = "CB_ST2";
            this.CB_ST2.Size = new System.Drawing.Size(121, 21);
            this.CB_ST2.TabIndex = 17;
            this.CB_ST2.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // NUP_ST1
            // 
            this.NUP_ST1.Location = new System.Drawing.Point(198, 139);
            this.NUP_ST1.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_ST1.Name = "NUP_ST1";
            this.NUP_ST1.Size = new System.Drawing.Size(26, 20);
            this.NUP_ST1.TabIndex = 16;
            this.NUP_ST1.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_ST1
            // 
            this.CB_ST1.FormattingEnabled = true;
            this.CB_ST1.Location = new System.Drawing.Point(74, 139);
            this.CB_ST1.Name = "CB_ST1";
            this.CB_ST1.Size = new System.Drawing.Size(121, 21);
            this.CB_ST1.TabIndex = 15;
            this.CB_ST1.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // NUP_Trait
            // 
            this.NUP_Trait.Location = new System.Drawing.Point(161, 100);
            this.NUP_Trait.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_Trait.Name = "NUP_Trait";
            this.NUP_Trait.Size = new System.Drawing.Size(26, 20);
            this.NUP_Trait.TabIndex = 14;
            this.NUP_Trait.ValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(230, 144);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "SubTrait 2:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "SubTrait 1:";
            // 
            // CHK_MEvo
            // 
            this.CHK_MEvo.AutoSize = true;
            this.CHK_MEvo.Location = new System.Drawing.Point(66, 45);
            this.CHK_MEvo.Name = "CHK_MEvo";
            this.CHK_MEvo.Size = new System.Drawing.Size(111, 17);
            this.CHK_MEvo.TabIndex = 11;
            this.CHK_MEvo.Text = "Can Mega Evolve";
            this.CHK_MEvo.UseVisualStyleBackColor = true;
            this.CHK_MEvo.CheckedChanged += new System.EventHandler(this.StoreMon);
            // 
            // PB_SelectedMon
            // 
            this.PB_SelectedMon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_SelectedMon.Location = new System.Drawing.Point(12, 17);
            this.PB_SelectedMon.Name = "PB_SelectedMon";
            this.PB_SelectedMon.Size = new System.Drawing.Size(48, 48);
            this.PB_SelectedMon.TabIndex = 10;
            this.PB_SelectedMon.TabStop = false;
            // 
            // CB_Move2
            // 
            this.CB_Move2.FormattingEnabled = true;
            this.CB_Move2.Location = new System.Drawing.Point(247, 100);
            this.CB_Move2.Name = "CB_Move2";
            this.CB_Move2.Size = new System.Drawing.Size(180, 21);
            this.CB_Move2.TabIndex = 9;
            this.CB_Move2.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_Move1
            // 
            this.CB_Move1.FormattingEnabled = true;
            this.CB_Move1.Location = new System.Drawing.Point(247, 68);
            this.CB_Move1.Name = "CB_Move1";
            this.CB_Move1.Size = new System.Drawing.Size(180, 21);
            this.CB_Move1.TabIndex = 8;
            this.CB_Move1.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_Trait
            // 
            this.CB_Trait.FormattingEnabled = true;
            this.CB_Trait.Location = new System.Drawing.Point(66, 100);
            this.CB_Trait.Name = "CB_Trait";
            this.CB_Trait.Size = new System.Drawing.Size(89, 21);
            this.CB_Trait.TabIndex = 7;
            this.CB_Trait.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // CB_Species
            // 
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(66, 68);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(121, 21);
            this.CB_Species.TabIndex = 6;
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.StoreMon);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(195, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Move 2:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Trait:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(195, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Move 1:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Species:";
            // 
            // CB_MonSelection
            // 
            this.CB_MonSelection.FormattingEnabled = true;
            this.CB_MonSelection.Location = new System.Drawing.Point(173, 18);
            this.CB_MonSelection.Name = "CB_MonSelection";
            this.CB_MonSelection.Size = new System.Drawing.Size(254, 21);
            this.CB_MonSelection.TabIndex = 1;
            this.CB_MonSelection.SelectedIndexChanged += new System.EventHandler(this.CB_MonSelection_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Selected Pokemon: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 381);
            this.Controls.Add(this.GB_MonEdit);
            this.Controls.Add(this.NUP_Rank);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NUP_P);
            this.Controls.Add(this.NUP_Diamonds);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.B_Save_DEC);
            this.Controls.Add(this.B_Save_CMP);
            this.Controls.Add(this.B_Open_DEC);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.B_Open_CMP);
            this.MaximumSize = new System.Drawing.Size(485, 420);
            this.MinimumSize = new System.Drawing.Size(485, 330);
            this.Name = "Form1";
            this.Text = " Rumble World Save Tool";
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Diamonds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Rank)).EndInit();
            this.GB_MonEdit.ResumeLayout(false);
            this.GB_MonEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_ST1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Trait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SelectedMon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button B_Open_CMP;
        private System.Windows.Forms.Button B_Open_DEC;
        private System.Windows.Forms.Button B_Save_CMP;
        private System.Windows.Forms.Button B_Save_DEC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NUP_Diamonds;
        private System.Windows.Forms.NumericUpDown NUP_P;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NUP_Rank;
        private System.Windows.Forms.GroupBox GB_MonEdit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox CB_MonSelection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox CB_Move2;
        private System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.ComboBox CB_Trait;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.PictureBox PB_SelectedMon;
        private System.Windows.Forms.CheckBox CHK_MEvo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown NUP_Trait;
        private System.Windows.Forms.NumericUpDown NUP_ST6;
        private System.Windows.Forms.ComboBox CB_ST6;
        private System.Windows.Forms.NumericUpDown NUP_ST5;
        private System.Windows.Forms.ComboBox CB_ST5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown NUP_ST4;
        private System.Windows.Forms.ComboBox CB_ST4;
        private System.Windows.Forms.NumericUpDown NUP_ST3;
        private System.Windows.Forms.ComboBox CB_ST3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown NUP_ST2;
        private System.Windows.Forms.ComboBox CB_ST2;
        private System.Windows.Forms.NumericUpDown NUP_ST1;
        private System.Windows.Forms.ComboBox CB_ST1;
    }
}

