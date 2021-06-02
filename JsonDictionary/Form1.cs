// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JsonDictionary.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static JsonDictionary.JsonIo;
using static JsonDictionary.SearchItem;

namespace JsonDictionary
{
    public partial class Form1 : Form
    {
        private readonly List<ContentTypeItem> _fileTypes = new List<ContentTypeItem>
        {
            new ContentTypeItem
            {
                FileTypeMask = "dataviews.jsonc",
                PropertyTypeName = "dataviews",
                FileType = JsoncContentType.DataViews
            },
            new ContentTypeItem
            {
                FileTypeMask = "events.jsonc",
                PropertyTypeName = "events",
                FileType = JsoncContentType.Events
            },
            new ContentTypeItem
            {
                FileTypeMask = "layout.jsonc",
                PropertyTypeName = "layout",
                FileType = JsoncContentType.Layout
            },
            new ContentTypeItem
            {
                FileTypeMask = "rules.jsonc",
                PropertyTypeName = "rules",
                FileType = JsoncContentType.Rules
            },
            new ContentTypeItem
            {
                FileTypeMask = "search.jsonc",
                PropertyTypeName = "search",
                FileType = JsoncContentType.Search
            },
            new ContentTypeItem
            {
                FileTypeMask = "combo.jsonc",
                PropertyTypeName = "combo",
                FileType = JsoncContentType.Combo
            },
            new ContentTypeItem
            {
                FileTypeMask = "tools.jsonc",
                PropertyTypeName = "tools",
                FileType = JsoncContentType.Tools
            },
            new ContentTypeItem
            {
                FileTypeMask = "strings.jsonc",
                PropertyTypeName = "strings",
                FileType = JsoncContentType.Strings
            },
            new ContentTypeItem
            {
                FileTypeMask = "patch.jsonc",
                PropertyTypeName = "patch",
                FileType = JsoncContentType.Patch
            }
        };

        // pre-defined constants
        private string _fileMask = "*.jsonc";
        private readonly string[] _exampleGridColumnsNames = { "Version", "Example", "File Name", "Json Path", "Line#" };

        private const string DefaultVersionCaption = "Any";
        private const string VersionTagName = "contentVersion";
        private const string ImportTagName = "imports";
        private const char Delimiter = ';';
        private const string RootNodeName = "Kinetic";
        private const float CellSizeAdjust = 0.7f;
        private const string PreViewCaption = "[Preview] ";
        private const string DefaultFormCaption = "JsonDictionary";
        private string DefaultDescriptionFileName = "descriptions.json";

        // behavior options
        private static bool _reformatJson;
        private bool _showPreview;
        private bool _alwaysOnTop;
        private bool _loadDbOnStart;
        private bool _useVsCode;

        // global variables
        private readonly StringBuilder _textLog = new StringBuilder();

        // global data storage
        private readonly DataTable _examplesTable;
        private TreeNode _rootNodeExamples = new TreeNode();
        private Dictionary<string, List<JsonProperty>> _exampleLinkCollection = new Dictionary<string, List<JsonProperty>>();
        private volatile bool _isDoubleClick;

        // last used values for UI processing optimization
        private TreeNode _lastExSelectedNode;
        private List<SearchItem> _lastExSearchList = new List<SearchItem>();

        private JsonViewer _sideViewer;

        private struct WinPosition
        {
            public int WinX;
            public int WinY;
            public int WinW;
            public int WinH;

            public bool Initialized
            {
                get => !(WinX == 0 && WinY == 0 && WinW == 0 && WinH == 0);
            }
        }

        private WinPosition _editorPosition = new WinPosition();

        private Dictionary<string, string> _nodeDescription = new Dictionary<string, string>();

        private struct ProcessingOptions
        {
            public JsoncContentType ContentType;
            public string ItemName;
            public string[] ParentNames;
        }

        private readonly ProcessingOptions[] _flattenParameters = new ProcessingOptions[]
        {
            new ProcessingOptions
            {
                ContentType = JsoncContentType.Events,
                ItemName = "type",
                ParentNames = new string[]
                {
                    "actions",
                    "onsuccess",
                    "onfailure",
                    "onerror",
                    "onsinglematch",
                    "onmultiplematch",
                    "onnomatch",
                    "onyes",
                    "onno",
                    "onok",
                    "oncancel",
                    "onabort",
                    "onempty"
                }
            },
            new ProcessingOptions
            {
                ContentType = JsoncContentType.Layout,
                ItemName = "sourcetypeid",
                ParentNames = new string[] { "components" }
            },
            new ProcessingOptions
            {
                ContentType = JsoncContentType.Rules,
                ItemName = "action",
                ParentNames = new string[] { "actions" }
            },
            new ProcessingOptions
            {
                ContentType = JsoncContentType.Search,
                ItemName = "sourcetypeid",
                ParentNames = new string[] { "component" }
            }
        };

        private string FormCaption
        {
            get => base.Text;
            set => base.Text = value;
        }

        #region GUI

        public Form1()
        {
            InitializeComponent();

            FormCaption = DefaultFormCaption;

            checkBox_reformatJson.Checked = _reformatJson = Settings.Default.ReformatJson;
            checkBox_showPreview.Checked = _showPreview = Settings.Default.ShowPreview;
            checkBox_alwaysOnTop.Checked = _alwaysOnTop = Settings.Default.AlwaysOnTop;
            checkBox_loadDbOnStart.Checked = _loadDbOnStart = Settings.Default.LoadDbOnStartUp;
            checkBox_vsCode.Checked = _useVsCode = Settings.Default.UseVsCode;
            folderBrowserDialog1.SelectedPath = Settings.Default.LastRootFolder;
            _fileMask = Settings.Default.FileMask;
            DefaultDescriptionFileName = Settings.Default.DefaultDescriptionFileName;
            _nodeDescription = LoadJson<Dictionary<string, string>>(DefaultDescriptionFileName);
            if (_nodeDescription == null)
                _nodeDescription = new Dictionary<string, string>();

            _editorPosition = new WinPosition()
            {
                WinX = Settings.Default.EditorPositionX,
                WinY = Settings.Default.EditorPositionY,
                WinW = Settings.Default.EditorWidth,
                WinH = Settings.Default.EditorHeight,
            };

            var _mainFormPosition = new WinPosition()
            {
                WinX = Settings.Default.MainWindowPositionX,
                WinY = Settings.Default.MainWindowPositionY,
                WinW = Settings.Default.MainWindowWidth,
                WinH = Settings.Default.MainWindowHeight,
            };

            if (_mainFormPosition.Initialized)
            {
                this.Location = new Point
                { X = _mainFormPosition.WinX, Y = _mainFormPosition.WinY };
                this.Width = _mainFormPosition.WinW;
                this.Height = _mainFormPosition.WinH;
            }

            splitContainer1.SplitterDistance = Settings.Default.TreeSplitterDistance;
            splitContainer3.SplitterDistance = Settings.Default.DescriptionSplitterDistance;
            splitContainer4.SplitterDistance = Settings.Default.FileListSplitterDistance;

            TopMost = _alwaysOnTop;

            comboBox_ExCondition.Items.AddRange(typeof(SearchCondition).GetEnumNames());
            comboBox_ExCondition.SelectedIndex = 0;

            _examplesTable = new DataTable("Examples");
            for (var i = 0; i < _exampleGridColumnsNames.Length; i++)
            {
                _examplesTable.Columns.Add(_exampleGridColumnsNames[i]);
            }

            dataGridView_examples.DataError += delegate
            { };
            dataGridView_examples.DataSource = _examplesTable;

            comboBox_ExVersions.SelectedIndexChanged -= ComboBox_ExVersions_SelectedIndexChanged;
            comboBox_ExVersions.Items.Clear();
            comboBox_ExVersions.Items.Add(DefaultVersionCaption);
            comboBox_ExVersions.SelectedIndex = 0;
            comboBox_ExVersions.SelectedIndexChanged += ComboBox_ExVersions_SelectedIndexChanged;

            if (_loadDbOnStart)
                LoadDb(Settings.Default.LastDbName).ConfigureAwait(true);
        }

        private void Button_loadDb_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Open KineticScheme data";
            openFileDialog1.DefaultExt = "kineticLib";
            openFileDialog1.Filter = "Binary files|*.kineticLib|All files|*.*";
            openFileDialog1.ShowDialog();
        }

        private async void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            ActivateUiControls(false);
            if (await LoadDb(openFileDialog1.FileName).ConfigureAwait(true))
            {
                FormCaption = DefaultFormCaption + " " + ShortFileName(openFileDialog1.FileName);
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                Settings.Default.LastDbName = openFileDialog1.FileName;
                Settings.Default.Save();
            }

            ActivateUiControls(true);
        }

        private async void Button_collectDatabase_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;

            ActivateUiControls(false);
            treeView_examples.Nodes.Clear();
            _examplesTable.Clear();
            _exampleLinkCollection = new Dictionary<string, List<JsonProperty>>();
            var startPath = folderBrowserDialog1.SelectedPath;
            toolStripStatusLabel1.Text = "Searching files...";
            var startTime = DateTime.Now;
            var startOperationTime = DateTime.Now;
            var endTime = DateTime.Now;

            await Task.Run(() =>
            {
                var jsonPropertiesCollection = RunFileCollection(startPath, _fileMask);
                Invoke((MethodInvoker)delegate
               {
                   endTime = DateTime.Now;
                   _textLog.AppendLine("Collection time: " + endTime.Subtract(startOperationTime).TotalSeconds);
                   startOperationTime = DateTime.Now;
                   FlushLog();
                   toolStripStatusLabel1.Text = "Processing events collection";
               });

                Parallel.ForEach(_flattenParameters, param =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        startOperationTime = DateTime.Now;
                        FlushLog();
                        toolStripStatusLabel1.Text = "Processing " + param.ContentType.ToString() + " collection";
                    });

                    FlattenCollection(jsonPropertiesCollection, param.ContentType, param.ItemName, param.ParentNames);

                    Invoke((MethodInvoker)delegate
                    {
                        endTime = DateTime.Now;
                        _textLog.AppendLine(param.ContentType.ToString() + " processing time: " + endTime.Subtract(startOperationTime).TotalSeconds);
                    });
                });

                _rootNodeExamples = GenerateTreeFromList(jsonPropertiesCollection);
            }).ConfigureAwait(true);

            endTime = DateTime.Now;
            _textLog.AppendLine("Tree generating time: " + endTime.Subtract(startOperationTime).TotalSeconds);
            FlushLog();

            treeView_examples.Nodes.Add(_rootNodeExamples);
            treeView_examples.Sort();
            treeView_examples.Nodes[0].Expand();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            endTime = DateTime.Now;
            _textLog.AppendLine("Total processing time: " + endTime.Subtract(startTime).TotalSeconds);
            FlushLog();

            toolStripStatusLabel1.Text = "";
            ActivateUiControls(true);
        }

        private void Button_saveDb_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save KineticScheme data";
            saveFileDialog1.DefaultExt = "json";
            saveFileDialog1.Filter = "Binary files|*.kineticLib|All files|*.*";
            saveFileDialog1.FileName =
                "KineticDictionary_" + DateTime.Today.ToShortDateString().Replace("/", "_") + ".kineticLib";
            saveFileDialog1.ShowDialog();
        }

        private async void SaveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (!_exampleLinkCollection.Any() || _rootNodeExamples?.Nodes?.Count <= 0)
                return;

            toolStripStatusLabel1.Text = "Saving database...";
            await Task.Run(() =>
             {
                 try
                 {
                     SaveBinary(_exampleLinkCollection, saveFileDialog1.FileName);
                     SaveBinary(_rootNodeExamples, saveFileDialog1.FileName + ".tree");
                     GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                     Settings.Default.LastDbName = saveFileDialog1.FileName;
                     Settings.Default.Save();
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("File write exception [" + saveFileDialog1.FileName + "]: " + ex.Message);
                 }
             }).ContinueWith((t) =>
             {
                 toolStripStatusLabel1.Text = "";
             }).ConfigureAwait(false);
        }

        private async void Button_ExAdjustRows_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false, false);
            await ReadjustRows(dataGridView_examples).ConfigureAwait(true);
            ActivateUiControls(true);
        }

        private async void TextBox_ExSearchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            var searchParam = new SearchItem(comboBox_ExVersions.SelectedItem?.ToString())
            {
                CaseSensitive = checkBox_ExCaseSensitive.Checked
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase,
                Value = textBox_ExSearchString.Text,
                Condition = (SearchCondition)comboBox_ExCondition.SelectedIndex
            };
            if (_lastExSearchList.Contains(searchParam))
                return;

            ActivateUiControls(false);
            await FilterExamples(_exampleLinkCollection, searchParam).ConfigureAwait(true);
            ActivateUiControls(true);
            e.SuppressKeyPress = true;
        }

        private void Button_ExClearSearch_Click(object sender, EventArgs e)
        {
            ExClearSearch();
            FillExamplesGrid(_exampleLinkCollection, treeView_examples.SelectedNode);
        }

        private async void ComboBox_ExVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateUiControls(false);
            var searchParam = new SearchItem(comboBox_ExVersions.SelectedItem.ToString());

            await FilterExamplesVersion(_exampleLinkCollection, searchParam).ConfigureAwait(true);
            //dataGridView_examples.Invalidate();

            ActivateUiControls(true);
        }

        #region Prevent_treenode_collapse

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            _isDoubleClick = e.Clicks > 1;
        }

        private void TreeView_No_Expand_Collapse(object sender, TreeViewCancelEventArgs e)
        {
            if (!_isDoubleClick)
                return;

            _isDoubleClick = false;
            e.Cancel = true;
        }

        #endregion

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (sender is TreeView tree)
                tree.SelectedNode = e.Node;
        }

        private void TreeView_examples_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (treeView_examples.SelectedNode == null
                    || treeView_examples.SelectedNode.Parent.Text == RootNodeName)
                    return;

                ActivateUiControls(false);
                FillExamplesGrid(_exampleLinkCollection, treeView_examples.SelectedNode);
                ///dataGridView_examples.Invalidate();
                ActivateUiControls(true);
                _lastExSearchList.Clear();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (treeView_examples.SelectedNode == null)
                    return;

                if (MessageBox.Show("Are you sure to remove the selected node?", "Remove node", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                ActivateUiControls(false);

                var records = _exampleLinkCollection.Where(n => n.Key.StartsWith(treeView_examples.SelectedNode.Name.TrimEnd('.'))).Select(n => n.Key).ToArray();
                for (var i = 0; i < records.Length; i++)
                {
                    _exampleLinkCollection.Remove(records[i]);
                }

                treeView_examples.Nodes.Remove(treeView_examples.SelectedNode);
                //dataGridView_examples.Invalidate();
                ActivateUiControls(true);
                _lastExSearchList.Clear();
            }
            else if (e.KeyCode == Keys.C && e.Control == true)
            {
                if (treeView_examples.SelectedNode == null)
                    return;

                Clipboard.SetText(treeView_examples.SelectedNode.Text);
            }
        }

        private async void TreeView_examples_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null /*|| e.Node.Parent != null && e.Node.Parent.Text == RootNodeName*/)
                return;

            ActivateUiControls(false);
            if (FillExamplesGrid(_exampleLinkCollection, e.Node))
            {
                _lastExSearchList.Clear();
            }
            ActivateUiControls(true);
            ActivateUiControls(false, false);
            dataGridView_examples.Invalidate();
            await ReadjustRows(dataGridView_examples).ConfigureAwait(true);
            ActivateUiControls(true, false);
        }

        private void DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (sender is DataGridView dataGrid)
            {
                dataGrid.ClearSelection();
                dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
        }

        private void DataGridView_examples_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView dataGrid)
            {
                if (dataGrid.Rows.Count <= 0 || e.RowIndex < 0)
                    return;

                var fileNumber = listBox_fileList.SelectedIndex;
                var jsonPaths = dataGrid.Rows[e.RowIndex]?.Cells[3]?.Value?.ToString().Split(Delimiter);
                var lineNumbers = dataGrid.Rows[e.RowIndex]?.Cells[4]?.Value?.ToString().Split(Delimiter);

                var fileName = listBox_fileList.SelectedItem.ToString();
                var jsonPath = jsonPaths[fileNumber];
                int.TryParse(lineNumbers[fileNumber], out var lineNumber);

                ShowPreviewEditor(fileName, jsonPath, lineNumber, true);
            }
        }

        private void DataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is DataGridView dataGrid)
                ReadjustRow(dataGrid, e.RowIndex);
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView dataGrid)
            {
                if (dataGrid.Rows.Count <= 0 || e.RowIndex < 0)
                    return;

                var fileNames = dataGrid.Rows[e.RowIndex]?.Cells[2]?.Value?.ToString().Split(Delimiter);

                this.listBox_fileList.SelectedValueChanged -= new EventHandler(this.ListBox_fileList_SelectedValueChanged);

                listBox_fileList.Items.Clear();
                listBox_fileList.Items.AddRange(fileNames ?? Array.Empty<string>());
                if (fileNames.Length > 0)
                    listBox_fileList.SetSelected(0, true);

                this.listBox_fileList.SelectedValueChanged += new EventHandler(this.ListBox_fileList_SelectedValueChanged);

                if (_showPreview)
                {
                    var fileName = listBox_fileList.Items[listBox_fileList.SelectedIndex].ToString();
                    var jsonPath = dataGrid.Rows[e.RowIndex]?.Cells[3]?.Value?.ToString().Split(Delimiter)?.FirstOrDefault();
                    int.TryParse(dataGrid.Rows[e.RowIndex]?.Cells[4]?.Value?.ToString().Split(Delimiter)?.FirstOrDefault(), out var lineNumber);

                    ShowPreviewEditor(fileName, jsonPath, lineNumber);
                }
            }
        }

        private void OnClosingEditor(object sender, CancelEventArgs e)
        {
            if (sender is Form s)
            {
                _editorPosition.WinX = s.Location.X;
                _editorPosition.WinY = s.Location.Y;
                _editorPosition.WinW = s.Width;
                _editorPosition.WinH = s.Height;
            }
        }

        private void CheckBox_reformatJson_CheckedChanged(object sender, EventArgs e)
        {
            _reformatJson = checkBox_reformatJson.Checked;
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

        private void CheckBox_vsCode_CheckedChanged(object sender, EventArgs e)
        {
            _useVsCode = checkBox_vsCode.Checked;
        }

        private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex != 2)
                return;
        }

        private void TextBox_description_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (textBox_description.ReadOnly)
            {
                if (string.IsNullOrEmpty(textBox_description.Text))
                {
                    textBox_description.Text = "Description: \r\n*Note: ";
                }
                textBox_description.ReadOnly = false;
                label_descSave.Visible = true;
            }
        }

        private void TreeView_examples_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox_description.ReadOnly = true;
            label_descSave.Visible = false;
            var descText = "";
            _nodeDescription?.TryGetValue(e?.Node?.Name?.TrimEnd('.'), out descText);
            textBox_description.Text = descText;
        }

        private void TextBox_description_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox_description.ReadOnly == false)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    _nodeDescription.TryGetValue(treeView_examples?.SelectedNode?.Name?.TrimEnd('.') ?? "", out var descText);
                    textBox_description.Text = descText;
                    textBox_description.ReadOnly = true;
                    label_descSave.Visible = false;
                }
                else if (e.KeyCode == Keys.Enter && e.Control == true)
                {
                    try
                    {
                        _nodeDescription[treeView_examples?.SelectedNode?.Name?.TrimEnd('.') ?? ""] = textBox_description.Text;
                    }
                    catch
                    {
                        _nodeDescription.Add(treeView_examples?.SelectedNode?.Name?.TrimEnd('.') ?? "", textBox_description.Text);
                    }

                    textBox_description.ReadOnly = true;
                    label_descSave.Visible = false;
                }
            }
        }

        private void ListBox_fileList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView_examples.SelectedCells.Count <= 0)
                return;

            var jsonPaths = dataGridView_examples.Rows[dataGridView_examples.SelectedCells[0].RowIndex]?.Cells[3]?.Value?.ToString().Split(Delimiter);
            var lineNumbers = dataGridView_examples.Rows[dataGridView_examples.SelectedCells[0].RowIndex]?.Cells[4]?.Value?.ToString().Split(Delimiter);

            var fileNumber = listBox_fileList.SelectedIndex;
            var fileName = listBox_fileList.Items[fileNumber].ToString();
            var jsonPath = "";
            var lineNumber = -1;

            if (jsonPaths.Length >= fileNumber)
            {
                jsonPath = jsonPaths[fileNumber];
            }

            if (jsonPath.Length >= fileNumber)
            {
                jsonPath = jsonPaths[fileNumber];
                int.TryParse(lineNumbers[fileNumber], out lineNumber);
            }

            ShowPreviewEditor(fileName, jsonPath, lineNumber, true);
        }

        private void ListBox_fileList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_showPreview)
            {
                return;
            }

            if (dataGridView_examples.SelectedCells.Count <= 0)
                return;

            var jsonPaths = dataGridView_examples.Rows[dataGridView_examples.SelectedCells[0].RowIndex]?.Cells[3]?.Value?.ToString().Split(Delimiter);
            var lineNumbers = dataGridView_examples.Rows[dataGridView_examples.SelectedCells[0].RowIndex]?.Cells[4]?.Value?.ToString().Split(Delimiter);

            var fileNumber = listBox_fileList.SelectedIndex;
            var fileName = listBox_fileList.Items[fileNumber].ToString();
            var jsonPath = "";
            var lineNumber = -1;

            if (jsonPaths.Length >= fileNumber)
            {
                jsonPath = jsonPaths[fileNumber];
            }

            if (jsonPath.Length >= fileNumber)
            {
                jsonPath = jsonPaths[fileNumber];
                int.TryParse(lineNumbers[fileNumber], out lineNumber);
            }

            ShowPreviewEditor(fileName, jsonPath, lineNumber);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveJson(_nodeDescription, DefaultDescriptionFileName, true);

            Settings.Default.LastRootFolder = folderBrowserDialog1.SelectedPath;
            Settings.Default.ReformatJson = _reformatJson;
            Settings.Default.ShowPreview = _showPreview;
            Settings.Default.AlwaysOnTop = _alwaysOnTop;
            Settings.Default.LoadDbOnStartUp = _loadDbOnStart;

            Settings.Default.MainWindowPositionX = Location.X;
            Settings.Default.MainWindowPositionY = Location.Y;
            Settings.Default.MainWindowWidth = Width;
            Settings.Default.MainWindowHeight = Height;

            if (_sideViewer != null && !_sideViewer.IsDisposed)
            {
                _editorPosition.WinX = _sideViewer.Location.X;
                _editorPosition.WinY = _sideViewer.Location.Y;
                _editorPosition.WinW = _sideViewer.Width;
                _editorPosition.WinH = _sideViewer.Height;
            }

            Settings.Default.EditorPositionX = _editorPosition.WinX;
            Settings.Default.EditorPositionY = _editorPosition.WinY;
            Settings.Default.EditorWidth = _editorPosition.WinW;
            Settings.Default.EditorHeight = _editorPosition.WinH;

            Settings.Default.TreeSplitterDistance = splitContainer1.SplitterDistance;
            Settings.Default.DescriptionSplitterDistance = splitContainer3.SplitterDistance;
            Settings.Default.FileListSplitterDistance = splitContainer4.SplitterDistance;

            Settings.Default.Save();
        }

        #endregion

        #region Helpers

        private BlockingCollection<JsonProperty> RunFileCollection(string collectionPath, string fileMask)
        {
            // Searching project files                        
            var filesList = Directory.GetFiles(collectionPath, fileMask, SearchOption.AllDirectories);
            _textLog.AppendLine(
                $"Files found: {filesList.Length}");
            FlushLog();

            var jsonPropertiesCollection = new BlockingCollection<JsonProperty>();
            // parse all files            
            var fileNumber = 0;
            Parallel.ForEach(filesList, fileName =>
            {
                var fileType = GetFileTypeFromFileName(fileName, _fileTypes);
                DeserializeFile(fileName, fileType, jsonPropertiesCollection, 0);

                if (fileNumber % 10 == 0)
                {
                    Invoke((MethodInvoker)delegate
                   {
                       toolStripStatusLabel1.Text = "Files parsed " + fileNumber + "/" + filesList.Length;
                   });
                }

                fileNumber++;
            });

            return jsonPropertiesCollection;
        }

        private void DeserializeFile(
    string fullFileName,
    JsoncContentType fileType,
    BlockingCollection<JsonProperty> rootCollection,
    int jsonDepth)
        {
            string jsonStr;
            try
            {
                jsonStr = File.ReadAllText(fullFileName);
            }
            catch (Exception ex)
            {
                /*var report = new ReportItem
                {
                    ProjectName = _projectName,
                    FullFileName = fullFileName,
                    Message = "File read exception: " + Utilities.ExceptionPrint(ex),
                    ValidationType = ValidationTypeEnum.File.ToString(),
                    Severity = ImportanceEnum.Error.ToString(),
                    Source = "DeserializeFile"
                };
                deserializeFileReportsCollection.Add(report);*/

                return;
            }

            if (string.IsNullOrEmpty(jsonStr))
            {
                /*var report = new ReportItem
                {
                    ProjectName = _projectName,
                    FullFileName = fullFileName,
                    Message = "File is empty",
                    ValidationType = ValidationTypeEnum.File.ToString(),
                    Severity = ImportanceEnum.Note.ToString(),
                    Source = "DeserializeFile"
                };
                deserializeFileReportsCollection.Add(report);*/

                return;
            }

            dynamic jsonObject;
            try
            {
                var options = new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Load,
                    LineInfoHandling = LineInfoHandling.Load,
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                };

                jsonObject = JObject.Parse(jsonStr, options);
            }
            catch (Exception ex)
            {
                /*var report = new ReportItem
                {
                    ProjectName = _projectName,
                    FullFileName = fullFileName,
                    Message = "File parse exception: " + Utilities.ExceptionPrint(ex),
                    ValidationType = ValidationTypeEnum.Parse.ToString(),
                    Severity = ImportanceEnum.Error.ToString(),
                    Source = "DeserializeFile"
                };
                deserializeFileReportsCollection.Add(report);*/

                return;
            }

            var ver = "";
            ParseJsonObject(
                jsonObject,
                fileType,
                rootCollection,
                fullFileName,
                ref ver,
                jsonDepth,
                "",
                true);
        }

        private void ParseJsonObject(
            JToken token,
            JsoncContentType fileType,
            BlockingCollection<JsonProperty> rootCollection,
            string fullFileName,
            ref string version,
            int jsonDepth,
            string parent,
            bool propertiesOnly = false)
        {
            if (token == null || rootCollection == null)
                return;

            switch (token)
            {
                case JProperty jProperty:
                {
                    if (jProperty.Value is JArray jArrayValue)
                    {
                        var arrayPath = jArrayValue.Path;
                        var arrayName = jProperty.Name;

                        // get new file type
                        if (arrayPath == arrayName && _fileTypes.Any(n => n.PropertyTypeName == arrayName))
                        {
                            fileType = _fileTypes
                               .FirstOrDefault(n => n.PropertyTypeName == arrayName).FileType;
                        }
                    }
                    else if (jProperty.Value is JObject jObjectValue)
                    {
                        var arrayPath = jObjectValue.Path;
                        var arrayName = jProperty.Name;

                        // get new file type
                        if (arrayPath == arrayName && _fileTypes.Any(n => n.PropertyTypeName == arrayName))
                        {
                            fileType = _fileTypes
                               .FirstOrDefault(n => n.PropertyTypeName == arrayName).FileType;
                        }
                    }

                    var jsonPath = jProperty.Path;
                    var name = jProperty.Name;
                    var lineNumber = ((IJsonLineInfo)jProperty)?.LineNumber ?? -1;
                    var propValue = jProperty.Value?.ToString();
                    var varType = jProperty.Value?.Type ?? JTokenType.Undefined;

                    // get schema version
                    if (name == VersionTagName)
                    {
                        /*if (!string.IsNullOrEmpty(version))
                        {
                            var report = new ReportItem
                            {
                                ProjectName = _projectName,
                                FullFileName = fullFileName,
                                JsonPath = jsonPath,
                                LineNumber = lineNumber.ToString(),
                                Message = $"Scheme version inconsistent: {version}->{propValue}",
                                ValidationType = ValidationTypeEnum.Logic.ToString(),
                                Severity = ImportanceEnum.Error.ToString(),
                                Source = "ParseJsonObject"
                            };
                            parseJsonObjectReportsCollection.Add(report);
                        }*/

                        version = propValue;
                    }

                    var newProperty = new JsonProperty
                    {
                        Value = propValue,
                        FileType = fileType,
                        FullFileName = fullFileName,
                        JsonPath = jsonPath,
                        Name = name,
                        Version = version,
                        ItemType = JsonItemType.Property,
                        Parent = parent,
                        SourceLineNumber = lineNumber,
                        VariableType = varType
                    };
                    rootCollection.Add(newProperty);

                    foreach (var child in jProperty.Children())
                    {
                        if (child is JArray || child is JObject)
                        {
                            jsonDepth++;
                            var newParent = string.IsNullOrEmpty(name) ? parent : name;
                            ParseJsonObject(
                                child,
                                fileType,
                                rootCollection,
                                fullFileName,
                                ref version,
                                jsonDepth,
                                newParent,
                                propertiesOnly);
                            jsonDepth--;
                        }
                    }

                    break;
                }
                case JObject jObject:
                {
                    if (!propertiesOnly)
                    {
                        var newProperty = new JsonProperty
                        {
                            FileType = fileType,
                            FullFileName = fullFileName,
                            Name = "{",
                            Version = version,
                            ItemType = JsonItemType.ObjectStart,
                            Parent = parent,
                        };
                        rootCollection.Add(newProperty);
                    }

                    foreach (var child in jObject.Children())
                        ParseJsonObject(
                            child,
                            fileType,
                            rootCollection,
                            fullFileName,
                            ref version,
                            jsonDepth,
                            parent,
                            propertiesOnly);

                    if (!propertiesOnly)
                    {
                        var newProperty = new JsonProperty
                        {
                            FileType = fileType,
                            FullFileName = fullFileName,
                            Name = "}",
                            Version = version,
                            ItemType = JsonItemType.ObjectEnd,
                            Parent = parent,
                        };
                        rootCollection.Add(newProperty);
                    }
                    break;
                }
                case JArray jArray:
                {
                    if (!propertiesOnly)
                    {
                        var newProperty = new JsonProperty
                        {
                            FileType = fileType,
                            FullFileName = fullFileName,
                            Name = "[",
                            Version = version,
                            ItemType = JsonItemType.ArrayStart,
                            Parent = parent,
                        };
                        rootCollection.Add(newProperty);
                    }

                    foreach (var child in jArray.Children())
                        ParseJsonObject(
                            child,
                            fileType,
                            rootCollection,
                            fullFileName,
                            ref version,
                            jsonDepth,
                            parent,
                            propertiesOnly);

                    if (!propertiesOnly)
                    {
                        var newProperty = new JsonProperty
                        {
                            FileType = fileType,
                            FullFileName = fullFileName,
                            Name = "]",
                            Version = version,
                            ItemType = JsonItemType.ArrayEnd,
                            Parent = parent,
                        };
                        rootCollection.Add(newProperty);
                    }
                    break;
                }
                default:
                {
                    if (token.Children().Any())
                    {
                        if (!propertiesOnly)
                        {
                            var newProperty = new JsonProperty
                            {
                                FileType = fileType,
                                FullFileName = fullFileName,
                                Name = "}",
                                Version = version,
                                ItemType = JsonItemType.Unknown,
                                Parent = parent,
                            };
                            rootCollection.Add(newProperty);
                        }
                        /*var report = new ReportItem
                        {
                            ProjectName = _projectName,
                            FullFileName = fullFileName,
                            JsonPath = token.Path,
                            Message = "Unknown node skipped by parser: " + token,
                            ValidationType = ValidationTypeEnum.Parse.ToString(),
                            Severity = ImportanceEnum.Error.ToString(),
                            Source = "ParseJsonObject"
                        };
                        parseJsonObjectReportsCollection.Add(report);*/
                    }
                    break;
                }
            }
        }

        private bool FlattenCollection(IEnumerable<JsonProperty> propertiesCollection,
            JsoncContentType contentType,
            string elementName,
            string[] parentName)
        {
            if (propertiesCollection == null
                || propertiesCollection.Count() <= 0
                || string.IsNullOrEmpty(elementName)
                || parentName == null
                || parentName.Length <= 0)
                return false;

            var typedCollection = propertiesCollection
                .Where(n => n.FileType == contentType)
                .ToArray();

            IEnumerable<IGrouping<string, JsonProperty>> FileGroupedCollection;
            if (parentName.Length > 1)
            {
                for (var i = 0; i < parentName.Length; i++)
                {
                    parentName[i] = parentName[i].ToLower();
                }

                FileGroupedCollection = typedCollection
                    .Where(n =>
                        n.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase)
                        && parentName.Contains(n.Parent.ToLower()))
                    .ToArray()
                    .GroupBy(n => n.FullFileName);
            }
            else
            {
                FileGroupedCollection = typedCollection
                   .Where(n =>
                        n.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase)
                        && parentName[0].Equals(n.Parent, StringComparison.OrdinalIgnoreCase))
                   .ToArray()
                   .GroupBy(n => n.FullFileName);
            }

            var processedFilesNumber = 0;
            var totalFilesNumber = FileGroupedCollection.Count();

            //get every file name
            foreach (var actionCollection in FileGroupedCollection)
            {
                var fileActionCollection = typedCollection
                    .Where(n =>
                            n.FullFileName == actionCollection.Key)
                    .ToArray();

                // iterate through single file one by one
                foreach (var actionProperty in actionCollection)
                {
                    //get a collection of events in the file
                    var actionMembers = fileActionCollection
                        .Where(n =>
                            n.FullFileName == actionProperty.FullFileName &&
                            n.JsonPath.Contains(actionProperty.ParentPath))
                        .ToArray();

                    foreach (var actionMember in actionMembers)
                    {
                        var flattenedPath = actionMember.JsonPath.Replace(actionProperty.ParentPath,
                            "events.actions.<" + actionProperty.Value.Replace('.', '_') + ">");
                        if (contentType == JsoncContentType.Events)
                        {
                            flattenedPath = actionMember.JsonPath.Replace(actionProperty.ParentPath,
                            "events.actions.<" + actionProperty.Value.Replace('.', '_') + ">");
                        }
                        else
                        {
                            flattenedPath = actionMember.JsonPath.Replace(actionProperty.ParentPath,
                                actionProperty.ParentPath + ".<" + actionProperty.Value.Replace('.', '_') + ">");
                        }
                        actionMember.FlattenedJsonPath = flattenedPath;
                    }
                }

                if (processedFilesNumber % 20 == 0)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        toolStripStatusLabel1.Text =
                            contentType.ToString() + " converted " + processedFilesNumber + "/" + totalFilesNumber;
                    });
                }

                processedFilesNumber++;
            }

            return true;
        }

        private TreeNode GenerateTreeFromList(IEnumerable<JsonProperty> rootCollection)
        {
            var node = new TreeNode(RootNodeName);
            var itemNumber = 0;
            var totalItemNumber = rootCollection.Count();

            foreach (var propertyItem in rootCollection)
            {
                var itemName = "<" + propertyItem.FileType + ">." + propertyItem.UnifiedFlattenedPath;
                if (!_exampleLinkCollection.ContainsKey(itemName))
                {
                    _exampleLinkCollection.Add(itemName, new List<JsonProperty>() { propertyItem });
                }
                else
                {
                    _exampleLinkCollection[itemName].Add(propertyItem);
                }

                var tmpNode = node;
                var tmpPath = new StringBuilder();

                foreach (var token in itemName.Split('.'))
                {
                    tmpPath.Append(token + ".");
                    if (!tmpNode.Nodes.ContainsKey(tmpPath.ToString()))
                    {
                        tmpNode.Nodes.Add(tmpPath.ToString(), token);
                    }

                    tmpNode = tmpNode.Nodes[tmpPath.ToString()];
                }

                if (itemNumber % 1000 == 0)
                {
                    Invoke((MethodInvoker)delegate
                   {
                       toolStripStatusLabel1.Text =
                           "Properties processed  " + itemNumber + "/" + totalItemNumber;
                   });
                }

                itemNumber++;
            }

            return node;
        }

        private async Task<bool> LoadDb(string fileName)
        {
            try
            {
                if (!File.Exists(fileName)
                    || !File.Exists(fileName + ".tree"))
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("File read exception [" + fileName + "]: " + ex.Message);
                toolStripStatusLabel1.Text = "Failed to load database";
                return false;
            }

            FormCaption = DefaultFormCaption;
            if (string.IsNullOrEmpty(fileName))
                return false;

            ActivateUiControls(false, false);
            toolStripStatusLabel1.Text = "Loading database...";
            var rootNodeExamples = new TreeNode();
            var exampleLinkCollection = new Dictionary<string, List<JsonProperty>>();
            try
            {
                var t = Task.Run(() =>
                    {
                        rootNodeExamples = LoadBinary<TreeNode>(fileName + ".tree");
                        exampleLinkCollection = LoadBinary<Dictionary<string, List<JsonProperty>>>(fileName);
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

            if (exampleLinkCollection != null && rootNodeExamples != null)
            {
                _rootNodeExamples = rootNodeExamples;
                _exampleLinkCollection = exampleLinkCollection;
                tabControl1.TabPages[1].Enabled = true;
                FormCaption = DefaultFormCaption + " " + ShortFileName(fileName);
                treeView_examples.Nodes.Clear();
                treeView_examples.Nodes.Add(_rootNodeExamples);
                treeView_examples.Nodes[0].Expand();
                tabControl1.SelectedTab = tabControl1.TabPages[1];

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                toolStripStatusLabel1.Text = "";
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool FillExamplesGrid(Dictionary<string, List<JsonProperty>> exampleLinkCollection, TreeNode currentNode,
            SearchItem searchParam = null)
        {
            if (currentNode == null)
                return false;

            if (!exampleLinkCollection.ContainsKey(currentNode.Name.TrimEnd('.')))
                return false;

            var records = exampleLinkCollection[currentNode.Name.TrimEnd('.')];
            toolStripStatusLabel1.Text = "Displaying " + records.Count + " records";

            if (_lastExSelectedNode != null)
                _lastExSelectedNode.NodeFont = new Font(treeView_examples.Font.FontFamily, treeView_examples.Font.Size,
                    FontStyle.Regular);

            _lastExSelectedNode = currentNode;
            currentNode.NodeFont =
                new Font(treeView_examples.Font.FontFamily, treeView_examples.Font.Size, FontStyle.Bold);

            ExClearSearch();

            _examplesTable.Rows.Clear();
            comboBox_ExVersions.Items.Clear();
            comboBox_ExVersions.Items.Add(DefaultVersionCaption);
            comboBox_ExVersions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in records)
            {
                if (record == null)
                    continue;

                if (!versionCollection.Contains(record.Version))
                    versionCollection.Add(record.Version);
                if (searchParam != null && searchParam.Version != DefaultVersionCaption &&
                    record.Version != searchParam.Version)
                    continue;

                if (_reformatJson)
                    BeautifyJson(record.Value, _reformatJson);
                if ((searchParam == null || string.IsNullOrEmpty(searchParam.Value)) &&
                    FilterCellOut(record.Value, searchParam, true))
                    continue;

                var existingRow = _examplesTable
                    .AsEnumerable()
                    .Where(row => row.Field<string>(_exampleGridColumnsNames[0]) == record.Version
                    && row.Field<string>(_exampleGridColumnsNames[1]) == record.Value);

                if (!existingRow.Any())
                {
                    var newRow = _examplesTable.NewRow();
                    newRow[_exampleGridColumnsNames[0]] = record.Version;
                    newRow[_exampleGridColumnsNames[1]] = record.Value;
                    newRow[_exampleGridColumnsNames[2]] = record.FullFileName;
                    newRow[_exampleGridColumnsNames[3]] = record.JsonPath;
                    newRow[_exampleGridColumnsNames[4]] = record.SourceLineNumber;
                    _examplesTable.Rows.Add(newRow);
                }
                else
                {
                    existingRow.FirstOrDefault()[2] = existingRow.FirstOrDefault()[2] + Delimiter.ToString() + record.FullFileName;
                    existingRow.FirstOrDefault()[3] = existingRow.FirstOrDefault()[3] + Delimiter.ToString() + record.JsonPath;
                    existingRow.FirstOrDefault()[4] = existingRow.FirstOrDefault()[4] + Delimiter.ToString() + record.SourceLineNumber;
                }
            }

            if (searchParam == null)
                searchParam = new SearchItem(DefaultVersionCaption);
            if (!_lastExSearchList.Contains(searchParam))
                _lastExSearchList.Add(searchParam);

            SetSearchText(textBox_ExSearchHistory, _lastExSearchList);
            comboBox_ExVersions.Items.AddRange(versionCollection.ToArray());
            toolStripStatusLabel1.Text = "";

            return true;
        }

        private async Task FilterExamples(Dictionary<string, List<JsonProperty>> exampleLinkCollection, SearchItem searchParam,
            bool compactJson = false)
        {
            if (_lastExSearchList.Contains(searchParam))
                return;

            _lastExSearchList.Add(searchParam);

            if (searchParam == null || string.IsNullOrEmpty(searchParam.Value))
            {
                FillExamplesGrid(exampleLinkCollection, _lastExSelectedNode, searchParam);
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

        private async Task FilterExamplesVersion(Dictionary<string, List<JsonProperty>> exampleLinkCollection, SearchItem searchParam)
        {
            if (_lastExSearchList == null)
                _lastExSearchList = new List<SearchItem>();

            if (!_lastExSearchList.Any())
                _lastExSearchList.Add(new SearchItem(DefaultVersionCaption));
            var lastSearch = _lastExSearchList.Last();
            if (lastSearch.Version != DefaultVersionCaption)
                FillExamplesGrid(exampleLinkCollection, _lastExSelectedNode, searchParam);

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

        private void SetSearchText(TextBox textBox, List<SearchItem> searchList)
        {
            var searchString = new StringBuilder();
            foreach (var lastSearch in searchList)
            {
                if (lastSearch == null || string.IsNullOrEmpty(lastSearch.Value))
                    continue;

                var cs = lastSearch.CaseSensitive == StringComparison.Ordinal ? "'CS'" : "";
                searchString.Append(searchString.Length <= 0 ? "[" : " -> [");
                searchString.Append("ver.:\"" + lastSearch.Version + "\";");
                searchString.Append(lastSearch.Condition + cs + ":\"" + lastSearch.Value + "\"]");
            }

            Invoke((MethodInvoker)delegate
            { textBox.Text = searchString.ToString(); });
        }

        private void ExClearSearch()
        {
            _lastExSearchList.Clear();
            Invoke((MethodInvoker)delegate
            { textBox_ExSearchHistory.Clear(); });
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

                   dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                   for (var rowNumber = 0; rowNumber < dgView.RowCount; rowNumber++)
                   {
                       var row = dgView.Rows[rowNumber];
                       row.HeaderCell.Value = (rowNumber + 1).ToString();
                       var newHeight = row.GetPreferredHeight(rowNumber,
                           DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);
                       var currentHeight = dgView.Height;
                       if (newHeight == row.Height && newHeight <= currentHeight * CellSizeAdjust)
                           return;

                       if (newHeight > currentHeight * CellSizeAdjust)
                           newHeight = (ushort)(currentHeight * CellSizeAdjust);
                       row.Height = newHeight;
                   }

                   for (var columnNumber = 0; columnNumber < dgView.ColumnCount; columnNumber++)
                   {
                       var column = dgView.Columns[columnNumber];
                       var newWidth = column.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
                       var currentWidth = dgView.Width;
                       if (newWidth == column.Width && newWidth <= currentWidth * CellSizeAdjust)
                           return;

                       if (newWidth > currentWidth * CellSizeAdjust)
                           newWidth = (ushort)(currentWidth * CellSizeAdjust);
                       column.Width = newWidth;
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
            if (newHeight == row.Height && newHeight <= currentHeight * CellSizeAdjust)
                return;

            if (newHeight > currentHeight * CellSizeAdjust)
                newHeight = (ushort)(currentHeight * CellSizeAdjust);
            row.Height = newHeight;
        }

        private void ActivateUiControls(bool active, bool processTable = true)
        {
            if (!active)
            {
                comboBox_ExVersions.SelectedIndexChanged -= ComboBox_ExVersions_SelectedIndexChanged;
            }

            if (active)
                FlushLog();

            dataGridView_examples.Enabled = active;

            if (processTable)
            {
                if (active)
                {
                    dataGridView_examples.DataSource = _examplesTable;
                    dataGridView_examples.Columns[2].Visible = false;
                    dataGridView_examples.Columns[3].Visible = false;
                    dataGridView_examples.Columns[4].Visible = false;
                    dataGridView_examples.Invalidate();
                }
                else
                {
                    dataGridView_examples.DataSource = null;
                }
            }

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

            tabControl1.Enabled = active;

            if (active)
            {
                comboBox_ExVersions.SelectedIndexChanged += ComboBox_ExVersions_SelectedIndexChanged;
            }

            Refresh();
        }

        private void ShowPreviewEditor(string fileName, string jsonPath, int lineNumber, bool newWindow = false)
        {
            if (_useVsCode)
            {
                var execParams = "-r -g " + fileName + ":" + lineNumber;
                VsCodeOpenFile(execParams);
                return;
            }

            var textEditor = _sideViewer;
            if (newWindow)
            {
                textEditor = null;
            }

            bool fileLoaded;
            if (textEditor != null && !textEditor.IsDisposed)
            {
                if (textEditor.SingleLineBrackets != _reformatJson || textEditor.Text != PreViewCaption + fileName)
                {
                    textEditor.SingleLineBrackets = _reformatJson;
                    fileLoaded = textEditor.LoadJsonFromFile(fileName);
                }
                else
                {
                    fileLoaded = true;
                }
            }
            else
            {
                if (textEditor != null)
                {
                    textEditor.Close();
                }

                textEditor = new JsonViewer("", "", newWindow)
                {
                    SingleLineBrackets = _reformatJson
                };

                if (!newWindow)
                    textEditor.Closing += OnClosingEditor;

                fileLoaded = textEditor.LoadJsonFromFile(fileName);
            }

            if (_sideViewer == null)
                _sideViewer = textEditor;

            textEditor.AlwaysOnTop = _alwaysOnTop;
            textEditor.Show();

            if (!fileLoaded)
                return;

            if (!newWindow)
            {
                textEditor.Text = PreViewCaption + fileName;

                if (!(_editorPosition.WinX == 0
                      && _editorPosition.WinY == 0
                      && _editorPosition.WinW == 0
                      && _editorPosition.WinH == 0))
                {
                    textEditor.Location = new Point(_editorPosition.WinX, _editorPosition.WinY);
                    textEditor.Width = _editorPosition.WinW;
                    textEditor.Height = _editorPosition.WinH;
                }
            }
            else
            {
                textEditor.Text = fileName;
            }

            if (TryGetPositionByPathStr(textEditor.EditorText, jsonPath, out var startPos, out var endPos))
            {
                textEditor.PermanentHighlight(startPos, endPos + 1);
            }
            else
            {
                textEditor.SelectTextLines(lineNumber - 1, 1);
            }
        }

        #endregion

        #region Utilities

        private static JsoncContentType GetFileTypeFromFileName(string fullFileName,
    IEnumerable<ContentTypeItem> _fileTypes)
        {
            var shortFileName = GetShortFileName(fullFileName);

            return (from item in _fileTypes where shortFileName.EndsWith(item.FileTypeMask) select item.FileType).FirstOrDefault();
        }

        private static string GetShortFileName(string longFileName)
        {
            if (string.IsNullOrEmpty(longFileName))
                return longFileName;

            var i = longFileName.LastIndexOf('\\');
            if (i < 0)
                return longFileName;

            if (i + 1 >= 0 && longFileName.Length > i + 1)
            {
                return longFileName.Substring(i + 1);
            }

            return longFileName;
        }

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
            if (cellValue == null || string.IsNullOrEmpty(cellValue))
                return true;

            if (searchParam == null || string.IsNullOrEmpty(searchParam.Value))
                return false;

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

        private static string ShortFileName(string longFileName)
        {
            var i = longFileName.LastIndexOf('\\');
            return i < 0 ? longFileName : longFileName.Substring(i + 1);
        }

        private void FlushLog()
        {
            if (_textLog.Length > 0)
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += _textLog.ToString();
                    _textLog.Clear();
                    textBox_logText.SelectionStart = textBox_logText.Text.Length;
                    textBox_logText.ScrollToCaret();
                });
            }
        }

        private static bool TryGetPositionByPathStr(string json, string path, out int startPos, out int endPos)
        {
            startPos = -1;
            endPos = -1;

            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(path))
                return false;

            var pathList = JsonPathParser.ParseJsonToPathList(json.Replace(' ', ' '), out var _, out var _, "", '.', false);

            var pathItems = pathList.Where(n => n.Path == "." + path).ToArray();
            if (!pathItems.Any())
                return false;

            startPos = pathItems.Last().StartPosition;
            endPos = pathItems.Last().EndPosition;
            return true;
        }

        private void VsCodeOpenFile(string command)
        {
            var ProcessInfo = new ProcessStartInfo("code", command)
            {
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                var process = Process.Start(ProcessInfo);
            }
            catch (Exception ex)
            {
                textBox_logText.Text += ex.Message;
            }
        }

        #endregion
    }
}
