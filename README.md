# JsonDictionary

MetaUI source processor to ease samples search

It's intended to ease sample search in .json files. As Kinetic manuals are poor for now there are many cases we need to search through the existing projects to find the example.
The tool collects database of the fields and values of the project .jsonc files so one can search the needed parameter/keyword in the tree to find usage sample.

Read quick note inside for use tips.

Created using 
https://github.com/RicoSuter/NJsonSchema
https://github.com/robinrodricks/ScintillaNET.Demo

To do:

1. Comfortable way to show multiple file names for each example cell
2. file masks for initial filenames list to include \*.layout.jsonc from /pages and /views
3. show title/description from schema for each node in the tree.