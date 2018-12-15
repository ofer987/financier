namespace Financier.Common.Models
{
    public interface IAsset
    {
        decimal ValueAt(int monthAfterInception);

        decimal ValueBy(int monthAfterInception);
    }
}
