#nullable enable

namespace Escaper.Utility.Domain.Scene
{
    public class EscapeScene : IdentifiedDomain
    {
        public EscapeScene(
            string id,
            EscapeScene? right,
            EscapeScene? left,
            EscapeScene? back
        ) : base(id)
        {
            Right = right;
            Left = left;
            Back = back;
        }

        public EscapeScene? Right { get; }
        public EscapeScene? Left { get; }
        public EscapeScene? Back { get; }
    }
}