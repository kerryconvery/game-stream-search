using System;

namespace GameStreamSearch.Application.ValueObjects
{
    public class NumericPageOffset
    {
        internal readonly int pageOffset;
        private readonly int pageSize;

        public NumericPageOffset(int pageSize, string pageOffset)
        {
            this.pageOffset = !string.IsNullOrEmpty(pageOffset) ? int.Parse(pageOffset) : 0;
            this.pageSize = pageSize;
        }

        public NumericPageOffset(int pageOffset)
        {
            this.pageOffset = pageOffset;
        }

        public NumericPageOffset GetNextOffset()
        {
            return new NumericPageOffset(pageSize + pageOffset);
        }

        public static implicit operator string(NumericPageOffset numericOffset) => numericOffset.pageOffset.ToString();
        public static implicit operator int(NumericPageOffset numericOffset) => numericOffset.pageOffset;
    }
}