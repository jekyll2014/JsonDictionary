using JsonDictionary.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NJsonSchema;
using NJsonSchema.Validation;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonDictionary
{
    public partial class Form1 : Form
    {
        // pre-defined constants
        private readonly string[] _exampleGridColumnsNames = { "Version", "Example", "File Name" };
        private const string DefaultVersionCaption = "Any";
        private const string VersionTagName = "contentVersion";
        private readonly ValidationErrorKind[] _suppressErrors = { ValidationErrorKind.ArrayItemNotValid, ValidationErrorKind.PropertyRequired, ValidationErrorKind.NoAdditionalPropertiesAllowed };
        private const string SchemaTag = "\"$schema\": \"";
        private const string RootNodeName = "root";
        private const int MinProgressDisplay = 5;
        private const float CellHeightAdjust = 0.7f;
        private const string LogFileName = "hiddenerrors.log";
        private const string PreViewCaption = "Preview";
        private const string FormCaption = "KineticJsonDictionary";
        private const string BackupSchemaExtension = ".original";
        // experimental options
        private static bool _reformatJson;
        private bool _collectAllFileNames;
        private bool _ignoreHttpsError;
        private bool _showPreview;
        private bool _alwaysOnTop;

        // global variables
        // private readonly string _currentDirectory;
        private string _version = "";
        private string _fileName = "";
        private JsoncContentType _fileType = JsoncContentType.DataViews;
        private TreeNode _rootNodeExamples;
        private TreeNode _rootNodeKeywords;
        private DataTable _examplesTable;
        private DataTable _keywordsTable;
        private List<JsoncDictionary> _metaDictionary = new List<JsoncDictionary>();
        private StringBuilder textLog = new StringBuilder();
        private volatile bool _isDoubleClick;

        // last used values for UI processing optimization
        private TreeNode _lastExSelectedNode;
        private string _lastExSelectedVersion;
        private string _lastExFilterValue = "";
        private SearchCondition _lastExSelectedCondition;
        private StringComparison _lastExCaseSensitive;

        private TreeNode _lastKwSelectedNode;
        private string _lastKwSelectedVersion;
        private string _lastKwFilterValue = "";
        private SearchCondition _lastKwSelectedCondition;
        private StringComparison _lastKwCaseSensitive;

        private JsonViewer _sideViewer;
        private enum SearchCondition
        {
            Contains,
            StartsWith,
            EndsWith
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #region GUI

        public Form1()
        {
            InitializeComponent();

            Text = FormCaption;

            _collectAllFileNames = Settings.Default.CollectAllFileNames;
            _reformatJson = Settings.Default.ReformatJson;
            folderBrowserDialog1.SelectedPath = Settings.Default.LastRootFolder;
            _ignoreHttpsError = Settings.Default.IgnoreHttpsError;
            _showPreview = Settings.Default.ShowPreview;
            _alwaysOnTop = Settings.Default.AlwaysOnTop;

            checkBox_collectAllFileNames.Checked = _collectAllFileNames;
            checkBox_reformatJson.Checked = _reformatJson;
            checkBox_ignoreHttpsError.Checked = _ignoreHttpsError;
            checkBox_showPreview.Checked = _showPreview;
            checkBox_alwaysOnTop.Checked = _alwaysOnTop;

            TopMost = _alwaysOnTop;

            comboBox_condition.Items.AddRange(typeof(SearchCondition).GetEnumNames() as string[]);
            comboBox_condition.SelectedIndex = 0;

            comboBox_p_condition.Items.AddRange(typeof(SearchCondition).GetEnumNames() as string[]);
            comboBox_p_condition.SelectedIndex = 0;

            foreach (var fileName in JsoncDictionary.FileNames)
            {
                checkedListBox_params.Items.Add(fileName.Key);
                checkedListBox_params.SetItemChecked(checkedListBox_params.Items.Count - 1, true);
            }

            _examplesTable = new DataTable("Examples");
            for (var i = 0; i < _exampleGridColumnsNames.Length; i++)
            {
                _examplesTable.Columns.Add(_exampleGridColumnsNames[i]);
                _examplesTable.Columns[i].ReadOnly = true;

                var column = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = _exampleGridColumnsNames[i],
                    Name = _exampleGridColumnsNames[i]
                };
                dataGridView_examples.Columns.Add(column);
            }
            dataGridView_examples.DataError += delegate { };

            _keywordsTable = new DataTable("Keywords");
            for (var i = 0; i < _exampleGridColumnsNames.Length; i++)
            {
                _keywordsTable.Columns.Add(_exampleGridColumnsNames[i]);
                _keywordsTable.Columns[i].ReadOnly = true;

                var column = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = _exampleGridColumnsNames[i],
                    Name = _exampleGridColumnsNames[i]
                };
                dataGridView_keywords.Columns.Add(column);
            }
            dataGridView_keywords.DataError += delegate { };

            comboBox_versions.Items.Clear();
            comboBox_versions.Items.Add(DefaultVersionCaption);
            comboBox_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            comboBox_versions.SelectedIndex = 0;
            comboBox_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;
            _lastExSelectedVersion = DefaultVersionCaption;

            comboBox_p_versions.Items.Clear();
            comboBox_p_versions.Items.Add(DefaultVersionCaption);
            comboBox_p_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            comboBox_p_versions.SelectedIndex = 0;
            comboBox_p_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;
            _lastKwSelectedVersion = DefaultVersionCaption;

            dataGridView_examples.ContextMenuStrip = contextMenuStrip_findValue;
            treeView_examples.ContextMenuStrip = contextMenuStrip_findField;

            if (_showPreview)
            {
                _sideViewer = new JsonViewer(PreViewCaption, " ") { AlwaysOnTop = _alwaysOnTop, ReformatJson = _reformatJson };
                _sideViewer.Show();
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Text = FormCaption;

            if (string.IsNullOrEmpty(Settings.Default.LastDbName)) return;

            ActivateUiControls(false);
            toolStripStatusLabel1.Text = "Loading database...";
            try
            {
                await Task.Run(() =>
                    {
                        _metaDictionary = JsonIo.LoadBinary<List<JsoncDictionary>>(Settings.Default.LastDbName);
                        _rootNodeExamples = JsonIo.LoadBinary<TreeNode>(Settings.Default.LastDbName + ".tree");
                        _rootNodeKeywords = JsonIo.LoadBinary<TreeNode>(Settings.Default.LastDbName + ".keywords");
                    }
                ).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("File read exception [" + Settings.Default.LastDbName + "]: " + ex.Message);
            }

            if (_metaDictionary != null && _rootNodeExamples != null)
            {
                Text = FormCaption + " " + ShortFileName(Settings.Default.LastDbName);
                treeView_examples.Nodes.Clear();
                treeView_examples.Nodes.Add(_rootNodeExamples);
                treeView_examples.Sort();
                treeView_examples.Nodes[0].Expand();

                if (_rootNodeKeywords != null)
                {
                    tabControl1.SelectedTab = tabControl1.TabPages[1];
                    treeView_keywords.Nodes.Clear();
                    treeView_keywords.Nodes.Add(_rootNodeKeywords);
                    treeView_keywords.Sort();
                    treeView_keywords.Nodes[0].Expand();
                }
            }
            toolStripStatusLabel1.Text = "";
            ActivateUiControls(true);
        }

        private void Button_loadDb_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Open JSON data";
            openFileDialog1.DefaultExt = "metalib";
            openFileDialog1.Filter = "Binary files|*.metalib|All files|*.*";
            openFileDialog1.ShowDialog();
        }

        private async void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            ActivateUiControls(false);
            if (await LoadDb(openFileDialog1.FileName))
            {
                Text = FormCaption + " " + ShortFileName(openFileDialog1.FileName);
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                Settings.Default.LastDbName = openFileDialog1.FileName;
                Settings.Default.Save();
            }
            ActivateUiControls(true);
        }

        private async void Button_collectDatabase_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            ActivateUiControls(false);
            treeView_examples.Nodes.Clear();
            treeView_keywords.Nodes.Clear();
            _examplesTable.Clear();
            _keywordsTable.Clear();
            _rootNodeExamples = new TreeNode(RootNodeName);
            _rootNodeKeywords = new TreeNode(RootNodeName);
            _metaDictionary = new List<JsoncDictionary>();
            var startPath = folderBrowserDialog1.SelectedPath;
            var filesList = new List<string>();
            toolStripStatusLabel1.Text = "Searching files...";
            var filesCount = 0;
            await Task.Run(() =>
                {
                    foreach (var fileName in checkedListBox_params.CheckedItems)
                    {
                        filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(),
                            SearchOption.AllDirectories));
                        var filesNumber = filesList.Count;
                        /*var currentFileNumber = 0;
                        var oldProgress = 0;
                        toolStripProgressBar1.Maximum = 100;
                        toolStripProgressBar1.Value = 0;*/
                        Invoke((MethodInvoker)delegate
                       {
                           toolStripStatusLabel1.Text =
                               "Collecting \"" + fileName + "\" database from " + filesNumber + " files";
                       });
                        foreach (var file in filesList)
                        {
                            /*currentFileNumber++;
                            var newProgress = (int)((float)currentFileNumber / filesNumber * 100);
                            if (newProgress >= oldProgress + MinProgressDisplay)
                            {
                                toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                                    ? newProgress
                                    : toolStripProgressBar1.Maximum;
                                if (textLog.Length > 0)
                                {
                                    textBox_logText.Text += textLog.ToString();
                                    textLog.Clear();
                                    textBox_logText.SelectionStart = textBox_logText.Text.Length;
                                    textBox_logText.ScrollToCaret();
                                }
                            }*/

                            _version = "";
                            DeserializeFile(file, file.Substring(file.LastIndexOf('\\') + 1), _rootNodeExamples);
                        }

                        filesCount += filesList.Count;
                        filesList.Clear();
                    }
                }
            ).ConfigureAwait(true);


            toolStripStatusLabel1.Text = "";
            textLog.AppendLine("Files parsed: " + filesCount.ToString());
            textBox_logText.Text += textLog.ToString();
            textLog.Clear();
            textBox_logText.SelectionStart = textBox_logText.Text.Length;
            textBox_logText.ScrollToCaret();

            if (_rootNodeExamples == null) return;

            treeView_examples.Nodes.Add(_rootNodeExamples);
            treeView_examples.Sort();
            treeView_examples.Nodes[0].Expand();
            CollectKeywords(_metaDictionary, _rootNodeKeywords);
            if (_rootNodeKeywords != null)
            {
                treeView_keywords.Nodes.Add(_rootNodeKeywords);
                treeView_keywords.Sort();
                treeView_keywords.Nodes[0].Expand();
            }
            ActivateUiControls(true);
        }

        private async void Button_validateFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            ActivateUiControls(false);

            var startPath = folderBrowserDialog1.SelectedPath;

            var filesList = new List<string>();
            toolStripStatusLabel1.Text = "Searching files...";
            foreach (var fileName in checkedListBox_params.CheckedItems) filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(), SearchOption.AllDirectories));

            var filesNumber = filesList.Count;
            var currentFileNumber = 0;
            var oldProgress = 0;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Validating " + filesNumber + " files";
            foreach (var file in filesList)
            {
                currentFileNumber++;
                var newProgress = (int)((float)currentFileNumber / filesNumber * 100);
                if (newProgress >= oldProgress + MinProgressDisplay)
                {
                    toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                        ? newProgress
                        : toolStripProgressBar1.Maximum;
                    if (textLog.Length > 0)
                    {
                        textBox_logText.Text += textLog.ToString();
                        textLog.Clear();
                        textBox_logText.SelectionStart = textBox_logText.Text.Length;
                        textBox_logText.ScrollToCaret();
                    }
                }

                await Task.Run(() =>
                {
                    ValidateFile(file);
                }).ConfigureAwait(true);
            }
            toolStripStatusLabel1.Text = "";

            textLog.AppendLine("Files validated: " + filesList.Count.ToString());
            textBox_logText.Text += textLog.ToString();
            textLog.Clear();
            textBox_logText.SelectionStart = textBox_logText.Text.Length;
            textBox_logText.ScrollToCaret();
            ActivateUiControls(true);
        }

        private void Button_saveDb_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save data as JSON...";
            saveFileDialog1.DefaultExt = "json";
            saveFileDialog1.Filter = "Binary files|*.metalib|All files|*.*";
            saveFileDialog1.FileName = "metaUiDictionary_" + DateTime.Today.ToShortDateString().Replace("/", "_") + ".metalib";
            saveFileDialog1.ShowDialog();
        }

        private async void SaveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    JsonIo.SaveBinary<List<JsoncDictionary>>(_metaDictionary, saveFileDialog1.FileName);
                    JsonIo.SaveBinary<TreeNode>(_rootNodeExamples, saveFileDialog1.FileName + ".tree");
                    JsonIo.SaveBinary<TreeNode>(_rootNodeKeywords, saveFileDialog1.FileName + ".keywords");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File write exception [" + saveFileDialog1.FileName + "]: " + ex.Message);
                }
            }).ConfigureAwait(true);
        }

        private void Button_readjust_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false, false);
            ReadjustRows(dataGridView_examples);
            ActivateUiControls(true);
        }

        private void Button_p_reAdjust_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false, false);
            ReadjustRows(dataGridView_keywords);
            ActivateUiControls(true);
        }

        private void TextBox_searchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            var condition = (SearchCondition)comboBox_condition.SelectedIndex;
            var searchString = textBox_searchString.Text;
            var caseSensitive = checkBox_seachCaseSensitive.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            if (_lastExFilterValue == searchString && _lastExSelectedCondition == condition && _lastExCaseSensitive == caseSensitive) return;

            ActivateUiControls(false);
            dataGridView_examples.DataSource = null;

            FilterExamples(condition, searchString, comboBox_versions.SelectedItem?.ToString(), caseSensitive);
            dataGridView_examples.DataSource = _examplesTable;
            ActivateUiControls(true);
            e.SuppressKeyPress = true;
        }

        private void TextBox_p_searchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            var condition = (SearchCondition)comboBox_p_condition.SelectedIndex;
            var searchString = textBox_p_searchString.Text;
            var caseSensitive = checkBox_p_seachCaseSensitive.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            if (_lastKwFilterValue == searchString && _lastKwSelectedCondition == condition && _lastKwCaseSensitive == caseSensitive) return;

            ActivateUiControls(false);
            dataGridView_keywords.DataSource = null;
            FilterKeywords(condition, searchString, comboBox_p_versions.SelectedItem?.ToString(), caseSensitive);
            dataGridView_keywords.DataSource = _examplesTable;
            ActivateUiControls(true);
            e.SuppressKeyPress = true;

        }

        private void ComboBox_versions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateUiControls(false);
            FilterExamplesVersion(comboBox_versions.SelectedItem.ToString());
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void ComboBox_p_versions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateUiControls(false);
            FilterKeywordsVersion(comboBox_p_versions.SelectedItem.ToString());
            dataGridView_keywords.Invalidate();
            ActivateUiControls(true);
        }

        #region Prevent_treenode_collapse

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            _isDoubleClick = e.Clicks > 1;
        }

        private void TreeView_No_Expand_Collapse(object sender, TreeViewCancelEventArgs e)
        {
            if (!_isDoubleClick || e.Action != TreeViewAction.Expand) return;

            _isDoubleClick = false;
            e.Cancel = true;
        }

        #endregion

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            if (sender is TreeView tree) tree.SelectedNode = e.Node;
        }

        private void TreeView_examples_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            if (treeView_examples.SelectedNode == null || treeView_examples.SelectedNode.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillExamplesGrid(treeView_examples.SelectedNode);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
            _lastExFilterValue = "";
        }

        private void TreeView_keywords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            if (treeView_keywords.SelectedNode == null || treeView_keywords.SelectedNode.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillKeywordsGrid(treeView_keywords.SelectedNode);
            dataGridView_keywords.Invalidate();
            ActivateUiControls(true);
            _lastKwFilterValue = "";
        }

        private void TreeView_examples_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || (e.Node.Parent != null && e.Node.Parent.Text == RootNodeName)) return;

            ActivateUiControls(false);
            FillExamplesGrid(e.Node);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
            _lastExFilterValue = "";
        }

        private void TreeView_keywords_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || (e.Node.Parent != null && e.Node.Parent.Text == RootNodeName)) return;

            ActivateUiControls(false);
            FillKeywordsGrid(e.Node);
            dataGridView_keywords.Invalidate();
            ActivateUiControls(true);
            _lastKwFilterValue = "";
        }

        private void DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            if (sender is DataGridView dataGrid)
            {
                dataGrid.ClearSelection();
                dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var column = e.ColumnIndex;
            if (sender is DataGridView dataGrid)
            {
                if (!_collectAllFileNames && column == 2)
                {
                    var editor = new JsonViewer(dataGrid.Rows[e.RowIndex].Cells[column].Value.ToString(),
                        "")
                    {
                        ReformatJson = _reformatJson
                    };
                    editor.Show();

                    editor.SelectText(dataGrid.Rows[e.RowIndex].Cells[1].Value.ToString());
                }
                else if (column == 1)
                {
                    var cell = dataGrid.Rows[e.RowIndex].Cells[column];
                    dataGrid.CurrentCell = cell;

                    var editor = new JsonViewer(dataGrid.Rows[e.RowIndex].Cells[column].Value.ToString(),
                        cell.Value.ToString())
                    {
                        ReformatJson = _reformatJson
                    };
                    editor.Show();
                }
            }
        }

        private void DataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is DataGridView dataGrid)
            {
                ReadjustRow(dataGrid, e.RowIndex);
            }
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!_showPreview) return;

            if (_sideViewer == null || _sideViewer.IsDisposed)
            {
                _sideViewer = new JsonViewer(PreViewCaption, " ") { AlwaysOnTop = _alwaysOnTop };
                _sideViewer.Show();
            }

            if (sender is DataGridView dataGrid)
            {
                _sideViewer.EditorText = dataGrid.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
        }

        private void FindValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lastExSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells.Count < 1
                || dataGridView_examples.SelectedCells[0]?.ColumnIndex != 1
                || _lastExSelectedNode.Parent?.Parent?.Parent == null) return;

            var paramName = _lastExSelectedNode.Text;
            var paramValue = JsonIo.CompactJson(dataGridView_examples.SelectedCells[0].Value.ToString());

            ActivateUiControls(false);
            FillExamplesGrid(_lastExSelectedNode.Parent, comboBox_versions.SelectedItem.ToString(), "\"" + paramName + "\":", SearchCondition.Contains, StringComparison.Ordinal);
            FilterExamples(SearchCondition.Contains, paramValue, comboBox_versions.SelectedItem.ToString(), StringComparison.Ordinal, true);

            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView_examples.SelectedNode?.Parent?.Parent?.Parent == null || treeView_examples.SelectedNode.Parent.Parent.Text == RootNodeName) return;

            var paramName = treeView_examples.SelectedNode.Text;

            ActivateUiControls(false);
            FillExamplesGrid(treeView_examples.SelectedNode.Parent);
            FilterExamples(SearchCondition.Contains, "\"" + paramName + "\":", comboBox_versions.SelectedItem.ToString(), StringComparison.Ordinal, false);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void ContextMenuStrip_findValue_Opening(object sender, CancelEventArgs e)
        {
            if (_lastExSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells.Count < 1
                || dataGridView_examples.SelectedCells[0]?.ColumnIndex != 1
                || _lastExSelectedNode.Parent?.Parent?.Parent == null)
                FindAllStripMenuItem.Enabled = false;
            else FindAllStripMenuItem.Enabled = true;
        }

        private void ContextMenuStrip_findField_Opening(object sender, CancelEventArgs e)
        {
            if (treeView_examples.SelectedNode?.Parent?.Parent?.Parent == null || treeView_examples.SelectedNode.Parent.Parent.Text == RootNodeName) FindFieldToolStripMenuItem.Enabled = false;
            else FindFieldToolStripMenuItem.Enabled = true;
        }

        private void CheckBox_collectAllFileNames_CheckedChanged(object sender, EventArgs e)
        {
            _collectAllFileNames = checkBox_collectAllFileNames.Checked;
        }

        private void CheckBox_reformatJson_CheckedChanged(object sender, EventArgs e)
        {
            _reformatJson = checkBox_reformatJson.Checked;
        }

        private void CheckBox_ignoreHttpsError_CheckedChanged(object sender, EventArgs e)
        {
            _ignoreHttpsError = checkBox_ignoreHttpsError.Checked;
        }

        private void CheckBox_showPreview_CheckedChanged(object sender, EventArgs e)
        {
            _showPreview = checkBox_showPreview.Checked;
        }

        private void CheckBox_alwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            _alwaysOnTop = checkBox_alwaysOnTop.Checked;
            TopMost = _alwaysOnTop;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.LastRootFolder = folderBrowserDialog1.SelectedPath;
            Settings.Default.ReformatJson = _reformatJson;
            Settings.Default.CollectAllFileNames = _collectAllFileNames;
            Settings.Default.IgnoreHttpsError = _ignoreHttpsError;
            Settings.Default.ShowPreview = _showPreview;
            Settings.Default.AlwaysOnTop = _alwaysOnTop;
            Settings.Default.Save();
        }

        #endregion

        #region Helpers

        private async Task<bool> LoadDb(string fileName)
        {
            toolStripProgressBar1.Maximum = 3;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Loading database \"" + fileName + "\"";
            await Task.Run(() =>
            {
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        _metaDictionary = JsonIo.LoadBinary<List<JsoncDictionary>>(fileName);
                        toolStripProgressBar1.Value = 1;

                        _rootNodeExamples = JsonIo.LoadBinary<TreeNode>(openFileDialog1.FileName + ".tree");
                        toolStripProgressBar1.Value = 2;

                        if (_metaDictionary == null || _rootNodeExamples == null) return;

                        treeView_examples.Nodes.Clear();
                        treeView_examples.Nodes.Add(_rootNodeExamples);
                        treeView_examples.Sort();
                        treeView_examples.Nodes[0].Expand();

                        _rootNodeKeywords = JsonIo.LoadBinary<TreeNode>(openFileDialog1.FileName + ".keywords");
                        toolStripProgressBar1.Value = 3;

                        if (_rootNodeKeywords != null)
                        {
                            treeView_keywords.Nodes.Clear();
                            treeView_keywords.Nodes.Add(_rootNodeKeywords);
                            treeView_keywords.Sort();
                            treeView_keywords.Nodes[0].Expand();
                        }

                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File read exception [" + openFileDialog1.FileName + "]: " + ex.Message);
                }
            }).ConfigureAwait(true);
            toolStripStatusLabel1.Text = "";

            return true;
        }

        private void ValidateFile(string fullFileName)
        {
            string jsonText;
            try
            {
                jsonText = File.ReadAllText(fullFileName);
            }
            catch (Exception ex)
            {
                textLog.AppendLine(Environment.NewLine + fullFileName + " file read exception: " + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                return;
            }

            var versionIndex = jsonText.IndexOf(SchemaTag, StringComparison.Ordinal);
            if (versionIndex <= 0)
            {
                textLog.AppendLine(Environment.NewLine + fullFileName + " schema not found" + Environment.NewLine);
                return;
            }

            versionIndex += SchemaTag.Length;
            var strEnd = versionIndex;
            while (strEnd < jsonText.Length && jsonText[strEnd] != '"' && jsonText[strEnd] != '\r' && jsonText[strEnd] != '\n') strEnd++;

            var schemaUrl = jsonText.Substring(versionIndex, strEnd - versionIndex).Trim();

            if (!schemaUrl.EndsWith(".json"))
            {
                textLog.AppendLine(Environment.NewLine + fullFileName + " URL not found [" + schemaUrl + "]" + Environment.NewLine);
                return;
            }

            var schemaList = new Dictionary<string, string>();

            if (!schemaList.ContainsKey(schemaUrl))
            {
                var schemaData = "";
                try
                {
                    schemaData = GetSchemaText(schemaUrl);
                }
                catch (Exception ex)
                {
                    textLog.AppendLine(Environment.NewLine + fullFileName + " schema download exception: [" + schemaUrl + "]:" + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                }
                schemaList.Add(schemaUrl, schemaData);
            }
            var schemaText = schemaList[schemaUrl];

            if (string.IsNullOrEmpty(schemaText))
            {
                textLog.AppendLine(Environment.NewLine + fullFileName + " schema not loaded [" + schemaUrl + "]:" + Environment.NewLine);
                return;
            }

            JsonSchema schema;
            try
            {
                schema = JsonSchema.FromJsonAsync(schemaText).Result;
            }
            catch (Exception ex)
            {
                textLog.AppendLine(Environment.NewLine + fullFileName + " schema parse exception [" + schemaUrl + "]:" + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                return;
            }

            if (schema == null) return;

            ICollection<ValidationError> errors;
            try
            {
                errors = schema.Validate(jsonText);
            }
            catch (Exception ex)
            {
                textLog.AppendLine(Environment.NewLine + fullFileName + " file validation exception :" +
                                   Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                return;
            }

            foreach (var error in errors)
            {
                var errorText = PrintError(fullFileName, error);
                if (_suppressErrors.Contains(error.Kind))
                    File.AppendAllText(LogFileName, errorText);
                else
                    textLog.Append(errorText);
            }

        }

        private static string PrintError(string fullFileName, ValidationError error)
        {
            var errorText = new StringBuilder();
            errorText.AppendLine(fullFileName + ": line #" + error.LineNumber + " " + error.Kind + ", path=" + error.Path);

            if (error is ChildSchemaValidationError subErrorCollection)
                foreach (var subError in subErrorCollection.Errors)
                    foreach (var subErrorItem in subError.Value) errorText.AppendLine("\t" + "- line #" + subErrorItem.LineNumber + " " + subErrorItem.Kind + ", path=" + subErrorItem.Path);

            return errorText.ToString();
        }

        private static string ExceptionPrint(Exception ex)
        {
            var exceptionMessage = new StringBuilder();

            exceptionMessage.AppendLine(ex.Message);
            if (ex.InnerException != null) exceptionMessage.AppendLine(ExceptionPrint(ex.InnerException));

            return exceptionMessage.ToString();
        }

        private void DeserializeFile(string fullFileName, string shortFileName, TreeNode parentNode)
        {
            try
            {
                var jsonStr = File.ReadAllText(fullFileName);

                _fileName = fullFileName;
                if (!JsoncDictionary.FileNames.TryGetValue(shortFileName, out _fileType)) return;

                TreeNode fileNode;

                if (_metaDictionary.Any(n => n.Type == _fileType))
                {
                    fileNode = _rootNodeExamples.Nodes[_rootNodeExamples.Nodes.Count - 1];
                }
                else
                {
                    _metaDictionary.Add(new JsoncDictionary(_fileType, _collectAllFileNames));
                    fileNode = new TreeNode(shortFileName)
                    {
                        Name = shortFileName,
                    };
                    parentNode.Nodes.Add(fileNode);
                }

                var jsonSettings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                };
                dynamic jsonObject = JsonConvert.DeserializeObject(jsonStr, jsonSettings);
                if (jsonObject != null) ParseJsonObject(jsonObject, 0, shortFileName, fileNode);
            }
            catch (Exception ex)
            {
                textLog.AppendLine(Environment.NewLine + _fileName + " file parse exception: " + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
            }
        }

        private void ParseJsonObject(JToken token, int depth, string parent, TreeNode parentNode)
        {
            JsoncDictionary newItem;
            try
            {
                var obj = _metaDictionary.Where(n => n.Type == _fileType).ToArray();
                if (obj.Count() > 1) textLog.AppendLine(Environment.NewLine + "More than 1 similar file types found on parse" + Environment.NewLine);
                newItem = obj.FirstOrDefault();
            }
            catch (Exception ex)
            {
                textLog.AppendLine(Environment.NewLine + _fileName + " content parse exception: " + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                return;
            }

            switch (token)
            {
                case JProperty jProperty:
                    {
                        string printValue;
                        string saveValue;
                        var jValue = jProperty.Value;
                        if (jValue is JValue jPropertyValue)
                        {
                            printValue = saveValue = jPropertyValue.Value?.ToString();
                        }
                        else
                        {
                            printValue = jValue?.GetType().Name;
                            saveValue = jValue?.ToString();
                        }

                        if (!string.IsNullOrEmpty(saveValue))
                        {
                            var propName = jProperty.Name;
                            var obj = parentNode.Nodes.Cast<TreeNode>().Where(r => r.Text == propName).ToArray();
                            if (obj.Count() > 1)
                            {
                                textLog.AppendLine(Environment.NewLine
                                    + "More than 1 similar object found in the tree:"
                                    + Environment.NewLine
                                    + obj.Select(n => n.FullPath).Aggregate("", (current, next) => current + ", " + next)
                                    + Environment.NewLine);
                            }
                            var exNode = obj.FirstOrDefault();

                            TreeNode childNode;
                            if (exNode != null)
                            {
                                childNode = exNode;
                            }
                            else
                            {
                                childNode = new TreeNode(propName)
                                {
                                    Name = propName,
                                };
                                parentNode.Nodes.Add(childNode);
                            }

                            if (propName == VersionTagName) _version = printValue;
                            var nodeType = JsoncNodeType.Property;
                            var jPropType = jProperty.Children().FirstOrDefault().Type;
                            if (jPropType == JTokenType.Object)
                                nodeType = JsoncNodeType.Object;
                            else if (jPropType == JTokenType.Array)
                                nodeType = JsoncNodeType.Array;

                            var node = new MetaNode(propName, parent, nodeType, depth, saveValue, _fileName, _version);
                            var errorString = newItem?.Add(node);
                            if (!string.IsNullOrEmpty(errorString))
                            {
                                textLog.AppendLine(Environment.NewLine + _fileName
                                                                       + " node add error: "
                                                                       + Environment.NewLine
                                                                       + " Node ["
                                                                       + propName
                                                                       + "] "
                                                                       + errorString
                                                                       + Environment.NewLine);
                            }

                            foreach (var child in jProperty.Children())
                            {
                                if (child is JArray || child is JObject || child is JProperty) ParseJsonObject(child, depth + 1, jProperty.Path, childNode);
                            }
                        }
                        /*else
                        {
                            textLog.AppendLine(Environment.NewLine + _fileName
                                                                   + " Empty node: "
                                                                   + " Node ["
                                                                   + jProperty.Path
                                                                   + "] "
                                                                   + Environment.NewLine);
                        }*/
                        break;
                    }
                case JObject jObject:
                    {
                        string newParent;

                        if (!string.IsNullOrEmpty(jObject.Path))
                        {
                            newParent = jObject.Path.EndsWith("]") ? jObject.Path.Substring(0, jObject.Path.LastIndexOf('[')) : jObject.Path;
                            if (newParent.Contains(".")) newParent = newParent.Substring(newParent.LastIndexOf('.') + 1);
                        }
                        else
                        {
                            newParent = parent;
                        }

                        foreach (var child in jObject.Children()) ParseJsonObject(child, depth, newParent, parentNode);
                        break;
                    }
                case JArray jArray:
                    {
                        string newParent;

                        if (!string.IsNullOrEmpty(jArray.Path))
                        {
                            newParent = jArray.Path.EndsWith("]") ? jArray.Path.Substring(0, jArray.Path.LastIndexOf('[')) : jArray.Path;
                            if (newParent.Contains(".")) newParent = newParent.Substring(newParent.LastIndexOf('.') + 1);
                        }
                        else
                        {
                            newParent = parent;
                        }

                        foreach (var child in jArray.Children()) ParseJsonObject(child, depth, newParent, parentNode);
                        break;
                    }
                default:
                    {
                        if (token.Children().Any())
                        {
                            textLog.AppendLine(Environment.NewLine + _fileName
                                                                   + " Node missed: ["
                                                                   + token.Path
                                                                   + "] of type \""
                                                                   + token.Type
                                                                   + "\""
                                                                   + Environment.NewLine);
                        }
                    }
                    break;
            }
        }

        private async void CollectKeywords(IReadOnlyCollection<JsoncDictionary> sourceCollection, TreeNode parentNode)
        {
            /*var recordsNumber = sourceCollection.Count;
            var currentRecordNumber = 0;
            var oldProgress = 0;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;*/


            toolStripStatusLabel1.Text = "Collecting keywords from " + sourceCollection.Count + " nodes";
            var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);

            await Task.Run(() =>
            {
                // assuming 1st level nodes are file types/names
                foreach (var currentCollection in sourceCollection)
                {
                    var fileName = JsoncDictionary.FileNames.FirstOrDefault(s => s.Value == currentCollection.Type).Key;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        textLog.AppendLine(Environment.NewLine + currentCollection.Type + " file type not found in collection" + Environment.NewLine);
                        continue;
                    }

                    // select or create node for current file type
                    var currentFileNode = parentNode.Nodes.Find(fileName, true);
                    if (currentFileNode == null || !currentFileNode.Any())
                    {
                        var fileNode = new TreeNode(fileName)
                        {
                            Name = fileName,
                        };
                        parentNode.Nodes.Add(fileNode);
                        currentFileNode = parentNode.Nodes.Find(fileName, false);
                    }
                    if (currentFileNode == null)
                    {
                        textLog.AppendLine(Environment.NewLine + currentCollection.Type + " error finding/creating node for file type \"" + fileName + "\"" + Environment.NewLine);
                        continue;
                    }
                    if (currentFileNode.Count() > 1)
                    {
                        textLog.AppendLine(Environment.NewLine + currentCollection.Type + " parent node has duplicate file type records" + Environment.NewLine);
                    }

                    // collect all schema texts to search for keywords. simple but ineffective. temporary solution
                    // rewrite to use schema dictionary
                    /*var filesList = new List<string>();
                    filesList.AddRange(Directory.GetFiles(currentDirectory, fileName.Replace(".jsonc", ".json" + BackupSchemaExtension),
                        SearchOption.AllDirectories));
                    var schemaData = "";
                    foreach (var schemaFile in filesList)
                    {
                        try
                        {
                            schemaData += File.ReadAllText(schemaFile);
                        }
                        catch (Exception ex)
                        {
                            textLog.AppendLine(Environment.NewLine + schemaFile + " file read exception:" + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                        }
                    }*/

                    // collect nodes from dictionary to the tree
                    foreach (var record in currentCollection.Nodes)
                    {
                        TreeNode[] parNodeList;
                        var pName = record.ParentName;
                        if (pName == "action")
                            pName = "actions";

                        var cName = record.Name;
                        if (cName == "action")
                            cName = "actions";

                        if (pName == fileName)
                        {
                            parNodeList = currentFileNode;
                        }
                        else
                        {
                            parNodeList = currentFileNode[0].Nodes.Find(pName, true);
                            if (!parNodeList.Any())
                            {
                                var newNode = new TreeNode(pName)
                                {
                                    Name = pName,
                                };
                                currentFileNode[0].Nodes.Add(newNode);
                                parNodeList = currentFileNode[0].Nodes.Find(pName, false);
                            }
                        }
                        if (!parNodeList.Any())
                        {
                            textLog.AppendLine(Environment.NewLine + currentCollection.Type + " error finding/creating parent node for record \"" + record.ParentName + "." + record.Name + "\"" + Environment.NewLine);
                            continue;
                        }
                        // very often
                        if (parNodeList.Count() > 1)
                        {
                            // try to find proper parent to attach value
                            // get the parent name of current parent
                        }

                        var childNodesList = parNodeList[0].Nodes.Find(cName, false);
                        if (childNodesList == null || !childNodesList.Any())
                        {
                            var newNode = new TreeNode(cName)
                            {
                                Name = cName,
                            };
                            parNodeList[0].Nodes.Add(newNode);
                            childNodesList = parNodeList[0].Nodes.Find(cName, false);
                        }
                        if (childNodesList?.Count() > 1)
                        {
                            textLog.AppendLine(Environment.NewLine + currentCollection.Type + " error finding/creating child node for record \"" + record.ParentName + "." + record.Name + "\"" + Environment.NewLine);
                            continue;
                        }

                        // array/object value is always a complex object
                        if (record.Type != JsoncNodeType.Property) continue;

                        foreach (var prop in record.ExamplesList)
                        {
                            var propValue = prop.Key;
                            //var propValue = prop.Key.Trim();
                            //if (propValue.StartsWith("{") || propValue.StartsWith("[")) continue;
                            //if (!schemaData.Contains(propValue)) continue;

                            var propNodesList = childNodesList[0].Nodes.Find(propValue, false);
                            if (propNodesList == null || !propNodesList.Any())
                            {
                                var newNode = new TreeNode(propValue)
                                {
                                    Name = propValue,
                                };
                                childNodesList[0].Nodes.Add(newNode);
                                //propNodesList = childNodesList[0].Nodes.Find(pName, false);
                            }
                            /*if (propNodesList?.Count() > 1)
                            {
                                //not likely
                            }*/

                        }

                    }

                    /*Invoke((MethodInvoker)delegate
                    {
                        currentRecordNumber++;
                        var newProgress = (int)((float)currentRecordNumber / recordsNumber * 100);
                        if (newProgress >= oldProgress + MinProgressDisplay)
                        {
                            toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                                ? newProgress
                                : toolStripProgressBar1.Maximum;
                        }
                    });*/
                }
            }).ConfigureAwait(true);
            if (textLog.Length > 0)
            {
                textBox_logText.Text += textLog.ToString();
                textLog.Clear();
                textBox_logText.SelectionStart = textBox_logText.Text.Length;
                textBox_logText.ScrollToCaret();
            }
            toolStripStatusLabel1.Text = "";
        }

        private void FillExamplesGrid(TreeNode currentNode, string filterVersion = DefaultVersionCaption, string searchString = "", SearchCondition condition = SearchCondition.Contains, StringComparison caseSensitive = StringComparison.OrdinalIgnoreCase)
        {
            if (currentNode == null) return;

            var tokens = currentNode.FullPath.Split('\\');

            if (tokens.Length < 3) return;

            if (!JsoncDictionary.FileNames.TryGetValue(tokens[1], out var fileType))
            {
                textLog.AppendLine(Environment.NewLine
                    + "Unknown file type in the node: "
                    + tokens[1]
                    + Environment.NewLine);
                return;
            }

            var currentTypeNodeCollection = _metaDictionary?.Where(x => x.Type == fileType).ToArray();

            if (currentTypeNodeCollection == null) return;

            if (currentTypeNodeCollection.Count() > 1)
            {
                textLog.AppendLine(Environment.NewLine
                    + "More than 1 similar file types found on example print-out:"
                    + Environment.NewLine
                    + currentTypeNodeCollection.Select(n => n.Nodes).Aggregate("", (current, next) => current + ", " + next)
                    + Environment.NewLine);
            }
            var currentTypeNode = currentTypeNodeCollection.FirstOrDefault();

            var selectedExamples = currentTypeNode?.Nodes?.Where(n =>
                n.Name == currentNode.Text
                && n.Depth == tokens.Length - 3
                && n.ParentName == currentNode.Parent.Text);

            if (selectedExamples == null) return;

            toolStripStatusLabel1.Text = "Displaying " + selectedExamples.Count() + " records";

            if (_lastExSelectedNode != null) _lastExSelectedNode.NodeFont = new Font(treeView_examples.Font.FontFamily, treeView_examples.Font.Size, FontStyle.Regular);

            _lastExSelectedNode = currentNode;
            currentNode.NodeFont = new Font(treeView_examples.Font.FontFamily, treeView_examples.Font.Size, FontStyle.Bold);

            textBox_searchHistory.Clear();

            _examplesTable.Rows.Clear();
            dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            comboBox_versions.Items.Clear();
            comboBox_versions.Items.Add(DefaultVersionCaption);
            comboBox_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            comboBox_versions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in selectedExamples)
            {
                var examples = record.ExamplesList;
                if (examples == null) continue;

                if (!versionCollection.Contains(record.Version)) versionCollection.Add(record.Version);
                if (filterVersion != DefaultVersionCaption && record.Version != filterVersion) continue;

                foreach (var example in examples)
                {
                    var exampleText = JsonIo.BeautifyJson(example.Key, _reformatJson);
                    if (string.IsNullOrEmpty(searchString) && FilterCellOut(exampleText, condition, searchString, caseSensitive, true)) continue;
                    var newRow = _examplesTable.NewRow();
                    newRow[_exampleGridColumnsNames[0]] = record.Version;
                    newRow[_exampleGridColumnsNames[1]] = exampleText;
                    newRow[_exampleGridColumnsNames[2]] = example.Value;
                    _examplesTable.Rows.Add(newRow);
                }
            }
            _lastExFilterValue = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
                var csString = caseSensitive == StringComparison.Ordinal ? "CS:" : "";
                textBox_searchHistory.Text += transition + csString + condition.ToString() + "]\"" + searchString + "\"";
            }

            comboBox_versions.Items.AddRange(versionCollection.ToArray() as string[]);
            comboBox_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;
            toolStripStatusLabel1.Text = "";
        }

        private void FillKeywordsGrid(TreeNode currentNode, string filterVersion = DefaultVersionCaption, string searchString = "", SearchCondition condition = SearchCondition.Contains, StringComparison caseSensitive = StringComparison.OrdinalIgnoreCase)
        {
            if (currentNode == null) return;

            var tokens = currentNode.FullPath.Split('\\');

            if (tokens.Length < 3) return;

            if (!JsoncDictionary.FileNames.TryGetValue(tokens[1], out var fileType))
            {
                textLog.AppendLine(Environment.NewLine
                    + "Unknown file type in the node: "
                    + tokens[1]
                    + Environment.NewLine);
                return;
            }

            var currentTypeNodeCollection = _metaDictionary?.Where(x => x.Type == fileType).ToArray();

            if (currentTypeNodeCollection == null) return;

            if (currentTypeNodeCollection.Count() > 1)
            {
                textLog.AppendLine(Environment.NewLine
                    + "More than 1 similar file types found on example print-out:"
                    + Environment.NewLine
                    + currentTypeNodeCollection.Select(n => n.Nodes).Aggregate("", (current, next) => current + ", " + next)
                    + Environment.NewLine);
            }
            var currentTypeNode = currentTypeNodeCollection.FirstOrDefault();

            var cName = currentNode.Text;
            var pName = currentNode.Parent.Text;
            if (cName == "actions" && pName == "events")
                cName = "action";


            var selectedExamples = currentTypeNode?.Nodes?.Where(n =>
                n.Name == cName
                && n.ParentName == pName).ToList();

            if (selectedExamples == null) return;

            toolStripStatusLabel1.Text = "Displaying " + selectedExamples.Count() + " records";

            if (_lastKwSelectedNode != null) _lastKwSelectedNode.NodeFont = new Font(treeView_keywords.Font.FontFamily, treeView_keywords.Font.Size, FontStyle.Regular);

            _lastKwSelectedNode = currentNode;
            currentNode.NodeFont = new Font(treeView_keywords.Font.FontFamily, treeView_keywords.Font.Size, FontStyle.Bold);

            textBox_p_searchHistory.Clear();

            _keywordsTable.Rows.Clear();
            dataGridView_keywords.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            comboBox_p_versions.Items.Clear();
            comboBox_p_versions.Items.Add(DefaultVersionCaption);
            comboBox_p_versions.SelectedIndexChanged -= ComboBox_p_versions_SelectedIndexChanged;
            comboBox_p_versions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in selectedExamples)
            {
                var examples = record?.ExamplesList;
                if (examples == null) continue;

                if (!versionCollection.Contains(record.Version)) versionCollection.Add(record.Version);
                if (filterVersion != DefaultVersionCaption && record.Version != filterVersion) continue;

                foreach (var example in examples)
                {
                    var exampleText = JsonIo.BeautifyJson(example.Key, _reformatJson);
                    if (string.IsNullOrEmpty(searchString) && FilterCellOut(exampleText, condition, searchString, caseSensitive, true)) continue;
                    // remove duplicate records
                    var found = _keywordsTable.Rows.OfType<DataRow>().Any(row => row[0].ToString() == record.Version && row[1].ToString() == exampleText);
                    foreach (var row in _keywordsTable.Rows)
                    {
                        if (((DataRow) row).ItemArray[0].ToString() == record.Version &&
                            ((DataRow) row).ItemArray[1].ToString() == exampleText)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        continue;

                    var newRow = _keywordsTable.NewRow();
                    newRow[_exampleGridColumnsNames[0]] = record.Version;
                    newRow[_exampleGridColumnsNames[1]] = exampleText;
                    newRow[_exampleGridColumnsNames[2]] = example.Value;
                    _keywordsTable.Rows.Add(newRow);
                }
            }
            _lastKwFilterValue = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                var transition = textBox_p_searchHistory.Text == "" ? "[" : " ->[";
                var csString = caseSensitive == StringComparison.Ordinal ? "CS:" : "";
                textBox_p_searchHistory.Text += transition + csString + condition.ToString() + "]\"" + searchString + "\"";
            }

            comboBox_p_versions.Items.AddRange(versionCollection.ToArray() as string[]);
            comboBox_p_versions.SelectedIndexChanged += ComboBox_p_versions_SelectedIndexChanged;
            toolStripStatusLabel1.Text = "";
        }

        private async void FilterExamples(SearchCondition condition, string searchString, string filterVersion = DefaultVersionCaption, StringComparison caseSensitive = StringComparison.OrdinalIgnoreCase, bool compactJson = false)
        {
            if (_lastExFilterValue == searchString && _lastExSelectedCondition == condition && _lastExCaseSensitive == caseSensitive) return;

            _lastExFilterValue = searchString;
            _lastExSelectedCondition = condition;
            _lastExCaseSensitive = caseSensitive;

            if (string.IsNullOrEmpty(searchString))
            {
                FillExamplesGrid(_lastExSelectedNode, filterVersion);
                return;
            }

            var rows = _examplesTable.Rows;
            var rowsNumber = rows.Count;
            /*var currentRowNumber = 0;
            var oldProgress = 0;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;*/
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                //Invoke((MethodInvoker)delegate
                //{
                for (var i = 0; i < rows.Count; i++)
                {
                    /*currentRowNumber++;
                    var newProgress = (int)((float)currentRowNumber / rowsNumber * 100);
                    if (newProgress >= oldProgress + MinProgressDisplay)
                    {
                        toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                            ? newProgress
                            : toolStripProgressBar1.Maximum;
                        if (textLog.Length > 0)
                        {
                            textBox_logText.Text += textLog.ToString();
                            textLog.Clear();
                            textBox_logText.SelectionStart = textBox_logText.Text.Length;
                            textBox_logText.ScrollToCaret();
                        }
                    }*/
                    var cellValue = rows[i].ItemArray[1];
                    if (cellValue == null || FilterCellOut(cellValue.ToString(), condition, searchString, caseSensitive, compactJson))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
                //});
            }).ConfigureAwait(true);

            toolStripStatusLabel1.Text = "";

            var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
            var csString = caseSensitive == StringComparison.Ordinal ? "CS:" : "";
            textBox_searchHistory.Text += transition + csString + condition.ToString() + "]\"" + searchString + "\"";
        }

        private async void FilterKeywords(SearchCondition condition, string searchString, string filterVersion = DefaultVersionCaption, StringComparison caseSensitive = StringComparison.OrdinalIgnoreCase, bool compactJson = false)
        {
            if (_lastKwFilterValue == searchString && _lastKwSelectedCondition == condition && _lastKwCaseSensitive == caseSensitive) return;

            _lastKwFilterValue = searchString;
            _lastKwSelectedCondition = condition;
            _lastKwCaseSensitive = caseSensitive;

            if (string.IsNullOrEmpty(searchString))
            {
                FillKeywordsGrid(_lastKwSelectedNode, filterVersion);
                return;
            }

            var rows = _keywordsTable.Rows;
            var rowsNumber = rows.Count;
            /*var currentRowNumber = 0;
            var oldProgress = 0;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;*/
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                //Invoke((MethodInvoker)delegate
                //{
                for (var i = 0; i < rows.Count; i++)
                {
                    /*currentRowNumber++;
                    var newProgress = (int)((float)currentRowNumber / rowsNumber * 100);
                    if (newProgress >= oldProgress + MinProgressDisplay)
                    {
                        toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                            ? newProgress
                            : toolStripProgressBar1.Maximum;
                        if (textLog.Length > 0)
                        {
                            textBox_logText.Text += textLog.ToString();
                            textLog.Clear();
                            textBox_logText.SelectionStart = textBox_logText.Text.Length;
                            textBox_logText.ScrollToCaret();
                        }
                    }*/
                    var cellValue = rows[i].ItemArray[1];
                    if (cellValue == null || FilterCellOut(cellValue.ToString(), condition, searchString, caseSensitive, compactJson))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
                //});
            }).ConfigureAwait(true);

            toolStripStatusLabel1.Text = "";

            var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
            var csString = caseSensitive == StringComparison.Ordinal ? "CS:" : "";
            textBox_p_searchHistory.Text += transition + csString + condition.ToString() + "]\"" + searchString + "\"";
        }

        private bool FilterCellOut(string cellValue, SearchCondition condition, string searchString, StringComparison caseSensitive, bool compactJson)
        {
            if (cellValue == null || string.IsNullOrEmpty(cellValue)) return true;

            if (string.IsNullOrEmpty(searchString)) return false;

            var newcellValue = compactJson
                ? JsonIo.CompactJson(cellValue)
                : cellValue;
            switch (condition)
            {
                case SearchCondition.Contains:
                    if (newcellValue.IndexOf(searchString, caseSensitive) < 0)
                        return true;
                    break;
                case SearchCondition.StartsWith:
                    if (!newcellValue.StartsWith(searchString, caseSensitive))
                        return true;
                    break;
                case SearchCondition.EndsWith:
                    if (!newcellValue.EndsWith(searchString, caseSensitive))
                        return true;
                    break;
                default:
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Incorrect filter condition: " + condition.ToString());
                    });
                    break;
            }
            return false;
        }

        private async void FilterExamplesVersion(string versionString)
        {
            if (_lastExSelectedVersion != DefaultVersionCaption) FillExamplesGrid(_lastExSelectedNode, versionString);

            comboBox_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            if (comboBox_versions.Items.Contains(versionString))
                comboBox_versions.SelectedItem = _lastExSelectedVersion = versionString;
            else
                comboBox_versions.SelectedItem = versionString = _lastExSelectedVersion = DefaultVersionCaption;
            comboBox_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;

            if (versionString == DefaultVersionCaption) return;

            var rows = _examplesTable.Rows;
            var rowsNumber = rows.Count;
            /*var currentRowNumber = 0;
            var oldProgress = 0;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;*/
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                //Invoke((MethodInvoker)delegate
                //{
                for (var i = 0; i < rows.Count; i++)
                {
                    /*currentRowNumber++;
                    var newProgress = (int)((float)currentRowNumber / rowsNumber * 100);
                    if (newProgress >= oldProgress + MinProgressDisplay)
                    {
                        toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                            ? newProgress
                            : toolStripProgressBar1.Maximum;
                        if (textLog.Length > 0)
                        {
                            textBox_logText.Text += textLog.ToString();
                            textLog.Clear();
                            textBox_logText.SelectionStart = textBox_logText.Text.Length;
                            textBox_logText.ScrollToCaret();
                        }
                    }*/
                    var cellValue = rows[i].ItemArray[0];
                    if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                    else if (cellValue.ToString() != versionString)
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
                //});
            }).ConfigureAwait(true);
            toolStripStatusLabel1.Text = "";
        }

        private async void FilterKeywordsVersion(string versionString)
        {
            if (_lastKwSelectedVersion != DefaultVersionCaption) FillExamplesGrid(_lastKwSelectedNode, versionString);

            comboBox_p_versions.SelectedIndexChanged -= ComboBox_p_versions_SelectedIndexChanged;
            if (comboBox_p_versions.Items.Contains(versionString))
                comboBox_p_versions.SelectedItem = _lastKwSelectedVersion = versionString;
            else
                comboBox_p_versions.SelectedItem = versionString = _lastKwSelectedVersion = DefaultVersionCaption;
            comboBox_p_versions.SelectedIndexChanged += ComboBox_p_versions_SelectedIndexChanged;

            if (versionString == DefaultVersionCaption) return;

            var rows = _keywordsTable.Rows;
            var rowsNumber = rows.Count;
            /*var currentRowNumber = 0;
            var oldProgress = 0;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;*/
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                //Invoke((MethodInvoker)delegate
                //{
                for (var i = 0; i < rows.Count; i++)
                {
                    /*currentRowNumber++;
                    var newProgress = (int)((float)currentRowNumber / rowsNumber * 100);
                    if (newProgress >= oldProgress + MinProgressDisplay)
                    {
                        toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                            ? newProgress
                            : toolStripProgressBar1.Maximum;
                        if (textLog.Length > 0)
                        {
                            textBox_logText.Text += textLog.ToString();
                            textLog.Clear();
                            textBox_logText.SelectionStart = textBox_logText.Text.Length;
                            textBox_logText.ScrollToCaret();
                        }
                    }*/
                    var cellValue = rows[i].ItemArray[0];
                    if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                    else if (cellValue.ToString() != versionString)
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
                //});
            }).ConfigureAwait(true);
            toolStripStatusLabel1.Text = "";
        }

        private async void ReadjustRows(DataGridView DGView)
        {
            var rowsNumber = DGView.RowCount;
            //var currentRowNumber = 0;
            //var oldProgress = 0;
            //toolStripProgressBar1.Maximum = 100;
            //toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Adjusting height for " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    if (DGView.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
                    {
                        LoopThroughRows(DGView);
                        DGView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    }
                    for (var rowNumber = 0; rowNumber < DGView.RowCount; rowNumber++)
                    {
                        //currentRowNumber++;
                        //var newProgress = (int)((float)currentRowNumber / rowsNumber * 100);
                        /*if (newProgress >= oldProgress + MinProgressDisplay)
                        {
                            toolStripProgressBar1.Value = oldProgress = newProgress <= toolStripProgressBar1.Maximum
                                ? newProgress
                                : toolStripProgressBar1.Maximum;
                            if (textLog.Length > 0)
                            {
                                textBox_logText.Text += textLog.ToString();
                                textLog.Clear();
                                textBox_logText.SelectionStart = textBox_logText.Text.Length;
                                textBox_logText.ScrollToCaret();
                            }
                        }*/

                        var row = DGView.Rows[rowNumber];
                        var newHeight = row.GetPreferredHeight(rowNumber,
                        DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);
                        var currentHeight = DGView.Height;
                        if (newHeight == row.Height && newHeight <= currentHeight * CellHeightAdjust) return;

                        if (newHeight > currentHeight * CellHeightAdjust)
                            newHeight = (ushort)(currentHeight * CellHeightAdjust);
                        row.Height = newHeight;
                    }
                });
            }).ConfigureAwait(true);
            toolStripStatusLabel1.Text = "";
        }

        private static void ReadjustRow(DataGridView dgView, int rowNumber)
        {
            if (dgView.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
            {
                LoopThroughRows(dgView);
                dgView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            var row = dgView.Rows[rowNumber];
            var newHeight = row.GetPreferredHeight(rowNumber, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);
            var currentHeight = dgView.Height;
            if (newHeight == row.Height && newHeight <= currentHeight * CellHeightAdjust) return;

            if (newHeight > currentHeight * CellHeightAdjust) newHeight = (ushort)(currentHeight * CellHeightAdjust);
            row.Height = newHeight;
        }

        // needed to get rid of exception on changing "dataGridView_examples.AutoSizeRowsMode"
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization | System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void LoopThroughRows(DataGridView dgv)
        {
            var rows = dgv.Rows;
            var rowsCount = rows.Count;
            for (var i = rowsCount - 1; i >= 0; i--)
            {
                var row = rows[i];
            }
        }

        private void ActivateUiControls(bool active, bool processTable = true)
        {

            if (active)
            {
                textBox_logText.Text += textLog.ToString();
                textLog.Clear();
                textBox_logText.SelectionStart = textBox_logText.Text.Length;
                textBox_logText.ScrollToCaret();
            }

            if (processTable)
            {
                if (active)
                {
                    dataGridView_examples.DataSource = _examplesTable;
                    dataGridView_examples.Invalidate();

                    dataGridView_keywords.DataSource = _keywordsTable;
                    dataGridView_keywords.Invalidate();
                }
                else
                {
                    dataGridView_examples.DataSource = null;
                    dataGridView_keywords.DataSource = null;
                }
            }

            dataGridView_examples.Enabled = active;
            dataGridView_keywords.Enabled = active;

            checkedListBox_params.Enabled = active;
            button_validateFiles.Enabled = active;
            button_collectDatabase.Enabled = active;
            button_loadDb.Enabled = active;
            button_saveDb.Enabled = active;

            comboBox_versions.Enabled = active;
            comboBox_condition.Enabled = active;
            textBox_searchString.Enabled = active;
            checkBox_seachCaseSensitive.Enabled = active;
            dataGridView_examples.Enabled = active;
            treeView_examples.Enabled = active;
            button_reAdjust.Enabled = active;

            comboBox_p_versions.Enabled = active;
            comboBox_p_condition.Enabled = active;
            textBox_p_searchString.Enabled = active;
            checkBox_p_seachCaseSensitive.Enabled = active;
            dataGridView_keywords.Enabled = active;
            treeView_keywords.Enabled = active;
            button_p_reAdjust.Enabled = active;

            tabControl1.Enabled = active;

            Refresh();
        }

        private string GetSchemaText(string schemaUrl)
        {
            var schemaData = "";
            if (string.IsNullOrEmpty(schemaUrl)) return schemaData;

            var localPath = GetLocalUrlPath(schemaUrl);
            if (File.Exists(localPath))
            {
                schemaData = File.ReadAllText(localPath);
            }
            else
            {
                if (_ignoreHttpsError) ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                using (var webClient = new System.Net.WebClient())
                {
                    schemaData = webClient.DownloadString(schemaUrl);
                    var dirPath = Path.GetDirectoryName(localPath);
                    if (dirPath != null)
                    {
                        Directory.CreateDirectory(dirPath);
                        File.WriteAllText(localPath + BackupSchemaExtension, schemaData);
                    }
                }
            }

            return schemaData;
        }

        private static string GetLocalUrlPath(string url)
        {
            if (!url.Contains("://")) return "";

            url = url.Replace("://", "");
            if (!url.Contains("/")) return "";

            var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
            var localPath = currentDirectory + url.Substring(url.IndexOf('/'));
            localPath = localPath.Replace('/', '\\');
            return localPath;
        }

        private static string ShortFileName(string longFileName)
        {
            var i = longFileName.LastIndexOf('\\');
            return i < 0 ? longFileName : longFileName.Substring(i + 1);
        }
        #endregion

    }
}
