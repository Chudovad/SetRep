namespace SetRep.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ImageUrlAttribute(string url) : Attribute
    {
        public string Url { get; } = url;
    }
}
