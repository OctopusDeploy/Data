using System;
using Octopus.TinyTypes;

namespace Octopus.Data.Model
{
    public interface IDocument : IDocument<string>
    {
    }

    public interface IDocument<out TId> : IId<TId>
    {
    }
}