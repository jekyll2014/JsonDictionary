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
Краткая инструкция:
1) Выделяем интересующие нас типы файлов в чек-листе.
2) Кнопкой "Collect database" указываем корневую папку с MetaUI. По "Ок" программа ищет все указанные ранее файлы и парсит их, выгребая ключевые слова. Строится дерево и к нему библиотека значений (примеров). Это может занять пару минут.
3) Собранную базу можно сохранить (в формате binary serializer), чтобы не генерить опять по "Save database". Сохраняются 2 файла - .metalib с базой и .metalib.tree с деревом.
4) Во вкладке "Samples tree" можно в дереве выбрать интересующее ключевое слово и по двойному клику на нем справа в гриде появится табличка с найденными на него примерами.
5) Табличку можно отфильтровать фильтрами сверху - по версии (из поля "contentVersion") и по ключевой фразе. Фильтр накладывается при нажатии ENTER в текстовом поле. Фильтруется текущее содержимое грида, так что можно последовательно наложить несколько фильтров. Фильтрация идет довольно медленно - при большом объеме данных в списке примеров может занять десятки секунд.
6) Наложенные фильтры заносятся в текстовое поле под табличкой.
7) Фильтр по версии всегда пересоздает сщдержимое таблицы при изменении версии. Текстовые фильтры при этом сбрасываются.
8) Для сброса фильтра можно очистить строку поиска или сделать двойной клик на элемент в дереве.
9) Двойным кликом в гриде на примере можно открыть ячейку на редактирование чтобы, например, скопировать какие-то отдельные слова. Изменения не сохраняются.
10) Двойным кликом на имя файла в гриде можно открыть файл, в котором найден этот пример. Пример будет выделен, если это возможно.
11) Поиск упоминания поля в родительском объекте запускается по правому клику на ноду в дереве.
12) Поиск упоминания поля и значения в родительском объекте запускается по правому клику на ячейку со значением в таблице.
13) Высота колонок таблицы может быть поправлена до 70% от высоты таблицы кнопкой Readjust.
14) Высота конкретной колонки может поправлена до 70% от высоты таблицы двойным кликом на заголовок столбца.
15) Кнопка "Validate files" запускает проверку выбранных файлов на соответствие схеме (указана в самом файле с тегом "$schema"). Схемы берутся по указанному URL и сохраняются на диск в каталог "\schemas\" с расширением ".original".
16) Для валидации можно использовать локальные файлы схем - программа сначала пытается взять фалй схем из каталога "\schemas\" (можно убрать расширение ".original" с сохраненных файлов).
17) Так как файлы схем бывают несоответствующими текущему API, я считаю некоторые ошибки некритичными и сохраняю их в файл "hiddenErrors.log" в текущем каталоге программы. А именно: "ArrayItemNotValid", "PropertyRequired", "NoAdditionalPropertiesAllowed". Потому что их очень много при использовании текущих схем...


Особенности работы:
В базу пишется имя только первого файла для каждого уникального ключевого поля (иначе к некоторым полям будет по сотне и более файлов)
На время всех длительных операций все критичные контролы на UI дезактивируются.
Предусмотрена возможность переформатировать json-выражения для выставления верхних/нижних скобок на один уровень в отдельную строку (по-умолчанию верхние скобки остаются на строке объекта). Срабатывает при создании базы. Включается настройкой "ReformatJson" в JsonDictionary.exe.config .
Предусмотрена возможность сохранять все имена фалов для каждого поля. Срабатывает при создании базы. Включается настройкой "CollectAllFileNames" в JsonDictionary.exe.config .

----
andrey.kalugin@epicor.com
