namespace Compiler
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
            this.btnCompiler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvTokens = new System.Windows.Forms.DataGridView();
            this.LexemeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tokenClassColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtbxSource = new System.Windows.Forms.TextBox();
            this.dgverrors = new System.Windows.Forms.DataGridView();
            this.columnErrorList = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnErrorType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTokens)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgverrors)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCompiler
            // 
            this.btnCompiler.Location = new System.Drawing.Point(631, 186);
            this.btnCompiler.Name = "btnCompiler";
            this.btnCompiler.Size = new System.Drawing.Size(133, 43);
            this.btnCompiler.TabIndex = 0;
            this.btnCompiler.Text = "Compile";
            this.btnCompiler.UseVisualStyleBackColor = true;
            this.btnCompiler.Click += new System.EventHandler(this.btnCompiler_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Fax", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Lucida Fax", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(377, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tokens";
            // 
            // dgvTokens
            // 
            this.dgvTokens.AllowUserToAddRows = false;
            this.dgvTokens.AllowUserToDeleteRows = false;
            this.dgvTokens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTokens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTokens.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LexemeColumn,
            this.tokenClassColumn});
            this.dgvTokens.Location = new System.Drawing.Point(380, 40);
            this.dgvTokens.Name = "dgvTokens";
            this.dgvTokens.ReadOnly = true;
            this.dgvTokens.Size = new System.Drawing.Size(245, 334);
            this.dgvTokens.TabIndex = 3;
            // 
            // LexemeColumn
            // 
            this.LexemeColumn.HeaderText = "Lexeme";
            this.LexemeColumn.Name = "LexemeColumn";
            this.LexemeColumn.ReadOnly = true;
            // 
            // tokenClassColumn
            // 
            this.tokenClassColumn.HeaderText = "Token Class";
            this.tokenClassColumn.Name = "tokenClassColumn";
            this.tokenClassColumn.ReadOnly = true;
            // 
            // txtbxSource
            // 
            this.txtbxSource.Location = new System.Drawing.Point(15, 41);
            this.txtbxSource.Multiline = true;
            this.txtbxSource.Name = "txtbxSource";
            this.txtbxSource.Size = new System.Drawing.Size(346, 334);
            this.txtbxSource.TabIndex = 4;
            // 
            // dgverrors
            // 
            this.dgverrors.AllowUserToAddRows = false;
            this.dgverrors.AllowUserToDeleteRows = false;
            this.dgverrors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgverrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgverrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnErrorList,
            this.columnErrorType});
            this.dgverrors.Location = new System.Drawing.Point(15, 430);
            this.dgverrors.Name = "dgverrors";
            this.dgverrors.ReadOnly = true;
            this.dgverrors.Size = new System.Drawing.Size(749, 159);
            this.dgverrors.TabIndex = 5;
            // 
            // columnErrorList
            // 
            this.columnErrorList.HeaderText = "Error List";
            this.columnErrorList.Name = "columnErrorList";
            this.columnErrorList.ReadOnly = true;
            // 
            // columnErrorType
            // 
            this.columnErrorType.HeaderText = "Error Type";
            this.columnErrorType.Name = "columnErrorType";
            this.columnErrorType.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Lucida Fax", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 395);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Error List";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 599);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgverrors);
            this.Controls.Add(this.txtbxSource);
            this.Controls.Add(this.dgvTokens);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCompiler);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTokens)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgverrors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCompiler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvTokens;
        private System.Windows.Forms.DataGridViewTextBoxColumn LexemeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tokenClassColumn;
        private System.Windows.Forms.TextBox txtbxSource;
        private System.Windows.Forms.DataGridView dgverrors;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnErrorList;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnErrorType;
        private System.Windows.Forms.Label label3;
    }
}

