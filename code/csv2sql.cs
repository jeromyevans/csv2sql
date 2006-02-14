// ----------------------------------------------------------------------------------------------------------
//
// Tool to read a CSV file and generate SQL insert statement to insert the data into a table
//
// Started 13 January 2006
// Written by Jeromy Evans
// 
// History:
//  14 Feb 2006 - csv parse now detects special values in the fields and remembers metadata about them.
// Currently used to treating blanks as null and not quoting the values 'null' and 'NOW()'.  This needs to be
// improved further
//              - reads a metadata file called <tablename>.clm.txt that currently only contains the names of
//  the columns in the csv file for inclusion in the insert statements (instead of manually typing it).  This
//  metadata file is generated at the same time as the sql file
// ---[ Blue Sky Minds Pty Ltd ]-----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace com.blueskyminds
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
   public class Csv2SqlForm : System.Windows.Forms.Form
   {
      private System.Windows.Forms.Label CsvFileLabel;
      private System.Windows.Forms.Button BrowseCSVButton;
      private System.Windows.Forms.Button StartButton;
      private System.Windows.Forms.OpenFileDialog openFileDialog;
      private System.Windows.Forms.TextBox fileNameTextBox;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.TextBox tableNameTextBox;
      private System.Windows.Forms.Label tableNameLabel;
      private System.Windows.Forms.TextBox SQLListBox;
      private System.Windows.Forms.CheckBox PrefixWithDeleteCheckbox;
      private System.Windows.Forms.GroupBox Options;
      private System.Windows.Forms.TextBox SqlQuoteCharTextBox;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox SqlQuoteCharEscapedTextBox;
      private System.Windows.Forms.CheckBox ColumnNameCheckbox;
      private System.Windows.Forms.TextBox ColumnNamesTextBox;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      protected const string META_DATA_FILE_EXTENSION = ".clm.txt";

      // ----------------------------------------------------------------------------------------------------

      public Csv2SqlForm()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         //
         // TODO: Add any constructor code after InitializeComponent call
         //
         StartButton.Enabled = false;
         PrefixWithDeleteCheckbox.Checked = true;
         ColumnNamesTextBox.Enabled = false;
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if (components != null) 
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

      // ----------------------------------------------------------------------------------------------------

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Csv2SqlForm));
         this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
         this.CsvFileLabel = new System.Windows.Forms.Label();
         this.BrowseCSVButton = new System.Windows.Forms.Button();
         this.StartButton = new System.Windows.Forms.Button();
         this.fileNameTextBox = new System.Windows.Forms.TextBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.SQLListBox = new System.Windows.Forms.TextBox();
         this.tableNameTextBox = new System.Windows.Forms.TextBox();
         this.tableNameLabel = new System.Windows.Forms.Label();
         this.PrefixWithDeleteCheckbox = new System.Windows.Forms.CheckBox();
         this.Options = new System.Windows.Forms.GroupBox();
         this.ColumnNamesTextBox = new System.Windows.Forms.TextBox();
         this.ColumnNameCheckbox = new System.Windows.Forms.CheckBox();
         this.SqlQuoteCharEscapedTextBox = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.SqlQuoteCharTextBox = new System.Windows.Forms.TextBox();
         this.groupBox1.SuspendLayout();
         this.Options.SuspendLayout();
         this.SuspendLayout();
         // 
         // openFileDialog
         // 
         this.openFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
         // 
         // CsvFileLabel
         // 
         this.CsvFileLabel.Location = new System.Drawing.Point(8, 8);
         this.CsvFileLabel.Name = "CsvFileLabel";
         this.CsvFileLabel.Size = new System.Drawing.Size(80, 23);
         this.CsvFileLabel.TabIndex = 1;
         this.CsvFileLabel.Text = "Input CSV File:";
         // 
         // BrowseCSVButton
         // 
         this.BrowseCSVButton.Location = new System.Drawing.Point(360, 8);
         this.BrowseCSVButton.Name = "BrowseCSVButton";
         this.BrowseCSVButton.Size = new System.Drawing.Size(72, 23);
         this.BrowseCSVButton.TabIndex = 2;
         this.BrowseCSVButton.Text = "Browse...";
         this.BrowseCSVButton.Click += new System.EventHandler(this.BrowseCSVButton_Click);
         // 
         // StartButton
         // 
         this.StartButton.Location = new System.Drawing.Point(360, 40);
         this.StartButton.Name = "StartButton";
         this.StartButton.Size = new System.Drawing.Size(72, 23);
         this.StartButton.TabIndex = 3;
         this.StartButton.Text = "Start";
         this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
         // 
         // fileNameTextBox
         // 
         this.fileNameTextBox.Location = new System.Drawing.Point(96, 8);
         this.fileNameTextBox.Name = "fileNameTextBox";
         this.fileNameTextBox.Size = new System.Drawing.Size(256, 20);
         this.fileNameTextBox.TabIndex = 4;
         this.fileNameTextBox.Text = "";
         this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.SQLListBox);
         this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.groupBox1.Location = new System.Drawing.Point(0, 246);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(440, 96);
         this.groupBox1.TabIndex = 7;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "SQL Preview";
         // 
         // SQLListBox
         // 
         this.SQLListBox.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.SQLListBox.Location = new System.Drawing.Point(3, 13);
         this.SQLListBox.Multiline = true;
         this.SQLListBox.Name = "SQLListBox";
         this.SQLListBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.SQLListBox.Size = new System.Drawing.Size(434, 80);
         this.SQLListBox.TabIndex = 0;
         this.SQLListBox.Text = "";
         // 
         // tableNameTextBox
         // 
         this.tableNameTextBox.Location = new System.Drawing.Point(96, 32);
         this.tableNameTextBox.Name = "tableNameTextBox";
         this.tableNameTextBox.Size = new System.Drawing.Size(256, 20);
         this.tableNameTextBox.TabIndex = 8;
         this.tableNameTextBox.Text = "";
         // 
         // tableNameLabel
         // 
         this.tableNameLabel.Location = new System.Drawing.Point(8, 32);
         this.tableNameLabel.Name = "tableNameLabel";
         this.tableNameLabel.Size = new System.Drawing.Size(80, 23);
         this.tableNameLabel.TabIndex = 9;
         this.tableNameLabel.Text = "Table Name";
         // 
         // PrefixWithDeleteCheckbox
         // 
         this.PrefixWithDeleteCheckbox.Location = new System.Drawing.Point(8, 24);
         this.PrefixWithDeleteCheckbox.Name = "PrefixWithDeleteCheckbox";
         this.PrefixWithDeleteCheckbox.Size = new System.Drawing.Size(168, 16);
         this.PrefixWithDeleteCheckbox.TabIndex = 11;
         this.PrefixWithDeleteCheckbox.Text = "prefix with DELETE FROM";
         // 
         // Options
         // 
         this.Options.Controls.Add(this.ColumnNamesTextBox);
         this.Options.Controls.Add(this.ColumnNameCheckbox);
         this.Options.Controls.Add(this.SqlQuoteCharEscapedTextBox);
         this.Options.Controls.Add(this.label2);
         this.Options.Controls.Add(this.label1);
         this.Options.Controls.Add(this.SqlQuoteCharTextBox);
         this.Options.Controls.Add(this.PrefixWithDeleteCheckbox);
         this.Options.Location = new System.Drawing.Point(0, 64);
         this.Options.Name = "Options";
         this.Options.Size = new System.Drawing.Size(440, 176);
         this.Options.TabIndex = 12;
         this.Options.TabStop = false;
         this.Options.Text = "Options";
         // 
         // ColumnNamesTextBox
         // 
         this.ColumnNamesTextBox.Location = new System.Drawing.Point(8, 88);
         this.ColumnNamesTextBox.Name = "ColumnNamesTextBox";
         this.ColumnNamesTextBox.Size = new System.Drawing.Size(424, 20);
         this.ColumnNamesTextBox.TabIndex = 17;
         this.ColumnNamesTextBox.Text = "";
         // 
         // ColumnNameCheckbox
         // 
         this.ColumnNameCheckbox.Location = new System.Drawing.Point(8, 64);
         this.ColumnNameCheckbox.Name = "ColumnNameCheckbox";
         this.ColumnNameCheckbox.Size = new System.Drawing.Size(152, 24);
         this.ColumnNameCheckbox.TabIndex = 16;
         this.ColumnNameCheckbox.Text = "Include column names";
         this.ColumnNameCheckbox.CheckedChanged += new System.EventHandler(this.ColumnNameCheckbox_CheckedChanged);
         // 
         // SqlQuoteCharEscapedTextBox
         // 
         this.SqlQuoteCharEscapedTextBox.Location = new System.Drawing.Point(264, 40);
         this.SqlQuoteCharEscapedTextBox.Name = "SqlQuoteCharEscapedTextBox";
         this.SqlQuoteCharEscapedTextBox.Size = new System.Drawing.Size(32, 20);
         this.SqlQuoteCharEscapedTextBox.TabIndex = 15;
         this.SqlQuoteCharEscapedTextBox.Text = "\'\'";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(160, 48);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(104, 16);
         this.label2.TabIndex = 14;
         this.label2.Text = "Sql Escaped Quote";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 48);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(104, 16);
         this.label1.TabIndex = 13;
         this.label1.Text = "Sql Quote";
         // 
         // SqlQuoteCharTextBox
         // 
         this.SqlQuoteCharTextBox.Location = new System.Drawing.Point(120, 40);
         this.SqlQuoteCharTextBox.Name = "SqlQuoteCharTextBox";
         this.SqlQuoteCharTextBox.Size = new System.Drawing.Size(32, 20);
         this.SqlQuoteCharTextBox.TabIndex = 12;
         this.SqlQuoteCharTextBox.Text = "\'";
         // 
         // Csv2SqlForm
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(440, 342);
         this.Controls.Add(this.tableNameLabel);
         this.Controls.Add(this.tableNameTextBox);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.fileNameTextBox);
         this.Controls.Add(this.StartButton);
         this.Controls.Add(this.BrowseCSVButton);
         this.Controls.Add(this.CsvFileLabel);
         this.Controls.Add(this.Options);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "Csv2SqlForm";
         this.Text = "CSV to SQL Conversion Tool";
         this.Load += new System.EventHandler(this.Csv2SqlForm_Load);
         this.groupBox1.ResumeLayout(false);
         this.Options.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main() 
      {
         Application.Run(new Csv2SqlForm());
      }

      // ----------------------------------------------------------------------------------------------------

      private void Csv2SqlForm_Load(object sender, System.EventArgs e)
      {
      
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// reads an entire text file into memory for parsing
      /// </summary>
      /// <param name="fileName"></param>
      /// <returns>a string containing the entire content</returns>
      protected string readTextFile(string fileName)
      {
         string content = "";
         StreamReader sr = null;

         try 
         {
            sr = File.OpenText(fileName);
            content = sr.ReadToEnd();
         }
         catch (Exception)
         {
         }
         finally 
         {
            if (sr != null)
            {
               sr.Close();
            }
         }
         return content;
      }

      // ----------------------------------------------------------------------------------------------------
      // ----------------------------------------------------------------------------------------------------

      private void BrowseCSVButton_Click(object sender, System.EventArgs e)
      {
         DialogResult result = openFileDialog.ShowDialog(this);
         string filename;
         FileInfo fileInfo;

         if (result == DialogResult.OK)
         {
            filename = openFileDialog.FileName;
            fileNameTextBox.Text = filename;
            fileInfo = new FileInfo(filename);
            tableNameTextBox.Text = fileInfo.Name.Replace(fileInfo.Extension, "");            
            
            // read the column name data
            readMetaData();
         }
      }

      // ----------------------------------------------------------------------------------------------------

      char sqlQuoteChar = '\'';
      string sqlQuoteEscaped = "\'\'";

      /// <summary>
      /// Escape text for SQL insert
      /// </summary>
      /// <param name="textToEscape"></param>
      /// <returns></returns>
      protected string escape(string textToEscape)
      {         
         return textToEscape.Replace(SqlQuoteCharTextBox.Text, SqlQuoteCharEscapedTextBox.Text);
      }

      // ----------------------------------------------------------------------------------------------------

      protected void SaveText(string text, string targetFile)
      {
         StreamWriter sw = null;

         try
         {
            if (File.Exists(targetFile))
            {
               File.Delete(targetFile);
            }

            sw = File.CreateText(targetFile);
            sw.Write(text);
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
         finally
         {
            if (sw != null)
            {
               sw.Close();
            }
         }
      }

      // ----------------------------------------------------------------------------------------------------

      private void StartButton_Click(object sender, System.EventArgs e)
      {
         string filename = fileNameTextBox.Text;
         CsvTextReader csv;
         int skippedLines = 0;
         int linesToSkip = 0;
         string[] values;
         StringBuilder sql = new StringBuilder();
         bool firstItem = true;         
         int lineNo = 0;

         StringBuilder sqlText = new StringBuilder();         
         SQLListBox.ResetText();

         string tableName = tableNameTextBox.Text;
         string targetFileName = tableName+".sql";         

         if ((filename != null) && (filename.Length > 0))
         {
            if (PrefixWithDeleteCheckbox.Checked)
            {
               sqlText.Append("\r\ndelete from "+tableName+";\r\n\r\n");
            }
            

            csv = new CsvTextReader(filename);
          
            // start reading the lines of the source file
            // each line of the csv file generates multiple DataSet entries
            while (csv.Read())
            {
               sql.Length = 0;

               if (skippedLines >= linesToSkip)
               {
                  if (csv.NonBlank)
                  {
                     values = csv.getAsStrings();

                     sql.Append("insert into ");
                     sql.Append(tableName);
                     if (ColumnNameCheckbox.Checked)
                     {
                        sql.Append(" ("+this.ColumnNamesTextBox.Text+")");
                     }
                     sql.Append(" values");

                     sql.Append(" (");
                     
                     firstItem = true;
                     foreach (string thisValue in values)
                     {                        
                        if (firstItem)
                        {
                           firstItem = false;
                        }
                        else
                        {                        
                           sql.Append(",");
                        }
                        sql.Append(thisValue);
                     }
                     sql.Append(");\r\n\r\n");
                     
                     lineNo++;
                  }                  
                  sqlText.Append(sql.ToString());               
               }
               else
               {
                  skippedLines++;
               }
            }

            SQLListBox.Text = sqlText.ToString();
            SaveText(sqlText.ToString(), targetFileName);

            // save the metadata
            if (ColumnNameCheckbox.Checked)
            {
               saveMetaData();
            }
         }
      }

      // ----------------------------------------------------------------------------------------------------
      /// <summary>
      /// Creates a file with metadata about the columns in the csv file
      /// </summary>
      protected void saveMetaData()
      {
         string tableName = tableNameTextBox.Text;
         string metaDataFileName = tableName+META_DATA_FILE_EXTENSION;

         SaveText(ColumnNamesTextBox.Text, metaDataFileName);
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Attempts to read a file that contains metadata about the columns in the csv file
      /// </summary>
      protected void readMetaData()
      {
         string metaData = this.readTextFile(tableNameTextBox.Text+META_DATA_FILE_EXTENSION);
         string[] lines;
         char[] separators = {'\n', '\r'};
         string metaDataLine = null;
         bool clearColumnNames = true;

         if (metaData != null)
         {
            // metadata has been provided
            lines = metaData.Split(separators);
            foreach (string line in lines)
            {
               if ((line != null) && (line.Length > 0))
               {
                  metaDataLine = line;
                  break;
               }                       
            } 

            if (metaDataLine != null)
            {
               // set the field on the form
               ColumnNamesTextBox.Enabled = true;
               ColumnNamesTextBox.Text = metaDataLine;
               ColumnNameCheckbox.Checked = true;
               clearColumnNames = false;
            }                          

         }

         if (clearColumnNames)
         {
            // clear the field on the form
            ColumnNamesTextBox.Enabled = false;
            ColumnNamesTextBox.Text = "";
            ColumnNameCheckbox.Checked = false;
         }         
      }

      // ----------------------------------------------------------------------------------------------------

      private void fileNameTextBox_TextChanged(object sender, System.EventArgs e)
      {
         if ((fileNameTextBox.Text != null) && (fileNameTextBox.Text.Length > 0))
         {
            StartButton.Enabled = true;
         }
         else
         {
            StartButton.Enabled = false;
         }      
      }

      // ----------------------------------------------------------------------------------------------------

      private void ColumnNameCheckbox_CheckedChanged(object sender, System.EventArgs e)
      {
         if (ColumnNameCheckbox.Checked)
         {
            ColumnNamesTextBox.Enabled = true;
         }
         else
         {
            ColumnNamesTextBox.Enabled = false;
         }
      }

	}
}
