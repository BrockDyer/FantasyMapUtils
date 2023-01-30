namespace OptimizationLib;

public class OptimizationParameters
{
    private readonly Dictionary<string, object> _parameters;

    public OptimizationParameters()
    {
        _parameters = new Dictionary<string, object>();
    }

    /// <summary>
    /// Set a parameter in the map. Does not override any existing parameter.
    /// </summary>
    /// <param name="label">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>True if the parameter was successfully set. False if the parameter already exists.</returns>
    public bool SetParameter(string label, object value)
    {
        if (_parameters.ContainsKey(label)) return false;
        _parameters.Add(label, value);
        return true;
    }

    /// <summary>
    /// Get a parameter from the map.
    /// </summary>
    /// <param name="label">The name of the parameter.</param>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <returns>The parameter value.</returns>
    /// <exception cref="KeyNotFoundException">If the parameter does not exist.</exception>
    /// <exception cref="InvalidCastException">If the type specified does not match the actual type.</exception>
    public T GetParameter<T>(string label)
    {
        if (!_parameters.ContainsKey(label)) throw new KeyNotFoundException($"The parameter {label} does not exist.");
        var value = _parameters[label];
        if (value.GetType() == typeof(T))
        {
            return (T)value;
        }

        throw new InvalidCastException($"The expected type of parameter {label} did not match the actual type.");
    }
}