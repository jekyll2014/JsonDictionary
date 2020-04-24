Short manual:
1) Select file types needed in the check list.
2) Push "Open directory" button to show root folder of the MetaUI repository. It'll search all files marked before to parse. A tree and a examples table is generated. It may take a couple of minutes.
3) Database collected can be saved (binary serializer format) with "Save database" button to avoid regenerating it next time. There are 2 files saved - .metalib database and .metalib.tree tree.
4) "Samples tree" contains a tree to select keywerd needed and dsiplay samples with double-click on a tree element.
5) Examples table can be filtered by schema version (got from "contentVersion" field) and a search phrase. Filter is applied to a current grid view so filters can be applied sequentially. Filtering can be slow on huge amount of data so may take some time.
6) Filters applied are shown in the text field below the grid.
7) Version filter always reset grid content on new value applied. Be aware the string filters will be reset.
8) To reset the filtering double-click on the tree item again to clear the search textbox and press Enter. Version filter set to "Any" does the same.
9) Double click on grid cell text opens it for editing (to select/copy string needed). Changes won't be saved.
10) File link in the grid cell can be opened with double-click.
11) Field search in the parent node can be executed by right clicking on the node tree.
12) Field+value can also be executed with right clicking at the grid cell.

P.S. UI critical controls are inactive on long operations.
====
Russian
Краткая инструкция:
1) Выделяем интересующие нас типы файлов в чек-листе.
2) Кнопкой "Open directory" указываем корневую папку с MetaUI. По "Ок" программа ищет все указанные ранее файлы и парсит их, выгребая ключевые слова. Строится дерево и к нему библиотека значений (примеров). Это может занять пару минут.
3) Собранную базу можно сохранить (в формате binary serializer), чтобы не генерить опять по "Save database". Сохраняются 2 файла - .metalib с базой и .metalib.tree с деревом.
4) Во вкладке "Samples tree" можно в дереве выбрать интересующее ключевое слово и по двойному клику на нем справа в гриде появится табличка с найденными на него примерами.
5) Табличку можно отфильтровать фильтрами сверху - по версии (из поля "contentVersion") и по ключевой фразе. Фильтр накладывается при нажатии ENTER в текстовом поле. Фильтруется текущее содержимое грида, так что можно последовательно наложить несколько фильтров. Фильтрация идет довольно медленно - при большом объеме данных в списке примеров может занять десятки секунд.
6) Наложенные фильтры заносятся в текстовое поле под табличкой.
7) Фильтр по версии всегда пересоздает сщдержимое таблицы при изменении версии. Текстовые фильтры при этом сбрасываются.
8) Для сброса фильтра можно очистить строку поиска или сделать двойной клик на элемент в дереве.
9) Двойным кликом в гриде на примере можно открыть ячейку на редактирование чтобы, например, скопировать какие-то отдельные слова. Изменения не сохраняются.
10) Двойным кликом на ссылке на файл в гриде можно открыть файл, в котором найден этот пример.
11) Поиск упоминания поля в родительском объекте запускается по правому клику на ноду в дереве.
12) Поиск упоминания поля и значения в родительском объекте запускается по правому клику на ячейку со значением в таблице.


P.S. На время всех длительных операций все критичные контролы на UI дезактивируются.

----
andrey.kalugin@epicor.com