using System.Drawing;
using System.Windows.Forms;
using System;

namespace Shaski_Bakhmut
{
    partial class SetupForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label player1Label;
        private TextBox player1TextBox;
        private Label player2Label;
        private TextBox player2TextBox;
        private Label colorLabel;
        private ComboBox colorComboBox1;
        private ComboBox colorComboBox2;
        private CheckBox randomColorsCheckBox;
        private Button startButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.player1Label = new System.Windows.Forms.Label();
            this.player1TextBox = new System.Windows.Forms.TextBox();
            this.player2Label = new System.Windows.Forms.Label();
            this.player2TextBox = new System.Windows.Forms.TextBox();
            this.colorLabel = new System.Windows.Forms.Label();
            this.colorComboBox1 = new System.Windows.Forms.ComboBox();
            this.colorComboBox2 = new System.Windows.Forms.ComboBox();
            this.randomColorsCheckBox = new System.Windows.Forms.CheckBox();
            this.startButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // player1Label
            // 
            this.player1Label.AutoSize = true;
            this.player1Label.Location = new System.Drawing.Point(9, 9);
            this.player1Label.Name = "player1Label";
            this.player1Label.Size = new System.Drawing.Size(114, 13);
            this.player1Label.TabIndex = 0;
            this.player1Label.Text = "Имя первого игрока:";
            // 
            // player1TextBox
            // 
            this.player1TextBox.Location = new System.Drawing.Point(140, 9);
            this.player1TextBox.Name = "player1TextBox";
            this.player1TextBox.Size = new System.Drawing.Size(129, 20);
            this.player1TextBox.TabIndex = 1;
            // 
            // player2Label
            // 
            this.player2Label.AutoSize = true;
            this.player2Label.Location = new System.Drawing.Point(9, 35);
            this.player2Label.Name = "player2Label";
            this.player2Label.Size = new System.Drawing.Size(113, 13);
            this.player2Label.TabIndex = 2;
            this.player2Label.Text = "Имя второго игрока:";
            // 
            // player2TextBox
            // 
            this.player2TextBox.Location = new System.Drawing.Point(140, 35);
            this.player2TextBox.Name = "player2TextBox";
            this.player2TextBox.Size = new System.Drawing.Size(129, 20);
            this.player2TextBox.TabIndex = 3;
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = true;
            this.colorLabel.Location = new System.Drawing.Point(9, 61);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(92, 13);
            this.colorLabel.TabIndex = 4;
            this.colorLabel.Text = "Выберите цвета:";
            // 
            // colorComboBox1
            // 
            this.colorComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorComboBox1.Items.AddRange(new object[] {
            "Белые",
            "Черные"});
            this.colorComboBox1.Location = new System.Drawing.Point(140, 61);
            this.colorComboBox1.Name = "colorComboBox1";
            this.colorComboBox1.Size = new System.Drawing.Size(129, 21);
            this.colorComboBox1.TabIndex = 5;
            // 
            // colorComboBox2
            // 
            this.colorComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorComboBox2.Enabled = false;
            this.colorComboBox2.Items.AddRange(new object[] {
            "Черные",
            "Белые"});
            this.colorComboBox2.Location = new System.Drawing.Point(140, 87);
            this.colorComboBox2.Name = "colorComboBox2";
            this.colorComboBox2.Size = new System.Drawing.Size(129, 21);
            this.colorComboBox2.TabIndex = 6;
            // 
            // randomColorsCheckBox
            // 
            this.randomColorsCheckBox.AutoSize = true;
            this.randomColorsCheckBox.Location = new System.Drawing.Point(9, 113);
            this.randomColorsCheckBox.Name = "randomColorsCheckBox";
            this.randomColorsCheckBox.Size = new System.Drawing.Size(113, 17);
            this.randomColorsCheckBox.TabIndex = 7;
            this.randomColorsCheckBox.Text = "Случайные цвета";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(100, 155);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(86, 26);
            this.startButton.TabIndex = 8;
            this.startButton.Text = "Начать игру";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 193);
            this.Controls.Add(this.player1Label);
            this.Controls.Add(this.player1TextBox);
            this.Controls.Add(this.player2Label);
            this.Controls.Add(this.player2TextBox);
            this.Controls.Add(this.colorLabel);
            this.Controls.Add(this.colorComboBox1);
            this.Controls.Add(this.colorComboBox2);
            this.Controls.Add(this.randomColorsCheckBox);
            this.Controls.Add(this.startButton);
            this.Name = "SetupForm";
            this.Text = "Настройка игры";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
