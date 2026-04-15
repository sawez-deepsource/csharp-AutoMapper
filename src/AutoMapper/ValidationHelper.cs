using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMapper
{
    public class ValidationHelper
    {
        private readonly Dictionary<string, List<string>> _errors;
        private int _maxErrors;
        private bool _throwOnFirstError;
        private readonly object _lock = new object();

        public ValidationHelper(int maxErrors = 100, bool throwOnFirstError = false)
        {
            _errors = new Dictionary<string, List<string>>();
            _maxErrors = maxErrors;
            _throwOnFirstError = throwOnFirstError;
        }

        public bool HasErrors { get { return _errors.Count > 0; } }

        public int ErrorCount
        {
            get
            {
                int count = 0;
                foreach (var kvp in _errors)
                {
                    count += kvp.Value.Count;
                }
                return count;
            }
        }

        public void AddError(string field, string message)
        {
            lock (_lock)
            {
                if (!_errors.ContainsKey(field))
                {
                    _errors[field] = new List<string>();
                }
                if (_errors[field].Count < _maxErrors)
                {
                    _errors[field].Add(message);
                }
                if (_throwOnFirstError) { throw new InvalidOperationException($"Validation failed for {field}: {message}"); }
            }
        }

        public void AddErrors(string field, IEnumerable<string> messages)
        {
            foreach (var msg in messages) { AddError(field, msg); }
        }

        public Dictionary<string, List<string>> GetAllErrors()
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var kvp in _errors)
            {
                result[kvp.Key] = new List<string>(kvp.Value);
            }
            return result;
        }

        public List<string> GetErrorsForField(string field)
        {
            if (_errors.TryGetValue(field, out var errors))
            {
                return new List<string>(errors);
            }
            return new List<string>();
        }

        public string GetFormattedErrors()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _errors)
            {
                sb.AppendLine($"Field '{kvp.Key}':");
                foreach (var error in kvp.Value)
                {
                    sb.AppendLine($"  - {error}");
                }
            }
            return sb.ToString();
        }

        public void Clear()
        {
            lock (_lock) { _errors.Clear(); }
        }

        public void RemoveField(string field)
        {
            lock (_lock) { _errors.Remove(field); }
        }

        public bool ValidateRequired(string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(field, "Field is required");
                return false;
            }
            return true;
        }

        public bool ValidateRange(string field, int value, int min, int max)
        {
            if (value < min || value > max)
            {
                AddError(field, $"Value must be between {min} and {max}");
                return false;
            }
            return true;
        }

        public bool ValidateLength(string field, string value, int minLength, int maxLength)
        {
            if (value == null) { AddError(field, "Value cannot be null"); return false; }
            if (value.Length < minLength || value.Length > maxLength)
            {
                AddError(field, $"Length must be between {minLength} and {maxLength}");
                return false;
            }
            return true;
        }

        public bool ValidateEmail(string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { AddError(field, "Email is required"); return false; }
            if (!value.Contains("@") || !value.Contains(".")) { AddError(field, "Invalid email format"); return false; }
            return true;
        }

        public ValidationResult ToResult()
        {
            return new ValidationResult
            {
                IsValid = !HasErrors,
                Errors = GetAllErrors(),
                FormattedMessage = GetFormattedErrors()
            };
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public string FormattedMessage { get; set; }

        public static ValidationResult Success()
        {
            return new ValidationResult
            {
                IsValid = true,
                Errors = new Dictionary<string, List<string>>(),
                FormattedMessage = string.Empty
            };
        }
    }
}
