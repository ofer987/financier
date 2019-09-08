using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System;
using System.Linq;

using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Web.ViewModels;

namespace Financier.Web.Controllers
{
    public class MonthlyStatementsController : Controller
    {
        public IActionResult Get(int year)
        {
            var statements = GetStatements(year, 1, 12);

            // var statement = new MonthlyStatement(year, month);
            // var viewModel = new MonthlyExpenses(year, month, statement);

            return View(statements);
            // return $"yo man {year} and {month} and {nameof(id)} = {id} and {nameof(myIds)} = {myIds}";
        }

        private IEnumerable<MonthlyStatement> GetStatements(int year, int startMonth, int endMonth)
        {
            for (var i = startMonth; i <= endMonth; i += 1) 
            {
                yield return new MonthlyStatement(year, i);
            }
        }
    }
}
