using ScintillaNET;

using ScintillaNETviewer.Utils;

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JsonDictionary
{
    public partial class JsonViewer : Form
    {
        /// <summary>
        /// the background color of the text area
        /// </summary>
        private readonly Color BACK_COLOR = Color.DarkCyan;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private readonly Color FORE_COLOR = Color.White;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;

        public JsonViewer()
        {
            InitializeComponent();
        }

        public JsonViewer(string fileName, string text)
        {
            InitializeComponent();
            this._fileName = fileName;
            this._text = text;
        }

        public string EditorText => _textArea.Text;

        private Scintilla _textArea;
        private readonly string _text = "";
        private readonly string _fileName = "";

        private void MainForm_Load(object sender, EventArgs e)
        {
            // CREATE CONTROL
            _textArea = new Scintilla();
            TextPanel.Controls.Add(_textArea);

            // BASIC CONFIG
            _textArea.Dock = DockStyle.Fill;

            // INITIAL VIEW CONFIG
            _textArea.WrapMode = WrapMode.None;
            _textArea.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors();
            InitSyntaxColoring();

            // NUMBER MARGIN
            InitNumberMargin();

            // BOOKMARK MARGIN
            InitBookmarkMargin();

            // CODE FOLDING MARGIN
            InitCodeFolding();

            // DEFAULT FILE
            if (string.IsNullOrEmpty(this._text))
            {
                LoadDataFromFile(this._fileName);
            }
            else
            {
                this.Text += this._fileName;
                _textArea.Text = this._text;
            }

            // INIT HOTKEYS
            InitHotkeys();
        }

        private void InitColors()
        {
            _textArea.SetSelectionBackColor(true, Color.DodgerBlue);
        }

        private void InitHotkeys()
        {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(this, OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(this, Uppercase, Keys.Up, true);
            HotKeyManager.AddHotKey(this, Lowercase, Keys.Down, true);
            HotKeyManager.AddHotKey(this, ZoomIn, Keys.Oemplus, true);
            HotKeyManager.AddHotKey(this, ZoomOut, Keys.OemMinus, true);
            HotKeyManager.AddHotKey(this, ZoomDefault, Keys.D0, true);
            HotKeyManager.AddHotKey(this, CloseSearch, Keys.Escape);
            HotKeyManager.AddHotKey(this, CollapseAll, Keys.Left, true);
            HotKeyManager.AddHotKey(this, ExpandAll, Keys.Right, true);

            // remove conflicting hotkeys from scintilla
            _textArea.ClearCmdKey(Keys.Control | Keys.F);
            _textArea.ClearCmdKey(Keys.Control | Keys.R);
            _textArea.ClearCmdKey(Keys.Control | Keys.H);
            _textArea.ClearCmdKey(Keys.Control | Keys.L);
            _textArea.ClearCmdKey(Keys.Control | Keys.U);
        }

        private void InitSyntaxColoring()
        {
            // Configure the default style
            _textArea.StyleResetDefault();
            _textArea.Styles[Style.Default].Font = "Consolas";
            _textArea.Styles[Style.Default].Size = 12;
            _textArea.Styles[Style.Default].BackColor = BACK_COLOR;
            _textArea.Styles[Style.Default].ForeColor = FORE_COLOR;
            _textArea.StyleClearAll();

            _textArea.Styles[Style.Json.Default].ForeColor = FORE_COLOR;
            _textArea.Styles[Style.Json.BlockComment].ForeColor = Color.DarkGray;
            _textArea.Styles[Style.Json.CompactIRI].ForeColor = Color.White;
            _textArea.Styles[Style.Json.Error].ForeColor = Color.OrangeRed;
            _textArea.Styles[Style.Json.EscapeSequence].ForeColor = Color.Orange;
            _textArea.Styles[Style.Json.Keyword].ForeColor = Color.White;
            _textArea.Styles[Style.Json.LdKeyword].ForeColor = Color.DarkGreen;
            _textArea.Styles[Style.Json.LineComment].ForeColor = Color.DarkGray;
            _textArea.Styles[Style.Json.Number].ForeColor = Color.Aqua;
            _textArea.Styles[Style.Json.Operator].ForeColor = Color.Magenta;
            _textArea.Styles[Style.Json.PropertyName].ForeColor = Color.GreenYellow;
            _textArea.Styles[Style.Json.String].ForeColor = Color.Yellow;
            _textArea.Styles[Style.Json.StringEol].ForeColor = Color.White;
            _textArea.Styles[Style.Json.Uri].ForeColor = Color.Blue;

            _textArea.Lexer = Lexer.Json;
        }

        #region Numbers, Bookmarks, Code Folding

        private void InitNumberMargin()
        {
            _textArea.Styles[Style.LineNumber].BackColor = (BACK_COLOR);
            _textArea.Styles[Style.LineNumber].ForeColor = (FORE_COLOR);
            _textArea.Styles[Style.IndentGuide].ForeColor = (FORE_COLOR);
            _textArea.Styles[Style.IndentGuide].BackColor = (BACK_COLOR);

            var nums = _textArea.Margins[NUMBER_MARGIN];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            _textArea.MarginClick += TextArea_MarginClick;
        }

        private void InitBookmarkMargin()
        {
            //TextArea.SetFoldMarginColor(true, BACK_COLOR);

            var margin = _textArea.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = _textArea.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(Color.Red);
            marker.SetForeColor(BACK_COLOR);
            marker.SetAlpha(100);
        }

        private void InitCodeFolding()
        {
            _textArea.SetFoldMarginColor(true, BACK_COLOR);
            _textArea.SetFoldMarginHighlightColor(true, BACK_COLOR);

            // Enable code folding
            _textArea.SetProperty("fold", "1");
            _textArea.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            _textArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            _textArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            _textArea.Margins[FOLDING_MARGIN].Sensitive = true;
            _textArea.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (var i = 25; i <= 31; i++)
            {
                _textArea.Markers[i].SetForeColor(BACK_COLOR); // styles for [+] and [-]
                _textArea.Markers[i].SetBackColor(FORE_COLOR); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            _textArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            _textArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            _textArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            _textArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            _textArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            _textArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            _textArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            _textArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin != BOOKMARK_MARGIN) return;

            // Do we have a marker for this line?
            const uint mask = (1 << BOOKMARK_MARKER);
            var line = _textArea.Lines[_textArea.LineFromPosition(e.Position)];
            if ((line.MarkerGet() & mask) > 0)
            {
                // Remove existing bookmark
                line.MarkerDelete(BOOKMARK_MARKER);
            }
            else
            {
                // Add bookmark
                line.MarkerAdd(BOOKMARK_MARKER);
            }
        }

        #endregion

        #region Load File

        private void LoadDataFromFile(string path)
        {
            if (!File.Exists(path)) return;

            this.Text += path;
            _textArea.Text = JsonDictionary.Form1.BeautifyJson(File.ReadAllText(path));
        }

        public void SelectTextLines(int lineStart, int lineNum)
        {
            var startLine = _textArea.Lines[lineStart];
            var endLine = _textArea.Lines[lineStart + lineNum];
            _textArea.SetSelection(startLine.Position, endLine.Position + endLine.Length);

            _textArea.ScrollCaret();
        }

        #endregion

        #region Main Menu Commands

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSearch();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textArea.Cut();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textArea.Copy();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textArea.Paste();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textArea.SelectAll();
        }

        private void SelectLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var line = _textArea.Lines[_textArea.CurrentLine];
            _textArea.SetSelection(line.Position + line.Length, line.Position);
        }

        private void ClearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textArea.SetEmptySelection(0);
        }

        private void IndentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Indent();
        }

        private void OutdentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Outdent();
        }

        private void UppercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uppercase();
        }

        private void LowercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lowercase();
        }

        private void WordWrapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // toggle word wrap
            wordWrapItem.Checked = !wordWrapItem.Checked;
            _textArea.WrapMode = wordWrapItem.Checked ? WrapMode.Word : WrapMode.None;
        }

        private void IndentGuidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // toggle indent guides
            indentGuidesItem.Checked = !indentGuidesItem.Checked;
            _textArea.IndentationGuides = indentGuidesItem.Checked ? IndentView.LookBoth : IndentView.None;
        }

        private void FormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textArea.Text = JsonDictionary.Form1.BeautifyJson(_textArea.Text);
            _textArea.SelectionStart = _textArea.SelectionEnd = 0;
            _textArea.ScrollCaret();
        }

        private void HiddenCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // toggle view whitespace
            hiddenCharactersItem.Checked = !hiddenCharactersItem.Checked;
            _textArea.ViewWhitespace = hiddenCharactersItem.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void Zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomDefault();
        }

        private void CollapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollapseAll();
        }

        private void ExpandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpandAll();
        }

        private void CollapseAll()
        {
            _textArea.FoldAll(FoldAction.Contract);
        }

        private void ExpandAll()
        {
            _textArea.FoldAll(FoldAction.Expand);
        }

        #endregion

        #region Uppercase / Lowercase

        private void Lowercase()
        {
            // save the selection
            var start = _textArea.SelectionStart;
            var end = _textArea.SelectionEnd;

            // modify the selected text
            _textArea.ReplaceSelection(_textArea.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            _textArea.SetSelection(start, end);
        }

        private void Uppercase()
        {
            // save the selection
            var start = _textArea.SelectionStart;
            var end = _textArea.SelectionEnd;

            // modify the selected text
            _textArea.ReplaceSelection(_textArea.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            _textArea.SetSelection(start, end);
        }

        #endregion

        #region Indent / Outdent

        private void Indent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to indent,
            // although the indentation function exists. Pressing TAB with the editor focused confirms this.
            GenerateKeystrokes("{TAB}");
        }

        private void Outdent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to outdent,
            // although the indentation function exists. Pressing Shift+Tab with the editor focused confirms this.
            GenerateKeystrokes("+{TAB}");
        }

        private void GenerateKeystrokes(string keys)
        {
            HotKeyManager.Enable = false;
            _textArea.Focus();
            SendKeys.Send(keys);
            HotKeyManager.Enable = true;
        }

        #endregion

        #region Zoom

        private void ZoomIn()
        {
            _textArea.ZoomIn();
        }

        private void ZoomOut()
        {
            _textArea.ZoomOut();
        }

        private void ZoomDefault()
        {
            _textArea.Zoom = 0;
        }

        #endregion

        #region Quick Search Bar

        private bool _searchIsOpen;

        private void OpenSearch()
        {
            SearchManager.SearchBox = TxtSearch;
            SearchManager.TextArea = _textArea;

            if (!_searchIsOpen)
            {
                _searchIsOpen = true;
                InvokeIfNeeded(delegate ()
                {
                    PanelSearch.Visible = true;
                    TxtSearch.Text = SearchManager.LastSearch;
                    TxtSearch.Focus();
                    TxtSearch.SelectAll();
                });
            }
            else
            {
                InvokeIfNeeded(delegate ()
                {
                    TxtSearch.Focus();
                    TxtSearch.SelectAll();
                });
            }
        }

        private void CloseSearch()
        {
            if (!_searchIsOpen) return;

            _searchIsOpen = false;
            InvokeIfNeeded(delegate ()
            {
                PanelSearch.Visible = false;
                //CurBrowser.GetBrowser().StopFinding(true);
            });
        }

        private void BtnCloseSearch_Click(object sender, EventArgs e)
        {
            CloseSearch();
        }

        private void BtnPrevSearch_Click(object sender, EventArgs e)
        {
            SearchManager.Find(false, false);
        }

        private void BtnNextSearch_Click(object sender, EventArgs e)
        {
            SearchManager.Find(true, false);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchManager.Find(true, true);
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                SearchManager.Find(true, false);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
            {
                SearchManager.Find(false, false);
            }
        }

        #endregion

        #region Utils

        private void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        #endregion

    }
}
