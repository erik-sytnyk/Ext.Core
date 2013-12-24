using System;

namespace Ext.Core.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumItemAttribute : Attribute
    {
        private string _tagValue = string.Empty;
        private string _itemName = string.Empty;
        private int _sortOrder = int.MaxValue;
        private bool _isDefaultItem = false;

        #region Properties

        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public string TagValue
        {
            get { return _tagValue; }
            set { _tagValue = value; }
        }

        public bool IsDefaultItem
        {
            get { return _isDefaultItem; }
            set { _isDefaultItem = value; }
        }

        #endregion

        public EnumItemAttribute()
        {
        }

        public EnumItemAttribute(string _tagValue)
        {
            this.TagValue = _tagValue;
        }

        public EnumItemAttribute(string _tagValue, bool isDefaultItem)
        {
            this.TagValue = _tagValue;
            this.IsDefaultItem = isDefaultItem;
        }

        public EnumItemAttribute(string tagValue, string itemName)
        {
            _tagValue = tagValue;
            _itemName = itemName;
        }

        public EnumItemAttribute(string tagValue, string itemName, bool isDefaultItem)
        {
            _tagValue = tagValue;
            _itemName = itemName;
            _isDefaultItem = isDefaultItem;
        }

        public EnumItemAttribute(string tagValue, string itemName, int sortOrder)
        {
            _tagValue = tagValue;
            _itemName = itemName;
            _sortOrder = sortOrder;
        }

        public EnumItemAttribute(string tagValue, string itemName, int sortOrder, bool isDefaultItem)
        {
            _tagValue = tagValue;
            _itemName = itemName;
            _sortOrder = sortOrder;
            _isDefaultItem = isDefaultItem;
        }


    }
}