using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xunit;

namespace SyncData.Test.Localization
{
    /// <summary>
    /// Ensures every translation stays in sync with the neutral (English) resources:
    /// same keys, same format placeholders, no empty values and no duplicate keys.
    /// </summary>
    public class ResourceParityTests
    {
        private static readonly string ResxRoot = Path.Combine(AppContext.BaseDirectory, "resx");

        public static IEnumerable<object[]> ResourceSets()
        {
            yield return new object[] { Path.Combine(ResxRoot, "core") };
            yield return new object[] { Path.Combine(ResxRoot, "gui") };
        }

        [Theory]
        [MemberData(nameof(ResourceSets))]
        public void Translations_HaveSameKeysAsNeutral(string directory)
        {
            var neutralKeys = LoadValues(Path.Combine(directory, "Strings.resx")).Keys.ToHashSet();

            foreach (var translation in TranslationFiles(directory))
            {
                var keys = LoadValues(translation).Keys.ToHashSet();
                var missing = neutralKeys.Except(keys).OrderBy(k => k).ToList();
                var extra = keys.Except(neutralKeys).OrderBy(k => k).ToList();

                Assert.True(missing.Count == 0,
                    $"{Path.GetFileName(translation)} is missing keys: {string.Join(", ", missing)}");
                Assert.True(extra.Count == 0,
                    $"{Path.GetFileName(translation)} has extra keys: {string.Join(", ", extra)}");
            }
        }

        [Theory]
        [MemberData(nameof(ResourceSets))]
        public void Translations_HaveSamePlaceholdersAsNeutral(string directory)
        {
            var neutral = LoadValues(Path.Combine(directory, "Strings.resx"));

            foreach (var translation in TranslationFiles(directory))
            {
                var values = LoadValues(translation);

                foreach (var (key, neutralValue) in neutral)
                {
                    if (!values.TryGetValue(key, out var translated))
                    {
                        continue; // reported by the key parity test
                    }

                    var expected = ExtractPlaceholders(neutralValue);
                    var actual = ExtractPlaceholders(translated);

                    Assert.True(expected.SetEquals(actual),
                        $"{Path.GetFileName(translation)}: key '{key}' has different placeholders " +
                        $"(expected {{{string.Join(",", expected.OrderBy(x => x))}}}, got {{{string.Join(",", actual.OrderBy(x => x))}}})");
                }
            }
        }

        [Theory]
        [MemberData(nameof(ResourceSets))]
        public void Resources_HaveNoEmptyValues(string directory)
        {
            foreach (var file in AllResourceFiles(directory))
            {
                foreach (var (key, value) in LoadValues(file))
                {
                    Assert.True(!string.IsNullOrWhiteSpace(value), $"{Path.GetFileName(file)}: key '{key}' is empty");
                }
            }
        }

        [Theory]
        [MemberData(nameof(ResourceSets))]
        public void Resources_HaveNoDuplicateKeys(string directory)
        {
            foreach (var file in AllResourceFiles(directory))
            {
                var duplicates = XDocument.Load(file).Root!
                    .Elements("data")
                    .Select(element => (string?)element.Attribute("name"))
                    .Where(name => name is not null)
                    .GroupBy(name => name!)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();

                Assert.True(duplicates.Count == 0,
                    $"{Path.GetFileName(file)} has duplicate keys: {string.Join(", ", duplicates)}");
            }
        }

        private static IEnumerable<string> AllResourceFiles(string directory) =>
            Directory.GetFiles(directory, "Strings*.resx");

        private static IEnumerable<string> TranslationFiles(string directory) =>
            AllResourceFiles(directory)
                .Where(path => !string.Equals(Path.GetFileName(path), "Strings.resx", StringComparison.OrdinalIgnoreCase));

        private static Dictionary<string, string> LoadValues(string path)
        {
            var result = new Dictionary<string, string>();
            foreach (var data in XDocument.Load(path).Root!.Elements("data"))
            {
                var name = (string?)data.Attribute("name");
                if (name is not null)
                {
                    result[name] = data.Element("value")?.Value ?? string.Empty;
                }
            }

            return result;
        }

        private static HashSet<int> ExtractPlaceholders(string value)
        {
            var result = new HashSet<int>();
            foreach (Match match in Regex.Matches(value, @"\{(\d+)[^}]*\}"))
            {
                result.Add(int.Parse(match.Groups[1].Value));
            }

            return result;
        }
    }
}
