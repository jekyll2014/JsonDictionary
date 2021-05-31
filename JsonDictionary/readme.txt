English
Short manual:
1) Push "Collect database" button to show root folder of the MetaUI repository. It'll search all files marked before to parse. A tree and a examples table is generated. It may take a couple of minutes.
2) Database collected can be saved (binary serializer format) with "Save database" button to avoid regenerating it next time. There are 2 files saved - .metalib database and .metalib.tree tree.
3) "Samples tree" contains a tree to select keywerd needed and dsiplay samples with double-click on a tree element.
4) Examples table can be filtered by schema version (got from "contentVersion" field) and a search phrase. Filter is applied to a current grid view so filters can be applied sequentially. Filtering can be slow on huge amount of data so may take some time.
5) Filters applied are shown in the text field below the grid.
6) Version filter always reset grid content on new value applied. Be aware the string filters will be reset.
7) To reset the filtering double-click on the tree item again to clear the search textbox and press Enter. Version filter set to "Any" does the same.
8) Double click on grid cell text opens it for editing (to select/copy string needed). Changes won't be saved.
9) Use Readjust button to adjust grid rows height (70% of grid control height) if needed (will be reset to auto-height on next grid refill).
10) Single column height can be auto-adjusted with double-click on row header as well.
11) Following actions works in tree:
    - double-click - show examples for the selected node in the grid
    - Delete - delete node
    - Ctrl-C - copy node name to clipboard
12) Following shotcuts works in description:
    - double-click - edit the description text
    - Ctrl-Enter - save text
13) Following shotcuts works in grid:
    - double-click - open file in editor
14) Descriptions edited are saved to file on program close.

Tech. notes:
- Only the first file name for every unique example is displayed in grid by default (otherwise there'll be hundreds of files for some fields). Can be switched by "Collect all fileNames" setting.
- UI critical controls are inactive on long operations.
- It's possible to re-format json-queries to place starting brackets ("[,{") on the next line (I personally prefer this way). Works on database creation time. Can be enabled by "Reformat JSON" setting.

====
Russian
Краткая инструкция:
1) Кнопкой "Collect database" указываем корневую папку с JSONC файлами. По "Ок" программа ищет все *.jsonc файлы и парсит их, выгребая ключевые слова. Строится дерево и к нему библиотека значений (примеров). Это может занять немало времени (30 минут на полный комплект).
2) Собранную базу можно сохранить (в формате binary serializer), чтобы не генерить опять по "Save database". Сохраняются 2 файла - .kineticLib с базой примеров и .kineticLib.tree с деревом.
3) Во вкладке "Schema tree" можно в дереве выбрать интересующее ключевое слово и по двойному клику на нем справа в гриде появится табличка с найденными на него примерами.
4) Табличку можно отфильтровать фильтрами сверху - по версии (из поля "contentVersion") и по ключевой фразе. Фильтр накладывается при нажатии ENTER в текстовом поле. Фильтруется текущее содержимое грида, так что можно последовательно наложить несколько фильтров. Наложенные фильтры заносятся в текстовое поле под табличкой.
5) Наложенные фильтры заносятся в текстовое поле под табличкой.
6) Фильтр по версии всегда пересоздает содержимое таблицы при изменении версии. Текстовые фильтры при этом сбрасываются.
7) Для сброса фильтра можно очистить строку поиска или сделать двойной клик на элемент в дереве.
8) Двойным кликом в гриде можно открыть файл, в котором найден этот пример. Пример будет выделен, если это возможно.
9) Высота колонок таблицы может быть поправлена до 70% от высоты таблицы кнопкой "Adjust rows".
10) Высота конкретной колонки может поправлена до 70% от высоты таблицы двойным кликом на заголовок столбца.
11) В дереве работают следующие действия:
    - двойной клик - вывести примеры для выбранного элемента в таблицу
    - Delete - удалить элемент
    - Ctrl-C - скопировать название элемента в буфер обмена
12) В поле описания работают следующие действия:
    - двойной клик - радактировать текст описания
    - Ctrl-Enter - сохранить текст
13) В таблице работают следующие действия:
    - двойной клик - открыть файл в редакторе
14) Отредактированные описания сохраняются в файл при закрытии программы.

Особенности работы:
- По-умолчанию в поле "File Name" сохраняется имя только первого файла для каждого уникального ключевого поля (иначе к некоторым полям будет по сотне и более файлов).  Включается настройкой "Collect all fileNames".
- На время всех длительных операций все критичные контролы на UI дезактивируются.
- Предусмотрена возможность переформатировать json-выражения для выставления верхних/нижних скобок на один уровень в отдельную строку (по-умолчанию верхние скобки остаются на строке объекта). Включается настройкой "Reformat JSON".

----
andrey.kalugin@epicor.com
