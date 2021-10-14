namespace Shellcodev
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
            this.instructionTxt = new System.Windows.Forms.TextBox();
            this.instructionGrid = new System.Windows.Forms.DataGridView();
            this.addinstructionBtn = new System.Windows.Forms.Button();
            this.genBtn = new System.Windows.Forms.Button();
            this.radioC = new System.Windows.Forms.RadioButton();
            this.radioCS = new System.Windows.Forms.RadioButton();
            this.makeBtn = new System.Windows.Forms.Button();
            this.bytesBox = new System.Windows.Forms.RichTextBox();
            this.Instructions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.instructionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // instructionTxt
            // 
            this.instructionTxt.Location = new System.Drawing.Point(24, 388);
            this.instructionTxt.Name = "instructionTxt";
            this.instructionTxt.Size = new System.Drawing.Size(256, 20);
            this.instructionTxt.TabIndex = 0;
            this.instructionTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.instructionTxt_KeyDown);
            // 
            // instructionGrid
            // 
            this.instructionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.instructionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Instructions});
            this.instructionGrid.Location = new System.Drawing.Point(24, 27);
            this.instructionGrid.Name = "instructionGrid";
            this.instructionGrid.Size = new System.Drawing.Size(343, 341);
            this.instructionGrid.TabIndex = 1;
            // 
            // addinstructionBtn
            // 
            this.addinstructionBtn.Location = new System.Drawing.Point(292, 388);
            this.addinstructionBtn.Name = "addinstructionBtn";
            this.addinstructionBtn.Size = new System.Drawing.Size(75, 20);
            this.addinstructionBtn.TabIndex = 2;
            this.addinstructionBtn.Text = "Add";
            this.addinstructionBtn.UseVisualStyleBackColor = true;
            this.addinstructionBtn.Click += new System.EventHandler(this.addinstructionBtn_Click);
            // 
            // genBtn
            // 
            this.genBtn.Location = new System.Drawing.Point(24, 444);
            this.genBtn.Name = "genBtn";
            this.genBtn.Size = new System.Drawing.Size(75, 23);
            this.genBtn.TabIndex = 2;
            this.genBtn.Text = "Generate";
            this.genBtn.UseVisualStyleBackColor = true;
            // 
            // radioC
            // 
            this.radioC.AutoSize = true;
            this.radioC.Location = new System.Drawing.Point(105, 438);
            this.radioC.Name = "radioC";
            this.radioC.Size = new System.Drawing.Size(67, 17);
            this.radioC.TabIndex = 3;
            this.radioC.TabStop = true;
            this.radioC.Text = "C Format";
            this.radioC.UseVisualStyleBackColor = true;
            // 
            // radioCS
            // 
            this.radioCS.AutoSize = true;
            this.radioCS.Location = new System.Drawing.Point(105, 458);
            this.radioCS.Name = "radioCS";
            this.radioCS.Size = new System.Drawing.Size(74, 17);
            this.radioCS.TabIndex = 3;
            this.radioCS.TabStop = true;
            this.radioCS.Text = "C# Format";
            this.radioCS.UseVisualStyleBackColor = true;
            // 
            // makeBtn
            // 
            this.makeBtn.Location = new System.Drawing.Point(636, 133);
            this.makeBtn.Name = "makeBtn";
            this.makeBtn.Size = new System.Drawing.Size(75, 23);
            this.makeBtn.TabIndex = 2;
            this.makeBtn.Text = "Make";
            this.makeBtn.UseVisualStyleBackColor = true;
            this.makeBtn.Click += new System.EventHandler(this.makeBtn_Click);
            // 
            // bytesBox
            // 
            this.bytesBox.Location = new System.Drawing.Point(498, 27);
            this.bytesBox.Name = "bytesBox";
            this.bytesBox.Size = new System.Drawing.Size(362, 100);
            this.bytesBox.TabIndex = 4;
            this.bytesBox.Text = "";
            // 
            // Instructions
            // 
            this.Instructions.HeaderText = "Instruction";
            this.Instructions.Name = "Instructions";
            this.Instructions.Width = 300;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 490);
            this.Controls.Add(this.bytesBox);
            this.Controls.Add(this.radioCS);
            this.Controls.Add(this.radioC);
            this.Controls.Add(this.genBtn);
            this.Controls.Add(this.makeBtn);
            this.Controls.Add(this.addinstructionBtn);
            this.Controls.Add(this.instructionGrid);
            this.Controls.Add(this.instructionTxt);
            this.Name = "Main";
            this.Text = "Shellcodev";
            ((System.ComponentModel.ISupportInitialize)(this.instructionGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox instructionTxt;
        private System.Windows.Forms.Button addinstructionBtn;
        private System.Windows.Forms.Button genBtn;
        private System.Windows.Forms.RadioButton radioC;
        private System.Windows.Forms.RadioButton radioCS;
        public System.Windows.Forms.DataGridView instructionGrid;
        private System.Windows.Forms.Button makeBtn;
        public System.Windows.Forms.RichTextBox bytesBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Instructions;
    }
}

