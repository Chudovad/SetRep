using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SetRep.Core.Entities;

namespace SetRep.MiniApp.Components.Dialog
{
    public partial class EditExerciseDialog
    {

        [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
        [Parameter] public Exercise Exercise { get; set; } = null!;

        private MudForm? form;
        private bool isFormValid;
        private bool isSaving = false;
        private string? errorMessage;
        private IBrowserFile? selectedMainImageFile;
        private Exercise exercise = null!;
        private long maxFileSize;

        protected override void OnInitialized()
        {
            maxFileSize = FileUploadOptionsAccessor.Value.MaxFileSizeBytes;

            exercise = new Exercise
            {
                Id = Exercise.Id,
                Name = Exercise.Name,
                Image = Exercise.Image ?? string.Empty,
                MainImage = Exercise.MainImage,
                Description = Exercise.Description,
                MuscleGroup = Exercise.MuscleGroup,
                WeightUnit = Exercise.WeightUnit,
                IsSystem = Exercise.IsSystem,
                CreatedByUserId = Exercise.CreatedByUserId,
                CreatedAt = Exercise.CreatedAt,
                UpdatedAt = Exercise.UpdatedAt
            };
        }

        private void Cancel()
        {
            MudDialog?.Cancel();
        }

        private async Task OnBeforeMainImageSelected()
        {
            await RemoveImageFromStorage();
        }

        private void OnMainImageFileSelected(IBrowserFile file)
        {
            selectedMainImageFile = file;
        }

        private async Task UploadImageAsync(IBrowserFile file, bool isMainImage)
        {
            if (file == null) return;

            // Проверка размера файла
            if (file.Size > maxFileSize)
            {
                var maxSizeMB = maxFileSize / 1024.0 / 1024.0;
                Snackbar.Add($"Размер файла превышает {maxSizeMB:F1} МБ", Severity.Error);
                return;
            }

            try
            {
                using var stream = file.OpenReadStream(maxAllowedSize: maxFileSize);
                var contentType = file.ContentType ?? "image/jpeg";
                var url = await FileUploadService.UploadImageAsync(stream, file.Name, contentType);

                if (!string.IsNullOrEmpty(url))
                {
                    if (isMainImage)
                    {
                        exercise.MainImage = url;
                    }
                    else
                    {
                        exercise.Image = url;
                    }
                }
                else
                {
                    Snackbar.Add("Ошибка при загрузке изображения", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Ошибка при загрузке изображения: {ex.Message}", Severity.Error);
            }
        }

        private async Task Save()
        {
            await form!.Validate();

            if (!isFormValid || isSaving)
                return;

            isSaving = true;
            errorMessage = null;

            try
            {
                if (selectedMainImageFile != null)
                    await UploadImageAsync(selectedMainImageFile, isMainImage: true);

                exercise.UpdatedAt = DateTime.UtcNow;

                var success = await ExerciseService.UpdateExerciseAsync(exercise);
                if (success)
                {
                    MudDialog?.Close(DialogResult.Ok(exercise));
                }
                else
                {
                    errorMessage = "Ошибка при обновлении упражнения. Проверьте подключение к API.";
                }
            }
            catch (HttpRequestException ex)
            {
                errorMessage = $"Ошибка сети: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Не удалось обновить упражнение: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task RemoveImage()
        {
            await RemoveImageFromStorage();
            selectedMainImageFile = null;
            exercise.MainImage = null;
            StateHasChanged();
        }

        private async Task RemoveImageFromStorage()
        {
            // Сохраняем текущий URL изображения перед очисткой
            var imageUrlToDelete = exercise.MainImage;

            // Удаляем изображение из облака, если это существующий URL (не preview)
            if (!string.IsNullOrEmpty(imageUrlToDelete) && !imageUrlToDelete.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var deleted = await FileUploadService.DeleteImageAsync(imageUrlToDelete);
                    if (!deleted)
                    {
                        Logger?.LogWarning("Не удалось удалить изображение из облака: {Url}", imageUrlToDelete);
                        Snackbar.Add("Не удалось удалить изображение из облака", Severity.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Ошибка при удалении изображения из облака: {Url}", imageUrlToDelete);
                    Snackbar.Add("Ошибка при удалении изображения из облака", Severity.Error);
                }
            }
        }
    }
}
