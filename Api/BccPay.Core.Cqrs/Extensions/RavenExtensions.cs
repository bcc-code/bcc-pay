using System.Linq;
using Raven.Client.Documents.Linq;

namespace BccPay.Core.Cqrs.Extensions;

public static class RavenExtensions
{
    public static IRavenQueryable<TSource> WithPagination<TSource>(this IRavenQueryable<TSource> source, int page, int size) =>
        (page <= 0 || size <= 0)
            ? source
            : source.Skip(size * (page - 1)).Take(size);
}
