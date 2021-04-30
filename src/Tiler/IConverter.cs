using Tiler.Models;

namespace Tiler
{
    public interface IConverter
    {
        TilingResult Convert(TilingRequest tilingRequest);
    }
}