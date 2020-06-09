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

        // experimental options
        private static bool _reformatJson;
        private readonly bool _collectAllFileNames;
        private readonly bool _ignoreHttpsError;

        // global variables
        //private readonly string _currentDirectory;
        private string _version = "";
        private string _fileName = "";
        private JsoncContentType _fileType = JsoncContentType.DataViews;
        private TreeNode _rootNode;
        private TreeNode _rootNodeKeywords;
        private DataTable _examplesTable;
        private List<JsoncDictionary> _metaDictionary = new List<JsoncDictionary>();
        private StringBuilder textLog = new StringBuilder();
        private volatile bool _isDoubleClick;

        // last used values for UI processing optimization
        private TreeNode _lastSelectedNode;
        private string _lastSelectedVersion;
        private string _lastFilterValue = "";
        private SearchCondition _lastSelectedCondition;
        private bool _lastCaseSensitive;

        private enum SearchCondition
        {
            Contains,
            StartsWith,
            EndsWith
        }

        #region GUI

        public Form1()
        {
            InitializeComponent();

            _reformatJson = Settings.Default.ReformatJson;
            _collectAllFileNames = Settings.Default.CollectAllFileNames;
            folderBrowserDialog1.SelectedPath = Settings.Default.LastRootFolder;
            _ignoreHttpsError = Settings.Default.IgnoreHttpsError;

            comboBox_condition.Items.AddRange(typeof(SearchCondition).GetEnumNames() as string[]);
            comboBox_condition.SelectedIndex = 0;

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

            comboBox_versions.Items.Clear();
            comboBox_versions.Items.Add(DefaultVersionCaption);
            comboBox_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            comboBox_versions.SelectedIndex = 0;
            comboBox_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;
            _lastSelectedVersion = DefaultVersionCaption;

            dataGridView_examples.ContextMenuStrip = contextMenuStrip_findValue;
            treeView_json.ContextMenuStrip = contextMenuStrip_findField;

            //_currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.LastDbName)) return;

            ActivateUiControls(false);
            tabControl1.TabPages[2].IsAccessible = false;

            List<JsoncDictionary> dataCollection = null;
            TreeNode nodeCollection = null;
            TreeNode nodeCollectionKeywords = null;

            try
            {
                await Task.Run(() =>
                    {
                        dataCollection = JsonIo.LoadBinary<List<JsoncDictionary>>(Settings.Default.LastDbName);
                        nodeCollection = JsonIo.LoadBinary<TreeNode>(Settings.Default.LastDbName + ".tree");
                        nodeCollectionKeywords = JsonIo.LoadBinary<TreeNode>(Settings.Default.LastDbName + ".keywords");
                    }
                ).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("File read exception [" + openFileDialog1.FileName + "]: " + ex.Message);
            }

            if (dataCollection == null || nodeCollection == null) return;

            _metaDictionary = dataCollection;

            _rootNode = nodeCollection;
            treeView_json.Nodes.Clear();
            treeView_json.Nodes.Add(_rootNode);
            treeView_json.Sort();
            treeView_json.Nodes[0].Expand();

            ActivateUiControls(true);

            if (nodeCollectionKeywords == null) return;

            tabControl1.SelectedTab = tabControl1.TabPages[1];

            _rootNodeKeywords = nodeCollectionKeywords;
            treeView_properties.Nodes.Clear();
            treeView_properties.Nodes.Add(_rootNodeKeywords);
            treeView_properties.Sort();
            treeView_properties.Nodes[0].Expand();

            tabControl1.TabPages[2].IsAccessible = true;
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

            var result = await LoadDb(openFileDialog1.FileName);

            ActivateUiControls(true);

            if (result)
            {
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                Settings.Default.LastDbName = openFileDialog1.FileName;
                Settings.Default.Save();
            }
        }

        private async Task<bool> LoadDb(string FileName)
        {
            List<JsoncDictionary> dataCollection = null;
            TreeNode nodeCollection = null;
            TreeNode nodeCollectionKeywords = null;

            progressBar1.Maximum = 3;
            progressBar1.Value = 0;
            await Task.Run(() =>
            {
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        dataCollection = JsonIo.LoadBinary<List<JsoncDictionary>>(FileName);
                        progressBar1.Value = 1;

                        nodeCollection = JsonIo.LoadBinary<TreeNode>(openFileDialog1.FileName + ".tree");
                        progressBar1.Value = 2;

                        if (dataCollection == null || nodeCollection == null) return;

                        _metaDictionary = dataCollection;

                        _rootNode = nodeCollection;
                        treeView_json.Nodes.Clear();
                        treeView_json.Nodes.Add(_rootNode);
                        treeView_json.Sort();
                        treeView_json.Nodes[0].Expand();

                        tabControl1.TabPages[2].IsAccessible = false;
                        nodeCollectionKeywords = JsonIo.LoadBinary<TreeNode>(openFileDialog1.FileName + ".keywords");
                        progressBar1.Value = 3;

                        if (nodeCollectionKeywords == null) return;
                        _rootNodeKeywords = nodeCollectionKeywords;
                        treeView_properties.Nodes.Clear();
                        treeView_properties.Nodes.Add(_rootNodeKeywords);
                        treeView_properties.Sort();
                        treeView_properties.Nodes[0].Expand();
                        tabControl1.TabPages[2].IsAccessible = true;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File read exception [" + openFileDialog1.FileName + "]: " + ex.Message);
                }
            }).ConfigureAwait(true);

            return true;
        }

        private async void Button_collectDatabase_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            ActivateUiControls(false);

            treeView_json.Nodes.Clear();
            _examplesTable.Clear();
            _rootNode = new TreeNode(RootNodeName);
            _metaDictionary = new List<JsoncDictionary>();

            var startPath = folderBrowserDialog1.SelectedPath;

            var filesList = new List<string>();
            foreach (var fileName in checkedListBox_params.CheckedItems) filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(), SearchOption.AllDirectories));

            var filesNumber = filesList.Count;
            var currentFileNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            foreach (var file in filesList)
            {
                currentFileNumber++;
                var newProgress = (ushort)((float)currentFileNumber / filesNumber * 100);
                if (newProgress >= oldProgress + MinProgressDisplay)
                {
                    progressBar1.Value = oldProgress = newProgress;
                    textBox_logText.Text += textLog.ToString();
                    textLog.Clear();
                    textBox_logText.SelectionStart = textBox_logText.Text.Length;
                    textBox_logText.ScrollToCaret();
                }

                _version = "";
                await Task.Run(() =>
                {
                    DeserializeFile(file, file.Substring(file.LastIndexOf('\\') + 1), _rootNode);
                }).ConfigureAwait(true);
            }


            textLog.AppendLine("Files parsed: " + filesList.Count.ToString());
            textBox_logText.Text += textLog.ToString();
            textLog.Clear();
            textBox_logText.SelectionStart = textBox_logText.Text.Length;
            textBox_logText.ScrollToCaret();

            if (_rootNode == null) return;

            treeView_json.Nodes.Add(_rootNode);
            treeView_json.Sort();
            treeView_json.Nodes[0].Expand();

            ActivateUiControls(true);

            tabControl1.TabPages[2].IsAccessible = false;
            _rootNodeKeywords = new TreeNode(RootNodeName);
            await CollectKeywords(_metaDictionary, _rootNodeKeywords);

            if (_rootNodeKeywords == null) return;

            treeView_properties.Nodes.Add(_rootNodeKeywords);
            treeView_properties.Sort();
            treeView_properties.Nodes[0].Expand();
            tabControl1.TabPages[2].IsAccessible = true;
        }

        private async void Button_validateFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            ActivateUiControls(false);

            var startPath = folderBrowserDialog1.SelectedPath;

            var filesList = new List<string>();
            foreach (var fileName in checkedListBox_params.CheckedItems) filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(), SearchOption.AllDirectories));

            var filesNumber = filesList.Count;
            var currentFileNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            foreach (var file in filesList)
            {
                currentFileNumber++;
                var newProgress = (ushort)((float)currentFileNumber / filesNumber * 100);
                if (newProgress >= oldProgress + MinProgressDisplay)
                {
                    progressBar1.Value = oldProgress = newProgress;
                    textBox_logText.Text += textLog.ToString();
                    textLog.Clear();
                    textBox_logText.SelectionStart = textBox_logText.Text.Length;
                    textBox_logText.ScrollToCaret();
                }

                await Task.Run(() =>
                {
                    ValidateFile(file);
                }).ConfigureAwait(true);
            }

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
                    JsonIo.SaveBinary<TreeNode>(_rootNode, saveFileDialog1.FileName + ".tree");
                    JsonIo.SaveBinary<TreeNode>(_rootNodeKeywords, saveFileDialog1.FileName + ".keywords");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File write exception [" + saveFileDialog1.FileName + "]: " + ex.Message);
                }
            }).ConfigureAwait(true);
        }

        private void DataGridView_examples_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var column = e.ColumnIndex;
            if (!_collectAllFileNames && column == 2)
            {
                var editor = new JsonViewer(dataGridView_examples.Rows[e.RowIndex].Cells[column].Value.ToString(), "");
                editor.Show();

                if (FindTextLines(editor.EditorText, dataGridView_examples.Rows[e.RowIndex].Cells[1].Value.ToString(), out var startLine, out var lineNum)) editor.SelectTextLines(startLine, lineNum);
            }
            else if (column == 1)
            {
                var cell = dataGridView_examples.Rows[e.RowIndex].Cells[column];
                dataGridView_examples.CurrentCell = cell;

                var editor = new JsonViewer(dataGridView_examples.Rows[e.RowIndex].Cells[2].Value.ToString(), cell.Value.ToString());
                editor.Show();
            }
        }

        private void TextBox_searchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            var condition = (SearchCondition)comboBox_condition.SelectedIndex;
            var searchString = textBox_searchString.Text;
            var caseSensitive = checkBox_seachCaseSensitive.Checked;
            if (_lastFilterValue == searchString && _lastSelectedCondition == condition && _lastCaseSensitive == caseSensitive) return;

            ActivateUiControls(false);
            dataGridView_examples.DataSource = null;

            FilterExamples(condition, searchString, comboBox_versions.SelectedItem?.ToString(), caseSensitive);
            dataGridView_examples.DataSource = _examplesTable;
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

        private void DataGridView_examples_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ReadjustRow(e.RowIndex);
        }

        private void Button_readjust_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false, false);

            var rowsNumber = dataGridView_examples.RowCount;
            var currentRowNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            for (var i = 0; i < dataGridView_examples.RowCount; i++)
            {
                currentRowNumber++;
                var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                if (newProgress >= oldProgress + MinProgressDisplay) progressBar1.Value = oldProgress = newProgress;

                ReadjustRow(i);
            }
            ActivateUiControls(true);
        }

        private void TreeView_json_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || e.Node.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillGrid(e.Node);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
            _lastFilterValue = "";
        }

        private void TreeView_json_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            if (treeView_json.SelectedNode == null || treeView_json.SelectedNode.Parent.Text == RootNodeName) return;

            ActivateUiControls(false);
            FillGrid(treeView_json.SelectedNode);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
            _lastFilterValue = "";

        }

        private void FindValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lastSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells.Count < 1
                || dataGridView_examples.SelectedCells[0]?.ColumnIndex != 1
                || _lastSelectedNode.Parent?.Parent?.Parent == null) return;

            var paramName = _lastSelectedNode.Text;
            var paramValue = CompactJson(dataGridView_examples.SelectedCells[0].Value.ToString());

            ActivateUiControls(false);
            FillGrid(_lastSelectedNode.Parent, comboBox_versions.SelectedItem.ToString(), "\"" + paramName + "\":", SearchCondition.Contains, true);
            FilterExamples(SearchCondition.Contains, paramValue, comboBox_versions.SelectedItem.ToString(), true, true);

            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView_json.SelectedNode?.Parent?.Parent?.Parent == null || treeView_json.SelectedNode.Parent.Parent.Text == RootNodeName) return;

            var paramName = treeView_json.SelectedNode.Text;

            ActivateUiControls(false);
            FillGrid(treeView_json.SelectedNode.Parent);
            FilterExamples(SearchCondition.Contains, "\"" + paramName + "\":", comboBox_versions.SelectedItem.ToString(), true, false);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void TreeView_json_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeView_json.SelectedNode = e.Node;
        }

        private void DataGridView_examples_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            dataGridView_examples.ClearSelection();
            dataGridView_examples.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.LastRootFolder = folderBrowserDialog1.SelectedPath;
            Settings.Default.Save();
        }

        private void ContextMenuStrip_findValue_Opening(object sender, CancelEventArgs e)
        {
            if (_lastSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells.Count < 1
                || dataGridView_examples.SelectedCells[0]?.ColumnIndex != 1
                || _lastSelectedNode.Parent?.Parent?.Parent == null)
                FindAllStripMenuItem.Enabled = false;
            else FindAllStripMenuItem.Enabled = true;
        }

        private void ContextMenuStrip_findField_Opening(object sender, CancelEventArgs e)
        {
            if (treeView_json.SelectedNode?.Parent?.Parent?.Parent == null || treeView_json.SelectedNode.Parent.Parent.Text == RootNodeName) FindFieldToolStripMenuItem.Enabled = false;
            else FindFieldToolStripMenuItem.Enabled = true;
        }

        #region Prevent_treenode_collapse
        private void TreeView_json_MouseDown(object sender, MouseEventArgs e)
        {
            _isDoubleClick = e.Clicks > 1;
        }

        private void TreeView_json_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (!_isDoubleClick || e.Action != TreeViewAction.Expand) return;

            _isDoubleClick = false;
            e.Cancel = true;
        }

        private void TreeView_json_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (!_isDoubleClick || e.Action != TreeViewAction.Collapse) return;

            _isDoubleClick = false;
            e.Cancel = true;
        }
        #endregion

        #endregion

        #region Helpers

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
            {
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
                        File.WriteAllText(localPath + ".original", schemaData);
                    }
                }
            }

            return schemaData;
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
                    fileNode = _rootNode.Nodes[_rootNode.Nodes.Count - 1];
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
                        if (jProperty.Value is JValue jPropertyValue)
                        {
                            printValue = saveValue = jPropertyValue.Value?.ToString();
                        }
                        else
                        {
                            printValue = jProperty.Value?.GetType().Name;
                            saveValue = jProperty.Value?.ToString();
                        }

                        TreeNode childNode;

                        var obj = parentNode.Nodes.Cast<TreeNode>().Where(r => r.Text == jProperty.Name).ToArray();

                        if (obj.Count() > 1)
                        {
                            textLog.AppendLine(Environment.NewLine
                                + "More than 1 similar object found in the tree:"
                                + Environment.NewLine
                                + obj.Select(n => n.FullPath).Aggregate("", (current, next) => current + ", " + next)
                                + Environment.NewLine);
                        }
                        var exNode = obj.FirstOrDefault();

                        if (exNode != null)
                        {
                            childNode = exNode;
                        }
                        else
                        {
                            childNode = new TreeNode(jProperty.Name)
                            {
                                Name = jProperty.Name,
                            };
                            parentNode.Nodes.Add(childNode);
                        }

                        if (jProperty.Name == VersionTagName) _version = printValue;

                        if (!JsoncDictionary.NodeTypes.TryGetValue(token.GetType().Name, out var nodeType))
                        {
                            nodeType = JsoncNodeType.Unknown;
                            textLog.AppendLine(Environment.NewLine + _fileName + " node type unknown: " + Environment.NewLine + " Node [" + jProperty.Name + "] " + Environment.NewLine);
                        }
                        if (!string.IsNullOrEmpty(saveValue))
                        {
                            var node = new MetaNode(jProperty.Name, parent, nodeType, depth, saveValue, _fileName, _version);
                            var errorString = newItem?.Add(node);
                            if (!string.IsNullOrEmpty(errorString)) textLog.AppendLine(Environment.NewLine + _fileName + " node add error: " + Environment.NewLine + " Node [" + jProperty.Name + "] " + errorString + Environment.NewLine);
                        }

                        foreach (var child in jProperty.Children()) ParseJsonObject(child, depth + 1, jProperty.Path, childNode);
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
            }
        }

        private async Task CollectKeywords(IReadOnlyCollection<JsoncDictionary> sourceCollection, TreeNode parentNode)
        {
            progressBar1.Maximum = sourceCollection.Count;
            progressBar1.Value = 0;

            await Task.Run(() =>
            {
                foreach (var fileType in sourceCollection)
                {
                    var fileName = "";
                    foreach (var s in JsoncDictionary.FileNames)
                    {
                        if (s.Value != fileType.Type) continue;
                        fileName = s.Key;
                        break;
                    }

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

                    //if (currentFileNode?.Count() > 1)
                    {
                        //not possible
                    }

                    // collect all schema texts to search for keywords. simple but ineffective. temporary solution
                    // rewrite to use schema dictionary
                    var filesList = new List<string>();
                    var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
                    filesList.AddRange(Directory.GetFiles(currentDirectory, fileName.Replace(".jsonc", ".json" + ".original"),
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
                    }

                    foreach (var record in fileType.Nodes)
                    {


                        TreeNode[] parNodeList;
                        if (record.ParentName == "action")
                            record.ParentName = "actions";
                        if (record.Name == "action")
                            record.Name = "actions";

                        if (record.ParentName == fileName)
                        {
                            parNodeList = currentFileNode;
                        }
                        else
                        {
                            parNodeList = currentFileNode[0].Nodes.Find(record.ParentName, true);
                            if (parNodeList == null || !parNodeList.Any())
                            {
                                var newNode = new TreeNode(record.ParentName)
                                {
                                    Name = record.ParentName,
                                };
                                currentFileNode[0].Nodes.Add(newNode);
                                parNodeList = currentFileNode[0].Nodes.Find(record.ParentName, false);
                            }
                        }
                        //if (parNodeList?.Count() > 1)
                        {
                            //very often
                            //try to find proper parent to attach value
                        }

                        var childNodesList = parNodeList[0].Nodes.Find(record.Name, false);
                        if (childNodesList == null || !childNodesList.Any())
                        {
                            var newNode = new TreeNode(record.Name)
                            {
                                Name = record.Name,
                            };
                            parNodeList[0].Nodes.Add(newNode);
                            childNodesList = parNodeList[0].Nodes.Find(record.Name, false);
                        }
                        //if (chldNodesList?.Count() > 1)
                        {
                            //not likely
                        }

                        if (record.Type != JsoncNodeType.Property) continue;


                        foreach (var prop in record.ExamplesList)
                        {
                            var propValue = prop.Key.Trim();
                            if (propValue.StartsWith("{") || propValue.StartsWith("[")) continue;
                            if (!schemaData.Contains(propValue)) continue;

                            var propNodesList = childNodesList[0].Nodes.Find(propValue, false);
                            if (propNodesList == null || !propNodesList.Any())
                            {
                                var newNode = new TreeNode(propValue)
                                {
                                    Name = propValue,
                                };
                                childNodesList[0].Nodes.Add(newNode);
                                //propNodesList = chldNodesList[0].Nodes.Find(record.ParentName, false);
                            }
                            //if (propNodesList?.Count() > 1)
                            {
                                //not likely
                            }

                        }

                    }

                    Invoke((MethodInvoker)delegate
                    {
                        progressBar1.Value++;
                    });
                }
            }).ConfigureAwait(true);
        }

        private void FillGrid(TreeNode currentNode, string filterVersion = DefaultVersionCaption, string searchString = "", SearchCondition condition = SearchCondition.Contains, bool caseSensitive = false)
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

            var obj = _metaDictionary?.Where(x => x.Type == fileType).ToArray();

            if (obj == null) return;

            if (obj.Count() > 1)
            {
                textLog.AppendLine(Environment.NewLine
                    + "More than 1 similar file types found on example print-out:"
                    + Environment.NewLine
                    + obj.Select(n => n.Nodes).Aggregate("", (current, next) => current + ", " + next)
                    + Environment.NewLine);
            }
            var element = obj.FirstOrDefault();

            var obj2 = element?.Nodes?.Where(n =>
                n.Name == currentNode.Text
                && n.Depth == tokens.Length - 3
                && n.ParentName == tokens[tokens.Length - 2]);

            if (obj2 == null) return;

            if (_lastSelectedNode != null) _lastSelectedNode.NodeFont = new Font(treeView_json.Font.FontFamily, treeView_json.Font.Size, FontStyle.Regular);

            _lastSelectedNode = currentNode;
            currentNode.NodeFont = new Font(treeView_json.Font.FontFamily, treeView_json.Font.Size, FontStyle.Bold);

            textBox_searchHistory.Clear();

            _examplesTable.Rows.Clear();
            dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            comboBox_versions.Items.Clear();
            comboBox_versions.Items.Add(DefaultVersionCaption);
            comboBox_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            comboBox_versions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in obj2)
            {
                var examples = record.ExamplesList;
                if (examples == null) continue;

                if (!versionCollection.Contains(record.Version)) versionCollection.Add(record.Version);
                if (filterVersion != DefaultVersionCaption && record.Version != filterVersion) continue;

                foreach (var example in examples)
                {
                    var exampleText = BeautifyJson(example.Key);
                    if (string.IsNullOrEmpty(searchString) && FilterCellOut(exampleText, condition, searchString, caseSensitive, true)) continue;
                    var newRow = _examplesTable.NewRow();
                    newRow[_exampleGridColumnsNames[0]] = record.Version;
                    newRow[_exampleGridColumnsNames[1]] = exampleText;
                    newRow[_exampleGridColumnsNames[2]] = example.Value;
                    _examplesTable.Rows.Add(newRow);
                }
            }
            _lastFilterValue = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
                var csString = caseSensitive ? "CS:" : "";
                textBox_searchHistory.Text += transition + csString + condition.ToString() + "]\"" + searchString + "\"";
            }

            comboBox_versions.Items.AddRange(versionCollection.ToArray() as string[]);
            comboBox_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;
        }

        private void FilterExamples(SearchCondition condition, string searchString, string filterVersion = DefaultVersionCaption, bool caseSensitive = false, bool compactJson = false)
        {
            if (_lastFilterValue == searchString && _lastSelectedCondition == condition && _lastCaseSensitive == caseSensitive) return;

            _lastFilterValue = searchString;
            _lastSelectedCondition = condition;
            _lastCaseSensitive = caseSensitive;

            if (string.IsNullOrEmpty(searchString))
            {
                FillGrid(_lastSelectedNode, filterVersion);
                return;
            }

            var rowsNumber = _examplesTable.Rows.Count;
            var currentRowNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            for (var i = 0; i < _examplesTable.Rows.Count; i++)
            {
                currentRowNumber++;
                var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                if (newProgress >= oldProgress + MinProgressDisplay) progressBar1.Value = oldProgress = newProgress;
                var cellValue = _examplesTable.Rows[i].ItemArray[1];

                if (cellValue == null || FilterCellOut(cellValue.ToString(), condition, searchString, caseSensitive, compactJson))
                {
                    _examplesTable.Rows.RemoveAt(i);
                    i--;
                }
            }

            var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
            var csString = caseSensitive ? "CS:" : "";
            textBox_searchHistory.Text += transition + csString + condition.ToString() + "]\"" + searchString + "\"";
        }

        private bool FilterCellOut(string cellValue, SearchCondition condition, string searchString, bool caseSensitive, bool compactJson)
        {
            if (cellValue == null || string.IsNullOrEmpty(cellValue)) return true;

            if (string.IsNullOrEmpty(searchString)) return false;

            var caseSens = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            switch (condition)
            {
                case SearchCondition.Contains:
                    if ((compactJson ? CompactJson(cellValue) : cellValue)
                        .IndexOf(searchString, caseSens) < 0)
                        return true;
                    break;
                case SearchCondition.StartsWith:
                    if (!(compactJson
                        ? CompactJson(cellValue)
                        : cellValue.Trim()).StartsWith(searchString, caseSens))
                        return true;
                    break;
                case SearchCondition.EndsWith:
                    if (!(compactJson
                        ? CompactJson(cellValue)
                        : cellValue.Trim()).EndsWith(searchString, caseSens))
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

        private void FilterExamplesVersion(string versionString)
        {
            if (_lastSelectedVersion != DefaultVersionCaption) FillGrid(_lastSelectedNode, versionString);

            comboBox_versions.SelectedIndexChanged -= ComboBox_versions_SelectedIndexChanged;
            if (comboBox_versions.Items.Contains(versionString))
                comboBox_versions.SelectedItem = _lastSelectedVersion = versionString;
            else
                comboBox_versions.SelectedItem = versionString = _lastSelectedVersion = DefaultVersionCaption;
            comboBox_versions.SelectedIndexChanged += ComboBox_versions_SelectedIndexChanged;

            if (versionString == DefaultVersionCaption) return;

            var rowsNumber = _examplesTable.Rows.Count;
            var currentRowNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            for (var i = 0; i < _examplesTable.Rows.Count; i++)
            {
                currentRowNumber++;
                var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                if (newProgress >= oldProgress + 5) progressBar1.Value = oldProgress = newProgress;

                var cellValue = _examplesTable.Rows[i].ItemArray[0];
                if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                {
                    _examplesTable.Rows.RemoveAt(i);
                    i--;
                }
                else if (cellValue.ToString() != versionString)
                {
                    _examplesTable.Rows.RemoveAt(i);
                    i--;
                }
            }
        }

        private void ReadjustRow(int rowNumber)
        {
            if (dataGridView_examples.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
            {
                LoopThroughRows(dataGridView_examples);
                dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            var row = dataGridView_examples.Rows[rowNumber];
            var newHeight = row.GetPreferredHeight(rowNumber, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);

            if (newHeight == row.Height && newHeight <= dataGridView_examples.Height * CellHeightAdjust) return;

            if (newHeight > dataGridView_examples.Height * CellHeightAdjust) newHeight = (ushort)(dataGridView_examples.Height * CellHeightAdjust);
            row.Height = newHeight;

        }

        // needed to get rid of exception on changing "dataGridView_examples.AutoSizeRowsMode"
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization | System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void LoopThroughRows(DataGridView dgv)
        {
            var rows = dgv.Rows;
            for (var i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
            }
        }

        private void ActivateUiControls(bool active, bool processTable = true)
        {
            if (processTable)
            {
                if (active)
                {
                    dataGridView_examples.DataSource = _examplesTable;
                    dataGridView_examples.Invalidate();
                }
                else
                {
                    dataGridView_examples.DataSource = null;
                }
            }

            if (active)
            {
                textBox_logText.Text += textLog.ToString();
                textLog.Clear();
                textBox_logText.SelectionStart = textBox_logText.Text.Length;
                textBox_logText.ScrollToCaret();
            }

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
            treeView_json.Enabled = active;
            button_reAdjust.Enabled = active;
            Refresh();
        }

        private static bool FindTextLines(string text, string sample, out int startLine, out int lineNum)
        {
            startLine = 0;
            lineNum = 0;
            var compactText = TrimJson(text, true);
            var compactSample = TrimJson(sample, true);
            var startIndex = compactText.IndexOf(compactSample, StringComparison.Ordinal);
            if (startIndex < 0) return false;

            startLine = CountLines(compactText, 0, startIndex);
            lineNum = CountLines(compactText, startIndex, startIndex + compactSample.Length);

            return true;
        }

        private static int CountLines(string text, int startIndex, int endIndex)
        {
            var linesCount = 0;
            for (; startIndex < endIndex; startIndex++)
            {
                if (text[startIndex] != '\r' && text[startIndex] != '\n') continue;

                linesCount++;
                if (text[startIndex] != text[startIndex + 1] && (text[startIndex + 1] == '\r' || text[startIndex + 1] == '\n')) startIndex++;
            }

            return linesCount;
        }

        // possibly need rework
        private static string JsonShiftBrackets(string original)
        {
            var searchTokens = new[] { ": {", ": [" };
            foreach (var token in searchTokens)
            {
                var i = original.IndexOf(token, StringComparison.Ordinal);
                int currentPos;
                while (i >= 0)
                {
                    if (original[i + token.Length] != '\r' && original[i + token.Length] != '\n') // not a single bracket
                    {
                        currentPos = i + 3;
                    }
                    else // need to shift bracket down the line
                    {
                        var j = i - 1;
                        var trail = 0;

                        if (j >= 0)
                            while (original[j] != '\n' && original[j] != '\r' && j >= 0)
                            {
                                if (original[j] == ' ') trail++;
                                else trail = 0;
                                j--;
                            }

                        if (j < 0) j = 0;

                        if (!(original[j] == '/' && original[j + 1] == '/')) // if it's a comment
                            original = original.Insert(i + 2, Environment.NewLine + new string(' ', trail));
                        currentPos = i + 3;
                    }

                    i = original.IndexOf(token, currentPos, StringComparison.Ordinal);
                }
            }

            return original;
        }

        // possibly need rework
        private static string JsonShiftBrackets_v2(string original)
        {
            var searchTokens = new[] { ": {", ": [" };
            foreach (var token in searchTokens)
            {
                var i = original.IndexOf(token, StringComparison.Ordinal);
                while (i >= 0)
                {
                    var currentPos = 0;
                    if (original[i + token.Length] != '\r' && original[i + token.Length] != '\n') // not a single bracket
                    {
                        currentPos = i + 3;
                    }
                    else // need to shift bracket down the line
                    {
                        var j = i - 1;
                        var trail = 0;

                        if (j >= 0)
                            while (original[j] != '\n' && original[j] != '\r' && j >= 0)
                            {
                                if (original[j] == ' ') trail++;
                                else trail = 0;
                                j--;
                            }

                        if (j < 0) j = 0;

                        if (!(original[j] == '/' && original[j + 1] == '/')) // if it's a comment
                            original = original.Insert(i + 2, Environment.NewLine + new string(' ', trail));
                        currentPos = i + 3;
                    }

                    i = original.IndexOf(token, currentPos, StringComparison.Ordinal);
                }
            }

            var stringList = ConvertTextToStringList(original);

            const char prefixItem = ' ';
            const int prefixStep = 2;
            var openBrackets = new[] { '{', '[' };
            var closeBrackets = new[] { '}', ']' };

            var prefixLength = 0;
            var prefix = "";
            var result = new StringBuilder();
            for (var i = 0; i < stringList.Count(); i++)
            {
                stringList[i] = stringList[i].Trim();
                if (closeBrackets.Contains(stringList[i][0]))
                {
                    prefixLength -= prefixStep;
                    prefix = new string(prefixItem, prefixLength);
                }

                result.AppendLine(prefix + stringList[i]);

                if (openBrackets.Contains(stringList[i][0]))
                {
                    prefixLength += prefixStep;
                    prefix = new string(prefixItem, prefixLength);
                    if (stringList[i].Length > 1 && closeBrackets.Contains(stringList[i][stringList[i].Length - 1]))
                    {
                        prefixLength -= prefixStep;
                        prefix = new string(prefixItem, prefixLength);
                    }
                }
            }

            return result.ToString().Trim();
        }

        private static string[] ConvertTextToStringList(string data)
        {
            var lineDivider = new List<char>() { '\x0d', '\x0a' };
            var stringCollection = new List<string>();
            var unparsedData = "";
            foreach (var t in data)
                if (lineDivider.Contains(t))
                {
                    if (unparsedData.Length > 0)
                    {
                        stringCollection.Add(unparsedData);
                        unparsedData = "";
                    }
                }
                else
                {
                    unparsedData += t;
                }

            if (unparsedData.Length > 0) stringCollection.Add(unparsedData);
            return stringCollection.ToArray();
        }

        private static string TrimJson(string original, bool trimEol)
        {
            original = original.Trim();
            if (trimEol)
            {
                original = original.Replace("\r\n", "\n");
                original = original.Replace('\r', '\n');
            }

            var i = original.IndexOf("\n ", StringComparison.Ordinal);
            while (i >= 0)
            {
                original = original.Replace("\n ", "\n");
                i = original.IndexOf("\n ", i, StringComparison.Ordinal);
            }

            if (trimEol) return original;

            i = original.IndexOf("\r ", StringComparison.Ordinal);
            while (i >= 0)
            {
                original = original.Replace("\r ", "\r");
                i = original.IndexOf("\r ", i, StringComparison.Ordinal);
            }

            return original;
        }

        private static string CompactJson(string json)
        {
            json = json.Trim();
            try
            {
                return ReformatJson(json, Formatting.None);
            }
            catch
            {
                return json;
            }
        }

        public static string BeautifyJson(string json)
        {
            json = json.Trim();
            try
            {
                json = ReformatJson(json, Formatting.Indented);
            }
            catch
            {
            }
            return _reformatJson ? JsonShiftBrackets_v2(json) : json;
        }

        private static string ReformatJson(string json, Formatting formatting)
        {
            using (var stringReader = new StringReader(json))
            {
                using (var stringWriter = new StringWriter())
                {
                    ReformatJson(stringReader, stringWriter, formatting);
                    return stringWriter.ToString();
                }
            }
        }

        private static void ReformatJson(TextReader textReader, TextWriter textWriter, Formatting formatting)
        {
            using (var jsonReader = new JsonTextReader(textReader))
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.Formatting = formatting;
                    jsonWriter.WriteToken(jsonReader);
                }
            }
        }

        private static string GetLocalUrlPath(string url)
        {
            if (!url.Contains("://") || !url.Contains("/")) return "";

            url = url.Replace("://", "");
            var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
            var localPath = currentDirectory + url.Substring(url.IndexOf('/'));
            localPath = localPath.Replace('/', '\\');
            return localPath;
        }

        #endregion

    }
}
