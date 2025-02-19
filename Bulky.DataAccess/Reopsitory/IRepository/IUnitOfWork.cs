namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        void Save();

    }
}
