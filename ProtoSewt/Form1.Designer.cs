using System.Windows.Forms;

namespace ProtoSewt
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.imgUpldBtn = new System.Windows.Forms.Button();
            this.imgUplddDisp = new System.Windows.Forms.PictureBox();
            this.txtXtrBtn = new System.Windows.Forms.Button();
            this.txtBoxRes = new System.Windows.Forms.TextBox();
            this.listBoxFonts = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgUplddDisp)).BeginInit();
            this.SuspendLayout();
            // 
            // imgUpldBtn
            // 
            this.imgUpldBtn.Location = new System.Drawing.Point(493, 32);
            this.imgUpldBtn.Name = "imgUpldBtn";
            this.imgUpldBtn.Size = new System.Drawing.Size(155, 23);
            this.imgUpldBtn.TabIndex = 0;
            this.imgUpldBtn.Text = "Ajouter une image";
            this.imgUpldBtn.UseVisualStyleBackColor = true;
            this.imgUpldBtn.Click += new System.EventHandler(this.imgUpldBtn_Click);
            // 
            // imgUplddDisp
            // 
            this.imgUplddDisp.Location = new System.Drawing.Point(493, 61);
            this.imgUplddDisp.Name = "imgUplddDisp";
            this.imgUplddDisp.Size = new System.Drawing.Size(155, 113);
            this.imgUplddDisp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgUplddDisp.TabIndex = 1;
            this.imgUplddDisp.TabStop = false;
            // 
            // txtXtrBtn
            // 
            this.txtXtrBtn.Location = new System.Drawing.Point(654, 93);
            this.txtXtrBtn.Name = "txtXtrBtn";
            this.txtXtrBtn.Size = new System.Drawing.Size(130, 42);
            this.txtXtrBtn.TabIndex = 2;
            this.txtXtrBtn.Text = "Extraire le texte\n de l\'image";
            this.txtXtrBtn.UseVisualStyleBackColor = true;
            this.txtXtrBtn.Click += new System.EventHandler(this.txtXtrBtn_Click);
            // 
            // txtBoxRes
            // 
            this.txtBoxRes.Location = new System.Drawing.Point(258, 107);
            this.txtBoxRes.Multiline = true;
            this.txtBoxRes.Name = "txtBoxRes";
            this.txtBoxRes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBoxRes.Size = new System.Drawing.Size(229, 293);
            this.txtBoxRes.TabIndex = 3;
            // 
            // listBoxFonts
            // 
            this.listBoxFonts.FormattingEnabled = true;
            this.listBoxFonts.Location = new System.Drawing.Point(654, 163);
            this.listBoxFonts.Name = "listBoxFonts";
            this.listBoxFonts.Size = new System.Drawing.Size(120, 95);
            this.listBoxFonts.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 519);
            this.Controls.Add(this.listBoxFonts);
            this.Controls.Add(this.txtBoxRes);
            this.Controls.Add(this.txtXtrBtn);
            this.Controls.Add(this.imgUplddDisp);
            this.Controls.Add(this.imgUpldBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imgUplddDisp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button imgUpldBtn;
        private System.Windows.Forms.PictureBox imgUplddDisp;
        private System.Windows.Forms.Button txtXtrBtn;
        private System.Windows.Forms.TextBox txtBoxRes;
        private ListBox listBoxFonts;

        public ScrollBars Vertical { get; private set; }
    }
}

