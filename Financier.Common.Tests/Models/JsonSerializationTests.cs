using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections;
using System.Text.Json.Serialization;
using NUnit.Framework;

#nullable enable
namespace Financier.Common.Tests.Models
{
    public class Model
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("score")]
        public float Score { get; init; }

        public override string ToString()
        {
            if (Success)
            {
                return $"Success! Has a Score of {Score}";
            }

            return $"Failure! Has a Score of {Score}";
        }

        public override int GetHashCode()
        {
            return Success.GetHashCode() + Score.GetHashCode();
        }

        public override bool Equals([AllowNull] object obj)
        {
            var other = obj as Model;
            if (other is null)
            {
                return false;
            }

            if (Success != other.Success || Score != other.Score)
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Model x, Model y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Model x, Model y)
        {
            return !(x == y);
        }
    }

    public class JsonSerializationTests
    {
        public static IEnumerable GetTestCases_Success()
        {
            yield return new TestCaseData
            (
                 "{\"success\": true, \"score\": 0.9}",
                 new Model
                 {
                     Success = true,
                     Score = 0.9F
                 }
            );

            yield return new TestCaseData
            (
                 "{\"success\": true, \"score\": 1.0}",
                 new Model
                 {
                     Success = true,
                     Score = 1.0F
                 }
            );

            yield return new TestCaseData
            (
                 "{\"success\": false, \"score\": 1.0}",
                 new Model
                 {
                     Success = false,
                     Score = 1.0F
                 }
            );
        }

        public static IEnumerable GetTestCases_Failure()
        {
            yield return new TestCaseData
            (
                 "{\"success\": f2alse, \"score\": 1.0}",
                 new Model
                 {
                     Success = false,
                     Score = 1.0F
                 }
            );

            yield return new TestCaseData
            (
                 "{\"success\": false, \"score\": 1.0}",
                 new Model
                 {
                     Success = false,
                     Score = 0.0F
                 }
            );

            yield return new TestCaseData
            (
                 "{\"success\": true, \"score\": 1.0}",
                 new Model
                 {
                     Success = false,
                     Score = 1.0F
                 }
            );

            yield return new TestCaseData
            (
                 "{ }",
                 new Model
                 {
                     Success = false,
                     Score = 1.0F
                 }
            );

            yield return new TestCaseData
            (
                 "{ }",
                 null
            );

            yield return new TestCaseData
            (
                 string.Empty,
                 null
            );

            yield return new TestCaseData
            (
                 string.Empty,
                 null
            );
        }

        [TestCaseSource(nameof(GetTestCases_Success))]
        public async Task Test_JsonSerialization_Deserialize_Success(string value, Model expected)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            var actual = await JsonSerializer.DeserializeAsync<Model?>(stream);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(GetTestCases_Failure))]
        public async Task Test_JsonSerialization_Deserialize_Failure(string value, Model expected)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));

            try
            {
                var actual = await JsonSerializer.DeserializeAsync<Model?>(stream);

                Assert.That(actual, Is.Not.EqualTo(expected));
                return;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
#nullable disable
