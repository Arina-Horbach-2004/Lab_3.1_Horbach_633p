namespace Lab_3._1_Horbach_633p
{
    partial class Lab_3_1_Horbach_633p
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_result = new Button();
            label1 = new Label();
            textBox_matrix = new TextBox();
            textBox_A = new TextBox();
            label2 = new Label();
            label3 = new Label();
            textBox_B = new TextBox();
            label4 = new Label();
            textBox_cost = new TextBox();
            button_example1 = new Button();
            button_example2 = new Button();
            button_example3 = new Button();
            textBox_rounds = new TextBox();
            label5 = new Label();
            button_modeling = new Button();
            button_protocol = new Button();
            SuspendLayout();
            // 
            // button_result
            // 
            button_result.Location = new Point(288, 203);
            button_result.Name = "button_result";
            button_result.Size = new Size(222, 47);
            button_result.TabIndex = 0;
            button_result.Text = "Знайти розв'язки гри";
            button_result.UseVisualStyleBackColor = true;
            button_result.Click += button_result_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(102, 20);
            label1.TabIndex = 1;
            label1.Text = "Матриця гри:";
            // 
            // textBox_matrix
            // 
            textBox_matrix.Location = new Point(12, 32);
            textBox_matrix.Multiline = true;
            textBox_matrix.Name = "textBox_matrix";
            textBox_matrix.Size = new Size(244, 218);
            textBox_matrix.TabIndex = 2;
            // 
            // textBox_A
            // 
            textBox_A.Location = new Point(288, 32);
            textBox_A.Name = "textBox_A";
            textBox_A.Size = new Size(222, 27);
            textBox_A.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(288, 9);
            label2.Name = "label2";
            label2.Size = new Size(215, 20);
            label2.TabIndex = 4;
            label2.Text = "Змішані стратегії 1-го гравця:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(288, 72);
            label3.Name = "label3";
            label3.Size = new Size(215, 20);
            label3.TabIndex = 5;
            label3.Text = "Змішані стратегії 2-го гравця:";
            // 
            // textBox_B
            // 
            textBox_B.Location = new Point(288, 95);
            textBox_B.Name = "textBox_B";
            textBox_B.Size = new Size(222, 27);
            textBox_B.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(288, 137);
            label4.Name = "label4";
            label4.Size = new Size(72, 20);
            label4.TabIndex = 7;
            label4.Text = "Ціна гри:";
            // 
            // textBox_cost
            // 
            textBox_cost.Location = new Point(288, 160);
            textBox_cost.Name = "textBox_cost";
            textBox_cost.Size = new Size(222, 27);
            textBox_cost.TabIndex = 8;
            // 
            // button_example1
            // 
            button_example1.Location = new Point(290, 270);
            button_example1.Name = "button_example1";
            button_example1.Size = new Size(222, 37);
            button_example1.TabIndex = 9;
            button_example1.Text = "Приклад 1";
            button_example1.UseVisualStyleBackColor = true;
            button_example1.Click += button_example1_Click;
            // 
            // button_example2
            // 
            button_example2.Location = new Point(290, 322);
            button_example2.Name = "button_example2";
            button_example2.Size = new Size(222, 34);
            button_example2.TabIndex = 10;
            button_example2.Text = "Приклад 2";
            button_example2.UseVisualStyleBackColor = true;
            button_example2.Click += button_example2_Click;
            // 
            // button_example3
            // 
            button_example3.Location = new Point(290, 370);
            button_example3.Name = "button_example3";
            button_example3.Size = new Size(222, 36);
            button_example3.TabIndex = 11;
            button_example3.Text = "Приклад 3";
            button_example3.UseVisualStyleBackColor = true;
            button_example3.Click += button_example3_Click;
            // 
            // textBox_rounds
            // 
            textBox_rounds.Location = new Point(12, 283);
            textBox_rounds.Name = "textBox_rounds";
            textBox_rounds.Size = new Size(244, 27);
            textBox_rounds.TabIndex = 13;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 260);
            label5.Name = "label5";
            label5.Size = new Size(122, 20);
            label5.TabIndex = 12;
            label5.Text = "Кількість партій:";
            // 
            // button_modeling
            // 
            button_modeling.Location = new Point(12, 325);
            button_modeling.Name = "button_modeling";
            button_modeling.Size = new Size(244, 39);
            button_modeling.TabIndex = 14;
            button_modeling.Text = "Змоделювати гру";
            button_modeling.UseVisualStyleBackColor = true;
            button_modeling.Click += button_modeling_Click;
            // 
            // button_protocol
            // 
            button_protocol.Location = new Point(12, 370);
            button_protocol.Name = "button_protocol";
            button_protocol.Size = new Size(244, 34);
            button_protocol.TabIndex = 15;
            button_protocol.Text = "Протокол обчислення";
            button_protocol.UseVisualStyleBackColor = true;
            button_protocol.Click += button_protocol_Click;
            // 
            // Lab_3_1_Horbach_633p
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 419);
            Controls.Add(button_protocol);
            Controls.Add(button_modeling);
            Controls.Add(textBox_rounds);
            Controls.Add(label5);
            Controls.Add(button_example3);
            Controls.Add(button_example2);
            Controls.Add(button_example1);
            Controls.Add(textBox_cost);
            Controls.Add(label4);
            Controls.Add(textBox_B);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox_A);
            Controls.Add(textBox_matrix);
            Controls.Add(label1);
            Controls.Add(button_result);
            Name = "Lab_3_1_Horbach_633p";
            Text = "Lab_3.1_Horbach_633p";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_result;
        private Label label1;
        private TextBox textBox_matrix;
        private TextBox textBox_A;
        private Label label2;
        private Label label3;
        private TextBox textBox_B;
        private Label label4;
        private TextBox textBox_cost;
        private Button button_example1;
        private Button button_example2;
        private Button button_example3;
        private TextBox textBox_rounds;
        private Label label5;
        private Button button_modeling;
        private Button button_protocol;
    }
}
