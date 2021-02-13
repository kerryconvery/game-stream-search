using System;

namespace GameStreamSearch.Application.ValueObjects
{
    public class NumericPageOffset
    {
        internal readonly int pageOffset;
        private readonly int pageSize;

        internal NumericPageOffset(int pageOffset)
        {
            this.pageOffset = pageOffset;
        }

        public NumericPageOffset(int pageSize, string pageOffset)
        {
            this.pageOffset = !string.IsNullOrEmpty(pageOffset) ? int.Parse(pageOffset) : 0;
            this.pageSize = pageSize;
        }

        public NumericPageOffset GetNextOffset(int numberOfItemsInPage)
        {
            return numberOfItemsInPage >= pageSize ? new NumericPageOffset(pageSize + pageOffset) : new NumericPageOffset(0);
        }

        public static implicit operator string(NumericPageOffset numericOffset) =>
            numericOffset.pageOffset > 0 ? numericOffset.pageOffset.ToString() : string.Empty;
        public static implicit operator int(NumericPageOffset numericOffset) => numericOffset.pageOffset;
    }
}