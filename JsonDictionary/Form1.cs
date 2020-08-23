using JsonDictionary.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NJsonSchema;
using NJsonSchema.Validation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static JsonDictionary.JsonIo;

namespace JsonDictionary
{
    public partial class Form1 : Form
    {
        // pre-defined constants
        private readonly string[] _exampleGridColumnsNames = { "Version", "Example", "File Name" };
        private const string DefaultVersionCaption = "Any";
        private const string VersionTagName = "contentVersion";

        private readonly ValidationErrorKind[] _suppressErrors =
        {
            ValidationErrorKind.ArrayItemNotValid, ValidationErrorKind.PropertyRequired,
            ValidationErrorKind.NoAdditionalPropertiesAllowed
        };

        private const string SchemaTag = "\"$schema\": \"";
        private const string RootNodeName = "root";
        private const float CellHeightAdjust = 0.7f;
        private const string LogFileName = "hiddenerrors.log";
        private const string PreViewCaption = "Preview";
        private const string FormCaption = "KineticJsonDictionary";
        private const string BackupSchemaExtension = ".original";

        // behavior options
        private static bool _reformatJson;
        private bool _collectAllFileNames;
        private bool _ignoreHttpsError;
        private bool _showPreview;
        private bool _alwaysOnTop;
        private bool _loadDbOnStart;

        // global variables
        //logging only
        private string _fileName = "";
        private readonly StringBuilder _textLog = new StringBuilder();

        // global data storage
        private readonly DataTable _examplesTable;
        private readonly DataTable _keywordsTable;
        private List<JsoncDictionary> _metaDictionary = new List<JsoncDictionary>();
        private volatile bool _isDoubleClick;
        private volatile string _version = "";

        // last used values for UI processing optimization
        private TreeNode _lastExSelectedNode;
        private List<SearchItem> _lastExSearchList = new List<SearchItem>();

        private TreeNode _lastKwSelectedNode;
        private List<SearchItem> _lastKwSearchList = new List<SearchItem>();

        private JsonViewer _sideViewer;

        private enum SearchCondition
        {
            Contains,
            StartsWith,
            EndsWith
        }

        private class SearchItem : ICloneable, IEqualityComparer, IEquatable<SearchItem>
        {
            public string Version = DefaultVersionCaption;
            public SearchCondition Condition = SearchCondition.Contains;
            public StringComparison CaseSensitive = StringComparison.OrdinalIgnoreCase;
            public string Value = "";

            public object Clone()
            {
                return new SearchItem
                {
                    Condition = Condition,
                    CaseSensitive = CaseSensitive,
                    Value = Value,
                    Version = Version
                };
            }

            bool IEqualityComparer.Equals(object x, object y)
            {
                if (x is SearchItem itemX && y is SearchItem itemY)
                    return itemX.Version == itemY.Version
                           && itemX.Condition == itemY.Condition
                           && itemX.CaseSensitive == itemY.CaseSensitive
                           && itemX.Value == itemY.Value;

                return false;
            }

            public int GetHashCode(object obj)
            {
                if (obj is SearchItem item)
                    return (item.Version + item.Condition + item.CaseSensitive + item.Value).GetHashCode();

                return obj.GetHashCode();
            }


            public bool Equals(SearchItem other)
            {
                    return Version == other.Version
                           && Condition == other.Condition
                           && CaseSensitive == other.CaseSensitive
                           && Value == other.Value;
            }
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
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
            _loadDbOnStart = Settings.Default.LoadDbOnStartUp;

            checkBox_collectAllFileNames.Checked = _collectAllFileNames;
            checkBox_reformatJson.Checked = _reformatJson;
            checkBox_ignoreHttpsError.Checked = _ignoreHttpsError;
            checkBox_showPreview.Checked = _showPreview;
            checkBox_alwaysOnTop.Checked = _alwaysOnTop;
            checkBox_loadDbOnStart.Checked = _loadDbOnStart;

            TopMost = _alwaysOnTop;

            comboBox_ExCondition.Items.AddRange(typeof(SearchCondition).GetEnumNames());
            comboBox_ExCondition.SelectedIndex = 0;

            comboBox_KwCondition.Items.AddRange(typeof(SearchCondition).GetEnumNames());
            comboBox_KwCondition.SelectedIndex = 0;

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

            comboBox_ExVersions.SelectedIndexChanged -= ComboBox_ExVersions_SelectedIndexChanged;
            comboBox_ExVersions.Items.Clear();
            comboBox_ExVersions.Items.Add(DefaultVersionCaption);
            comboBox_ExVersions.SelectedIndex = 0;
            comboBox_ExVersions.SelectedIndexChanged += ComboBox_ExVersions_SelectedIndexChanged;

            comboBox_KwVersions.SelectedIndexChanged -= ComboBox_KwVersions_SelectedIndexChanged;
            comboBox_KwVersions.Items.Clear();
            comboBox_KwVersions.Items.Add(DefaultVersionCaption);
            comboBox_KwVersions.SelectedIndex = 0;
            comboBox_KwVersions.SelectedIndexChanged += ComboBox_KwVersions_SelectedIndexChanged;

            dataGridView_examples.ContextMenuStrip = contextMenuStrip_ExFindValue;
            treeView_examples.ContextMenuStrip = contextMenuStrip_ExFindField;

            dataGridView_keywords.ContextMenuStrip = contextMenuStrip_KwFindValue;
            treeView_keywords.ContextMenuStrip = contextMenuStrip_KwFindField;

            _sideViewer = new JsonViewer();
            _sideViewer.Dispose();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (_loadDbOnStart) await LoadDb(Settings.Default.LastDbName);
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
            var rootNodeExamples = new TreeNode(RootNodeName);
            _metaDictionary = new List<JsoncDictionary>();
            var startPath = folderBrowserDialog1.SelectedPath;
            var filesList = new List<string>();
            toolStripStatusLabel1.Text = "Searching files...";
            var filesCount = 0;

            // Multithread task execution sample
            /*var taskList = new List<Task<Dictionary<IPAddress, string>>>();
            var addressBytes = IPAddress.Parse(_ipAddress).GetAddressBytes();
            for (var i = 0; i < 255; i++)
            {
                var task = new Func<int, Task<Dictionary<IPAddress, string>>>(p =>
                    Task.Run(async () => await PingAddress(addressBytes, p, _ipPort).ConfigureAwait(true))).Invoke(i);
                taskList.Add(task);
            }
            await Task.WhenAll(taskList.ToArray()).ConfigureAwait(true);
            foreach (var ip in taskList.Where(x => x.Result != null))
                _logger.AddText(ip.Result?.FirstOrDefault() + Environment.NewLine, DateTime.Now,
                    (byte)DataDirection.Note);*/

            await Task.Run(() =>
            {
                foreach (var fileName in checkedListBox_params.CheckedItems)
                {
                    filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(),
                        SearchOption.AllDirectories));
                    var filesNumber = filesList.Count;
                    Invoke((MethodInvoker)delegate
                   {
                       toolStripStatusLabel1.Text =
                           "Collecting \"" + fileName + "\" database from " + filesNumber + " files";
                   });

                    foreach (var file in filesList)
                    {
                        _version = "";
                        DeserializeFile(file, fileName.ToString(), _metaDictionary, rootNodeExamples);
                    }

                    filesCount += filesList.Count;
                    filesList.Clear();
                }
            }).ConfigureAwait(true);


            toolStripStatusLabel1.Text = "";
            _textLog.AppendLine("Files parsed: " + filesCount);
            textBox_logText.Text += _textLog.ToString();
            _textLog.Clear();
            textBox_logText.SelectionStart = textBox_logText.Text.Length;
            textBox_logText.ScrollToCaret();

            treeView_examples.Nodes.Add(rootNodeExamples);
            treeView_examples.Sort();
            treeView_examples.Nodes[0].Expand();

            var rootNodeKeywords = new TreeNode(RootNodeName);
            await CollectKeywords(_metaDictionary, rootNodeKeywords);
            treeView_keywords.Nodes.Add(rootNodeKeywords);
            treeView_keywords.Sort();
            treeView_keywords.Nodes[0].Expand();

            ActivateUiControls(true);
        }

        private async void Button_validateFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            ActivateUiControls(false);

            var startPath = folderBrowserDialog1.SelectedPath;
            var filesCount = 0;

            toolStripStatusLabel1.Text = "Searching files...";
            await Task.Run(() =>
            {
                foreach (var fileName in checkedListBox_params.CheckedItems)
                {
                    var filesList = new List<string>();
                    filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(), SearchOption.AllDirectories));
                    Invoke((MethodInvoker)delegate
                   {
                       toolStripStatusLabel1.Text =
                           "Validating \"" + fileName + "\" files from " + filesList.Count + " files";
                       if (_textLog.Length > 0)
                       {
                           textBox_logText.Text += _textLog.ToString();
                           _textLog.Clear();
                           textBox_logText.SelectionStart = textBox_logText.Text.Length;
                           textBox_logText.ScrollToCaret();
                       }
                   });

                    foreach (var file in filesList) ValidateFile(file);

                    filesCount += filesList.Count;
                    filesList.Clear();
                }
            }).ConfigureAwait(true);

            toolStripStatusLabel1.Text = "";

            _textLog.AppendLine("Files validated: " + filesCount);
            ActivateUiControls(true);
        }

        private void Button_saveDb_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save data as JSON...";
            saveFileDialog1.DefaultExt = "json";
            saveFileDialog1.Filter = "Binary files|*.metalib|All files|*.*";
            saveFileDialog1.FileName =
                "metaUiDictionary_" + DateTime.Today.ToShortDateString().Replace("/", "_") + ".metalib";
            saveFileDialog1.ShowDialog();
        }

        private async void SaveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (!_metaDictionary.Any() || treeView_examples?.Nodes?.Count <= 0 ||
                treeView_keywords?.Nodes?.Count <= 0) return;
            await Task.Run(() =>
            {
                try
                {
                    SaveBinary(_metaDictionary, saveFileDialog1.FileName);
                    SaveBinary(treeView_examples.Nodes[0], saveFileDialog1.FileName + ".tree");
                    SaveBinary(treeView_keywords.Nodes[0], saveFileDialog1.FileName + ".keywords");
                    Settings.Default.LastDbName = saveFileDialog1.FileName;
                    Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File write exception [" + saveFileDialog1.FileName + "]: " + ex.Message);
                }
            }).ConfigureAwait(true);
        }

        private async void Button_ExAdjustRows_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false, false);
            await ReadjustRows(dataGridView_examples);
            ActivateUiControls(true);
        }

        private async void Button_KwAdjustRows_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false, false);
            await ReadjustRows(dataGridView_keywords);
            ActivateUiControls(true);
        }

        private async void TextBox_ExSearchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            var searchParam = new SearchItem
            {
                Version = comboBox_ExVersions.SelectedItem?.ToString(),
                CaseSensitive = checkBox_ExCaseSensitive.Checked
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase,
                Value = textBox_ExSearchString.Text,
                Condition = (SearchCondition)comboBox_ExCondition.SelectedIndex
            };
            if (_lastExSearchList.Contains(searchParam)) return;

            ActivateUiControls(false);
            await FilterExamples(_metaDictionary, searchParam);
            ActivateUiControls(true);
            e.SuppressKeyPress = true;
        }

        private async void TextBox_KwSearchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            var searchParam = new SearchItem
            {
                Version = comboBox_KwVersions.SelectedItem?.ToString(),
                CaseSensitive = checkBox_KwCaseSensitive.Checked
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase,
                Value = textBox_KwSearchString.Text,
                Condition = (SearchCondition)comboBox_KwCondition.SelectedIndex
            };
            if (_lastKwSearchList.Contains(searchParam)) return;

            ActivateUiControls(false);
            await FilterKeywords(_metaDictionary, searchParam);
            ActivateUiControls(true);
            e.SuppressKeyPress = true;
        }

        private void Button_ExClearSearch_Click(object sender, EventArgs e)
        {
            ExClearSearch();
            FillExamplesGrid(_metaDictionary, treeView_examples.SelectedNode);
        }

        private void Button_KwClearSearch_Click(object sender, EventArgs e)
        {
            KwClearSearch();
            FillKeywordsGrid(_metaDictionary, treeView_keywords.SelectedNode);
        }

        private async void ComboBox_ExVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateUiControls(false);
            var searchParam = new SearchItem
            {
                Version = comboBox_ExVersions.SelectedItem.ToString()
            };
            await FilterExamplesVersion(_metaDictionary, searchParam);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private async void ComboBox_KwVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateUiControls(false);
            var searchParam = new SearchItem
            {
                Version = comboBox_KwVersions.SelectedItem.ToString()
            };
            await FilterKeywordsVersion(_metaDictionary, searchParam);
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
            if (!_isDoubleClick) return;

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

            if (treeView_examples.SelectedNode == null ||
                treeView_examples.SelectedNode.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillExamplesGrid(_metaDictionary, treeView_examples.SelectedNode);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
            _lastExSearchList.Clear();
        }

        private void TreeView_keywords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            if (treeView_keywords.SelectedNode == null ||
                treeView_keywords.SelectedNode.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillKeywordsGrid(_metaDictionary, treeView_keywords.SelectedNode);
            dataGridView_keywords.Invalidate();
            ActivateUiControls(true);
            _lastKwSearchList.Clear();
        }

        private void TreeView_examples_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || e.Node.Parent != null && e.Node.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillExamplesGrid(_metaDictionary, e.Node);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
            _lastExSearchList.Clear();
        }

        private void TreeView_keywords_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || e.Node.Parent != null && e.Node.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillKeywordsGrid(_metaDictionary, e.Node);
            dataGridView_keywords.Invalidate();
            ActivateUiControls(true);
            _lastKwSearchList.Clear();
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
                    var editor = new JsonViewer(dataGrid.Rows[e.RowIndex].Cells[2].Value.ToString(),
                        "")
                    {
                        ReformatJson = _reformatJson,
                        AlwaysOnTop = _alwaysOnTop
                    };
                    editor.Show();

                    editor.SelectText(dataGrid.Rows[e.RowIndex].Cells[1].Value.ToString());
                }
                else if (column == 1)
                {
                    var cell = dataGrid.Rows[e.RowIndex].Cells[column];
                    dataGrid.CurrentCell = cell;

                    var editor = new JsonViewer(dataGrid.Rows[e.RowIndex].Cells[2].Value.ToString(),
                        cell.Value.ToString())
                    {
                        ReformatJson = _reformatJson,
                        AlwaysOnTop = _alwaysOnTop
                    };
                    editor.Show();
                }
            }
        }

        private void DataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is DataGridView dataGrid) ReadjustRow(dataGrid, e.RowIndex);
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!_showPreview) return;

            if (_sideViewer == null || _sideViewer.IsDisposed)
            {
                _sideViewer = new JsonViewer(PreViewCaption, " ")
                {
                    ReformatJson = _reformatJson,
                    AlwaysOnTop = _alwaysOnTop
                };
                _sideViewer.Show();
            }

            if (sender is DataGridView dataGrid)
            {
                if (dataGrid.Rows.Count <= 0) return;
                _sideViewer.EditorText = dataGrid.Rows[e.RowIndex]?.Cells[1]?.Value?.ToString();
            }
        }

        private void ContextMenuStrip_ExFindValue_Opening(object sender, CancelEventArgs e)
        {
            if (_lastExSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells.Count < 1
                || dataGridView_examples.SelectedCells[0]?.ColumnIndex != 1
                || _lastExSelectedNode.Parent?.Parent?.Parent == null)
                ExFindAllStripMenuItem.Enabled = false;
            else ExFindAllStripMenuItem.Enabled = true;
        }

        private void ContextMenuStrip_KwFindValue_Opening(object sender, CancelEventArgs e)
        {
            if (_lastKwSelectedNode == null
                || dataGridView_keywords == null
                || dataGridView_keywords.SelectedCells.Count < 1
                || dataGridView_keywords.SelectedCells[0]?.ColumnIndex != 1
                || _lastKwSelectedNode.Parent?.Parent?.Parent == null)
                KwFindAllStripMenuItem.Enabled = false;
            else KwFindAllStripMenuItem.Enabled = true;
        }

        private void ContextMenuStrip_ExFindField_Opening(object sender, CancelEventArgs e)
        {
            if (treeView_examples.SelectedNode?.Parent?.Parent?.Parent == null ||
                treeView_examples.SelectedNode.Parent.Parent.Text == RootNodeName)
                ExFindFieldToolStripMenuItem.Enabled = false;
            else ExFindFieldToolStripMenuItem.Enabled = true;
        }

        private void ContextMenuStrip_KwFindField_Opening(object sender, CancelEventArgs e)
        {
            if (treeView_keywords.SelectedNode?.Parent?.Parent?.Parent == null ||
                treeView_keywords.SelectedNode.Parent.Parent.Text == RootNodeName)
                KwFindFieldToolStripMenuItem.Enabled = false;
            else KwFindFieldToolStripMenuItem.Enabled = true;
        }

        private async void ExFindValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lastExSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells.Count < 1
                || dataGridView_examples.SelectedCells[0]?.ColumnIndex != 1
                || _lastExSelectedNode.Parent?.Parent?.Parent == null) return;

            var searchValue = CompactJson(dataGridView_examples.SelectedCells[0].Value.ToString());

            var searchParam = new SearchItem
            {
                Version = comboBox_ExVersions.SelectedItem.ToString(),
                CaseSensitive = StringComparison.Ordinal,
                Value = "\"" + _lastExSelectedNode.Text + "\":",
                Condition = SearchCondition.Contains
            };
            ActivateUiControls(false);
            FillExamplesGrid(_metaDictionary, _lastExSelectedNode.Parent, searchParam);

            searchParam = new SearchItem
            {
                Version = comboBox_ExVersions.SelectedItem.ToString(),
                CaseSensitive = StringComparison.Ordinal,
                Value = searchValue,
                Condition = SearchCondition.Contains
            };
            await FilterExamples(_metaDictionary, searchParam, true);

            ActivateUiControls(true);
        }

        private async void KwFindValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lastKwSelectedNode == null
                || dataGridView_keywords == null
                || dataGridView_keywords.SelectedCells.Count < 1
                || dataGridView_keywords.SelectedCells[0]?.ColumnIndex != 1
                || _lastKwSelectedNode.Parent?.Parent?.Parent == null) return;

            var searchValue = CompactJson(dataGridView_keywords.SelectedCells[0].Value.ToString());

            var searchParam = new SearchItem
            {
                Version = comboBox_KwVersions.SelectedItem.ToString(),
                CaseSensitive = StringComparison.Ordinal,
                Value = "\"" + _lastKwSelectedNode.Text + "\":",
                Condition = SearchCondition.Contains
            };
            ActivateUiControls(false);
            FillKeywordsGrid(_metaDictionary, _lastKwSelectedNode.Parent, searchParam);

            searchParam = new SearchItem
            {
                Version = comboBox_KwVersions.SelectedItem.ToString(),
                CaseSensitive = StringComparison.Ordinal,
                Value = searchValue,
                Condition = SearchCondition.Contains
            };
            await FilterKeywords(_metaDictionary, searchParam, true);

            ActivateUiControls(true);
        }

        private async void ExFindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView_examples.SelectedNode?.Parent?.Parent?.Parent == null ||
                treeView_examples.SelectedNode.Parent.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillExamplesGrid(_metaDictionary, treeView_examples.SelectedNode.Parent);

            var searchParam = new SearchItem
            {
                Version = comboBox_ExVersions.SelectedItem.ToString(),
                CaseSensitive = StringComparison.Ordinal,
                Value = "\"" + treeView_examples.SelectedNode.Text + "\":",
                Condition = SearchCondition.Contains
            };
            await FilterExamples(_metaDictionary, searchParam);
            ActivateUiControls(true);
        }

        private async void KwFindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView_keywords.SelectedNode?.Parent?.Parent?.Parent == null ||
                treeView_keywords.SelectedNode.Parent.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillKeywordsGrid(_metaDictionary, treeView_keywords.SelectedNode.Parent);

            var searchParam = new SearchItem
            {
                Version = comboBox_KwVersions.SelectedItem.ToString(),
                CaseSensitive = StringComparison.Ordinal,
                Value = "\"" + treeView_keywords.SelectedNode.Text + "\":",
                Condition = SearchCondition.Contains
            };
            await FilterKeywords(_metaDictionary, searchParam);
            ActivateUiControls(true);
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

        private void CheckBox_loadDbOnStart_CheckedChanged(object sender, EventArgs e)
        {
            _loadDbOnStart = checkBox_loadDbOnStart.Checked;
        }

        private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex != 2) return;

            if (treeView_keywords.Nodes.Count <= 0)
                e.Cancel = true;
            else
                treeView_keywords.Nodes[0].Expand();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.LastRootFolder = folderBrowserDialog1.SelectedPath;
            Settings.Default.ReformatJson = _reformatJson;
            Settings.Default.CollectAllFileNames = _collectAllFileNames;
            Settings.Default.IgnoreHttpsError = _ignoreHttpsError;
            Settings.Default.ShowPreview = _showPreview;
            Settings.Default.AlwaysOnTop = _alwaysOnTop;
            Settings.Default.LoadDbOnStartUp = _loadDbOnStart;

            Settings.Default.Save();
        }

        #endregion

        #region Helpers

        private async Task<bool> LoadDb(string fileName)
        {
            try
            {
                if (!File.Exists(fileName)
                    || !File.Exists(fileName + ".tree")) return false;
            }
            catch
            {
            }

            Text = FormCaption;
            if (string.IsNullOrEmpty(fileName)) return false;

            treeView_keywords.Nodes.Clear();
            ActivateUiControls(false, false);
            toolStripStatusLabel1.Text = "Loading database...";
            var rootNodeExamples = new TreeNode();
            try
            {
                var t = Task.Run(() =>
                    {
                        _metaDictionary = LoadBinary<List<JsoncDictionary>>(fileName);
                        rootNodeExamples = LoadBinary<TreeNode>(fileName + ".tree");
                    }
                );
                await Task.WhenAll(t).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("File read exception [" + fileName + "]: " + ex.Message);
                toolStripStatusLabel1.Text = "Failed to load database";
            }

            ActivateUiControls(true, false);

            if (_metaDictionary != null && rootNodeExamples != null)
            {
                tabControl1.TabPages[1].Enabled = true;
                Text = FormCaption + " " + ShortFileName(fileName);
                treeView_examples.Nodes.Clear();
                treeView_examples.Nodes.Add(rootNodeExamples);
                treeView_examples.Sort();
                treeView_examples.Nodes[0].Expand();
                tabControl1.SelectedTab = tabControl1.TabPages[1];

                if (!File.Exists(fileName + ".keywords"))
                {
                    toolStripStatusLabel1.Text = "Failed to load keywords";
                    return true;
                }

                toolStripStatusLabel1.Text = "Loading keywords database...";

                var rootNodeKeywords = new TreeNode("Loading...");
                try
                {
                    var t = Task.Run(() =>
                    {
                        rootNodeKeywords = LoadBinary<TreeNode>(fileName + ".keywords");
                    });
                    await Task.WhenAll(t).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File read exception [" + fileName + "]: " + ex.Message);
                    toolStripStatusLabel1.Text = "Failed to load keywords";
                }

                treeView_keywords.Nodes.Add(rootNodeKeywords);
                treeView_keywords.Sort();
                toolStripStatusLabel1.Text = "";
            }
            else
            {
                return false;
            }

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
                _textLog.AppendLine(Environment.NewLine + fullFileName + " file read exception: " +
                                    Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                return;
            }

            var versionIndex = jsonText.IndexOf(SchemaTag, StringComparison.Ordinal);
            if (versionIndex <= 0)
            {
                _textLog.AppendLine(Environment.NewLine + fullFileName + " schema not found" + Environment.NewLine);
                return;
            }

            versionIndex += SchemaTag.Length;
            var strEnd = versionIndex;
            while (strEnd < jsonText.Length && jsonText[strEnd] != '"' && jsonText[strEnd] != '\r' &&
                   jsonText[strEnd] != '\n') strEnd++;

            var schemaUrl = jsonText.Substring(versionIndex, strEnd - versionIndex).Trim();

            if (!schemaUrl.EndsWith(".json"))
            {
                _textLog.AppendLine(Environment.NewLine + fullFileName + " URL not found [" + schemaUrl + "]" +
                                    Environment.NewLine);
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
                    _textLog.AppendLine(Environment.NewLine + fullFileName + " schema download exception: [" +
                                        schemaUrl + "]:" + Environment.NewLine + ExceptionPrint(ex) +
                                        Environment.NewLine);
                }

                schemaList.Add(schemaUrl, schemaData);
            }

            var schemaText = schemaList[schemaUrl];

            if (string.IsNullOrEmpty(schemaText))
            {
                _textLog.AppendLine(Environment.NewLine + fullFileName + " schema not loaded [" + schemaUrl + "]:" +
                                    Environment.NewLine);
                return;
            }

            JsonSchema schema;
            try
            {
                schema = JsonSchema.FromJsonAsync(schemaText).Result;
            }
            catch (Exception ex)
            {
                _textLog.AppendLine(Environment.NewLine + fullFileName + " schema parse exception [" + schemaUrl +
                                    "]:" + Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
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
                _textLog.AppendLine(Environment.NewLine + fullFileName + " file validation exception :" +
                                    Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
                return;
            }

            foreach (var error in errors)
            {
                var errorText = PrintError(fullFileName, error);
                if (_suppressErrors.Contains(error.Kind))
                    File.AppendAllText(LogFileName, errorText);
                else
                    _textLog.Append(errorText);
            }
        }

        private static string PrintError(string fullFileName, ValidationError error)
        {
            var errorText = new StringBuilder();
            errorText.AppendLine(fullFileName + ": line #" + error.LineNumber + " " + error.Kind + ", path=" +
                                 error.Path);

            if (error is ChildSchemaValidationError subErrorCollection)
                foreach (var subError in subErrorCollection.Errors)
                    foreach (var subErrorItem in subError.Value)
                        errorText.AppendLine("\t" + "- line #" + subErrorItem.LineNumber + " " + subErrorItem.Kind +
                                             ", path=" + subErrorItem.Path);

            return errorText.ToString();
        }

        private static string ExceptionPrint(Exception ex)
        {
            var exceptionMessage = new StringBuilder();

            exceptionMessage.AppendLine(ex.Message);
            if (ex.InnerException != null) exceptionMessage.AppendLine(ExceptionPrint(ex.InnerException));

            return exceptionMessage.ToString();
        }

        private void DeserializeFile(string fullFileName, string shortFileName, List<JsoncDictionary> rootCollection,
            TreeNode parentNode)
        {
            try
            {
                _fileName = fullFileName;
                if (!JsoncDictionary.FileNames.TryGetValue(shortFileName, out var fileType)) return;


                TreeNode fileNode;

                if (rootCollection.Any(n => n.Type == fileType))
                {
                    fileNode = parentNode.Nodes[parentNode.Nodes.Count - 1];
                }
                else
                {
                    rootCollection.Add(new JsoncDictionary(fileType, _collectAllFileNames));
                    fileNode = new TreeNode(shortFileName)
                    {
                        Name = shortFileName
                    };
                    parentNode.Nodes.Add(fileNode);
                }

                var jsonSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None
                };

                var jsonStr = File.ReadAllText(fullFileName);
                dynamic jsonObject = JsonConvert.DeserializeObject(jsonStr, jsonSettings);
                if (jsonObject != null && jsonObject is JToken)
                    ParseJsonObject(jsonObject, 0, shortFileName, rootCollection, fileNode, fileType);
            }
            catch (Exception ex)
            {
                _textLog.AppendLine(Environment.NewLine + _fileName + " file parse exception: " + Environment.NewLine +
                                    ExceptionPrint(ex) + Environment.NewLine);
            }
        }

        private void ParseJsonObject(JToken token, int depth, string parent, List<JsoncDictionary> rootCollection,
            TreeNode parentNode, JsoncContentType fileType)
        {
            if (token == null) return;
            if (rootCollection == null) rootCollection = new List<JsoncDictionary>();
            if (parentNode == null) parentNode = new TreeNode(RootNodeName);

            JsoncDictionary newItem;

            try
            {
                var obj = rootCollection.Where(n => n.Type == fileType).ToArray();
                if (obj.Length > 1)
                    _textLog.AppendLine(Environment.NewLine + "More than 1 similar file types found on parse" +
                                        Environment.NewLine);
                newItem = obj.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _textLog.AppendLine(Environment.NewLine + _fileName + " content parse exception: " +
                                    Environment.NewLine + ExceptionPrint(ex) + Environment.NewLine);
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
                            printValue = jValue.GetType().Name;
                            saveValue = jValue.ToString();
                        }

                        if (!string.IsNullOrEmpty(saveValue))
                        {
                            saveValue = CompactJson(saveValue);
                            var propName = jProperty.Name;
                            var obj = parentNode.Nodes.Cast<TreeNode>().Where(r => r.Text == propName).ToArray();
                            if (obj.Length > 1)
                                _textLog.AppendLine(Environment.NewLine
                                                    + "More than 1 similar object found in the tree:"
                                                    + Environment.NewLine
                                                    + obj.Select(n => n.FullPath).Aggregate("",
                                                        (current, next) => current + ", " + next)
                                                    + Environment.NewLine);
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
                                    Name = propName
                                };
                                parentNode.Nodes.Add(childNode);
                            }

                            if (propName == VersionTagName) _version = printValue;
                            var nodeType = JsoncNodeType.Property;
                            var jPropType = jProperty.Children().FirstOrDefault()?.Type;
                            if (jPropType == JTokenType.Object)
                                nodeType = JsoncNodeType.Object;
                            else if (jPropType == JTokenType.Array)
                                nodeType = JsoncNodeType.Array;

                            var node = new MetaNode(propName, parent, nodeType, depth, saveValue, _fileName, _version);
                            var errorString = newItem?.Add(node);
                            if (!string.IsNullOrEmpty(errorString))
                                _textLog.AppendLine(Environment.NewLine + _fileName
                                                                        + " node add error: "
                                                                        + Environment.NewLine
                                                                        + " Node ["
                                                                        + propName
                                                                        + "] "
                                                                        + errorString
                                                                        + Environment.NewLine);

                            foreach (var child in jProperty.Children())
                                if (child is JArray || child is JObject || child is JProperty)
                                    ParseJsonObject(child, depth + 1, jProperty.Path, rootCollection, childNode, fileType);
                        }

                        break;
                    }
                case JObject jObject:
                    {
                        string newParent;

                        if (!string.IsNullOrEmpty(jObject.Path))
                        {
                            newParent = jObject.Path.EndsWith("]")
                                ? jObject.Path.Substring(0, jObject.Path.LastIndexOf('['))
                                : jObject.Path;
                            if (newParent.Contains(".")) newParent = newParent.Substring(newParent.LastIndexOf('.') + 1);
                        }
                        else
                        {
                            newParent = parent;
                        }

                        foreach (var child in jObject.Children())
                            ParseJsonObject(child, depth, newParent, rootCollection, parentNode, fileType);
                        break;
                    }
                case JArray jArray:
                    {
                        string newParent;

                        if (!string.IsNullOrEmpty(jArray.Path))
                        {
                            newParent = jArray.Path.EndsWith("]")
                                ? jArray.Path.Substring(0, jArray.Path.LastIndexOf('['))
                                : jArray.Path;
                            if (newParent.Contains(".")) newParent = newParent.Substring(newParent.LastIndexOf('.') + 1);
                        }
                        else
                        {
                            newParent = parent;
                        }

                        foreach (var child in jArray.Children())
                            ParseJsonObject(child, depth, newParent, rootCollection, parentNode, fileType);
                        break;
                    }
                default:
                    {
                        if (token.Children().Any())
                            _textLog.AppendLine(Environment.NewLine + _fileName
                                                                    + " Node missed: ["
                                                                    + token.Path
                                                                    + "] of type \""
                                                                    + token.Type
                                                                    + "\""
                                                                    + Environment.NewLine);
                    }
                    break;
            }
        }

        private async Task CollectKeywords(List<JsoncDictionary> sourceCollection, TreeNode parentNode)
        {
            await Task.Run(() =>
            {
                // assuming 1st level nodes are file types/names
                foreach (var currentCollection in sourceCollection)
                {
                    var fileName = JsoncDictionary.FileNames.FirstOrDefault(s => s.Value == currentCollection.Type).Key;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        _textLog.AppendLine(Environment.NewLine + currentCollection.Type +
                                            " file type not found in collection" + Environment.NewLine);
                        continue;
                    }

                    Invoke((MethodInvoker)delegate
                   {
                       toolStripStatusLabel1.Text =
                           "Collecting keywords from \"" + fileName + "\" files from " + currentCollection.Type +
                           " files (" + currentCollection.Nodes.Count + " nodes)";
                   });

                    // select or create node for current file type
                    var currentFileNode = parentNode.Nodes.Find(fileName, true);
                    if (currentFileNode == null || !currentFileNode.Any())
                    {
                        var fileNode = new TreeNode(fileName)
                        {
                            Name = fileName
                        };
                        parentNode.Nodes.Add(fileNode);
                        currentFileNode = parentNode.Nodes.Find(fileName, false);
                    }

                    if (currentFileNode == null)
                    {
                        _textLog.AppendLine(Environment.NewLine + currentCollection.Type +
                                            " error finding/creating node for file type \"" + fileName + "\"" +
                                            Environment.NewLine);
                        continue;
                    }

                    if (currentFileNode.Length > 1)
                        _textLog.AppendLine(Environment.NewLine + currentCollection.Type +
                                            " parent node has duplicate file type records" + Environment.NewLine);

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
                                    Name = pName
                                };
                                currentFileNode[0].Nodes.Add(newNode);
                                parNodeList = currentFileNode[0].Nodes.Find(pName, false);
                            }
                        }

                        if (!parNodeList.Any())
                        {
                            _textLog.AppendLine(Environment.NewLine + currentCollection.Type +
                                                " error finding/creating parent node for record \"" +
                                                record.ParentName + "." + record.Name + "\"" + Environment.NewLine);
                            continue;
                        }

                        // very often
                        if (parNodeList.Length > 1)
                        {
                            // try to find proper parent to attach value
                            // get the parent name of current parent
                        }

                        var childNodesList = parNodeList[0].Nodes.Find(cName, false);
                        if (childNodesList == null || !childNodesList.Any())
                        {
                            var newNode = new TreeNode(cName)
                            {
                                Name = cName
                            };
                            parNodeList[0].Nodes.Add(newNode);
                            childNodesList = parNodeList[0].Nodes.Find(cName, false);
                        }

                        if (childNodesList.Length > 1)
                        {
                            _textLog.AppendLine(Environment.NewLine + currentCollection.Type +
                                                " error finding/creating child node for record \"" + record.ParentName +
                                                "." + record.Name + "\"" + Environment.NewLine);
                            continue;
                        }

                        // array/object value is always a complex object
                        if (record.Type != JsoncNodeType.Property) continue;

                        foreach (var prop in record.ExamplesList)
                        {
                            var propValue = prop.Key;

                            var propNodesList = childNodesList[0].Nodes.Find(propValue, false);
                            if (propNodesList == null || !propNodesList.Any())
                            {
                                var newNode = new TreeNode(propValue)
                                {
                                    Name = propValue
                                };
                                childNodesList[0].Nodes.Add(newNode);
                            }
                        }
                    }
                }
            }).ConfigureAwait(true);
            toolStripStatusLabel1.Text = "";
        }

        private void FillExamplesGrid(List<JsoncDictionary> rootCollection, TreeNode currentNode,
            SearchItem searchParam = null)
        {
            if (currentNode == null) return;

            var tokens = currentNode.FullPath.Split('\\');

            if (tokens.Length < 3) return;

            if (!JsoncDictionary.FileNames.TryGetValue(tokens[1], out var fileType))
            {
                _textLog.AppendLine(Environment.NewLine
                                    + "Unknown file type in the node: "
                                    + tokens[1]
                                    + Environment.NewLine);
                return;
            }

            var currentTypeNodeCollection = rootCollection?.Where(x => x.Type == fileType).ToArray();

            if (currentTypeNodeCollection == null) return;

            if (currentTypeNodeCollection.Length > 1)
                _textLog.AppendLine(Environment.NewLine
                                    + "More than 1 similar file types found on example print-out:"
                                    + Environment.NewLine
                                    + currentTypeNodeCollection.Select(n => n.Nodes)
                                        .Aggregate("", (current, next) => current + ", " + next)
                                    + Environment.NewLine);
            var currentTypeNode = currentTypeNodeCollection.FirstOrDefault();

            var selectedExamples = currentTypeNode?.Nodes?.Where(n =>
                n.Name == currentNode.Text
                && n.Depth == tokens.Length - 3
                && n.ParentName == currentNode.Parent.Text);

            if (selectedExamples == null) return;

            var metaNodes = selectedExamples as MetaNode[] ?? selectedExamples.ToArray();
            toolStripStatusLabel1.Text = "Displaying " + metaNodes.Length + " records";

            if (_lastExSelectedNode != null)
                _lastExSelectedNode.NodeFont = new Font(treeView_examples.Font.FontFamily, treeView_examples.Font.Size,
                    FontStyle.Regular);

            _lastExSelectedNode = currentNode;
            currentNode.NodeFont =
                new Font(treeView_examples.Font.FontFamily, treeView_examples.Font.Size, FontStyle.Bold);

            ExClearSearch();

            _examplesTable.Rows.Clear();
            dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            comboBox_ExVersions.Items.Clear();
            comboBox_ExVersions.Items.Add(DefaultVersionCaption);
            comboBox_ExVersions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in metaNodes)
            {
                var examples = record?.ExamplesList;
                if (examples == null) continue;

                if (!versionCollection.Contains(record?.Version)) versionCollection.Add(record?.Version);
                if (searchParam != null && searchParam.Version != DefaultVersionCaption &&
                    record?.Version != searchParam.Version) continue;

                foreach (var example in examples)
                {
                    var exampleText = BeautifyJson(example.Key, _reformatJson);
                    if ((searchParam == null || string.IsNullOrEmpty(searchParam.Value)) &&
                        FilterCellOut(exampleText, searchParam, true)) continue;
                    var newRow = _examplesTable.NewRow();
                    newRow[_exampleGridColumnsNames[0]] = record?.Version;
                    newRow[_exampleGridColumnsNames[1]] = exampleText;
                    newRow[_exampleGridColumnsNames[2]] = example.Value;
                    _examplesTable.Rows.Add(newRow);
                }
            }

            if (searchParam == null) searchParam = new SearchItem();
            if (!_lastExSearchList.Contains(searchParam)) _lastExSearchList.Add(searchParam);

            SetSearchText(textBox_ExSearchHistory, _lastKwSearchList);

            comboBox_ExVersions.Items.AddRange(versionCollection.ToArray());
            toolStripStatusLabel1.Text = "";
        }

        private void FillKeywordsGrid(List<JsoncDictionary> rootCollection, TreeNode currentNode,
            SearchItem searchParam = null)
        {
            if (currentNode == null) return;

            var tokens = currentNode.FullPath.Split('\\');

            if (tokens.Length < 3) return;

            if (!JsoncDictionary.FileNames.TryGetValue(tokens[1], out var fileType))
            {
                _textLog.AppendLine(Environment.NewLine
                                    + "Unknown file type in the node: "
                                    + tokens[1]
                                    + Environment.NewLine);
                return;
            }

            var currentTypeNodeCollection = rootCollection?.Where(x => x.Type == fileType).ToArray();

            if (currentTypeNodeCollection == null) return;

            if (currentTypeNodeCollection.Length > 1)
                _textLog.AppendLine(Environment.NewLine
                                    + "More than 1 similar file types found on example print-out:"
                                    + Environment.NewLine
                                    + currentTypeNodeCollection.Select(n => n.Nodes)
                                        .Aggregate("", (current, next) => current + ", " + next)
                                    + Environment.NewLine);
            var currentTypeNode = currentTypeNodeCollection.FirstOrDefault();

            var cName = currentNode.Text;
            var pName = currentNode.Parent.Text;
            if (cName == "actions" && pName == "events")
                cName = "action";

            var selectedExamples = currentTypeNode?.Nodes?.Where(n =>
                n.Name == cName
                && n.ParentName == pName).ToList();

            if (selectedExamples == null) return;

            toolStripStatusLabel1.Text = "Displaying " + selectedExamples.Count + " records";

            if (_lastKwSelectedNode != null)
                _lastKwSelectedNode.NodeFont = new Font(treeView_keywords.Font.FontFamily, treeView_keywords.Font.Size,
                    FontStyle.Regular);

            _lastKwSelectedNode = currentNode;
            currentNode.NodeFont =
                new Font(treeView_keywords.Font.FontFamily, treeView_keywords.Font.Size, FontStyle.Bold);

            KwClearSearch();

            _keywordsTable.Rows.Clear();
            dataGridView_keywords.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            comboBox_KwVersions.Items.Clear();
            comboBox_KwVersions.Items.Add(DefaultVersionCaption);
            comboBox_KwVersions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in selectedExamples)
            {
                var examples = record.ExamplesList;
                if (examples == null) continue;

                if (!versionCollection.Contains(record.Version)) versionCollection.Add(record.Version);
                if (searchParam != null && searchParam.Version != DefaultVersionCaption &&
                    record.Version != searchParam.Version) continue;

                foreach (var example in examples)
                {
                    var exampleText = BeautifyJson(example.Key, _reformatJson);
                    if ((searchParam == null || string.IsNullOrEmpty(searchParam.Value)) &&
                        FilterCellOut(exampleText, searchParam, true)) continue;

                    // remove duplicate records
                    var found = _keywordsTable.Rows.OfType<DataRow>().Any(row =>
                        row[0].ToString() == record.Version && row[1].ToString() == exampleText);
                    foreach (var row in _keywordsTable.Rows)
                    {
                        var items = ((DataRow)row).ItemArray;
                        if (items[0].ToString() == record.Version &&
                            items[1].ToString() == exampleText)
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

            if (searchParam == null) searchParam = new SearchItem();
            if (!_lastKwSearchList.Contains(searchParam)) _lastKwSearchList.Add(searchParam);

            SetSearchText(textBox_KwSearchHistory, _lastKwSearchList);

            comboBox_KwVersions.Items.AddRange(versionCollection.ToArray());
            toolStripStatusLabel1.Text = "";
        }

        private async Task FilterExamples(List<JsoncDictionary> rootCollection, SearchItem searchParam,
            bool compactJson = false)
        {
            if (_lastExSearchList.Contains(searchParam)) return;

            _lastExSearchList.Add(searchParam);

            if (searchParam == null || string.IsNullOrEmpty(searchParam.Value))
            {
                FillExamplesGrid(rootCollection, _lastExSelectedNode, searchParam);
                return;
            }

            var rows = _examplesTable.Rows;
            var rowsNumber = rows.Count;
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var cellValue = rows[i].ItemArray[1];
                    if (cellValue == null || FilterCellOut(cellValue.ToString(), searchParam, compactJson))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
            }).ConfigureAwait(true);

            SetSearchText(textBox_ExSearchHistory, _lastExSearchList);
            toolStripStatusLabel1.Text = "";
        }

        private async Task FilterKeywords(List<JsoncDictionary> rootCollection, SearchItem searchParam,
            bool compactJson = false)
        {
            if (_lastKwSearchList.Contains(searchParam)) return;

            _lastKwSearchList.Add(searchParam);

            if (string.IsNullOrEmpty(searchParam.Value))
            {
                FillKeywordsGrid(rootCollection, _lastKwSelectedNode, searchParam);
                return;
            }

            var rows = _keywordsTable.Rows;
            var rowsNumber = rows.Count;
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var cellValue = rows[i].ItemArray[1];
                    if (cellValue == null || FilterCellOut(cellValue.ToString(), searchParam, compactJson))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
            }).ConfigureAwait(true);

            SetSearchText(textBox_KwSearchHistory, _lastKwSearchList);
            toolStripStatusLabel1.Text = "";
        }

        private async Task FilterExamplesVersion(List<JsoncDictionary> rootCollection, SearchItem searchParam)
        {
            if (_lastExSearchList == null) _lastExSearchList = new List<SearchItem>();

            if (!_lastExSearchList.Any()) _lastExSearchList.Add(new SearchItem());
            var lastSearch = _lastExSearchList.Last();
            if (lastSearch.Version != DefaultVersionCaption)
                FillExamplesGrid(rootCollection, _lastExSelectedNode, searchParam);

            if (comboBox_ExVersions.Items.Contains(searchParam.Version))
            {
                comboBox_ExVersions.SelectedItem = searchParam.Version;
                lastSearch.Version = searchParam.Version;
            }
            else
            {
                comboBox_ExVersions.SelectedItem = DefaultVersionCaption;
                return;
            }

            lastSearch.Version = searchParam.Version;
            var rows = _examplesTable.Rows;
            var rowsNumber = rows.Count;
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var cellValue = rows[i].ItemArray[0];
                    if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()) ||
                        cellValue.ToString() != searchParam.Version)
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
            }).ConfigureAwait(true);

            SetSearchText(textBox_ExSearchHistory, _lastExSearchList);
            toolStripStatusLabel1.Text = "";
        }

        private async Task FilterKeywordsVersion(List<JsoncDictionary> rootCollection, SearchItem searchParam)
        {
            if (_lastKwSearchList == null) _lastKwSearchList = new List<SearchItem>();
            if (!_lastKwSearchList.Any()) _lastKwSearchList.Add(new SearchItem());
            var lastSearch = _lastKwSearchList.Last();
            if (lastSearch.Version != DefaultVersionCaption)
                FillExamplesGrid(rootCollection, _lastKwSelectedNode, searchParam);

            if (comboBox_KwVersions.Items.Contains(searchParam.Version))
            {
                comboBox_KwVersions.SelectedItem = searchParam.Version;
                lastSearch.Version = searchParam.Version;
            }
            else
            {
                comboBox_KwVersions.SelectedItem = DefaultVersionCaption;
                return;
            }

            lastSearch.Version = searchParam.Version;
            var rows = _keywordsTable.Rows;
            var rowsNumber = rows.Count;
            toolStripStatusLabel1.Text = "Filtering " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var cellValue = rows[i].ItemArray[0];
                    if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                    else if (cellValue.ToString() != searchParam.Version)
                    {
                        rows.RemoveAt(i);
                        i--;
                    }
                }
            }).ConfigureAwait(true);

            /*var searchParamsList = new List<SearchItem>();
            foreach (var item in _lastKwSearchList)
            {
                searchParamsList.Add((SearchItem)item.Clone());
            }

            _lastKwSearchList.Clear();
            foreach (var searchParam in searchParamsList)
            {
                await FilterExamples(rootCollection, searchParam, versionString);
            }*/

            SetSearchText(textBox_KwSearchHistory, _lastKwSearchList);
            toolStripStatusLabel1.Text = "";
        }

        private void SetSearchText(TextBox textBox, List<SearchItem> searchList)
        {
            var searchString = new StringBuilder();
            foreach (var lastSearch in searchList)
            {
                if (lastSearch == null || string.IsNullOrEmpty(lastSearch.Value)) continue;

                var cs = lastSearch.CaseSensitive == StringComparison.Ordinal ? "'CS'" : "";
                searchString.Append(searchString.Length <= 0 ? "[" : " -> [");
                searchString.Append("v:\"" + lastSearch.Version + "\";");
                searchString.Append(lastSearch.Condition + cs + ":\"" + lastSearch.Value + "\"]");
            }

            Invoke((MethodInvoker)delegate { textBox.Text = searchString.ToString(); });
        }

        private void ExClearSearch()
        {
            _lastExSearchList.Clear();
            Invoke((MethodInvoker)delegate { textBox_ExSearchHistory.Clear(); });
        }

        private void KwClearSearch()
        {
            _lastKwSearchList.Clear();
            Invoke((MethodInvoker)delegate { textBox_KwSearchHistory.Clear(); });
        }

        private async Task ReadjustRows(DataGridView dgView)
        {
            var rowsNumber = dgView.RowCount;
            toolStripStatusLabel1.Text = "Adjusting height for " + rowsNumber + " rows";

            await Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
               {
                   if (dgView.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
                   {
                       LoopThroughRows(dgView);
                       dgView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                   }

                   for (var rowNumber = 0; rowNumber < dgView.RowCount; rowNumber++)
                   {
                       var row = dgView.Rows[rowNumber];
                       row.HeaderCell.Value = (rowNumber + 1).ToString();
                       var newHeight = row.GetPreferredHeight(rowNumber,
                           DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);
                       var currentHeight = dgView.Height;
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
            row.HeaderCell.Value = (rowNumber + 1).ToString();
            var newHeight = row.GetPreferredHeight(rowNumber, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);
            var currentHeight = dgView.Height;
            if (newHeight == row.Height && newHeight <= currentHeight * CellHeightAdjust) return;

            if (newHeight > currentHeight * CellHeightAdjust) newHeight = (ushort)(currentHeight * CellHeightAdjust);
            row.Height = newHeight;
        }

        private void ActivateUiControls(bool active, bool processTable = true)
        {
            if (!active)
            {
                comboBox_ExVersions.SelectedIndexChanged -= ComboBox_ExVersions_SelectedIndexChanged;
                comboBox_KwVersions.SelectedIndexChanged -= ComboBox_KwVersions_SelectedIndexChanged;
            }

            if (active)
            {
                textBox_logText.Text += _textLog.ToString();
                _textLog.Clear();
                textBox_logText.SelectionStart = textBox_logText.Text.Length;
                textBox_logText.ScrollToCaret();
            }

            dataGridView_examples.Enabled = active;
            dataGridView_keywords.Enabled = active;

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

            checkedListBox_params.Enabled = active;
            button_validateFiles.Enabled = active;
            button_collectDatabase.Enabled = active;
            button_loadDb.Enabled = active;
            button_saveDb.Enabled = active;

            comboBox_ExVersions.Enabled = active;
            comboBox_ExCondition.Enabled = active;
            textBox_ExSearchString.Enabled = active;
            checkBox_ExCaseSensitive.Enabled = active;
            dataGridView_examples.Enabled = active;
            treeView_examples.Enabled = active;
            button_ExAdjustRows.Enabled = active;

            comboBox_KwVersions.Enabled = active;
            comboBox_KwCondition.Enabled = active;
            textBox_KwSearchString.Enabled = active;
            checkBox_KwCaseSensitive.Enabled = active;
            dataGridView_keywords.Enabled = active;
            treeView_keywords.Enabled = active;
            button_KwAdjustRows.Enabled = active;

            tabControl1.Enabled = active;

            if (active)
            {
                comboBox_ExVersions.SelectedIndexChanged += ComboBox_ExVersions_SelectedIndexChanged;
                comboBox_KwVersions.SelectedIndexChanged += ComboBox_KwVersions_SelectedIndexChanged;
            }

            Refresh();
        }

        #endregion

        #region Utilities

        // this is to get rid of exception on "dataGridView_examples.AutoSizeRowsMode" change
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static void LoopThroughRows(DataGridView dgv)
        {
            var rows = dgv.Rows;
            var rowsCount = rows.Count;
            for (var i = rowsCount - 1; i >= 0; i--)
            {
                var row = rows[i];
            }
        }

        private static bool FilterCellOut(string cellValue, SearchItem searchParam, bool compactJson)
        {
            if (cellValue == null || string.IsNullOrEmpty(cellValue)) return true;

            if (searchParam == null || string.IsNullOrEmpty(searchParam.Value)) return false;

            var newCellValue = compactJson
                ? CompactJson(cellValue)
                : cellValue;
            switch (searchParam.Condition)
            {
                case SearchCondition.Contains:
                    if (newCellValue.IndexOf(searchParam.Value, searchParam.CaseSensitive) < 0)
                        return true;
                    break;
                case SearchCondition.StartsWith:
                    if (!newCellValue.StartsWith(searchParam.Value, searchParam.CaseSensitive))
                        return true;
                    break;
                case SearchCondition.EndsWith:
                    if (!newCellValue.EndsWith(searchParam.Value, searchParam.CaseSensitive))
                        return true;
                    break;
            }

            return false;
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
                using (var webClient = new WebClient())
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
