namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }

        void Save();

    }
}
