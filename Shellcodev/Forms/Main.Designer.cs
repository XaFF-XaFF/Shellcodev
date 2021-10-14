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
            this.Instruction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bytesBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.instructionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // instructionGrid
            // 
            this.instructionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.instructionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Instruction});
            this.instructionGrid.Location = new System.Drawing.Point(38, 31);
            this.instructionGrid.Name = "instructionGrid";
            this.instructionGrid.Size = new System.Drawing.Size(343, 389);
            this.instructionGrid.TabIndex = 0;
            // 
            // instructionTxt
            // 
            this.instructionTxt.Location = new System.Drawing.Point(38, 436);
            this.instructionTxt.Name = "instructionTxt";
            this.instructionTxt.Size = new System.Drawing.Size(239, 20);
            this.instructionTxt.TabIndex = 1;
            this.instructionTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.instructionTxt_KeyDown);
            // 
            // addInstructionBtn
            // 
            this.addInstructionBtn.Location = new System.Drawing.Point(306, 436);
            this.addInstructionBtn.Name = "addInstructionBtn";
            this.addInstructionBtn.Size = new System.Drawing.Size(75, 22);
            this.addInstructionBtn.TabIndex = 2;
            this.addInstructionBtn.Text = "Add";
            this.addInstructionBtn.UseVisualStyleBackColor = true;
            this.addInstructionBtn.Click += new System.EventHandler(this.addInstructionBtn_Click);
            // 
            // Instruction
            // 
            this.Instruction.HeaderText = "Instruction";
            this.Instruction.Name = "Instruction";
            this.Instruction.Width = 300;
            // 
            // bytesBox
            // 
            this.bytesBox.Location = new System.Drawing.Point(549, 31);
            this.bytesBox.Name = "bytesBox";
            this.bytesBox.Size = new System.Drawing.Size(350, 96);
            this.bytesBox.TabIndex = 3;
            this.bytesBox.Text = "";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 485);
            this.Controls.Add(this.bytesBox);
            this.Controls.Add(this.addInstructionBtn);
            this.Controls.Add(this.instructionTxt);
            this.Controls.Add(this.instructionGrid);
            this.Name = "Main";
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.instructionGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox instructionTxt;
        private System.Windows.Forms.Button addInstructionBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Instruction;
        public System.Windows.Forms.DataGridView instructionGrid;
        public System.Windows.Forms.RichTextBox bytesBox;
    }
}