// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

namespace JsonDictionary
{
    public partial class Form1
    {
        internal class ContentTypeItem
        {
            public string FileTypeMask;
            public string PropertyTypeName;
            public JsoncContentType FileType;

            public ContentTypeItem()
            {
                FileTypeMask = "";
                PropertyTypeName = "";
                FileType = JsoncContentType.Unknown;
            }
        }
    }
}
