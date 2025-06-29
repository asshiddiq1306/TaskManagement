using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public List<string> ValidationErrors { get; private set; } = new();

        private ServiceResult() { }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static ServiceResult<T> Failure(List<string> validationErrors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ValidationErrors = validationErrors
            };
        }
    }

    // Non-generic version for operations that don't return data
    public class ServiceResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public List<string> ValidationErrors { get; private set; } = new();

        private ServiceResult() { }

        public static ServiceResult Success()
        {
            return new ServiceResult { IsSuccess = true };
        }

        public static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static ServiceResult Failure(List<string> validationErrors)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ValidationErrors = validationErrors
            };
        }
    }
}
