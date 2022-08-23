namespace Web.Models.Helper;

public class MovieEqualityComparer : IEqualityComparer<Movie>
{
    public bool Equals(Movie x, Movie y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.RatingKey == y.RatingKey;
    }

    public int GetHashCode(Movie obj)
    {
        return obj.RatingKey.GetHashCode();
    }
}