namespace Shellcodev.Forms
{
    partial class Main
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
            this.instructionGrid = new System.Windows.Forms.DataGridView();
            this.Instruction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.instructionTxt = new System.Windows.Forms.TextBox();
            this.addInstructionBtn = new System.Windows.Forms.Button();
            this.bytesBox = new System.Windows.Forms.RichTextBox();
            this.registersBox = new System.Windows.Forms.RichTextBox();
            this.pointersBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.testerBldBtn = new System.Windows.Forms.Button();
            this.shlcTestBtn = new System.Windows.Forms.Button();
            this.dllAddrBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.funcTxt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.getAddrBtn = new System.Windows.Forms.Button();
            this.cRBtn = new System.Windows.Forms.RadioButton();
            this.csRBtn = new System.Windows.Forms.RadioButton();
            this.generateBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.instructionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // instructionGrid
            // 
            this.instructionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.instructionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Instruction});
            this.instructionGrid.Location = new System.Drawing.Point(47, 198);
            this.instructionGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instructionGrid.Name = "instructionGrid";
            this.instructionGrid.RowHeadersWidth = 51;
            this.instructionGrid.Size = new System.Drawing.Size(495, 479);
            this.instructionGrid.TabIndex = 0;
            this.instructionGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.instructionGrid_CellEndEdit);
            this.instructionGrid.SelectionChanged += new System.EventHandler(this.instructionGrid_SelectionChanged);
            // 
            // Instruction
            // 
            this.Instruction.HeaderText = "Instruction";
            this.Instruction.MinimumWidth = 6;
            this.Instruction.Name = "Instruction";
            this.Instruction.Width = 328;
            // 
            // instructionTxt
            // 
            this.instructionTxt.Location = new System.Drawing.Point(47, 697);
            this.instructionTxt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instructionTxt.Name = "instructionTxt";
            this.instructionTxt.Size = new System.Drawing.Size(317, 22);
            this.instructionTxt.TabIndex = 1;
            this.instructionTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.instructionTxt_KeyDown);
            // 
            // addInstructionBtn
            // 
            this.addInstructionBtn.Location = new System.Drawing.Point(441, 694);
            this.addInstructionBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.addInstructionBtn.Name = "addInstructionBtn";
            this.addInstructionBtn.Size = new System.Drawing.Size(100, 27);
            this.addInstructionBtn.TabIndex = 2;
            this.addInstructionBtn.Text = "Add";
            this.addInstructionBtn.UseVisualStyleBackColor = true;
            this.addInstructionBtn.Click += new System.EventHandler(this.addInstructionBtn_Click);
            // 
            // bytesBox
            // 
            this.bytesBox.Location = new System.Drawing.Point(593, 198);
            this.bytesBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bytesBox.Name = "bytesBox";
            this.bytesBox.Size = new System.Drawing.Size(452, 478);
            this.bytesBox.TabIndex = 3;
            this.bytesBox.Text = "";
            // 
            // registersBox
            // 
            this.registersBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registersBox.Location = new System.Drawing.Point(47, 48);
            this.registersBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.registersBox.Multiline = false;
            this.registersBox.Name = "registersBox";
            this.registersBox.ReadOnly = true;
            this.registersBox.Size = new System.Drawing.Size(493, 25);
            this.registersBox.TabIndex = 4;
            this.registersBox.Text = "EAX: 00000000 EBX: 00000000 ECX: 00000000 EDX: 00000000";
            // 
            // pointersBox
            // 
            this.pointersBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pointersBox.Location = new System.Drawing.Point(47, 149);
            this.pointersBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pointersBox.Multiline = false;
            this.pointersBox.Name = "pointersBox";
            this.pointersBox.ReadOnly = true;
            this.pointersBox.Size = new System.Drawing.Size(369, 25);
            this.pointersBox.TabIndex = 4;
            this.pointersBox.Text = "EIP: 00000000 ESP: 00000000 EBP: 00000000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 129);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Pointers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Registers";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(589, 178);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Assembled instructions";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 178);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "Instructions";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.richTextBox1.Location = new System.Drawing.Point(47, 98);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBox1.Multiline = false;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(247, 26);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "EDI: 00000000 ESI: 00000000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 79);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Indexes";
            // 
            // testerBldBtn
            // 
            this.testerBldBtn.Location = new System.Drawing.Point(593, 694);
            this.testerBldBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.testerBldBtn.Name = "testerBldBtn";
            this.testerBldBtn.Size = new System.Drawing.Size(161, 27);
            this.testerBldBtn.TabIndex = 2;
            this.testerBldBtn.Text = "Build shellcode tester";
            this.testerBldBtn.UseVisualStyleBackColor = true;
            this.testerBldBtn.Click += new System.EventHandler(this.testerBldBtn_Click);
            // 
            // shlcTestBtn
            // 
            this.shlcTestBtn.Location = new System.Drawing.Point(885, 694);
            this.shlcTestBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.shlcTestBtn.Name = "shlcTestBtn";
            this.shlcTestBtn.Size = new System.Drawing.Size(161, 27);
            this.shlcTestBtn.TabIndex = 2;
            this.shlcTestBtn.Text = "Test shellcode";
            this.shlcTestBtn.UseVisualStyleBackColor = true;
            this.shlcTestBtn.Click += new System.EventHandler(this.shlcTestBtn_Click);
            // 
            // dllAddrBox
            // 
            this.dllAddrBox.Location = new System.Drawing.Point(637, 121);
            this.dllAddrBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dllAddrBox.Name = "dllAddrBox";
            this.dllAddrBox.Size = new System.Drawing.Size(160, 22);
            this.dllAddrBox.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(591, 100);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Get address:";
            // 
            // funcTxt
            // 
            this.funcTxt.Location = new System.Drawing.Point(885, 121);
            this.funcTxt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.funcTxt.Name = "funcTxt";
            this.funcTxt.Size = new System.Drawing.Size(160, 22);
            this.funcTxt.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(589, 124);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 17);
            this.label7.TabIndex = 5;
            this.label7.Text = "DLL:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(809, 124);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 17);
            this.label8.TabIndex = 5;
            this.label8.Text = "Function:";
            // 
            // getAddrBtn
            // 
            this.getAddrBtn.Location = new System.Drawing.Point(788, 153);
            this.getAddrBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.getAddrBtn.Name = "getAddrBtn";
            this.getAddrBtn.Size = new System.Drawing.Size(112, 27);
            this.getAddrBtn.TabIndex = 2;
            this.getAddrBtn.Text = "Get Address";
            this.getAddrBtn.UseVisualStyleBackColor = true;
            this.getAddrBtn.Click += new System.EventHandler(this.getAddrBtn_Click);
            // 
            // cRBtn
            // 
            this.cRBtn.AutoSize = true;
            this.cRBtn.Location = new System.Drawing.Point(868, 38);
            this.cRBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cRBtn.Name = "cRBtn";
            this.cRBtn.Size = new System.Drawing.Size(86, 21);
            this.cRBtn.TabIndex = 7;
            this.cRBtn.TabStop = true;
            this.cRBtn.Text = "C Format";
            this.cRBtn.UseVisualStyleBackColor = true;
            this.cRBtn.CheckedChanged += new System.EventHandler(this.cRBtn_CheckedChanged);
            // 
            // csRBtn
            // 
            this.csRBtn.AutoSize = true;
            this.csRBtn.Location = new System.Drawing.Point(868, 64);
            this.csRBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.csRBtn.Name = "csRBtn";
            this.csRBtn.Size = new System.Drawing.Size(94, 21);
            this.csRBtn.TabIndex = 7;
            this.csRBtn.TabStop = true;
            this.csRBtn.Text = "C# Format";
            this.csRBtn.UseVisualStyleBackColor = true;
            this.csRBtn.CheckedChanged += new System.EventHandler(this.csRBtn_CheckedChanged);
            // 
            // generateBtn
            // 
            this.generateBtn.Location = new System.Drawing.Point(720, 28);
            this.generateBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.generateBtn.Name = "generateBtn";
            this.generateBtn.Size = new System.Drawing.Size(133, 68);
            this.generateBtn.TabIndex = 2;
            this.generateBtn.Text = "Generate";
            this.generateBtn.UseVisualStyleBackColor = true;
            this.generateBtn.Click += new System.EventHandler(this.generateBtn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 732);
            this.Controls.Add(this.csRBtn);
            this.Controls.Add(this.cRBtn);
            this.Controls.Add(this.funcTxt);
            this.Controls.Add(this.dllAddrBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.pointersBox);
            this.Controls.Add(this.registersBox);
            this.Controls.Add(this.bytesBox);
            this.Controls.Add(this.shlcTestBtn);
            this.Controls.Add(this.testerBldBtn);
            this.Controls.Add(this.generateBtn);
            this.Controls.Add(this.getAddrBtn);
            this.Controls.Add(this.addInstructionBtn);
            this.Controls.Add(this.instructionTxt);
            this.Controls.Add(this.instructionGrid);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximumSize = new System.Drawing.Size(1127, 779);
            this.MinimumSize = new System.Drawing.Size(1127, 779);
            this.Name = "Main";
            this.Text = "Shellcodev";
            ((System.ComponentModel.ISupportInitialize)(this.instructionGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox instructionTxt;
        private System.Windows.Forms.Button addInstructionBtn;
        public System.Windows.Forms.DataGridView instructionGrid;
        public System.Windows.Forms.RichTextBox bytesBox;
        private System.Windows.Forms.RichTextBox registersBox;
        private System.Windows.Forms.RichTextBox pointersBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Instruction;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button testerBldBtn;
        private System.Windows.Forms.Button shlcTestBtn;
        private System.Windows.Forms.TextBox dllAddrBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox funcTxt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button getAddrBtn;
        private System.Windows.Forms.RadioButton cRBtn;
        private System.Windows.Forms.RadioButton csRBtn;
        private System.Windows.Forms.Button generateBtn;
    }
}