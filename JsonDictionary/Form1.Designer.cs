namespace JsonDictionary
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.checkedListBox_params = new System.Windows.Forms.CheckedListBox();
            this.button_saveDb = new System.Windows.Forms.Button();
            this.button_loadDb = new System.Windows.Forms.Button();
            this.button_validateFiles = new System.Windows.Forms.Button();
            this.button_collectDatabase = new System.Windows.Forms.Button();
            this.textBox_logText = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_json = new System.Windows.Forms.TreeView();
            this.button_reAdjust = new System.Windows.Forms.Button();
            this.comboBox_versions = new System.Windows.Forms.ComboBox();
            this.checkBox_seachCaseSensitive = new System.Windows.Forms.CheckBox();
            this.textBox_searchHistory = new System.Windows.Forms.TextBox();
            this.textBox_searchString = new System.Windows.Forms.TextBox();
            this.comboBox_condition = new System.Windows.Forms.ComboBox();
            this.dataGridView_examples = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip_findValue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FindAllStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_findField = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FindFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_examples)).BeginInit();
            this.contextMenuStrip_findValue.SuspendLayout();
            this.contextMenuStrip_findField.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(988, 726);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(980, 700);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data collection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.checkedListBox_params);
            this.splitContainer2.Panel1.Controls.Add(this.button_saveDb);
            this.splitContainer2.Panel1.Controls.Add(this.button_loadDb);
            this.splitContainer2.Panel1.Controls.Add(this.button_validateFiles);
            this.splitContainer2.Panel1.Controls.Add(this.button_collectDatabase);
            this.splitContainer2.Panel1MinSize = 120;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox_logText);
            this.splitContainer2.Size = new System.Drawing.Size(974, 694);
            this.splitContainer2.SplitterDistance = 209;
            this.splitContainer2.TabIndex = 18;
            // 
            // checkedListBox_params
            // 
            this.checkedListBox_params.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox_params.CheckOnClick = true;
            this.checkedListBox_params.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkedListBox_params.FormattingEnabled = true;
            this.checkedListBox_params.HorizontalScrollbar = true;
            this.checkedListBox_params.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox_params.Name = "checkedListBox_params";
            this.checkedListBox_params.Size = new System.Drawing.Size(208, 574);
            this.checkedListBox_params.TabIndex = 15;
            // 
            // button_saveDb
            // 
            this.button_saveDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_saveDb.Location = new System.Drawing.Point(0, 671);
            this.button_saveDb.Name = "button_saveDb";
            this.button_saveDb.Size = new System.Drawing.Size(208, 23);
            this.button_saveDb.TabIndex = 17;
            this.button_saveDb.Text = "Save database";
            this.button_saveDb.UseVisualStyleBackColor = true;
            this.button_saveDb.Click += new System.EventHandler(this.Button_saveDb_Click);
            // 
            // button_loadDb
            // 
            this.button_loadDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_loadDb.Location = new System.Drawing.Point(0, 642);
            this.button_loadDb.Name = "button_loadDb";
            this.button_loadDb.Size = new System.Drawing.Size(208, 23);
            this.button_loadDb.TabIndex = 16;
            this.button_loadDb.Text = "Load database";
            this.button_loadDb.UseVisualStyleBackColor = true;
            this.button_loadDb.Click += new System.EventHandler(this.Button_loadDb_Click);
            // 
            // button_validateFiles
            // 
            this.button_validateFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_validateFiles.Location = new System.Drawing.Point(0, 584);
            this.button_validateFiles.Name = "button_validateFiles";
            this.button_validateFiles.Size = new System.Drawing.Size(208, 23);
            this.button_validateFiles.TabIndex = 0;
            this.button_validateFiles.Text = "Validate files";
            this.button_validateFiles.UseVisualStyleBackColor = true;
            this.button_validateFiles.Click += new System.EventHandler(this.Button_validateFiles_Click);
            // 
            // button_collectDatabase
            // 
            this.button_collectDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_collectDatabase.Location = new System.Drawing.Point(-1, 613);
            this.button_collectDatabase.Name = "button_collectDatabase";
            this.button_collectDatabase.Size = new System.Drawing.Size(208, 23);
            this.button_collectDatabase.TabIndex = 0;
            this.button_collectDatabase.Text = "Collect database";
            this.button_collectDatabase.UseVisualStyleBackColor = true;
            this.button_collectDatabase.Click += new System.EventHandler(this.Button_collectDatabase_Click);
            // 
            // textBox_logText
            // 
            this.textBox_logText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_logText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_logText.Location = new System.Drawing.Point(0, 0);
            this.textBox_logText.Multiline = true;
            this.textBox_logText.Name = "textBox_logText";
            this.textBox_logText.ReadOnly = true;
            this.textBox_logText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_logText.Size = new System.Drawing.Size(761, 694);
            this.textBox_logText.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(980, 700);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Samples tree";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_json);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button_reAdjust);
            this.splitContainer1.Panel2.Controls.Add(this.comboBox_versions);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_seachCaseSensitive);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_searchHistory);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_searchString);
            this.splitContainer1.Panel2.Controls.Add(this.comboBox_condition);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView_examples);
            this.splitContainer1.Panel2MinSize = 350;
            this.splitContainer1.Size = new System.Drawing.Size(974, 694);
            this.splitContainer1.SplitterDistance = 324;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView_json
            // 
            this.treeView_json.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_json.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView_json.HideSelection = false;
            this.treeView_json.Location = new System.Drawing.Point(0, 0);
            this.treeView_json.Name = "treeView_json";
            this.treeView_json.Size = new System.Drawing.Size(324, 694);
            this.treeView_json.TabIndex = 0;
            this.treeView_json.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_json_NodeMouseClick);
            this.treeView_json.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_json_NodeMouseDoubleClick);
            // 
            // button_reAdjust
            // 
            this.button_reAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_reAdjust.Location = new System.Drawing.Point(571, 0);
            this.button_reAdjust.Name = "button_reAdjust";
            this.button_reAdjust.Size = new System.Drawing.Size(75, 23);
            this.button_reAdjust.TabIndex = 12;
            this.button_reAdjust.Text = "Readjust";
            this.button_reAdjust.UseVisualStyleBackColor = true;
            this.button_reAdjust.Click += new System.EventHandler(this.Button_readjust_Click);
            // 
            // comboBox_versions
            // 
            this.comboBox_versions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_versions.FormattingEnabled = true;
            this.comboBox_versions.Items.AddRange(new object[] {
            "Any"});
            this.comboBox_versions.Location = new System.Drawing.Point(0, 0);
            this.comboBox_versions.Name = "comboBox_versions";
            this.comboBox_versions.Size = new System.Drawing.Size(61, 21);
            this.comboBox_versions.TabIndex = 11;
            this.comboBox_versions.SelectedIndexChanged += new System.EventHandler(this.ComboBox_versions_SelectedIndexChanged);
            // 
            // checkBox_seachCaseSensitive
            // 
            this.checkBox_seachCaseSensitive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_seachCaseSensitive.AutoSize = true;
            this.checkBox_seachCaseSensitive.Location = new System.Drawing.Point(471, 3);
            this.checkBox_seachCaseSensitive.Name = "checkBox_seachCaseSensitive";
            this.checkBox_seachCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.checkBox_seachCaseSensitive.TabIndex = 10;
            this.checkBox_seachCaseSensitive.Text = "Case sensitive";
            this.checkBox_seachCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // textBox_searchHistory
            // 
            this.textBox_searchHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_searchHistory.Location = new System.Drawing.Point(0, 674);
            this.textBox_searchHistory.Name = "textBox_searchHistory";
            this.textBox_searchHistory.ReadOnly = true;
            this.textBox_searchHistory.Size = new System.Drawing.Size(646, 20);
            this.textBox_searchHistory.TabIndex = 9;
            // 
            // textBox_searchString
            // 
            this.textBox_searchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_searchString.Location = new System.Drawing.Point(143, 0);
            this.textBox_searchString.Name = "textBox_searchString";
            this.textBox_searchString.Size = new System.Drawing.Size(322, 20);
            this.textBox_searchString.TabIndex = 8;
            this.textBox_searchString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_searchString_KeyDown);
            // 
            // comboBox_condition
            // 
            this.comboBox_condition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_condition.FormattingEnabled = true;
            this.comboBox_condition.Location = new System.Drawing.Point(67, 0);
            this.comboBox_condition.Name = "comboBox_condition";
            this.comboBox_condition.Size = new System.Drawing.Size(70, 21);
            this.comboBox_condition.TabIndex = 7;
            // 
            // dataGridView_examples
            // 
            this.dataGridView_examples.AllowUserToAddRows = false;
            this.dataGridView_examples.AllowUserToDeleteRows = false;
            this.dataGridView_examples.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_examples.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView_examples.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_examples.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_examples.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_examples.Location = new System.Drawing.Point(0, 26);
            this.dataGridView_examples.Name = "dataGridView_examples";
            this.dataGridView_examples.RowHeadersWidth = 20;
            dataGridViewCellStyle2.NullValue = "Adjust";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_examples.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_examples.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_examples.Size = new System.Drawing.Size(646, 646);
            this.dataGridView_examples.TabIndex = 6;
            this.dataGridView_examples.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_examples_CellContentDoubleClick);
            this.dataGridView_examples.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_examples_CellMouseDown);
            this.dataGridView_examples.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_examples_RowHeaderMouseDoubleClick);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 723);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(988, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select root folder";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileDialog1_FileOk);
            // 
            // contextMenuStrip_findValue
            // 
            this.contextMenuStrip_findValue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindAllStripMenuItem});
            this.contextMenuStrip_findValue.Name = "contextMenuStrip_findInParent";
            this.contextMenuStrip_findValue.Size = new System.Drawing.Size(179, 26);
            this.contextMenuStrip_findValue.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_findValue_Opening);
            // 
            // FindAllStripMenuItem
            // 
            this.FindAllStripMenuItem.Name = "FindAllStripMenuItem";
            this.FindAllStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.FindAllStripMenuItem.Text = "Find value in parent";
            this.FindAllStripMenuItem.Click += new System.EventHandler(this.FindValueToolStripMenuItem_Click);
            // 
            // contextMenuStrip_findField
            // 
            this.contextMenuStrip_findField.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindFieldToolStripMenuItem});
            this.contextMenuStrip_findField.Name = "contextMenuStrip_findField";
            this.contextMenuStrip_findField.Size = new System.Drawing.Size(174, 26);
            this.contextMenuStrip_findField.Text = "Find field in parent";
            this.contextMenuStrip_findField.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_findField_Opening);
            // 
            // FindFieldToolStripMenuItem
            // 
            this.FindFieldToolStripMenuItem.Name = "FindFieldToolStripMenuItem";
            this.FindFieldToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.FindFieldToolStripMenuItem.Text = "Find field in parent";
            this.FindFieldToolStripMenuItem.Click += new System.EventHandler(this.FindFieldToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 749);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.progressBar1);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "Form1";
            this.Text = "KineticJsonDictionary";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_examples)).EndInit();
            this.contextMenuStrip_findValue.ResumeLayout(false);
            this.contextMenuStrip_findField.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TreeView treeView_json;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox_logText;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button_collectDatabase;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckedListBox checkedListBox_params;
        private System.Windows.Forms.Button button_saveDb;
        private System.Windows.Forms.Button button_loadDb;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dataGridView_examples;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox textBox_searchString;
        private System.Windows.Forms.ComboBox comboBox_condition;
        private System.Windows.Forms.CheckBox checkBox_seachCaseSensitive;
        private System.Windows.Forms.TextBox textBox_searchHistory;
        private System.Windows.Forms.ComboBox comboBox_versions;
        private System.Windows.Forms.Button button_reAdjust;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_findValue;
        private System.Windows.Forms.ToolStripMenuItem FindAllStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_findField;
        private System.Windows.Forms.ToolStripMenuItem FindFieldToolStripMenuItem;
        private System.Windows.Forms.Button button_validateFiles;
    }
}

