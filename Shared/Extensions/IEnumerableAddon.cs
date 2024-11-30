using ChessEngine.Structs;

namespace ChessEngine.Extensions;

public static class IEnumerableAddon
{
    public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value) {
        foreach ( var cur in e) {
            yield return cur;
        }
        yield return value;
    }
}