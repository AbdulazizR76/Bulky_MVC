namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }

        void Save();

        string GenerateRandomISBN();

    }
}
