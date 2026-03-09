namespace SetRep.MiniApp.Client.Options;

/// <summary>
/// Настройки готовых планов тренировок
/// </summary>
public class ReadyWorkoutPlanOptions
{
    public const string SectionName = "ReadyWorkoutPlan";

    /// <summary>
    /// TelegramId пользователей, которым разрешено добавлять планы в список готовых
    /// </summary>
    public long[] AllowedEditorTelegramIds { get; set; } = [381127795];
}
