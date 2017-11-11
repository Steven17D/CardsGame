namespace CardsGame
{
    partial class GameClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.YourScore = new System.Windows.Forms.Label();
            this.OpponentsScore = new System.Windows.Forms.Label();
            this.MainButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // YourScore
            // 
            this.YourScore.AutoSize = true;
            this.YourScore.Location = new System.Drawing.Point(10, 10);
            this.YourScore.Name = "YourScore";
            this.YourScore.Size = new System.Drawing.Size(35, 13);
            this.YourScore.TabIndex = 0;
            this.YourScore.Text = "label1";
            // 
            // OpponentsScore
            // 
            this.OpponentsScore.AutoSize = true;
            this.OpponentsScore.Location = new System.Drawing.Point(10, 26);
            this.OpponentsScore.Name = "OpponentsScore";
            this.OpponentsScore.Size = new System.Drawing.Size(35, 13);
            this.OpponentsScore.TabIndex = 1;
            this.OpponentsScore.Text = "label2";
            // 
            // MainButton
            // 
            this.MainButton.Location = new System.Drawing.Point(483, 10);
            this.MainButton.Name = "MainButton";
            this.MainButton.Size = new System.Drawing.Size(75, 23);
            this.MainButton.TabIndex = 2;
            this.MainButton.Text = "button1";
            this.MainButton.UseVisualStyleBackColor = true;
            this.MainButton.Click += new System.EventHandler(this.MainButton_Click);
            // 
            // GameClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(1109, 261);
            this.Controls.Add(this.MainButton);
            this.Controls.Add(this.OpponentsScore);
            this.Controls.Add(this.YourScore);
            this.MaximizeBox = false;
            this.Name = "GameClientForm";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.Desk_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label YourScore;
        private System.Windows.Forms.Label OpponentsScore;
        private System.Windows.Forms.Button MainButton;
    }
}

