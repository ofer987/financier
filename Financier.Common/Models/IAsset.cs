namespace Financier.Common.Models
{
    public interface IAsset : IProduct
    {
        decimal ValueAt(int monthAfterInception);

        decimal ValueBy(int monthAfterInception);
    }
}
