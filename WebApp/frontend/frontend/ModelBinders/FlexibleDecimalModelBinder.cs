using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace frontend.ModelBinders;

public class FlexibleDecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
        var rawValue = valueResult.FirstValue;

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            if (bindingContext.ModelMetadata.IsReferenceOrNullableType)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }

            return Task.CompletedTask;
        }

        if (TryParseDecimal(rawValue, out var decimalValue))
        {
            bindingContext.Result = ModelBindingResult.Success(decimalValue);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(
            bindingContext.ModelName,
            "Informe um numero valido.");

        return Task.CompletedTask;
    }

    private static bool TryParseDecimal(string rawValue, out decimal value)
    {
        var normalized = rawValue.Trim();

        if (normalized.Contains(",") && decimal.TryParse(normalized, NumberStyles.Number, new CultureInfo("pt-BR"), out value))
        {
            return true;
        }

        if (normalized.Contains(".") && decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        if (decimal.TryParse(normalized, NumberStyles.Number, new CultureInfo("pt-BR"), out value))
        {
            return true;
        }

        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }
}
