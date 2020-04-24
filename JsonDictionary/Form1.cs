using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonDictionary
{
    public partial class Form1 : Form
    {
        // experimental options
        private bool _reformatJson = true;
        private bool _collectAllFileNames = false;

        private string _version = "";
        private string _fileName = "";
        private JsoncContentType _fileType = JsoncContentType.DataViews;
        private TreeNode _rootNode;
        private string _oldFilterValue = "";
        private int _lastSelectedCondition = -1;
        private TreeNode _lastSelectedNode;
        private string _lastSelectedVersion;

        private readonly string[] _exampleGridColumns = { "Version", "Example", "File Name" };

        private const string DefaultVersionCaption = "Any";

        private const string VersionTagName = "contentVersion";

        private const string RootNodeName = "root";

        DataTable _examplesTable;

        private List<JsoncDictionary> _metaDictionary = new List<JsoncDictionary>();

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

            comboBox_condition.SelectedIndex = 0;

            foreach (var fileName in JsoncDictionary.FileNames)
            {
                checkedListBox_params.Items.Add(fileName.Key);
                checkedListBox_params.SetItemChecked(checkedListBox_params.Items.Count - 1, true);
            }

            _examplesTable = new DataTable("Examples");
            _examplesTable.Columns.Add(_exampleGridColumns[0]);
            _examplesTable.Columns.Add(_exampleGridColumns[1]);
            _examplesTable.Columns.Add(_exampleGridColumns[2]);
            _examplesTable.Columns[0].ReadOnly = true;
            _examplesTable.Columns[1].ReadOnly = true;
            _examplesTable.Columns[2].ReadOnly = true;

            DataGridViewTextBoxColumn col0 = new DataGridViewTextBoxColumn
            {
                DataPropertyName = _exampleGridColumns[0],
                Name = _exampleGridColumns[0]
            };
            dataGridView_examples.Columns.Add(col0);

            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn
            {
                DataPropertyName = _exampleGridColumns[1],
                Name = _exampleGridColumns[1]
            };
            dataGridView_examples.Columns.Add(col1);

            DataGridViewLinkColumn col2 = new DataGridViewLinkColumn
            {
                DataPropertyName = _exampleGridColumns[2],
                Name = _exampleGridColumns[2]
            };
            dataGridView_examples.Columns.Add(col2);

            dataGridView_examples.DataError += delegate { };

            comboBox_versions?.Items.Clear();
            comboBox_versions?.Items.Add(DefaultVersionCaption);
            _lastSelectedVersion = DefaultVersionCaption;

            dataGridView_examples.ContextMenuStrip = contextMenuStrip_findValue;
            treeView_json.ContextMenuStrip = contextMenuStrip_findField;
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
            List<JsoncDictionary> dataCollection = null;
            TreeNode nodeCollection = null;

            progressBar1.Maximum = 2;
            progressBar1.Value = 0;
            await Task.Run(() =>
            {
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        dataCollection = JsonIo.LoadBinary<List<JsoncDictionary>>(openFileDialog1.FileName);
                        progressBar1.Value = 1;

                        nodeCollection = JsonIo.LoadBinary<TreeNode>(openFileDialog1.FileName + ".tree");
                        progressBar1.Value = 2;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file " + openFileDialog1.FileName + ": " + ex.Message);
                }

                if (dataCollection == null || nodeCollection == null)
                {
                    return;
                }

                _metaDictionary = dataCollection;
                _rootNode = nodeCollection;
            }).ConfigureAwait(true);

            treeView_json.Nodes.Clear();
            treeView_json.Nodes.Add(_rootNode);
            treeView_json.Nodes[0].Expand();
            ActivateUiControls(true);
        }

        private async void Button_openDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ActivateUiControls(false);

            treeView_json.Nodes.Clear();
            _rootNode = new TreeNode(RootNodeName);
            _metaDictionary = new List<JsoncDictionary>();

            var startPath = folderBrowserDialog1.SelectedPath;

            var filesList = new List<string>();
            foreach (var fileName in checkedListBox_params.CheckedItems)
            {
                filesList.AddRange(Directory.GetFiles(startPath, fileName.ToString(), SearchOption.AllDirectories));
            }

            var filesNumber = filesList.Count;
            var currentFileNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            foreach (var file in filesList)
            {
                currentFileNumber++;
                var newProgress = (ushort)((float)currentFileNumber / (float)filesNumber * 100);
                if (newProgress >= oldProgress + 5)
                {
                    progressBar1.Value = oldProgress = newProgress;
                }

                _version = "";
                await Task.Run(() =>
                {
                    DeserializeFile(file, file.Substring(file.LastIndexOf('\\') + 1), _rootNode);
                }).ConfigureAwait(true);
            }

            treeView_json.Nodes.Add(_rootNode);
            treeView_json.Nodes[0].Expand();
            ActivateUiControls(true);
        }

        private void Button_saveDb_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save data as JSON...";
            saveFileDialog1.DefaultExt = "json";
            saveFileDialog1.Filter = "Binary files|*.metalib|All files|*.*";
            saveFileDialog1.FileName = "metaUIdictionary_" + DateTime.Today.ToShortDateString().Replace("/", "_") + ".metalib";
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error writing to file " + saveFileDialog1.FileName + ": " + ex.Message);
                }
            }).ConfigureAwait(true);
        }

        private void DataGridView_examples_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_collectAllFileNames && e.ColumnIndex == 2)
            {
                var fileName = dataGridView_examples.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                try
                {
                    var process = new Process
                    {
                        StartInfo =
                    {
                        FileName = fileName
                    }
                    };
                    process.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error running file: " + fileName + "\r\n" + ex.Message);
                }
            }
            else if (e.ColumnIndex == 1)
            {
                DataGridViewCell cell = dataGridView_examples.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dataGridView_examples.CurrentCell = cell;
                dataGridView_examples.CurrentCell.ReadOnly = false;
                dataGridView_examples.BeginEdit(true);
            }
        }

        private void TextBox_searchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            ActivateUiControls(false);
            dataGridView_examples.DataSource = null;
            FilterExamples(comboBox_condition.SelectedIndex, textBox_searchString.Text, checkBox_seachCaseSensitive.Checked);
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
            if (dataGridView_examples.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
            {
                LoopThroughRows(dataGridView_examples);
                dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            ReadjustRow(e.RowIndex);
        }

        private void Button_readjust_Click(object sender, EventArgs e)
        {
            ActivateUiControls(false);
            if (dataGridView_examples.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
            {
                LoopThroughRows(dataGridView_examples);
                dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            var rowsNumber = dataGridView_examples.RowCount;
            var currentRowNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            for (var i = 0; i < dataGridView_examples.RowCount; i++)
            {
                currentRowNumber++;
                var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                if (newProgress >= oldProgress + 5)
                {
                    progressBar1.Value = oldProgress = newProgress;
                }

                ReadjustRow(i);
            }
            ActivateUiControls(true);
        }

        private void TreeView_json_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null) return;

            ActivateUiControls(false);
            FillGrid(e.Node);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void FindAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lastSelectedNode == null
                || dataGridView_examples == null
                || dataGridView_examples.SelectedCells[0].ColumnIndex != 1
                || _lastSelectedNode.Parent?.Parent?.Parent == null) return;

            var paramName = _lastSelectedNode.Text;
            var paramValue = TrimMultiLineText(dataGridView_examples.SelectedCells[0]?.Value.ToString());

            ActivateUiControls(false);
            FillGrid(_lastSelectedNode.Parent);
            FilterExamples((int)SearchCondition.Contains, "\"" + paramName + "\":", true);
            FilterExamples((int)SearchCondition.Contains, paramValue, true, true);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView_json.SelectedNode?.Parent?.Parent?.Parent == null || treeView_json.SelectedNode.Parent.Parent.Text == RootNodeName) return;

            var paramName = treeView_json.SelectedNode.Text;

            ActivateUiControls(false);
            FillGrid(treeView_json.SelectedNode.Parent);
            FilterExamples((int)SearchCondition.Contains, "\"" + paramName + "\":", true);
            dataGridView_examples.Invalidate();
            ActivateUiControls(true);
        }

        private void TreeView_json_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeView_json.SelectedNode = e.Node;
        }

        private void DataGridView_examples_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridView_examples.ClearSelection();
                dataGridView_examples.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
        }

        #endregion

        #region Helpers

        private void DeserializeFile(string fullFileName, string shortFileName, TreeNode parentNode)
        {
            try
            {
                var jsonStr = File.ReadAllText(fullFileName);

                _fileName = fullFileName;
                if (!JsoncDictionary.FileNames.TryGetValue(shortFileName, out _fileType)) return;

                TreeNode childNode;

                if (_metaDictionary.Any(n => n.Type == _fileType))
                {
                    childNode = _rootNode.Nodes[_rootNode.Nodes.Count - 1];
                }
                else
                {
                    _metaDictionary.Add(new JsoncDictionary(_fileType, _collectAllFileNames));
                    childNode = new TreeNode(shortFileName);
                    parentNode.Nodes.Add(childNode);
                }

                dynamic jsonObject = JsonConvert.DeserializeObject(_reformatJson ? ReFormatJson(jsonStr) : jsonStr);
                if (jsonObject != null) ParseJsonObject(jsonObject, 0, shortFileName, childNode);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    // File.WriteAllText(fullFileName + ".error", ReFormatJson(File.ReadAllText(fullFileName)));
                    textBox_logText.Text += "File parse error: " + _fileName + "]:\r\n" + ex.Message + "\r\n\r\n";
                });
            }
        }

        private void ParseJsonObject(JToken token, int depth, string parent, TreeNode parentNode)
        {
            JsoncDictionary newItem;
            try
            {
                var obj = _metaDictionary.Where(n => n.Type == _fileType);
                if (obj.Count() > 1)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        textBox_logText.Text += "More than 1 similar file types found on parse\r\n\r\n";
                    });
                }
                newItem = obj.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "Content parse error: " + _fileName + "]:\r\n" + ex.Message + "\r\n\r\n";
                });

                return;
            }

            if (token is JProperty jProperty)
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

                var obj = parentNode.Nodes.Cast<TreeNode>().Where(r => r.Text == jProperty.Name);

                if (obj.Count() > 1)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        textBox_logText.Text += "More than 1 similar object found in the tree:\r\n" + obj.Select(n => n.FullPath).Aggregate("", (current, next) => current + ", " + next) + "\r\n\r\n";
                    });
                }
                var exNode = obj.FirstOrDefault();

                if (exNode != null)
                {
                    childNode = exNode;
                }
                else
                {
                    childNode = new TreeNode(jProperty.Name);
                    parentNode.Nodes.Add(childNode);
                }

                if (jProperty.Name == VersionTagName)
                {
                    _version = printValue;
                }

                if (!JsoncDictionary.NodeTypes.TryGetValue(token.GetType().Name, out var nodeType)) nodeType = JsoncNodeType.Unknown;
                if (!string.IsNullOrEmpty(saveValue))
                {
                    var node = new MetaNode(jProperty.Name, parent, nodeType, depth, saveValue, _fileName,
                        _version);
                    newItem?.Add(node);
                }

                foreach (var child in jProperty.Children())
                {
                    ParseJsonObject(child, depth + 1, jProperty.Path, childNode);
                }
            }
            else if (token is JObject jObject)
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

                foreach (var child in jObject.Children())
                {
                    ParseJsonObject(child, depth, newParent, parentNode);
                }
            }
            else if (token is JArray jArray)
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


                foreach (var child in jArray.Children())
                {
                    ParseJsonObject(child, depth, newParent, parentNode);
                }
            }
        }

        private void FillGrid(TreeNode currentNode)
        {
            if (currentNode == null) return;

            var tokens = currentNode.FullPath.Split('\\');

            if (tokens.Length < 3) return;

            if (!JsoncDictionary.FileNames.TryGetValue(tokens[1], out var fileType)) return;

            var obj = _metaDictionary?.Where(x => x.Type == fileType);

            if (obj == null) return;

            if (obj.Count() > 1)
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "More than 1 similar file types found on example print-out:\r\n" + obj.Select(n => n.Nodes).Aggregate("", (current, next) => current + ", " + next) + "\r\n\r\n";
                });
            }
            var element = obj.FirstOrDefault();

            var obj2 = element?.Nodes?.Where(n =>
                n.Name == currentNode?.Text && n.Depth == tokens.Length - 3 &&
                n.ParentName == tokens[tokens.Length - 2]);

            if (obj2 == null) return;

            if (_lastSelectedNode != null)
            {
                _lastSelectedNode.NodeFont = new Font(treeView_json.Font.FontFamily, treeView_json.Font.Size, FontStyle.Regular);
            }

            _lastSelectedNode = currentNode;
            currentNode.NodeFont = new Font(treeView_json.Font.FontFamily, treeView_json.Font.Size, FontStyle.Bold);

            textBox_searchHistory.Clear();

            dataGridView_examples.DataSource = null;
            _examplesTable.Rows.Clear();
            dataGridView_examples.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            comboBox_versions.Items.Clear();
            comboBox_versions.Items.Add(DefaultVersionCaption);
            comboBox_versions.SelectedIndexChanged -= this.ComboBox_versions_SelectedIndexChanged;
            comboBox_versions.SelectedIndex = 0;

            var versionCollection = new List<string>();
            foreach (var record in obj2)
            {
                var examples = record.ExamplesList;

                if (examples == null) continue;

                if (!versionCollection.Contains(record.Version)) versionCollection.Add(record.Version);

                foreach (var example in examples)
                {
                    var newRow = _examplesTable.NewRow();
                    newRow[_exampleGridColumns[0]] = record.Version;
                    newRow[_exampleGridColumns[1]] = example.Key;
                    newRow[_exampleGridColumns[2]] = example.Value;
                    _examplesTable.Rows.Add(newRow);
                }
            }

            comboBox_versions.Items.AddRange(versionCollection.ToArray());
            this.comboBox_versions.SelectedIndexChanged += this.ComboBox_versions_SelectedIndexChanged;
            dataGridView_examples.DataSource = _examplesTable;
            dataGridView_examples.Invalidate();
        }

        private void FilterExamples(int condition, string searchString, bool caseSensitive = false, bool trimMultiline = false)
        {
            if (_oldFilterValue == searchString && _lastSelectedCondition == condition) return;

            _oldFilterValue = searchString;

            if (string.IsNullOrEmpty(searchString))
            {
                FillGrid(_lastSelectedNode);
                return;
            }
            var caseSens = StringComparison.OrdinalIgnoreCase;
            if (caseSensitive) caseSens = StringComparison.Ordinal;

            var rowsNumber = _examplesTable.Rows.Count;
            var currentRowNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            switch (condition)
            {
                case (int)SearchCondition.Contains:
                    for (var i = 0; i < _examplesTable.Rows.Count; i++)
                    {
                        currentRowNumber++;
                        var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                        if (newProgress >= oldProgress + 5)
                        {
                            progressBar1.Value = oldProgress = newProgress;
                        }

                        var cellValue = _examplesTable.Rows[i].ItemArray[1];
                        if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                        {
                            _examplesTable.Rows.RemoveAt(i);
                            i--;
                        }
                        else if ((trimMultiline ? TrimMultiLineText(cellValue.ToString()) : cellValue.ToString()).ToString().IndexOf(searchString, caseSens) < 0)
                        {
                            _examplesTable.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case (int)SearchCondition.StartsWith:
                    for (var i = 0; i < _examplesTable.Rows.Count; i++)
                    {
                        currentRowNumber++;
                        var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                        if (newProgress >= oldProgress + 5)
                        {
                            progressBar1.Value = oldProgress = newProgress;
                        }

                        var cellValue = _examplesTable.Rows[i].ItemArray[1];
                        if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                        {
                            _examplesTable.Rows.RemoveAt(i);
                            i--;
                        }
                        else if (!(trimMultiline ? TrimMultiLineText(cellValue.ToString()) : cellValue.ToString().Trim()).StartsWith(searchString, caseSens))
                        {
                            _examplesTable.Rows.RemoveAt(i);
                            i--;
                        }
                    }

                    break;
                case (int)SearchCondition.EndsWith:
                    for (var i = 0; i < _examplesTable.Rows.Count; i++)
                    {
                        currentRowNumber++;
                        var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                        if (newProgress >= oldProgress + 5)
                        {
                            progressBar1.Value = oldProgress = newProgress;
                        }

                        var cellValue = _examplesTable.Rows[i].ItemArray[1];
                        if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                        {
                            _examplesTable.Rows.RemoveAt(i);
                            i--;
                        }
                        else if (!(trimMultiline ? TrimMultiLineText(cellValue.ToString()) : cellValue.ToString().Trim()).EndsWith(searchString, caseSens))
                        {
                            _examplesTable.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                default:
                    Invoke((MethodInvoker)delegate
                    {
                        textBox_logText.Text += "Incorrect filter condition: " + condition.ToString() + "\r\n\r\n";
                    });
                    break;
            }

            var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
            var CsString = caseSensitive ? "CS:" : "";
            textBox_searchHistory.Text += transition + CsString + comboBox_condition.SelectedItem.ToString() + "]\"" + searchString + "\"";
        }

        private void FilterExamplesVersion(string versionString)
        {
            if (_lastSelectedVersion != DefaultVersionCaption) FillGrid(_lastSelectedNode);

            if (comboBox_versions.Items.Contains(versionString))
            {
                comboBox_versions.SelectedIndexChanged -= this.ComboBox_versions_SelectedIndexChanged;
                comboBox_versions.SelectedItem = versionString;
                this.comboBox_versions.SelectedIndexChanged += this.ComboBox_versions_SelectedIndexChanged;
            }

            _lastSelectedVersion = versionString;

            if (versionString == DefaultVersionCaption)
            {
                return;
            }

            var rowsNumber = _examplesTable.Rows.Count;
            var currentRowNumber = 0;
            var oldProgress = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            for (int i = 0; i < _examplesTable.Rows.Count; i++)
            {
                currentRowNumber++;
                var newProgress = (ushort)((float)currentRowNumber / (float)rowsNumber * 100);
                if (newProgress >= oldProgress + 5)
                {
                    progressBar1.Value = oldProgress = newProgress;
                }

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
            var row = dataGridView_examples.Rows[rowNumber];
            var newHeight = row.GetPreferredHeight(rowNumber, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, true);

            if (newHeight != row.Height || newHeight > dataGridView_examples.Height * 0.7)
            {
                if (newHeight > dataGridView_examples.Height * 0.7)
                {
                    row.Height = (ushort)(dataGridView_examples.Height * 0.7);
                }
                else
                {
                    row.Height = newHeight;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization | System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void LoopThroughRows(DataGridView dgv)
        {
            DataGridViewRowCollection rows = dgv.Rows;
            for (int i = rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = rows[i];
            }
        }

        private void ActivateUiControls(bool active)
        {
            checkedListBox_params.Enabled = active;
            button_openDirectory.Enabled = active;
            button_loadDb.Enabled = active;
            button_saveDb.Enabled = active;
            comboBox_versions.Enabled = active;
            comboBox_condition.Enabled = active;
            textBox_searchString.Enabled = active;
            checkBox_seachCaseSensitive.Enabled = active;
            dataGridView_examples.Enabled = active;
            treeView_json.Enabled = active;
            button_reAdjust.Enabled = active;
            this.Refresh();
        }

        // to rework
        private string ReFormatJson(string original)
        {
            var searchTokens = new[] { ": {", ": [" };
            foreach (var token in searchTokens)
            {

                var i = original.IndexOf(token);
                while (i > 0)
                {
                    if (original[i + token.Length] != '\r' && original[i + token.Length] != '\n')
                    {
                        original = original.Insert(i + 2, " ");
                    }
                    else
                    {

                        var j = i - 1;
                        var trail = "";
                        while (original[j] != '\n' && original[j] != '\r')
                        {
                            j--;
                            if (j <= 0) break;
                        }

                        j++;
                        while (original[j] == ' ')
                        {
                            trail += " ";
                            j++;
                        }

                        if (!(original[j] == '/' && original[j + 1] == '/'))
                        {
                            original = original.Insert(i + 2, "\r\n" + trail);
                        }
                        else
                        {
                            original = original.Insert(i + 2, " ");
                        }
                    }

                    i = original.IndexOf(token);
                }
            }

            return original;
        }

        // to rework
        private string TrimMultiLineText(string original)
        {
            original = original.Trim();
            var i = original.IndexOf("\n ");
            while (i >= 0)
            {
                original = original.Replace("\n ", "\n");
                i = original.IndexOf("\n ");
            }

            i = original.IndexOf("\n ");
            while (i >= 0)
            {
                original = original.Replace("\n ", "\n");
                i = original.IndexOf("\n ");
            }

            return original;
        }
        #endregion

    }
}
