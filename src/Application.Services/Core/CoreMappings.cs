using Application.Models.Core;
using Domain.Core;
using Riok.Mapperly.Abstractions;

namespace Application.Services.Core;

[Mapper]
public static partial class CoreMappings
{
    public static partial PagingFilter MapToPagingFilter(this PagingModelFilter modelFilter);
    public static partial PagingModelFilter MapToPagingModelFilter(this PagingFilter pagingFilter);
}
