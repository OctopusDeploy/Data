using System;
using Octopus.TinyTypes;

namespace Octopus.Data.Model
{
    public interface IId : IId<string>
    {
    }

    public interface IId<out TId>
    {
        TId Id { get; }
    }
}