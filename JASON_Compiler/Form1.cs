﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            string Code=textBox1.Text.ToLower();
            JASON_Compiler.Start_Compiling(Code);
            PrintTokens();
            treeView1.Nodes.Add(Parser.PrintParseTree(JASON_Compiler.treeroot));
            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < JASON_Compiler.Jason_Scanner.Tokens.Count; i++)
            {
               dataGridView1.Rows.Add(JASON_Compiler.Jason_Scanner.Tokens.ElementAt(i).lex, JASON_Compiler.Jason_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for (int i = 0; i < Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i] + "\r\n";
            }

            textBox3.Text += "\nParser Errors:\r\n";

            for (int i = 0; i < Errors.Parser_Error_List.Count; i++)
            {
                textBox3.Text += Errors.Parser_Error_List[i] + "\r\n";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";

            textBox3.Text = "";
            JASON_Compiler.TokenStream.Clear();
            JASON_Compiler.Jason_Scanner.Tokens.Clear();
            dataGridView1.Rows.Clear();
            treeView1.Nodes.Clear();
            Errors.Error_List.Clear();
            Errors.Parser_Error_List.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
