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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_DataCollection = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.checkBox_loadDbOnStart = new System.Windows.Forms.CheckBox();
            this.checkBox_alwaysOnTop = new System.Windows.Forms.CheckBox();
            this.checkBox_showPreview = new System.Windows.Forms.CheckBox();
            this.checkBox_ignoreHttpsError = new System.Windows.Forms.CheckBox();
            this.checkBox_reformatJson = new System.Windows.Forms.CheckBox();
            this.checkBox_collectAllFileNames = new System.Windows.Forms.CheckBox();
            this.checkedListBox_params = new System.Windows.Forms.CheckedListBox();
            this.button_saveDb = new System.Windows.Forms.Button();
            this.button_loadDb = new System.Windows.Forms.Button();
            this.button_validateFiles = new System.Windows.Forms.Button();
            this.button_collectDatabase = new System.Windows.Forms.Button();
            this.textBox_logText = new System.Windows.Forms.TextBox();
            this.tabPage_SamplesTree = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_examples = new System.Windows.Forms.TreeView();
            this.button_ExClearSearch = new System.Windows.Forms.Button();
            this.button_ExAdjustRows = new System.Windows.Forms.Button();
            this.comboBox_ExVersions = new System.Windows.Forms.ComboBox();
            this.checkBox_ExCaseSensitive = new System.Windows.Forms.CheckBox();
            this.textBox_ExSearchHistory = new System.Windows.Forms.TextBox();
            this.textBox_ExSearchString = new System.Windows.Forms.TextBox();
            this.comboBox_ExCondition = new System.Windows.Forms.ComboBox();
            this.dataGridView_examples = new System.Windows.Forms.DataGridView();
            this.tabPage_Keywords = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.treeView_keywords = new System.Windows.Forms.TreeView();
            this.button_KwClearSearch = new System.Windows.Forms.Button();
            this.button_KwAdjustRows = new System.Windows.Forms.Button();
            this.comboBox_KwVersions = new System.Windows.Forms.ComboBox();
            this.checkBox_KwCaseSensitive = new System.Windows.Forms.CheckBox();
            this.textBox_KwSearchHistory = new System.Windows.Forms.TextBox();
            this.textBox_KwSearchString = new System.Windows.Forms.TextBox();
            this.comboBox_KwCondition = new System.Windows.Forms.ComboBox();
            this.dataGridView_keywords = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip_ExFindValue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ExFindAllStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_ExFindField = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ExFindFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip_KwFindValue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.KwFindAllStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_KwFindField = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.KwFindFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_examples)).BeginInit();
            this.tabPage_Keywords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_keywords)).BeginInit();
            this.contextMenuStrip_ExFindValue.SuspendLayout();
            this.contextMenuStrip_ExFindField.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip_KwFindValue.SuspendLayout();
            this.contextMenuStrip_KwFindField.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage_DataCollection);
            this.tabControl1.Controls.Add(this.tabPage_SamplesTree);
            this.tabControl1.Controls.Add(this.tabPage_Keywords);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(988, 726);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage_DataCollection
            // 
            this.tabPage_DataCollection.Controls.Add(this.splitContainer2);
            this.tabPage_DataCollection.Location = new System.Drawing.Point(4, 22);
            this.tabPage_DataCollection.Name = "tabPage_DataCollection";
            this.tabPage_DataCollection.Size = new System.Drawing.Size(980, 700);
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
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_loadDbOnStart);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_alwaysOnTop);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_showPreview);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_ignoreHttpsError);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_reformatJson);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_collectAllFileNames);
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
            this.splitContainer2.Size = new System.Drawing.Size(980, 700);
            this.splitContainer2.SplitterDistance = 210;
            this.splitContainer2.TabIndex = 18;
            // 
            // checkBox_loadDbOnStart
            // 
            this.checkBox_loadDbOnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_loadDbOnStart.AutoSize = true;
            this.checkBox_loadDbOnStart.Location = new System.Drawing.Point(5, 565);
            this.checkBox_loadDbOnStart.Name = "checkBox_loadDbOnStart";
            this.checkBox_loadDbOnStart.Size = new System.Drawing.Size(121, 17);
            this.checkBox_loadDbOnStart.TabIndex = 6;
            this.checkBox_loadDbOnStart.Text = "Load DB on start-up";
            this.checkBox_loadDbOnStart.UseVisualStyleBackColor = true;
            this.checkBox_loadDbOnStart.CheckedChanged += new System.EventHandler(this.checkBox_loadDbOnStart_CheckedChanged);
            // 
            // checkBox_alwaysOnTop
            // 
            this.checkBox_alwaysOnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_alwaysOnTop.AutoSize = true;
            this.checkBox_alwaysOnTop.Location = new System.Drawing.Point(5, 680);
            this.checkBox_alwaysOnTop.Name = "checkBox_alwaysOnTop";
            this.checkBox_alwaysOnTop.Size = new System.Drawing.Size(92, 17);
            this.checkBox_alwaysOnTop.TabIndex = 5;
            this.checkBox_alwaysOnTop.Text = "Always on top";
            this.checkBox_alwaysOnTop.UseVisualStyleBackColor = true;
            this.checkBox_alwaysOnTop.CheckedChanged += new System.EventHandler(this.CheckBox_alwaysOnTop_CheckedChanged);
            // 
            // checkBox_showPreview
            // 
            this.checkBox_showPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_showPreview.AutoSize = true;
            this.checkBox_showPreview.Location = new System.Drawing.Point(5, 657);
            this.checkBox_showPreview.Name = "checkBox_showPreview";
            this.checkBox_showPreview.Size = new System.Drawing.Size(122, 17);
            this.checkBox_showPreview.TabIndex = 5;
            this.checkBox_showPreview.Text = "Show preview editor";
            this.checkBox_showPreview.UseVisualStyleBackColor = true;
            this.checkBox_showPreview.CheckedChanged += new System.EventHandler(this.CheckBox_showPreview_CheckedChanged);
            // 
            // checkBox_ignoreHttpsError
            // 
            this.checkBox_ignoreHttpsError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_ignoreHttpsError.AutoSize = true;
            this.checkBox_ignoreHttpsError.Location = new System.Drawing.Point(5, 634);
            this.checkBox_ignoreHttpsError.Name = "checkBox_ignoreHttpsError";
            this.checkBox_ignoreHttpsError.Size = new System.Drawing.Size(119, 17);
            this.checkBox_ignoreHttpsError.TabIndex = 5;
            this.checkBox_ignoreHttpsError.Text = "Ignore HTTPS error";
            this.checkBox_ignoreHttpsError.UseVisualStyleBackColor = true;
            this.checkBox_ignoreHttpsError.CheckedChanged += new System.EventHandler(this.CheckBox_ignoreHttpsError_CheckedChanged);
            // 
            // checkBox_reformatJson
            // 
            this.checkBox_reformatJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_reformatJson.AutoSize = true;
            this.checkBox_reformatJson.Location = new System.Drawing.Point(5, 611);
            this.checkBox_reformatJson.Name = "checkBox_reformatJson";
            this.checkBox_reformatJson.Size = new System.Drawing.Size(100, 17);
            this.checkBox_reformatJson.TabIndex = 5;
            this.checkBox_reformatJson.Text = "Reformat JSON";
            this.checkBox_reformatJson.UseVisualStyleBackColor = true;
            this.checkBox_reformatJson.CheckedChanged += new System.EventHandler(this.CheckBox_reformatJson_CheckedChanged);
            // 
            // checkBox_collectAllFileNames
            // 
            this.checkBox_collectAllFileNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_collectAllFileNames.AutoSize = true;
            this.checkBox_collectAllFileNames.Location = new System.Drawing.Point(5, 588);
            this.checkBox_collectAllFileNames.Name = "checkBox_collectAllFileNames";
            this.checkBox_collectAllFileNames.Size = new System.Drawing.Size(118, 17);
            this.checkBox_collectAllFileNames.TabIndex = 5;
            this.checkBox_collectAllFileNames.Text = "Collect all filenames";
            this.checkBox_collectAllFileNames.UseVisualStyleBackColor = true;
            this.checkBox_collectAllFileNames.CheckedChanged += new System.EventHandler(this.CheckBox_collectAllFileNames_CheckedChanged);
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
            this.checkedListBox_params.Size = new System.Drawing.Size(209, 436);
            this.checkedListBox_params.TabIndex = 0;
            // 
            // button_saveDb
            // 
            this.button_saveDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_saveDb.Location = new System.Drawing.Point(0, 536);
            this.button_saveDb.Name = "button_saveDb";
            this.button_saveDb.Size = new System.Drawing.Size(209, 23);
            this.button_saveDb.TabIndex = 4;
            this.button_saveDb.Text = "Save database";
            this.button_saveDb.UseVisualStyleBackColor = true;
            this.button_saveDb.Click += new System.EventHandler(this.Button_saveDb_Click);
            // 
            // button_loadDb
            // 
            this.button_loadDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_loadDb.Location = new System.Drawing.Point(0, 507);
            this.button_loadDb.Name = "button_loadDb";
            this.button_loadDb.Size = new System.Drawing.Size(209, 23);
            this.button_loadDb.TabIndex = 3;
            this.button_loadDb.Text = "Load database";
            this.button_loadDb.UseVisualStyleBackColor = true;
            this.button_loadDb.Click += new System.EventHandler(this.Button_loadDb_Click);
            // 
            // button_validateFiles
            // 
            this.button_validateFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_validateFiles.Location = new System.Drawing.Point(0, 449);
            this.button_validateFiles.Name = "button_validateFiles";
            this.button_validateFiles.Size = new System.Drawing.Size(209, 23);
            this.button_validateFiles.TabIndex = 1;
            this.button_validateFiles.Text = "Validate files";
            this.button_validateFiles.UseVisualStyleBackColor = true;
            this.button_validateFiles.Click += new System.EventHandler(this.Button_validateFiles_Click);
            // 
            // button_collectDatabase
            // 
            this.button_collectDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_collectDatabase.Location = new System.Drawing.Point(0, 478);
            this.button_collectDatabase.Name = "button_collectDatabase";
            this.button_collectDatabase.Size = new System.Drawing.Size(209, 23);
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
            this.textBox_logText.Size = new System.Drawing.Size(766, 700);
            this.textBox_logText.TabIndex = 0;
            // 
            // tabPage_SamplesTree
            // 
            this.tabPage_SamplesTree.Controls.Add(this.splitContainer1);
            this.tabPage_SamplesTree.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SamplesTree.Name = "tabPage_SamplesTree";
            this.tabPage_SamplesTree.Size = new System.Drawing.Size(980, 700);
            this.tabPage_SamplesTree.TabIndex = 1;
            this.tabPage_SamplesTree.Text = "Samples tree";
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
            this.splitContainer1.Panel2.Controls.Add(this.button_ExClearSearch);
            this.splitContainer1.Panel2.Controls.Add(this.button_ExAdjustRows);
            this.splitContainer1.Panel2.Controls.Add(this.comboBox_ExVersions);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_ExCaseSensitive);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_ExSearchHistory);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_ExSearchString);
            this.splitContainer1.Panel2.Controls.Add(this.comboBox_ExCondition);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView_examples);
            this.splitContainer1.Panel2MinSize = 350;
            this.splitContainer1.Size = new System.Drawing.Size(980, 700);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView_examples
            // 
            this.treeView_examples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_examples.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView_examples.HideSelection = false;
            this.treeView_examples.Location = new System.Drawing.Point(0, 0);
            this.treeView_examples.Name = "treeView_examples";
            this.treeView_examples.Size = new System.Drawing.Size(215, 700);
            this.treeView_examples.TabIndex = 0;
            this.treeView_examples.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_No_Expand_Collapse);
            this.treeView_examples.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_No_Expand_Collapse);
            this.treeView_examples.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
            this.treeView_examples.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_examples_NodeMouseDoubleClick);
            this.treeView_examples.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeView_examples_KeyDown);
            this.treeView_examples.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
            // 
            // button_ExClearSearch
            // 
            this.button_ExClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ExClearSearch.Location = new System.Drawing.Point(686, 674);
            this.button_ExClearSearch.Name = "button_ExClearSearch";
            this.button_ExClearSearch.Size = new System.Drawing.Size(75, 23);
            this.button_ExClearSearch.TabIndex = 4;
            this.button_ExClearSearch.Text = "Clear";
            this.button_ExClearSearch.UseVisualStyleBackColor = true;
            this.button_ExClearSearch.Click += new System.EventHandler(this.Button_ExClearSearch_Click);
            // 
            // button_ExAdjustRows
            // 
            this.button_ExAdjustRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ExAdjustRows.Location = new System.Drawing.Point(686, 1);
            this.button_ExAdjustRows.Name = "button_ExAdjustRows";
            this.button_ExAdjustRows.Size = new System.Drawing.Size(75, 23);
            this.button_ExAdjustRows.TabIndex = 4;
            this.button_ExAdjustRows.Text = "Adjust rows";
            this.button_ExAdjustRows.UseVisualStyleBackColor = true;
            this.button_ExAdjustRows.Click += new System.EventHandler(this.Button_ExAdjustRows_Click);
            // 
            // comboBox_ExVersions
            // 
            this.comboBox_ExVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ExVersions.FormattingEnabled = true;
            this.comboBox_ExVersions.Items.AddRange(new object[] {
            "Any"});
            this.comboBox_ExVersions.Location = new System.Drawing.Point(0, 3);
            this.comboBox_ExVersions.Name = "comboBox_ExVersions";
            this.comboBox_ExVersions.Size = new System.Drawing.Size(61, 21);
            this.comboBox_ExVersions.TabIndex = 0;
            this.comboBox_ExVersions.SelectedIndexChanged += new System.EventHandler(this.ComboBox_ExVersions_SelectedIndexChanged);
            // 
            // checkBox_ExCaseSensitive
            // 
            this.checkBox_ExCaseSensitive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_ExCaseSensitive.AutoSize = true;
            this.checkBox_ExCaseSensitive.Location = new System.Drawing.Point(586, 5);
            this.checkBox_ExCaseSensitive.Name = "checkBox_ExCaseSensitive";
            this.checkBox_ExCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.checkBox_ExCaseSensitive.TabIndex = 3;
            this.checkBox_ExCaseSensitive.Text = "Case sensitive";
            this.checkBox_ExCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // textBox_ExSearchHistory
            // 
            this.textBox_ExSearchHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ExSearchHistory.Location = new System.Drawing.Point(0, 677);
            this.textBox_ExSearchHistory.Name = "textBox_ExSearchHistory";
            this.textBox_ExSearchHistory.ReadOnly = true;
            this.textBox_ExSearchHistory.Size = new System.Drawing.Size(680, 20);
            this.textBox_ExSearchHistory.TabIndex = 6;
            // 
            // textBox_ExSearchString
            // 
            this.textBox_ExSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ExSearchString.Location = new System.Drawing.Point(143, 3);
            this.textBox_ExSearchString.Name = "textBox_ExSearchString";
            this.textBox_ExSearchString.Size = new System.Drawing.Size(437, 20);
            this.textBox_ExSearchString.TabIndex = 2;
            this.textBox_ExSearchString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_ExSearchString_KeyDown);
            // 
            // comboBox_ExCondition
            // 
            this.comboBox_ExCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ExCondition.FormattingEnabled = true;
            this.comboBox_ExCondition.Location = new System.Drawing.Point(67, 3);
            this.comboBox_ExCondition.Name = "comboBox_ExCondition";
            this.comboBox_ExCondition.Size = new System.Drawing.Size(70, 21);
            this.comboBox_ExCondition.TabIndex = 1;
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
            this.dataGridView_examples.Size = new System.Drawing.Size(761, 646);
            this.dataGridView_examples.TabIndex = 5;
            this.dataGridView_examples.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentDoubleClick);
            this.dataGridView_examples.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_CellMouseDown);
            this.dataGridView_examples.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
            this.dataGridView_examples.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_RowHeaderMouseDoubleClick);
            // 
            // tabPage_Keywords
            // 
            this.tabPage_Keywords.Controls.Add(this.splitContainer3);
            this.tabPage_Keywords.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Keywords.Name = "tabPage_Keywords";
            this.tabPage_Keywords.Size = new System.Drawing.Size(980, 700);
            this.tabPage_Keywords.TabIndex = 2;
            this.tabPage_Keywords.Text = "Keywords";
            this.tabPage_Keywords.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.treeView_keywords);
            this.splitContainer3.Panel1MinSize = 100;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.button_KwClearSearch);
            this.splitContainer3.Panel2.Controls.Add(this.button_KwAdjustRows);
            this.splitContainer3.Panel2.Controls.Add(this.comboBox_KwVersions);
            this.splitContainer3.Panel2.Controls.Add(this.checkBox_KwCaseSensitive);
            this.splitContainer3.Panel2.Controls.Add(this.textBox_KwSearchHistory);
            this.splitContainer3.Panel2.Controls.Add(this.textBox_KwSearchString);
            this.splitContainer3.Panel2.Controls.Add(this.comboBox_KwCondition);
            this.splitContainer3.Panel2.Controls.Add(this.dataGridView_keywords);
            this.splitContainer3.Panel2MinSize = 350;
            this.splitContainer3.Size = new System.Drawing.Size(980, 700);
            this.splitContainer3.SplitterDistance = 215;
            this.splitContainer3.TabIndex = 2;
            // 
            // treeView_keywords
            // 
            this.treeView_keywords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_keywords.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView_keywords.HideSelection = false;
            this.treeView_keywords.Location = new System.Drawing.Point(0, 0);
            this.treeView_keywords.Name = "treeView_keywords";
            this.treeView_keywords.Size = new System.Drawing.Size(215, 700);
            this.treeView_keywords.TabIndex = 0;
            this.treeView_keywords.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_No_Expand_Collapse);
            this.treeView_keywords.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_No_Expand_Collapse);
            this.treeView_keywords.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
            this.treeView_keywords.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_keywords_NodeMouseDoubleClick);
            this.treeView_keywords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeView_keywords_KeyDown);
            this.treeView_keywords.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
            // 
            // button_KwClearSearch
            // 
            this.button_KwClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_KwClearSearch.Location = new System.Drawing.Point(686, 674);
            this.button_KwClearSearch.Name = "button_KwClearSearch";
            this.button_KwClearSearch.Size = new System.Drawing.Size(75, 23);
            this.button_KwClearSearch.TabIndex = 7;
            this.button_KwClearSearch.Text = "Clear";
            this.button_KwClearSearch.UseVisualStyleBackColor = true;
            this.button_KwClearSearch.Click += new System.EventHandler(this.Button_KwClearSearch_Click);
            // 
            // button_KwAdjustRows
            // 
            this.button_KwAdjustRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_KwAdjustRows.Location = new System.Drawing.Point(686, 1);
            this.button_KwAdjustRows.Name = "button_KwAdjustRows";
            this.button_KwAdjustRows.Size = new System.Drawing.Size(75, 23);
            this.button_KwAdjustRows.TabIndex = 4;
            this.button_KwAdjustRows.Text = "Adjust rows";
            this.button_KwAdjustRows.UseVisualStyleBackColor = true;
            this.button_KwAdjustRows.Click += new System.EventHandler(this.Button_KwAdjustRows_Click);
            // 
            // comboBox_KwVersions
            // 
            this.comboBox_KwVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_KwVersions.FormattingEnabled = true;
            this.comboBox_KwVersions.Items.AddRange(new object[] {
            "Any"});
            this.comboBox_KwVersions.Location = new System.Drawing.Point(0, 3);
            this.comboBox_KwVersions.Name = "comboBox_KwVersions";
            this.comboBox_KwVersions.Size = new System.Drawing.Size(61, 21);
            this.comboBox_KwVersions.TabIndex = 0;
            this.comboBox_KwVersions.SelectedIndexChanged += new System.EventHandler(this.ComboBox_KwVersions_SelectedIndexChanged);
            // 
            // checkBox_KwCaseSensitive
            // 
            this.checkBox_KwCaseSensitive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_KwCaseSensitive.AutoSize = true;
            this.checkBox_KwCaseSensitive.Location = new System.Drawing.Point(586, 5);
            this.checkBox_KwCaseSensitive.Name = "checkBox_KwCaseSensitive";
            this.checkBox_KwCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.checkBox_KwCaseSensitive.TabIndex = 3;
            this.checkBox_KwCaseSensitive.Text = "Case sensitive";
            this.checkBox_KwCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // textBox_KwSearchHistory
            // 
            this.textBox_KwSearchHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_KwSearchHistory.Location = new System.Drawing.Point(0, 677);
            this.textBox_KwSearchHistory.Name = "textBox_KwSearchHistory";
            this.textBox_KwSearchHistory.ReadOnly = true;
            this.textBox_KwSearchHistory.Size = new System.Drawing.Size(680, 20);
            this.textBox_KwSearchHistory.TabIndex = 6;
            // 
            // textBox_KwSearchString
            // 
            this.textBox_KwSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_KwSearchString.Location = new System.Drawing.Point(143, 3);
            this.textBox_KwSearchString.Name = "textBox_KwSearchString";
            this.textBox_KwSearchString.Size = new System.Drawing.Size(437, 20);
            this.textBox_KwSearchString.TabIndex = 2;
            this.textBox_KwSearchString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KwSearchString_KeyDown);
            // 
            // comboBox_KwCondition
            // 
            this.comboBox_KwCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_KwCondition.FormattingEnabled = true;
            this.comboBox_KwCondition.Location = new System.Drawing.Point(67, 3);
            this.comboBox_KwCondition.Name = "comboBox_KwCondition";
            this.comboBox_KwCondition.Size = new System.Drawing.Size(70, 21);
            this.comboBox_KwCondition.TabIndex = 1;
            // 
            // dataGridView_keywords
            // 
            this.dataGridView_keywords.AllowUserToAddRows = false;
            this.dataGridView_keywords.AllowUserToDeleteRows = false;
            this.dataGridView_keywords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_keywords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView_keywords.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_keywords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_keywords.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_keywords.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_keywords.Location = new System.Drawing.Point(0, 26);
            this.dataGridView_keywords.Name = "dataGridView_keywords";
            this.dataGridView_keywords.RowHeadersWidth = 20;
            dataGridViewCellStyle4.NullValue = "Adjust";
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_keywords.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_keywords.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_keywords.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_keywords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_keywords.Size = new System.Drawing.Size(761, 646);
            this.dataGridView_keywords.TabIndex = 5;
            this.dataGridView_keywords.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentDoubleClick);
            this.dataGridView_keywords.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_CellMouseDown);
            this.dataGridView_keywords.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
            this.dataGridView_keywords.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_RowHeaderMouseDoubleClick);
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
            // contextMenuStrip_ExFindValue
            // 
            this.contextMenuStrip_ExFindValue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExFindAllStripMenuItem});
            this.contextMenuStrip_ExFindValue.Name = "contextMenuStrip_findInParent";
            this.contextMenuStrip_ExFindValue.Size = new System.Drawing.Size(179, 26);
            this.contextMenuStrip_ExFindValue.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_ExFindValue_Opening);
            // 
            // ExFindAllStripMenuItem
            // 
            this.ExFindAllStripMenuItem.Name = "ExFindAllStripMenuItem";
            this.ExFindAllStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.ExFindAllStripMenuItem.Text = "Find value in parent";
            this.ExFindAllStripMenuItem.Click += new System.EventHandler(this.ExFindValueToolStripMenuItem_Click);
            // 
            // contextMenuStrip_ExFindField
            // 
            this.contextMenuStrip_ExFindField.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExFindFieldToolStripMenuItem});
            this.contextMenuStrip_ExFindField.Name = "contextMenuStrip_ExFindField";
            this.contextMenuStrip_ExFindField.Size = new System.Drawing.Size(174, 26);
            this.contextMenuStrip_ExFindField.Text = "Find field in parent";
            this.contextMenuStrip_ExFindField.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_ExFindField_Opening);
            // 
            // ExFindFieldToolStripMenuItem
            // 
            this.ExFindFieldToolStripMenuItem.Name = "ExFindFieldToolStripMenuItem";
            this.ExFindFieldToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.ExFindFieldToolStripMenuItem.Text = "Find field in parent";
            this.ExFindFieldToolStripMenuItem.Click += new System.EventHandler(this.ExFindFieldToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 727);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(988, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // contextMenuStrip_KwFindValue
            // 
            this.contextMenuStrip_KwFindValue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KwFindAllStripMenuItem});
            this.contextMenuStrip_KwFindValue.Name = "contextMenuStrip_findInParent";
            this.contextMenuStrip_KwFindValue.Size = new System.Drawing.Size(179, 26);
            this.contextMenuStrip_KwFindValue.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_KwFindValue_Opening);
            // 
            // KwFindAllStripMenuItem
            // 
            this.KwFindAllStripMenuItem.Name = "KwFindAllStripMenuItem";
            this.KwFindAllStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.KwFindAllStripMenuItem.Text = "Find value in parent";
            this.KwFindAllStripMenuItem.Click += new System.EventHandler(this.KwFindValueToolStripMenuItem_Click);
            // 
            // contextMenuStrip_KwFindField
            // 
            this.contextMenuStrip_KwFindField.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KwFindFieldToolStripMenuItem});
            this.contextMenuStrip_KwFindField.Name = "contextMenuStrip_ExFindField";
            this.contextMenuStrip_KwFindField.Size = new System.Drawing.Size(174, 26);
            this.contextMenuStrip_KwFindField.Text = "Find field in parent";
            this.contextMenuStrip_KwFindField.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_KwFindField_Opening);
            // 
            // KwFindFieldToolStripMenuItem
            // 
            this.KwFindFieldToolStripMenuItem.Name = "KwFindFieldToolStripMenuItem";
            this.KwFindFieldToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.KwFindFieldToolStripMenuItem.Text = "Find field in parent";
            this.KwFindFieldToolStripMenuItem.Click += new System.EventHandler(this.KwFindFieldToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 749);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "Form1";
            this.Text = "JsonDictionary";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
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
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_examples)).EndInit();
            this.tabPage_Keywords.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_keywords)).EndInit();
            this.contextMenuStrip_ExFindValue.ResumeLayout(false);
            this.contextMenuStrip_ExFindField.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip_KwFindValue.ResumeLayout(false);
            this.contextMenuStrip_KwFindField.ResumeLayout(false);
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
        private System.Windows.Forms.CheckedListBox checkedListBox_params;
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ExFindValue;
        private System.Windows.Forms.ToolStripMenuItem ExFindAllStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ExFindField;
        private System.Windows.Forms.ToolStripMenuItem ExFindFieldToolStripMenuItem;
        private System.Windows.Forms.Button button_validateFiles;
        private System.Windows.Forms.TabPage tabPage_Keywords;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView treeView_keywords;
        private System.Windows.Forms.Button button_KwAdjustRows;
        private System.Windows.Forms.ComboBox comboBox_KwVersions;
        private System.Windows.Forms.CheckBox checkBox_KwCaseSensitive;
        private System.Windows.Forms.TextBox textBox_KwSearchHistory;
        private System.Windows.Forms.TextBox textBox_KwSearchString;
        private System.Windows.Forms.ComboBox comboBox_KwCondition;
        private System.Windows.Forms.DataGridView dataGridView_keywords;
        private System.Windows.Forms.CheckBox checkBox_alwaysOnTop;
        private System.Windows.Forms.CheckBox checkBox_showPreview;
        private System.Windows.Forms.CheckBox checkBox_ignoreHttpsError;
        private System.Windows.Forms.CheckBox checkBox_reformatJson;
        private System.Windows.Forms.CheckBox checkBox_collectAllFileNames;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button_ExClearSearch;
        private System.Windows.Forms.Button button_KwClearSearch;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_KwFindValue;
        private System.Windows.Forms.ToolStripMenuItem KwFindAllStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_KwFindField;
        private System.Windows.Forms.ToolStripMenuItem KwFindFieldToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox_loadDbOnStart;
    }
}

