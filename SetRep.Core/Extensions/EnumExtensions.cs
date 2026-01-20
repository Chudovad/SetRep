using SetRep.Core.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace SetRep.Core.Extensions;

/// <summary>
/// Универсальные методы расширения для работы с enum типами
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Получает описание значения enum из атрибута Description
    /// </summary>
    /// <typeparam name="T">Тип enum</typeparam>
    /// <param name="enumValue">Значение enum</param>
    /// <returns>Описание значения enum</returns>
    public static string GetDescription<T>(this T enumValue) where T : struct, Enum
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        
        if (fieldInfo == null)
            return enumValue.ToString();
        
        var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
        
        return attribute?.Description ?? enumValue.ToString();
    }

    /// <summary>
    /// Получение ссылки на картинку значения enum из атрибута ImageUrl
    /// </summary>
    /// <param name="value">Значение enum</param>
    /// <returns>Ссылка на картинку</returns>
    public static string GetImageUrl(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<ImageUrlAttribute>();
        return attribute?.Url ?? string.Empty;
    }
}
