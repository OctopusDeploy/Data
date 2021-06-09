using System;
using Octopus.TinyTypes;

namespace Octopus.Data.Model
{
    public interface IDocument : IId
    {
    }

    public interface IDocument<out TId> : IId<TId>
        where TId : CaseInsensitiveStringTinyType
    {
    }
}