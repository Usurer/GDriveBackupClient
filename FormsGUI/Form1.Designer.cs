namespace FormsGUI
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableRemote = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLocal = new System.Windows.Forms.TableLayoutPanel();
            this.progressRemote = new System.Windows.Forms.ProgressBar();
            this.progressLocal = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableRemote);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 331);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Remote";
            // 
            // tableRemote
            // 
            this.tableRemote.ColumnCount = 1;
            this.tableRemote.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRemote.Location = new System.Drawing.Point(7, 20);
            this.tableRemote.Name = "tableRemote";
            this.tableRemote.RowCount = 1;
            this.tableRemote.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableRemote.Size = new System.Drawing.Size(298, 305);
            this.tableRemote.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLocal);
            this.groupBox2.Location = new System.Drawing.Point(381, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 331);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Local";
            // 
            // tableLocal
            // 
            this.tableLocal.ColumnCount = 1;
            this.tableLocal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLocal.Location = new System.Drawing.Point(7, 20);
            this.tableLocal.Name = "tableLocal";
            this.tableLocal.RowCount = 1;
            this.tableLocal.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLocal.Size = new System.Drawing.Size(298, 299);
            this.tableLocal.TabIndex = 1;
            // 
            // progressRemote
            // 
            this.progressRemote.Location = new System.Drawing.Point(13, 365);
            this.progressRemote.Name = "progressRemote";
            this.progressRemote.Size = new System.Drawing.Size(311, 23);
            this.progressRemote.TabIndex = 2;
            // 
            // progressLocal
            // 
            this.progressLocal.Location = new System.Drawing.Point(381, 365);
            this.progressLocal.Name = "progressLocal";
            this.progressLocal.Size = new System.Drawing.Size(311, 23);
            this.progressLocal.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 400);
            this.Controls.Add(this.progressLocal);
            this.Controls.Add(this.progressRemote);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableRemote;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLocal;
        private System.Windows.Forms.ProgressBar progressRemote;
        private System.Windows.Forms.ProgressBar progressLocal;
    }
}

