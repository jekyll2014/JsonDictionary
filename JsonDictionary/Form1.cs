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
using System.Threading.Tasks;
using System.Windows.Forms;
using JsonDictionary.Properties;

namespace JsonDictionary
{
    public partial class Form1 : Form
    {
        // pre-defined constants
        private readonly string[] _exampleGridColumnsNames = { "Version", "Example", "File Name" };
        private const string DefaultVersionCaption = "Any";
        private const string VersionTagName = "contentVersion";
        private readonly string[] _suppressErrors = { "#/imports[0]" };
        private readonly string _schemaTag = "\"$schema\": \"";
        private const string RootNodeName = "root";

        // experimental options
        private readonly bool _reformatJson;
        private readonly bool _collectAllFileNames;

        // global variables
        private string _version = "";
        private string _fileName = "";
        private JsoncContentType _fileType = JsoncContentType.DataViews;
        private TreeNode _rootNode;
        DataTable _examplesTable;
        private List<JsoncDictionary> _metaDictionary = new List<JsoncDictionary>();
        private Dictionary<string, string> _schemaList = new Dictionary<string, string>();

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

            comboBox_condition.Items.AddRange(items: typeof(SearchCondition).GetEnumNames() as string[]);
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
                //_examplesTable.Columns[i].ReadOnly = true;
            }

            var col0 = new DataGridViewTextBoxColumn
            {
                DataPropertyName = _exampleGridColumnsNames[0],
                Name = _exampleGridColumnsNames[0]
            };
            dataGridView_examples.Columns.Add(col0);

            var col1 = new DataGridViewTextBoxColumn
            {
                DataPropertyName = _exampleGridColumnsNames[1],
                Name = _exampleGridColumnsNames[1]
            };
            dataGridView_examples.Columns.Add(col1);

            var col2 = new DataGridViewLinkColumn
            {
                DataPropertyName = _exampleGridColumnsNames[2],
                Name = _exampleGridColumnsNames[2]
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
                    MessageBox.Show("File read exception [" + openFileDialog1.FileName + "]: " + ex.Message);
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
            treeView_json.Sort();
            treeView_json.Nodes[0].Expand();
            ActivateUiControls(true);
        }

        private async void Button_collectDatabase_Click(object sender, EventArgs e)
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

            textBox_logText.Text += "Files parsed: " + filesList.Count.ToString() + "\r\n";
            textBox_logText.SelectionStart = textBox_logText.Text.Length;
            textBox_logText.ScrollToCaret();

            treeView_json.Nodes.Add(_rootNode);
            treeView_json.Sort();
            treeView_json.Nodes[0].Expand();
            ActivateUiControls(true);
        }

        private async void Button_validateFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            ActivateUiControls(false);

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

                await Task.Run(() =>
                {
                    ValidateFile(file);
                }).ConfigureAwait(true);
            }

            textBox_logText.Text += "Files validated: " + filesList.Count.ToString() + "\r\n";
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File write exception [" + saveFileDialog1.FileName + "]: " + ex.Message);
                }
            }).ConfigureAwait(true);
        }

        private void DataGridView_examples_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_collectAllFileNames && e.ColumnIndex == 2)
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
                    MessageBox.Show("File execution exception [" + fileName + "]: " + ex.Message);
                }
            }
            else if (e.ColumnIndex == 1)
            {
                var cell = dataGridView_examples.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dataGridView_examples.CurrentCell = cell;
                dataGridView_examples.CurrentCell.ReadOnly = false;
                dataGridView_examples.BeginEdit(true);
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

            //Enum.TryParse(comboBox_condition.SelectedItem.ToString(), out SearchCondition condition);
            FilterExamples(condition, searchString, caseSensitive);
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
            var paramValue = TrimMultiLineText(dataGridView_examples.SelectedCells[0].Value.ToString());

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
            {
                FindAllStripMenuItem.Enabled = false;
            }
            else FindAllStripMenuItem.Enabled = true;
        }

        private void ContextMenuStrip_findField_Opening(object sender, CancelEventArgs e)
        {
            if (treeView_json.SelectedNode?.Parent?.Parent?.Parent == null || treeView_json.SelectedNode.Parent.Parent.Text == RootNodeName) FindFieldToolStripMenuItem.Enabled = false;
            else FindFieldToolStripMenuItem.Enabled = true;
        }

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
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "\r\nFile read exception: [" + fullFileName + "]:\r\n" + ex.Message + "\r\n\r\n";
                });

                return;
            }

            var versionIndex = jsonText.IndexOf(_schemaTag, StringComparison.Ordinal);
            if (versionIndex <= 0) return;

            versionIndex += _schemaTag.Length;
            var strEnd = versionIndex;
            while (strEnd < jsonText.Length && jsonText[strEnd] != '"' && jsonText[strEnd] != '\r' && jsonText[strEnd] != '\n') strEnd++;

            var schemaUrl = jsonText.Substring(versionIndex, strEnd - versionIndex);

            if (!schemaUrl.EndsWith(".json"))
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "\r\n" + fullFileName + ": URL not found [" + schemaUrl + "]\r\n\r\n";
                });
                return;
            }

            if (!_schemaList.ContainsKey(schemaUrl))
            {
                try
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var schemaData = webClient.DownloadString(schemaUrl);
                        _schemaList.Add(schemaUrl, schemaData);
                    }
                }
                catch (Exception ex)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        textBox_logText.Text += "\r\n" + fullFileName + "schema download exception: [" + schemaUrl + "]:\r\n" + ex.Message + "\r\n\r\n";
                    });
                    _schemaList.Add(schemaUrl, "");
                }
            }
            var schemaText = _schemaList[schemaUrl];

            if (string.IsNullOrEmpty(schemaText))
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "\r\n" + fullFileName + " schema not loaded : [" + schemaUrl + "]:\r\n\r\n";
                });

                return;
            }

            ICollection<ValidationError> errors;
            try
            {
                var schema = JsonSchema.FromJsonAsync(schemaText).Result;
                errors = schema.Validate(jsonText);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "\r\nFile validation exception: [" + fullFileName + "]:\r\n" + ex.Message + "\r\n\r\n";
                });

                return;
            }

            foreach (var error in errors)
            {
                if (!_suppressErrors.Contains(error.Path))
                    Invoke((MethodInvoker)delegate
                    {
                        textBox_logText.Text += fullFileName + ": line " + error.LineNumber + " " + error.Path + ": " + error.Kind + "\r\n";
                    });
            }
        }

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

                dynamic jsonObject = JsonConvert.DeserializeObject(jsonStr);
                if (jsonObject != null) ParseJsonObject(jsonObject, 0, shortFileName, childNode);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    // File.WriteAllText(fullFileName + ".error", jsonShiftBrackets(File.ReadAllText(fullFileName)));
                    textBox_logText.Text += "\r\nFile parse exception: " + _fileName + "]:\r\n" + ex.Message + "\r\n\r\n";
                });
            }
        }

        private void ParseJsonObject(JToken token, int depth, string parent, TreeNode parentNode)
        {
            JsoncDictionary newItem;
            try
            {
                var obj = _metaDictionary.Where(n => n.Type == _fileType).ToArray();
                if (obj.Count() > 1)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        textBox_logText.Text += "\r\nMore than 1 similar file types found on parse\r\n\r\n";
                    });
                }
                newItem = obj.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "\r\nContent parse exception: " + _fileName + "]:\r\n" + ex.Message + "\r\n\r\n";
                });

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
                            Invoke((MethodInvoker)delegate
                            {
                                textBox_logText.Text += "\r\nMore than 1 similar object found in the tree:\r\n" + obj.Select(n => n.FullPath).Aggregate("", (current, next) => current + ", " + next) + "\r\n\r\n";
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
                            var node = new MetaNode(jProperty.Name, parent, nodeType, depth, _reformatJson ? JsonShiftBrackets(saveValue) : saveValue, _fileName,
                                _version);
                            var errorString = newItem?.Add(node);
                            if (!string.IsNullOrEmpty(errorString))
                            {
                                Invoke((MethodInvoker)delegate
                                {
                                    textBox_logText.Text += "\r\nNode add error: " + _fileName + "]:\r\n Node [" + jProperty.Name + "] " + errorString + "\r\n\r\n";
                                });
                            }
                        }

                        foreach (var child in jProperty.Children())
                        {
                            ParseJsonObject(child, depth + 1, jProperty.Path, childNode);
                        }

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

                        foreach (var child in jObject.Children())
                        {
                            ParseJsonObject(child, depth, newParent, parentNode);
                        }

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


                        foreach (var child in jArray.Children())
                        {
                            ParseJsonObject(child, depth, newParent, parentNode);
                        }

                        break;
                    }
            }
        }

        private void FillGrid(TreeNode currentNode)
        {
            if (currentNode == null) return;

            var tokens = currentNode.FullPath.Split('\\');

            if (tokens.Length < 3) return;

            if (!JsoncDictionary.FileNames.TryGetValue(tokens[1], out var fileType)) return;

            var obj = _metaDictionary?.Where(x => x.Type == fileType).ToArray();

            if (obj == null) return;

            if (obj.Count() > 1)
            {
                Invoke((MethodInvoker)delegate
                {
                    textBox_logText.Text += "\r\nMore than 1 similar file types found on example print-out:\r\n" + obj.Select(n => n.Nodes).Aggregate("", (current, next) => current + ", " + next) + "\r\n\r\n";
                });
            }
            var element = obj.FirstOrDefault();

            var obj2 = element?.Nodes?.Where(n =>
                n.Name == currentNode.Text && n.Depth == tokens.Length - 3 &&
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
                    newRow[_exampleGridColumnsNames[0]] = record.Version;
                    newRow[_exampleGridColumnsNames[1]] = example.Key;
                    newRow[_exampleGridColumnsNames[2]] = example.Value;
                    _examplesTable.Rows.Add(newRow);
                }
            }

            comboBox_versions.Items.AddRange(versionCollection.ToArray() as string[]);
            this.comboBox_versions.SelectedIndexChanged += this.ComboBox_versions_SelectedIndexChanged;
            dataGridView_examples.DataSource = _examplesTable;
            dataGridView_examples.Invalidate();
        }

        private void FilterExamples(SearchCondition condition, string searchString, bool caseSensitive = false, bool trimMultiline = false)
        {
            if (_lastFilterValue == searchString && _lastSelectedCondition == condition) return; // && _lastCaseSensitive == caseSensitive

            _lastFilterValue = searchString;
            _lastSelectedCondition = condition;
            _lastCaseSensitive = caseSensitive;

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
                case SearchCondition.Contains:
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
                case SearchCondition.StartsWith:
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
                case SearchCondition.EndsWith:
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
                        MessageBox.Show("Incorrect filter condition: " + condition.ToString());
                    });
                    break;
            }

            var transition = textBox_searchHistory.Text == "" ? "[" : " ->[";
            var csString = caseSensitive ? "CS:" : "";
            textBox_searchHistory.Text += transition + csString + comboBox_condition.SelectedItem.ToString() + "]\"" + searchString + "\"";
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

            for (var i = 0; i < _examplesTable.Rows.Count; i++)
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

        private void ActivateUiControls(bool active)
        {
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
            this.Refresh();
        }

        // possibly to rework
        private string JsonShiftBrackets(string original)
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
                        {
                            while (original[j] != '\n' && original[j] != '\r' && j >= 0)
                            {
                                if (original[j] == ' ') trail++;
                                else trail = 0;
                                j--;
                            }
                        }
                        if (j < 0) j = 0;

                        if (!(original[j] == '/' && original[j + 1] == '/')) // if it's a comment
                        {
                            original = original.Insert(i + 2, "\r\n" + new string(' ', trail));
                        }
                        currentPos = i + 3;
                    }

                    i = original.IndexOf(token, currentPos, StringComparison.Ordinal);
                }
            }

            return original;
        }

        // simple version (2 times slower but no way to get exception)
        private string TrimMultiLineText_v1(string original)
        {
            original = original.Trim();
            var i = original.IndexOf("\n ", StringComparison.Ordinal);
            while (i >= 0)
            {
                original = original.Replace("\n ", "\n");
                i = original.IndexOf("\n ", i, StringComparison.Ordinal);
            }

            i = original.IndexOf("\r ", StringComparison.Ordinal);
            while (i >= 0)
            {
                original = original.Replace("\r ", "\r");
                i = original.IndexOf("\r ", i, StringComparison.Ordinal);
            }

            return original;
        }

        // TrimMultiLineText fast replacement (not tested properly for exceptions)
        private string TrimMultiLineText(string json)
        {
            try
            {
                return ReformatJson(json, Formatting.None);
            }
            catch
            {
                return json;
            }
        }

        private string Beautify(string json)
        {
            return ReformatJson(json, Formatting.Indented);
        }

        private string ReformatJson(string json, Formatting formatting)
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

        private void ReformatJson(TextReader textReader, TextWriter textWriter, Formatting formatting)
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

        #endregion

    }
}
