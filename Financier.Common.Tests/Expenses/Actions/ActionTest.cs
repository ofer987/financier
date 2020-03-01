using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class ActionTest
    {
        public static Product Television { get; } = new SimpleProduct("house", new Money(40.00M, new DateTime(2020, 1, 1)));

        [SetUp]
        public void Init()
        {
        }

        public static IEnumerable ValidActions()
        {
            yield return new TestCaseData(
                new List<IAction>
                {
                    new Purchase(Television, new DateTime(2020, 1, 1)),
                    new Sale(Television, new Money(2000, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1)),
                    new Purchase(Television, new DateTime(2020, 12, 1)),
                    new Sale(Television, new Money(2000, new DateTime(2020, 1, 1)), new DateTime(2020, 12, 1))
                }
            );
        }

        public static IEnumerable InvalidActions()
        {
            yield return new TestCaseData(
                new List<IAction>
                {
                    new Purchase(Television, new DateTime(2020, 1, 1)),
                    new Purchase(Television, new DateTime(2020, 12, 1))
                }
            );

            // Will not work because there is no way to check previous action
            // yield return new TestCaseData(
            //     new List<IAction>
            //     {
            //         new Sale(Television, new Money(2000, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1))
            //     }
            // );

            yield return new TestCaseData(
                new List<IAction>
                {
                    new Purchase(Television, new DateTime(2020, 1, 1)),
                    new Purchase(Television, new DateTime(2020, 12, 1))
                }
            );

            yield return new TestCaseData(
                new List<IAction>
                {
                    new Purchase(Television, new DateTime(2020, 1, 1)),
                    new Sale(Television, new Money(2000, new DateTime(2020, 1, 1)), new DateTime(2020, 12, 1)),
                    new Sale(Television, new Money(2000, new DateTime(2020, 1, 1)), new DateTime(2020, 12, 2))
                }
            );
        }

        [TestCaseSource(nameof(ValidActions))]
        public void Test_ActionList_ActionsAreValid(List<IAction> actions)
        {
            Assert.DoesNotThrow(() =>
            {
                var current = actions.First();
                foreach (var next in actions.Skip(1))
                {
                    current.Next = next;
                    current = next;
                }
            });
        }

        [TestCaseSource(nameof(ValidActions))]
        public void Test_ActionList_NextIsValid(List<IAction> actions)
        {
            var current = actions.First();
            foreach (var next in actions.Skip(1))
            {
                current.Next = next;
                Assert.That(current.Next, Is.EqualTo(next));

                current = next;
            }
        }

        [TestCaseSource(nameof(InvalidActions))]
        public void Test_ActionList_ActionsAreNotValid(List<IAction> actions)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var current = actions.First();
                foreach (var next in actions.Skip(1))
                {
                    current.Next = next;

                    current = next;
                }
            });
        }

        [Test]
        public void Test_NullAction_CannotSetNext()
        {
            Assert.Throws<NotImplementedException>(() => NullAction.Instance.Next = new Purchase(Television, new DateTime(2021, 1, 1)));
        }
    }
}
