using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VNCreator
{
    public class Collection<T> : IEnumerable<(string option, T value)>
    {
        private readonly int count;
        private readonly string[] sortedOptions;

        private readonly Dictionary<string, T> valuesDict = new();
        private readonly Dictionary<T, string> optionsDict = new();

        private string[] displayedOptions;

        public string[] Options => displayedOptions;
        public IEnumerable<T> Values { get; private set; }

        public Collection(IEnumerable<T> values, IEnumerable<string> options, bool autoSort = true)
        {
            // create dict
            var valueList = new List<T>(values);
            var optionList = new List<string>(options);

            Values = valueList;
            count = valueList.Count;

            for (var i = 0; i < count; i++)
            {
                var option = optionList[i];
                var value = valueList[i];

                valuesDict[option] = value;
                optionsDict[value] = option;
            }

            // sort
            sortedOptions = (autoSort ? options.OrderBy(x => x) : options).ToArray();

            // reset filter
            ResetFilter();
        }

        public int GetOptionIndex(T optionKey)
        {
            return optionKey != null && optionsDict.TryGetValue(optionKey, out var value)
                ? Array.IndexOf(displayedOptions, value)
                : -1;
        }

        public T GetValue(int optionIndex)
        {
            return GetValue(GetDisplayValue(optionIndex));
        }

        public T GetValue(string displayValue)
        {
            return !string.IsNullOrEmpty(displayValue) && valuesDict.TryGetValue(displayValue, out var value)
                ? value
                : default;
        }

        public string GetDisplayValue(int optionIndex)
        {
            return displayedOptions.ElementAtOrDefault(optionIndex);
        }

        public string GetDisplayValue(T value)
        {
            var optionIndex = GetOptionIndex(value);

            return displayedOptions.ElementAtOrDefault(optionIndex);
        }

        public void SetFilter(string filter)
        {
            displayedOptions = sortedOptions
                .Where(option => CheckFilter(option, filter))
                .ToArray();
        }

        private static bool CheckFilter(string option, string filter)
        {
            return option.Contains("<none>")
                || string.IsNullOrEmpty(filter)
                || option.ToLower().Contains(filter.ToLower());
        }

        public bool Contains(T value)
        {
            return valuesDict.ContainsValue(value);
        }

        public void ResetFilter()
        {
            displayedOptions = sortedOptions;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<(string option, T value)> GetEnumerator()
        {
            foreach (var v in valuesDict)
            {
                yield return (v.Key, v.Value);
            }
        }
    }
}