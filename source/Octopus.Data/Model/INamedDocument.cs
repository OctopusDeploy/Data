using Octopus.TinyTypes;

namespace Octopus.Data.Model
{
    public interface INamedDocument : INamed, IDocument
    {
    }

    public interface INamedDocument<TId> : INamed, IDocument<TId> where TId : CaseInsensitiveStringTinyType
    {
    }
}