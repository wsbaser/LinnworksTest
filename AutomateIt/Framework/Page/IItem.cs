namespace selenium.core.Framework.Page
{
    public interface IItem:IContainer
    {
        string ID { get; set; }
        string ItemScss { get; }
        IItem Clone();
    }
}
