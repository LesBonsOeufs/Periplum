public interface IUniqueID
{
    public string Id { get; }
    public void GiveId();

    //Copy/Paste this code in any Object which needs to have a unique identifier.
    //#region Unique Id

    //public string Id => _id;
    //[SerializeField, ReadOnly] private string _id;

    //[Button]
    //void IUniqueID.GiveId()
    //{
    //    _id = Guid.NewGuid().ToString();
    //}

    //#endregion
}