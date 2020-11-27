using System;
namespace GameStreamSearch.Domain
{
    public class ValidationMessage
    {
        public ValidationMessage(string propertyName, string message)
        {
            PropertyName = propertyName;
            Message = message;
        }

        public string PropertyName { get; set; }
        public string Message { get; set; }
    }
}
