/// <summary>
/// any -> object
/// 
/// string -> int
///        -> float
///        -> enum?
///        -> custom type that inherited special interface
/// 
/// int -> bool
///     -> float
///     -> string
///     -> enum
///     -> 
/// 
/// float -> bool
///       -> int
///       -> string
///       -> enum?
///       -> 
/// 
/// custom type that inherited special interface -> string
/// </summary>

namespace PotikotTools
{
    public interface IStringConvertible
    {
        bool FromString(string value);
    }
}