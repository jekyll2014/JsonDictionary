Short manual:
1) Select file types needed in the check list.
2) Push "Collect database" button to show root folder of the MetaUI repository. It'll search all files marked before to parse. A tree and a examples table is generated. It may take a couple of minutes.
3) Database collected can be saved (binary serializer format) with "Save database" button to avoid regenerating it next time. There are 2 files saved - .metalib database and .metalib.tree tree.
4) "Samples tree" contains a tree to select keywerd needed and dsiplay samples with double-click on a tree element.
5) Examples table can be filtered by schema version (got from "contentVersion" field) and a search phrase. Filter is applied to a current grid view so filters can be applied sequentially. Filtering can be slow on huge amount of data so may take some time.
6) Filters applied are shown in the text field below the grid.
7) Version filter always reset grid content on new value applied. Be aware the string filters will be reset.
8) To reset the filtering double-click on the tree item again to clear the search textbox and press Enter. Version filter set to "Any" does the same.
9) Double click on grid cell text opens it for editing (to select/copy string needed). Changes won't be saved.
10) File showed the grid cell can be opened with double-click on it. Will be opened with example text selected if possible.
11) Field search in the parent node can be executed by right clicking on the node tree.
12) Field+value search can also be executed with right click at the grid cell.
13) Use Readjust button to adjust grid rows height (70% of grid control height) if needed (will be reset to auto-height on next grid refill).
14) Single column height can be auto-adjusted with double-click on row header as well.
15) "Validate files" button execute all files selected validation against scheme (referenced inside the file itself with "$schema" tag). SchemaS used are downloaded from URL and saved to "\schemas\" directory with ".original" extension.
16) It's possible to validate using local schema files as program tries to get it from "\schemas\" folder first (remove ".original" extension to use saved schema files).
17) Due to a schema file could be outdated I consider some errors as usual/non critical and only save them in a log file "hiddenErrors.log" in a program folder. Namely: "ArrayItemNotValid", "PropertyRequired", "NoAdditionalPropertiesAllowed". Because the are lots of them when using current schema files...

Tech. notes:
Only the first file name for every unique field/example is saved to database (otherwise there'll be hundreds of files for some fields).
UI critical controls are inactive on long operations.
It's possible to re-format json-queries to place starting brackets ("[,{") on the next line (I personally prefer this way). Works on database creation time. Can be enabled by JsonDictionary.exe.config "ReformatJson" setting.
It's possible to save all file names for the fielsd/examples. Works on database creation time. Can be enabled by JsonDictionary.exe.config "CollectAllFileNames" setting.
Some minor(?) validation errors are suppressed. - the possible reason is outdated schema specification used by NJsonSchema library.

====
Russian
������� ����������:
1) �������� ������������ ��� ���� ������ � ���-�����.
2) ������� "Collect database" ��������� �������� ����� � MetaUI. �� "��" ��������� ���� ��� ��������� ����� ����� � ������ ��, �������� �������� �����. �������� ������ � � ���� ���������� �������� (��������). ��� ����� ������ ���� �����.
3) ��������� ���� ����� ��������� (� ������� binary serializer), ����� �� �������� ����� �� "Save database". ����������� 2 ����� - .metalib � ����� � .metalib.tree � �������.
4) �� ������� "Samples tree" ����� � ������ ������� ������������ �������� ����� � �� �������� ����� �� ��� ������ � ����� �������� �������� � ���������� �� ���� ���������.
5) �������� ����� ������������� ��������� ������ - �� ������ (�� ���� "contentVersion") � �� �������� �����. ������ ������������� ��� ������� ENTER � ��������� ����. ����������� ������� ���������� �����, ��� ��� ����� ��������������� �������� ��������� ��������. ���������� ���� �������� �������� - ��� ������� ������ ������ � ������ �������� ����� ������ ������� ������.
6) ���������� ������� ��������� � ��������� ���� ��� ���������.
7) ������ �� ������ ������ ����������� ���������� ������� ��� ��������� ������. ��������� ������� ��� ���� ������������.
8) ��� ������ ������� ����� �������� ������ ������ ��� ������� ������� ���� �� ������� � ������.
9) ������� ������ � ����� �� ������� ����� ������� ������ �� �������������� �����, ��������, ����������� �����-�� ��������� �����. ��������� �� �����������.
10) ������� ������ �� ��� ����� � ����� ����� ������� ����, � ������� ������ ���� ������. ������ ����� �������, ���� ��� ��������.
11) ����� ���������� ���� � ������������ ������� ����������� �� ������� ����� �� ���� � ������.
12) ����� ���������� ���� � �������� � ������������ ������� ����������� �� ������� ����� �� ������ �� ��������� � �������.
13) ������ ������� ������� ����� ���� ���������� �� 70% �� ������ ������� ������� Readjust.
14) ������ ���������� ������� ����� ���������� �� 70% �� ������ ������� ������� ������ �� ��������� �������.
15) ������ "Validate files" ��������� �������� ��������� ������ �� ������������ ����� (������� � ����� ����� � ����� "$schema"). ����� ������� �� ���������� URL � ����������� �� ���� � ������� "\schemas\" � ����������� ".original".
16) ��� ��������� ����� ������������ ��������� ����� ���� - ��������� ������� �������� ����� ���� ���� �� �������� "\schemas\" (����� ������ ���������� ".original" � ����������� ������).
17) ��� ��� ����� ���� ������ ������������������ �������� API, � ������ ��������� ������ ������������ � �������� �� � ���� "hiddenErrors.log" � ������� �������� ���������. � ������: "ArrayItemNotValid", "PropertyRequired", "NoAdditionalPropertiesAllowed". ������ ��� �� ����� ����� ��� ������������� ������� ����...


����������� ������:
� ���� ������� ��� ������ ������� ����� ��� ������� ����������� ��������� ���� (����� � ��������� ����� ����� �� ����� � ����� ������)
�� ����� ���� ���������� �������� ��� ��������� �������� �� UI ���������������.
������������� ����������� ����������������� json-��������� ��� ����������� �������/������ ������ �� ���� ������� � ��������� ������ (��-��������� ������� ������ �������� �� ������ �������). ����������� ��� �������� ����. ���������� ���������� "ReformatJson" � JsonDictionary.exe.config .
������������� ����������� ��������� ��� ����� ����� ��� ������� ����. ����������� ��� �������� ����. ���������� ���������� "CollectAllFileNames" � JsonDictionary.exe.config .

----
andrey.kalugin@epicor.com
