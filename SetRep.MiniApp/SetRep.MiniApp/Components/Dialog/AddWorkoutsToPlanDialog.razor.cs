using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetRep.Core.Entities;

namespace SetRep.MiniApp.Components.Dialog
{
    public partial class AddWorkoutsToPlanDialog
    {
        [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
        [Parameter] public Guid WorkoutPlanId { get; set; }
        [Parameter] public List<Guid>? ExcludedWorkoutIds { get; set; }

        private List<Workout> availableWorkouts = new();
        private string searchQuery = string.Empty;
        private IReadOnlyCollection<Guid> selectedWorkoutIds = [];
        private bool isLoading = true;
        private bool isAdding = false;
        private string? errorMessage;

        private IEnumerable<Workout> filteredWorkouts
        {
            get
            {
                var workouts = availableWorkouts;

                // Исключаем тренировки, которые уже в плане
                if (ExcludedWorkoutIds != null && ExcludedWorkoutIds.Count != 0)
                {
                    workouts = [.. workouts.Where(w => !ExcludedWorkoutIds.Contains(w.Id))];
                }

                if (string.IsNullOrWhiteSpace(searchQuery))
                    return workouts;

                var query = searchQuery.Trim().ToLowerInvariant();
                return workouts.Where(w =>
                    w.Name.ToLowerInvariant().Contains(query) ||
                    (!string.IsNullOrWhiteSpace(w.Notes) &&
                     w.Notes.ToLowerInvariant().Contains(query))
                );
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadWorkouts();
        }

        private async Task LoadWorkouts()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                Guid userId = Guid.Empty;
                if (TelegramWebAppStateService?.ValidationResult?.Data?.User != null)
                {
                    userId = TelegramWebAppStateService.ValidationResult.Data.User.Id;
                }

                if (userId == Guid.Empty)
                {
                    errorMessage = "Не удалось определить пользователя";
                    isLoading = false;
                    return;
                }

                var data = await WorkoutService.GetWorkoutsAsync(userId);
                if (data != null)
                {
                    // Фильтруем только тренировки без плана или с другим планом
                    availableWorkouts = data.Where(w => w.WorkoutPlanId == null || w.WorkoutPlanId != WorkoutPlanId).ToList();
                }
                else
                {
                    errorMessage = "Не удалось загрузить тренировки. Проверьте подключение к API.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Не удалось загрузить тренировки: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        private void OnSelectedValuesChanged(IReadOnlyCollection<Guid> values)
        {
            selectedWorkoutIds = values;
            StateHasChanged();
        }

        private void Cancel()
        {
            MudDialog?.Cancel();
        }

        private void AddWorkouts()
        {
            if (selectedWorkoutIds.Count == 0 || isAdding)
                return;

            var selectedWorkouts = availableWorkouts
                .Where(w => selectedWorkoutIds.Contains(w.Id))
                .ToList();

            if (selectedWorkouts.Count != 0)
            {
                MudDialog?.Close(DialogResult.Ok(selectedWorkouts));
            }
        }

        private string TruncateNotes(string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return string.Empty;

            const int maxLength = 50;
            if (notes.Length <= maxLength)
                return notes;

            return notes.Substring(0, maxLength) + "...";
        }
    }
}
