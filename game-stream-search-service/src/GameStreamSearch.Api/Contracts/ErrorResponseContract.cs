using System;
using System.Collections.Generic;

namespace GameStreamSearch.Api.Contracts
{
    public enum ErrorCodeType
    {
        ChannelNotFoundOnPlatform,
    }

    public class ErrorContract
    {
        public ErrorCodeType ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ErrorResponseContract
    {
        private List<ErrorContract> errors;

        public ErrorResponseContract ()
        {
            errors = new List<ErrorContract>();
        }

        public ErrorResponseContract AddError(ErrorContract errorContract)
        {
            errors.Add(errorContract);
            return this;
        }

        public IEnumerable<ErrorContract> Errors => errors;
    }
}
