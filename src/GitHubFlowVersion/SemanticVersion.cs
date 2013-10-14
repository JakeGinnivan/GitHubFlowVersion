using System;

namespace GitHubFlowVersion
{
    public class SemanticVersion
    {
        public SemanticVersion(int major, int minor, int patch, string suffix = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Suffix = suffix;
        }

        protected bool Equals(SemanticVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch && string.Equals(Suffix, other.Suffix);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SemanticVersion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Major;
                hashCode = (hashCode*397) ^ Minor;
                hashCode = (hashCode*397) ^ Patch;
                hashCode = (hashCode*397) ^ (Suffix != null ? Suffix.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return !Equals(left, right);
        }

        public static bool operator >(SemanticVersion v1, SemanticVersion v2)
        {
            return (v2 < v1);
        }

        public static bool operator >=(SemanticVersion v1, SemanticVersion v2)
        {
            return (v2 <= v1);
        }

        public static bool operator <=(SemanticVersion v1, SemanticVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) <= 0);
        }

        public static bool operator <(SemanticVersion v1, SemanticVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) < 0);
        }

        public int CompareTo(SemanticVersion value)
        {
            if (value == null)
            {
                return 1;
            }
            if (value.Equals(this))
            {
                return 0;
            }
            if (Major != value.Major)
            {
                if (Major > value.Major)
                {
                    return 1;
                }
                return -1;
            }
            if (Minor != value.Minor)
            {
                if (Minor > value.Minor)
                {
                    return 1;
                }
                return -1;
            }
            if (Patch != value.Patch)
            {
                if (Patch > value.Patch)
                {
                    return 1;
                }
                return -1;
            }
            return -1;
        }

        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Patch { get; private set; }
        public string Suffix { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}{3}", Major, Minor, Patch, string.IsNullOrEmpty(Suffix) ? null : "-" + Suffix);
        }
    }
}