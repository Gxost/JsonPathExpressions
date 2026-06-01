namespace JsonPathExpressions.Utils;

#if NET461 || NETSTANDARD2_0
internal static class HashCode
{
    public static int Combine<T1>(T1 first)
    {
        return first?.GetHashCode() ?? 0;
    }

    public static int Combine<T1, T2>(T1 first, T2 second)
    {
        unchecked
        {
            int hashCode = Combine(first);
            hashCode = (hashCode * 397) ^ Combine(second);

            return hashCode;
        }
    }

    public static int Combine<T1, T2, T3>(T1 first, T2 second, T3 third)
    {
        unchecked
        {
            int hashCode = Combine(first);
            hashCode = (hashCode * 397) ^ Combine(second);
            hashCode = (hashCode * 397) ^ Combine(third);

            return hashCode;
        }
    }

    public static int Combine<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
    {
        unchecked
        {
            int hashCode = Combine(first);
            hashCode = (hashCode * 397) ^ Combine(second);
            hashCode = (hashCode * 397) ^ Combine(third);
            hashCode = (hashCode * 397) ^ Combine(fourth);

            return hashCode;
        }
    }
}
#endif