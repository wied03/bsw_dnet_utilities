using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Bsw.Wpf.Utilities.ViewModels
{
    /// <summary>
    ///     A base classe for ViewModel classes which supports validation using IDataErrorInfo interface. Properties must
    ///     defines
    ///     validation rules by using validation attributes defined in System.ComponentModel.DataAnnotations.
    /// </summary>
    public class ValidationViewModelBase : ViewModelBase,
                                           IDataErrorInfo
    {
        readonly Dictionary<string, Func<ValidationViewModelBase, object>> _propertyGetters;
        readonly Dictionary<string, ValidationAttribute[]> _validators;
        public int ValidationExceptionCount { get; private set; }

        internal ValidationResult GetValidationResult(ValidationAttribute attr,
                                                      object propertyValue)
        {
            var result = attr.GetValidationResult(propertyValue,
                                                  new ValidationContext(this));
            return result;
        }

        /// <summary>
        ///     Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        public string this[string propertyName]
        {
            get
            {
                if (!_propertyGetters.ContainsKey(propertyName)) return string.Empty;
                var propertyValue = _propertyGetters[propertyName](this);
                var errorMessages = _validators[propertyName]
                    .Select(v => GetValidationResult(v,
                                                     propertyValue))
                    .Where(r => r != ValidationResult.Success)
                    .Select(v => v.ErrorMessage).ToArray();

                return string.Join(Environment.NewLine,
                                   errorMessages);
            }
        }

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                var validationResults = from validator in _validators
                                        from attribute in validator.Value
                                        select GetValidationResult(attribute,
                                                                   _propertyGetters[validator.Key](this))
                    ;
                var errors = from result in validationResults
                             where result != ValidationResult.Success
                             select result.ErrorMessage;

                return string.Join(Environment.NewLine,
                                   errors.ToArray());
            }
        }

        /// <summary>
        ///     Gets the number of properties which have a validation attribute and are currently valid
        /// </summary>
        public int ValidPropertiesCount
        {
            get
            {
                var query = from validator in _validators
                            where
                                validator.Value.All(attribute =>
                                                    GetValidationResult(attribute,
                                                                        _propertyGetters[validator.Key](this)) ==
                                                    ValidationResult.Success)
                            select validator;

                var count = query.Count() - ValidationExceptionCount;
                return count;
            }
        }

        public virtual bool AllValid
        {
            get
            {
                return _validators
                    .All(validator => validator.Value.All(attr => GetValidationResult(attr,
                                                                                      _propertyGetters[validator.Key](
                                                                                                                      this)) ==
                                                                  ValidationResult.Success));
            }
        }

        /// <summary>
        ///     Gets the number of properties which have a validation attribute
        /// </summary>
        public int TotalPropertiesWithValidationCount
        {
            get { return _validators.Count(); }
        }

        public ValidationViewModelBase()
        {
            _validators = GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name,
                              GetValidations);

            _propertyGetters = GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name,
                              GetValueGetter);
        }

        static ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[]) property.GetCustomAttributes(typeof (ValidationAttribute),
                                                                        true);
        }

        static Func<ValidationViewModelBase, object> GetValueGetter(PropertyInfo property)
        {
            return viewmodel => property.GetValue(viewmodel,
                                                  null);
        }
    }
}