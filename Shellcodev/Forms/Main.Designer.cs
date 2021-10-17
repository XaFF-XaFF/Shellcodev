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
            this.Instruction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.testerBldBtn = new System.Windows.Forms.Button();
            this.shlcTestBtn = new System.Windows.Forms.Button();
            this.dllAddrBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
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
            this.instructionGrid.Location = new System.Drawing.Point(35, 161);
            this.instructionGrid.Name = "instructionGrid";
            this.instructionGrid.Size = new System.Drawing.Size(371, 389);
            this.instructionGrid.TabIndex = 0;
            this.instructionGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.instructionGrid_CellEndEdit);
            this.instructionGrid.SelectionChanged += new System.EventHandler(this.instructionGrid_SelectionChanged);
            // 
            // instructionTxt
            // 
            this.instructionTxt.Location = new System.Drawing.Point(35, 566);
            this.instructionTxt.Name = "instructionTxt";
            this.instructionTxt.Size = new System.Drawing.Size(239, 20);
            this.instructionTxt.TabIndex = 1;
            this.instructionTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.instructionTxt_KeyDown);
            // 
            // addInstructionBtn
            // 
            this.addInstructionBtn.Location = new System.Drawing.Point(331, 564);
            this.addInstructionBtn.Name = "addInstructionBtn";
            this.addInstructionBtn.Size = new System.Drawing.Size(75, 22);
            this.addInstructionBtn.TabIndex = 2;
            this.addInstructionBtn.Text = "Add";
            this.addInstructionBtn.UseVisualStyleBackColor = true;
            this.addInstructionBtn.Click += new System.EventHandler(this.addInstructionBtn_Click);
            // 
            // bytesBox
            // 
            this.bytesBox.Location = new System.Drawing.Point(445, 161);
            this.bytesBox.Name = "bytesBox";
            this.bytesBox.Size = new System.Drawing.Size(340, 389);
            this.bytesBox.TabIndex = 3;
            this.bytesBox.Text = "";
            // 
            // registersBox
            // 
            this.registersBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registersBox.Location = new System.Drawing.Point(35, 39);
            this.registersBox.Multiline = false;
            this.registersBox.Name = "registersBox";
            this.registersBox.ReadOnly = true;
            this.registersBox.Size = new System.Drawing.Size(371, 21);
            this.registersBox.TabIndex = 4;
            this.registersBox.Text = "EAX: 00000000 EBX: 00000000 ECX: 00000000 EDX: 00000000";
            // 
            // pointersBox
            // 
            this.pointersBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pointersBox.Location = new System.Drawing.Point(35, 121);
            this.pointersBox.Multiline = false;
            this.pointersBox.Name = "pointersBox";
            this.pointersBox.ReadOnly = true;
            this.pointersBox.Size = new System.Drawing.Size(278, 21);
            this.pointersBox.TabIndex = 4;
            this.pointersBox.Text = "EIP: 00000000 ESP: 00000000 EBP: 00000000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Pointers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Registers";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(442, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Assembled instructions";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Instructions";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.richTextBox1.Location = new System.Drawing.Point(35, 80);
            this.richTextBox1.Multiline = false;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(186, 22);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "EDI: 00000000 ESI: 00000000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Indexes";
            // 
            // Instruction
            // 
            this.Instruction.HeaderText = "Instruction";
            this.Instruction.Name = "Instruction";
            this.Instruction.Width = 328;
            // 
            // testerBldBtn
            // 
            this.testerBldBtn.Location = new System.Drawing.Point(445, 564);
            this.testerBldBtn.Name = "testerBldBtn";
            this.testerBldBtn.Size = new System.Drawing.Size(121, 22);
            this.testerBldBtn.TabIndex = 2;
            this.testerBldBtn.Text = "Build shellcode tester";
            this.testerBldBtn.UseVisualStyleBackColor = true;
            // 
            // shlcTestBtn
            // 
            this.shlcTestBtn.Location = new System.Drawing.Point(664, 564);
            this.shlcTestBtn.Name = "shlcTestBtn";
            this.shlcTestBtn.Size = new System.Drawing.Size(121, 22);
            this.shlcTestBtn.TabIndex = 2;
            this.shlcTestBtn.Text = "Test shellcode";
            this.shlcTestBtn.UseVisualStyleBackColor = true;
            // 
            // dllAddrBox
            // 
            this.dllAddrBox.Location = new System.Drawing.Point(478, 98);
            this.dllAddrBox.Name = "dllAddrBox";
            this.dllAddrBox.Size = new System.Drawing.Size(121, 20);
            this.dllAddrBox.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(443, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Get address:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(664, 98);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(121, 20);
            this.textBox1.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(442, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "DLL:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(607, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Function:";
            // 
            // getAddrBtn
            // 
            this.getAddrBtn.Location = new System.Drawing.Point(591, 124);
            this.getAddrBtn.Name = "getAddrBtn";
            this.getAddrBtn.Size = new System.Drawing.Size(84, 22);
            this.getAddrBtn.TabIndex = 2;
            this.getAddrBtn.Text = "Get Address";
            this.getAddrBtn.UseVisualStyleBackColor = true;
            // 
            // cRBtn
            // 
            this.cRBtn.AutoSize = true;
            this.cRBtn.Location = new System.Drawing.Point(651, 31);
            this.cRBtn.Name = "cRBtn";
            this.cRBtn.Size = new System.Drawing.Size(67, 17);
            this.cRBtn.TabIndex = 7;
            this.cRBtn.TabStop = true;
            this.cRBtn.Text = "C Format";
            this.cRBtn.UseVisualStyleBackColor = true;
            // 
            // csRBtn
            // 
            this.csRBtn.AutoSize = true;
            this.csRBtn.Location = new System.Drawing.Point(651, 52);
            this.csRBtn.Name = "csRBtn";
            this.csRBtn.Size = new System.Drawing.Size(74, 17);
            this.csRBtn.TabIndex = 7;
            this.csRBtn.TabStop = true;
            this.csRBtn.Text = "C# Format";
            this.csRBtn.UseVisualStyleBackColor = true;
            // 
            // generateBtn
            // 
            this.generateBtn.Location = new System.Drawing.Point(540, 23);
            this.generateBtn.Name = "generateBtn";
            this.generateBtn.Size = new System.Drawing.Size(100, 55);
            this.generateBtn.TabIndex = 2;
            this.generateBtn.Text = "Generate";
            this.generateBtn.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 595);
            this.Controls.Add(this.csRBtn);
            this.Controls.Add(this.cRBtn);
            this.Controls.Add(this.textBox1);
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button getAddrBtn;
        private System.Windows.Forms.RadioButton cRBtn;
        private System.Windows.Forms.RadioButton csRBtn;
        private System.Windows.Forms.Button generateBtn;
    }
}