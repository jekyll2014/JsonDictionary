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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_DataCollection = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.checkBox_vsCode = new System.Windows.Forms.CheckBox();
            this.checkBox_loadDbOnStart = new System.Windows.Forms.CheckBox();
            this.checkBox_alwaysOnTop = new System.Windows.Forms.CheckBox();
            this.checkBox_showPreview = new System.Windows.Forms.CheckBox();
            this.checkBox_reformatJson = new System.Windows.Forms.CheckBox();
            this.button_saveDb = new System.Windows.Forms.Button();
            this.button_loadDb = new System.Windows.Forms.Button();
            this.button_collectDatabase = new System.Windows.Forms.Button();
            this.textBox_logText = new System.Windows.Forms.TextBox();
            this.tabPage_SamplesTree = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_examples = new System.Windows.Forms.TreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.dataGridView_examples = new System.Windows.Forms.DataGridView();
            this.listBox_fileList = new System.Windows.Forms.ListBox();
            this.textBox_ExSearchHistory = new System.Windows.Forms.TextBox();
            this.comboBox_ExVersions = new System.Windows.Forms.ComboBox();
            this.button_ExAdjustRows = new System.Windows.Forms.Button();
            this.comboBox_ExCondition = new System.Windows.Forms.ComboBox();
            this.textBox_ExSearchString = new System.Windows.Forms.TextBox();
            this.checkBox_ExCaseSensitive = new System.Windows.Forms.CheckBox();
            this.button_ExClearSearch = new System.Windows.Forms.Button();
            this.label_descSave = new System.Windows.Forms.Label();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage_DataCollection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPage_SamplesTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_examples)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage_DataCollection);
            this.tabControl1.Controls.Add(this.tabPage_SamplesTree);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(784, 516);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabControl1_Selecting);
            // 
            // tabPage_DataCollection
            // 
            this.tabPage_DataCollection.Controls.Add(this.splitContainer2);
            this.tabPage_DataCollection.Location = new System.Drawing.Point(4, 22);
            this.tabPage_DataCollection.Name = "tabPage_DataCollection";
            this.tabPage_DataCollection.Size = new System.Drawing.Size(776, 490);
            this.tabPage_DataCollection.TabIndex = 0;
            this.tabPage_DataCollection.Text = "Data collection";
            this.tabPage_DataCollection.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_vsCode);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_loadDbOnStart);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_alwaysOnTop);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_showPreview);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_reformatJson);
            this.splitContainer2.Panel1.Controls.Add(this.button_saveDb);
            this.splitContainer2.Panel1.Controls.Add(this.button_loadDb);
            this.splitContainer2.Panel1.Controls.Add(this.button_collectDatabase);
            this.splitContainer2.Panel1MinSize = 120;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox_logText);
            this.splitContainer2.Size = new System.Drawing.Size(776, 490);
            this.splitContainer2.SplitterDistance = 133;
            this.splitContainer2.TabIndex = 18;
            // 
            // checkBox_vsCode
            // 
            this.checkBox_vsCode.AutoSize = true;
            this.checkBox_vsCode.Location = new System.Drawing.Point(5, 159);
            this.checkBox_vsCode.Name = "checkBox_vsCode";
            this.checkBox_vsCode.Size = new System.Drawing.Size(87, 17);
            this.checkBox_vsCode.TabIndex = 11;
            this.checkBox_vsCode.Text = "Use VSCode";
            this.checkBox_vsCode.UseVisualStyleBackColor = true;
            this.checkBox_vsCode.CheckedChanged += new System.EventHandler(this.CheckBox_vsCode_CheckedChanged);
            // 
            // checkBox_loadDbOnStart
            // 
            this.checkBox_loadDbOnStart.AutoSize = true;
            this.checkBox_loadDbOnStart.Location = new System.Drawing.Point(5, 90);
            this.checkBox_loadDbOnStart.Name = "checkBox_loadDbOnStart";
            this.checkBox_loadDbOnStart.Size = new System.Drawing.Size(121, 17);
            this.checkBox_loadDbOnStart.TabIndex = 5;
            this.checkBox_loadDbOnStart.Text = "Load DB on start-up";
            this.checkBox_loadDbOnStart.UseVisualStyleBackColor = true;
            this.checkBox_loadDbOnStart.CheckedChanged += new System.EventHandler(this.CheckBox_loadDbOnStart_CheckedChanged);
            // 
            // checkBox_alwaysOnTop
            // 
            this.checkBox_alwaysOnTop.AutoSize = true;
            this.checkBox_alwaysOnTop.Location = new System.Drawing.Point(5, 182);
            this.checkBox_alwaysOnTop.Name = "checkBox_alwaysOnTop";
            this.checkBox_alwaysOnTop.Size = new System.Drawing.Size(92, 17);
            this.checkBox_alwaysOnTop.TabIndex = 10;
            this.checkBox_alwaysOnTop.Text = "Always on top";
            this.checkBox_alwaysOnTop.UseVisualStyleBackColor = true;
            this.checkBox_alwaysOnTop.CheckedChanged += new System.EventHandler(this.CheckBox_alwaysOnTop_CheckedChanged);
            // 
            // checkBox_showPreview
            // 
            this.checkBox_showPreview.AutoSize = true;
            this.checkBox_showPreview.Location = new System.Drawing.Point(5, 136);
            this.checkBox_showPreview.Name = "checkBox_showPreview";
            this.checkBox_showPreview.Size = new System.Drawing.Size(93, 17);
            this.checkBox_showPreview.TabIndex = 9;
            this.checkBox_showPreview.Text = "Show preview";
            this.checkBox_showPreview.UseVisualStyleBackColor = true;
            this.checkBox_showPreview.CheckedChanged += new System.EventHandler(this.CheckBox_showPreview_CheckedChanged);
            // 
            // checkBox_reformatJson
            // 
            this.checkBox_reformatJson.AutoSize = true;
            this.checkBox_reformatJson.Location = new System.Drawing.Point(5, 113);
            this.checkBox_reformatJson.Name = "checkBox_reformatJson";
            this.checkBox_reformatJson.Size = new System.Drawing.Size(100, 17);
            this.checkBox_reformatJson.TabIndex = 7;
            this.checkBox_reformatJson.Text = "Reformat JSON";
            this.checkBox_reformatJson.UseVisualStyleBackColor = true;
            this.checkBox_reformatJson.CheckedChanged += new System.EventHandler(this.CheckBox_reformatJson_CheckedChanged);
            // 
            // button_saveDb
            // 
            this.button_saveDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_saveDb.Location = new System.Drawing.Point(0, 61);
            this.button_saveDb.Name = "button_saveDb";
            this.button_saveDb.Size = new System.Drawing.Size(131, 23);
            this.button_saveDb.TabIndex = 4;
            this.button_saveDb.Text = "Save database";
            this.button_saveDb.UseVisualStyleBackColor = true;
            this.button_saveDb.Click += new System.EventHandler(this.Button_saveDb_Click);
            // 
            // button_loadDb
            // 
            this.button_loadDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_loadDb.Location = new System.Drawing.Point(0, 32);
            this.button_loadDb.Name = "button_loadDb";
            this.button_loadDb.Size = new System.Drawing.Size(131, 23);
            this.button_loadDb.TabIndex = 3;
            this.button_loadDb.Text = "Load database";
            this.button_loadDb.UseVisualStyleBackColor = true;
            this.button_loadDb.Click += new System.EventHandler(this.Button_loadDb_Click);
            // 
            // button_collectDatabase
            // 
            this.button_collectDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_collectDatabase.Location = new System.Drawing.Point(0, 3);
            this.button_collectDatabase.Name = "button_collectDatabase";
            this.button_collectDatabase.Size = new System.Drawing.Size(131, 23);
            this.button_collectDatabase.TabIndex = 2;
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
            this.textBox_logText.Size = new System.Drawing.Size(639, 490);
            this.textBox_logText.TabIndex = 0;
            // 
            // tabPage_SamplesTree
            // 
            this.tabPage_SamplesTree.Controls.Add(this.splitContainer1);
            this.tabPage_SamplesTree.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SamplesTree.Name = "tabPage_SamplesTree";
            this.tabPage_SamplesTree.Size = new System.Drawing.Size(776, 490);
            this.tabPage_SamplesTree.TabIndex = 1;
            this.tabPage_SamplesTree.Text = "Schema tree";
            this.tabPage_SamplesTree.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_examples);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2MinSize = 350;
            this.splitContainer1.Size = new System.Drawing.Size(776, 490);
            this.splitContainer1.SplitterDistance = 195;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView_examples
            // 
            this.treeView_examples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_examples.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView_examples.HideSelection = false;
            this.treeView_examples.Location = new System.Drawing.Point(0, 0);
            this.treeView_examples.Name = "treeView_examples";
            this.treeView_examples.Size = new System.Drawing.Size(195, 490);
            this.treeView_examples.TabIndex = 0;
            this.treeView_examples.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_No_Expand_Collapse);
            this.treeView_examples.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_No_Expand_Collapse);
            this.treeView_examples.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_examples_AfterSelect);
            this.treeView_examples.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
            this.treeView_examples.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_examples_NodeMouseDoubleClick);
            this.treeView_examples.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeView_examples_KeyDown);
            this.treeView_examples.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            this.splitContainer3.Panel1.Controls.Add(this.textBox_ExSearchHistory);
            this.splitContainer3.Panel1.Controls.Add(this.comboBox_ExVersions);
            this.splitContainer3.Panel1.Controls.Add(this.button_ExAdjustRows);
            this.splitContainer3.Panel1.Controls.Add(this.comboBox_ExCondition);
            this.splitContainer3.Panel1.Controls.Add(this.textBox_ExSearchString);
            this.splitContainer3.Panel1.Controls.Add(this.checkBox_ExCaseSensitive);
            this.splitContainer3.Panel1.Controls.Add(this.button_ExClearSearch);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.label_descSave);
            this.splitContainer3.Panel2.Controls.Add(this.textBox_description);
            this.splitContainer3.Size = new System.Drawing.Size(577, 490);
            this.splitContainer3.SplitterDistance = 412;
            this.splitContainer3.TabIndex = 9;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer4.Location = new System.Drawing.Point(0, 32);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.dataGridView_examples);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.listBox_fileList);
            this.splitContainer4.Size = new System.Drawing.Size(577, 350);
            this.splitContainer4.SplitterDistance = 393;
            this.splitContainer4.TabIndex = 8;
            // 
            // dataGridView_examples
            // 
            this.dataGridView_examples.AllowUserToAddRows = false;
            this.dataGridView_examples.AllowUserToDeleteRows = false;
            this.dataGridView_examples.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView_examples.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_examples.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView_examples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_examples.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_examples.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_examples.Name = "dataGridView_examples";
            this.dataGridView_examples.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle6.NullValue = "Adjust";
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView_examples.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_examples.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_examples.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_examples.Size = new System.Drawing.Size(393, 350);
            this.dataGridView_examples.TabIndex = 5;
            this.dataGridView_examples.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_examples_CellDoubleClick);
            this.dataGridView_examples.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_CellMouseDown);
            this.dataGridView_examples.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
            this.dataGridView_examples.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_RowHeaderMouseDoubleClick);
            // 
            // listBox_fileList
            // 
            this.listBox_fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_fileList.FormattingEnabled = true;
            this.listBox_fileList.HorizontalScrollbar = true;
            this.listBox_fileList.Location = new System.Drawing.Point(0, 0);
            this.listBox_fileList.Name = "listBox_fileList";
            this.listBox_fileList.Size = new System.Drawing.Size(180, 350);
            this.listBox_fileList.TabIndex = 0;
            this.listBox_fileList.SelectedValueChanged += new System.EventHandler(this.ListBox_fileList_SelectedValueChanged);
            this.listBox_fileList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListBox_fileList_MouseDoubleClick);
            // 
            // textBox_ExSearchHistory
            // 
            this.textBox_ExSearchHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ExSearchHistory.Location = new System.Drawing.Point(3, 388);
            this.textBox_ExSearchHistory.Name = "textBox_ExSearchHistory";
            this.textBox_ExSearchHistory.ReadOnly = true;
            this.textBox_ExSearchHistory.Size = new System.Drawing.Size(490, 20);
            this.textBox_ExSearchHistory.TabIndex = 6;
            // 
            // comboBox_ExVersions
            // 
            this.comboBox_ExVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ExVersions.FormattingEnabled = true;
            this.comboBox_ExVersions.Items.AddRange(new object[] {
            "Any"});
            this.comboBox_ExVersions.Location = new System.Drawing.Point(3, 5);
            this.comboBox_ExVersions.Name = "comboBox_ExVersions";
            this.comboBox_ExVersions.Size = new System.Drawing.Size(61, 21);
            this.comboBox_ExVersions.TabIndex = 0;
            this.comboBox_ExVersions.SelectedIndexChanged += new System.EventHandler(this.ComboBox_ExVersions_SelectedIndexChanged);
            // 
            // button_ExAdjustRows
            // 
            this.button_ExAdjustRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ExAdjustRows.Location = new System.Drawing.Point(499, 3);
            this.button_ExAdjustRows.Name = "button_ExAdjustRows";
            this.button_ExAdjustRows.Size = new System.Drawing.Size(75, 23);
            this.button_ExAdjustRows.TabIndex = 4;
            this.button_ExAdjustRows.Text = "Adjust rows";
            this.button_ExAdjustRows.UseVisualStyleBackColor = true;
            this.button_ExAdjustRows.Click += new System.EventHandler(this.Button_ExAdjustRows_Click);
            // 
            // comboBox_ExCondition
            // 
            this.comboBox_ExCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ExCondition.FormattingEnabled = true;
            this.comboBox_ExCondition.Location = new System.Drawing.Point(70, 5);
            this.comboBox_ExCondition.Name = "comboBox_ExCondition";
            this.comboBox_ExCondition.Size = new System.Drawing.Size(70, 21);
            this.comboBox_ExCondition.TabIndex = 1;
            // 
            // textBox_ExSearchString
            // 
            this.textBox_ExSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ExSearchString.Location = new System.Drawing.Point(146, 5);
            this.textBox_ExSearchString.Name = "textBox_ExSearchString";
            this.textBox_ExSearchString.Size = new System.Drawing.Size(247, 20);
            this.textBox_ExSearchString.TabIndex = 2;
            this.textBox_ExSearchString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_ExSearchString_KeyDown);
            // 
            // checkBox_ExCaseSensitive
            // 
            this.checkBox_ExCaseSensitive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_ExCaseSensitive.AutoSize = true;
            this.checkBox_ExCaseSensitive.Location = new System.Drawing.Point(399, 7);
            this.checkBox_ExCaseSensitive.Name = "checkBox_ExCaseSensitive";
            this.checkBox_ExCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.checkBox_ExCaseSensitive.TabIndex = 3;
            this.checkBox_ExCaseSensitive.Text = "Case sensitive";
            this.checkBox_ExCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // button_ExClearSearch
            // 
            this.button_ExClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ExClearSearch.Location = new System.Drawing.Point(499, 386);
            this.button_ExClearSearch.Name = "button_ExClearSearch";
            this.button_ExClearSearch.Size = new System.Drawing.Size(75, 23);
            this.button_ExClearSearch.TabIndex = 7;
            this.button_ExClearSearch.Text = "Clear";
            this.button_ExClearSearch.UseVisualStyleBackColor = true;
            this.button_ExClearSearch.Click += new System.EventHandler(this.Button_ExClearSearch_Click);
            // 
            // label_descSave
            // 
            this.label_descSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_descSave.AutoSize = true;
            this.label_descSave.Location = new System.Drawing.Point(415, 61);
            this.label_descSave.Name = "label_descSave";
            this.label_descSave.Size = new System.Drawing.Size(162, 13);
            this.label_descSave.TabIndex = 9;
            this.label_descSave.Text = "Ctrl-Enter to save, ESC to cancel";
            // 
            // textBox_description
            // 
            this.textBox_description.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_description.Location = new System.Drawing.Point(0, 0);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.ReadOnly = true;
            this.textBox_description.Size = new System.Drawing.Size(577, 74);
            this.textBox_description.TabIndex = 8;
            this.textBox_description.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_description_KeyDown);
            this.textBox_description.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_description_MouseDoubleClick);
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 517);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 539);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "Form1";
            this.Text = "JsonDictionary";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_DataCollection.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPage_SamplesTree.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_examples)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_DataCollection;
        private System.Windows.Forms.TabPage tabPage_SamplesTree;
        private System.Windows.Forms.TreeView treeView_examples;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox_logText;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button_collectDatabase;
        private System.Windows.Forms.Button button_saveDb;
        private System.Windows.Forms.Button button_loadDb;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dataGridView_examples;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox textBox_ExSearchString;
        private System.Windows.Forms.ComboBox comboBox_ExCondition;
        private System.Windows.Forms.CheckBox checkBox_ExCaseSensitive;
        private System.Windows.Forms.TextBox textBox_ExSearchHistory;
        private System.Windows.Forms.ComboBox comboBox_ExVersions;
        private System.Windows.Forms.Button button_ExAdjustRows;
        private System.Windows.Forms.CheckBox checkBox_alwaysOnTop;
        private System.Windows.Forms.CheckBox checkBox_showPreview;
        private System.Windows.Forms.CheckBox checkBox_reformatJson;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button_ExClearSearch;
        private System.Windows.Forms.CheckBox checkBox_loadDbOnStart;
        private System.Windows.Forms.CheckBox checkBox_vsCode;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox textBox_description;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.ListBox listBox_fileList;
        private System.Windows.Forms.Label label_descSave;
    }
}

