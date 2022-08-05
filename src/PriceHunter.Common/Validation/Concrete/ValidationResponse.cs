﻿namespace PriceHunter.Common.Validation.Concrete
{
    public class ValidationResponse
    {
        public ValidationResponse()
        {
            ErrorMessages = new List<string>();
        }

        public bool IsValid { get; set; }

        public string ErrorMessage { get; set; }

        public List<string> ErrorMessages { get; }
    }
}