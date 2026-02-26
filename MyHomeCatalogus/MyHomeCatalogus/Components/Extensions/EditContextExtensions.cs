using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Extensions;

public static class EditContextExtensions
{
    /// <summary>
    /// Maps UniqueConstraintException errors directly to the EditContext.
    /// </summary>
    /// <param name="editContext">The EditContext</param>
    /// <param name="exception">The UniqueConstraintException</param>
    public static void AddValidationErrors(this EditContext editContext, UniqueConstraintException exception)
    {
        if (editContext.Properties.TryGetValue("ValidationStore", out var storeObj)
            && storeObj is ValidationMessageStore messages)
        {
            messages.Clear();

            if (exception.ValidationErrors.Any())
            {
                foreach (var error in exception.ValidationErrors)
                {
                    messages.Add(editContext.Field(error.PropertyName), error.ErrorMessage);
                }
            }
            else
            {
                // Fallback: Add to model-level for the ValidationSummary
                messages.Add(editContext.Field(string.Empty), exception.Message);
            }

            editContext.NotifyValidationStateChanged();
        }
    }

    /// <summary>
    /// Clears all the errors from the ValidationMessageStore of the EditContext
    /// </summary>
    /// <param name="editContext">The EditContext</param>
    public static void ClearValidationErrors(this EditContext editContext)
    {
        if ((editContext.Properties.TryGetValue("ValidationStore", out var storeObj)
            && storeObj is ValidationMessageStore messages))
        {
            messages.Clear();
        }

        editContext.MarkAsUnmodified();
        editContext.NotifyValidationStateChanged();
    }
}

