using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SetRep.Core.Entities;
using SetRep.Core.Enums;

namespace SetRep.MiniApp.Client.Components.Dialog
{
    public partial class AddExerciseDialog
    {
        [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
        [Parameter] public MuscleGroup? PreselectedMuscleGroup { get; set; }
        [Parameter] public Exercise? Exercise { get; set; }

        private bool IsEditMode => Exercise != null;

        private MudForm? form;
        private bool isFormValid;
        private bool isSaving = false;
        private string? errorMessage;
        private IBrowserFile? selectedMainImageFile;
        private long maxFileSize;

        private Exercise exercise = null!;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            maxFileSize = FileUploadOptionsAccessor.Value.MaxFileSizeBytes;

            if (IsEditMode)
            {
                exercise = new Exercise
                {
                    Id = Exercise!.Id,
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
            else
            {
                exercise = new Exercise
                {
                    Name = string.Empty,
                    Image = string.Empty,
                    MainImage = null,
                    Description = null,
                    WeightUnit = "кг",
                    IsSystem = false
                };

                if (PreselectedMuscleGroup.HasValue)
                {
                    exercise.MuscleGroup = PreselectedMuscleGroup.Value;
                }
            }
        }

        private void Cancel()
        {
            MudDialog?.Cancel();
        }

        private async Task OnBeforeMainImageSelected()
        {
            if (IsEditMode)
            {
                await RemoveImageFromStorage();
            }
        }

        private void OnMainImageFileSelected(IBrowserFile file)
        {
            selectedMainImageFile = file;
        }

        private async Task UploadImageAsync(IBrowserFile file, bool isMainImage)
        {
            if (file == null) return;

            if (file.Size > maxFileSize)
            {
                var maxSizeMB = maxFileSize / 1024.0 / 1024.0;
                await TelegramHapticService.NotificationErrorAsync();
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
                    await TelegramHapticService.NotificationErrorAsync();
                    Snackbar.Add("Ошибка при загрузке изображения", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                await TelegramHapticService.NotificationErrorAsync();
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

                if (IsEditMode)
                {
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
                else
                {
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
            }
            catch (HttpRequestException ex)
            {
                errorMessage = $"Ошибка сети: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = IsEditMode
                    ? $"Не удалось обновить упражнение: {ex.Message}"
                    : $"Не удалось создать упражнение: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task RemoveImage()
        {
            if (IsEditMode)
            {
                await RemoveImageFromStorage();
            }
            selectedMainImageFile = null;
            exercise.MainImage = null;
            StateHasChanged();
        }

        private async Task RemoveImageFromStorage()
        {
            var imageUrlToDelete = exercise.MainImage;

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
                    await TelegramHapticService.NotificationErrorAsync();
                    Snackbar.Add("Ошибка при удалении изображения из облака", Severity.Error);
                }
            }
        }
    }
}
