using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SetRep.Core.Entities;
using SetRep.Core.Enums;

namespace SetRep.MiniApp.Components.Dialog
{
    public partial class AddExerciseDialog
    {
        [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
        [Parameter] public MuscleGroup? PreselectedMuscleGroup { get; set; }

        private MudForm? form;
        private bool isFormValid;
        private bool isSaving = false;
        private string? errorMessage;
        private IBrowserFile? selectedMainImageFile;
        private long maxFileSize;

        private Exercise exercise = new()
        {
            Name = string.Empty,
            Image = string.Empty,
            MainImage = null,
            Description = null,
            WeightUnit = "кг",
            IsSystem = false
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            maxFileSize = FileUploadOptionsAccessor.Value.MaxFileSizeBytes;

            if (PreselectedMuscleGroup.HasValue)
            {
                exercise.MuscleGroup = PreselectedMuscleGroup.Value;
            }
        }

        private void Cancel()
        {
            MudDialog?.Cancel();
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

                if (exercise.Id == Guid.Empty)
                {
                    exercise.Id = Guid.NewGuid();
                }
                exercise.CreatedAt = DateTime.UtcNow;

                if (TelegramWebAppStateService.ValidationResult?.Data?.User != null)
                {
                    exercise.CreatedByUserId = TelegramWebAppStateService.ValidationResult.Data.User.Id;
                }

                var createdExercise = await ExerciseService.CreateExerciseAsync(exercise);
                if (createdExercise != null)
                {
                    MudDialog?.Close(DialogResult.Ok(createdExercise));
                }
                else
                {
                    errorMessage = "Ошибка при создании упражнения. Проверьте подключение к API.";
                }
            }
            catch (HttpRequestException ex)
            {
                errorMessage = $"Ошибка сети: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Не удалось создать упражнение: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }

        private void RemoveImage()
        {
            selectedMainImageFile = null;
            exercise.MainImage = null;
            StateHasChanged();
        }
    }
}