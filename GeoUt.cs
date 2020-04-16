using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RT.TagSoup;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace Geocos
{
    static class GeoUt
    {
        public static void WriteHtml(string stuff, string filename = null)
        {
            File.WriteAllText(filename ?? @"D:\Daten\Upload\Geocos\Equations.htm",
                @"<!DOCTYPE html>

<head>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
    <title>Equations</title>
    <style type='text/css'>
        .possible {{ background: #eee; }}
        small {{ font-size: 50%; color: #ccc; }}
    </style>
    <script type=""text/x-mathjax-config"">
        MathJax.Hub.Config({{
          tex2jax: {{ inlineMath: [['$','$'], ['\\(','\\)']] }}
        }});
    </script>
    <script type='text/javascript' src='http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML'></script>
</head>

<body>{0}</body>".Fmt(stuff));
        }

        public static Dictionary<string, Dictionary<int, List<Tuple<string /* ID to link to */, string /* inference */>>>> _inferences =
            new Dictionary<string, Dictionary<int, List<Tuple<string, string>>>>();

        public static Dictionary<string, Tuple<Variable, Formula>[]> _knowns =
            new Dictionary<string, Tuple<Variable, Formula>[]>();

        public static void GenerateHtml(this Polynomial[] equations, List<Tuple<Variable, Formula>> knowns, string id, string nextId, int eqIndex, string inferred)
        {
            if (inferred != null)
                _inferences.AddSafe(id, eqIndex, Tuple.Create(nextId, inferred));
            else if (_inferences.ContainsKey(id))
                return;

            Dictionary<int, List<Tuple<string, string>>> tuples = null;
            _inferences.TryGetValue(id, out tuples);

            var filename = @"D:\Daten\Upload\Geocos\{0}.htm".Fmt(id);
            Console.WriteLine("Writing {0}".Fmt(filename));
            GeoUt.WriteHtml(
                @"<h2>Knowns</h2>{0}<h2>Equations</h2><ul>{1}</ul><h2>Maxima</h2><div>solve ( [ {2} ], [ {3} ] );</div><h2>Sage</h2><div>{3} = var('{3}')</div><div>solve([{4}], {3})</div>".Fmt(
                    knowns.OrderBy(v => v.Item1.Name).Select(v => "$ {0} = {1} $".Fmt(v.Item1, v.Item2)).JoinString(", "),
                    equations.Select((eq, i) =>
                    {
                        List<Tuple<string, string>> theseTuples = null;
                        if (tuples != null)
                            tuples.TryGetValue(i, out theseTuples);

                        return @"<li class='{0}'>$ {1} = 0 $ <small>{2}</small>{3}</li>".Fmt(
                            theseTuples != null ? "possible" : null,
                            eq,
                            eq.ToMaple(),
                            @"<blockquote><ol>{0}</ol>{1}</blockquote>".Fmt(
                                eq.Variables.Select((v, vi) =>
                                    "<li>$ = {0} $</li>".Fmt(
                                        eq.CollectTerms(vi)
                                            .Select((ct, ctp) => new { Term = ct, Power = ctp })
                                            .Where(inf => inf.Term != 0)
                                            .Select(inf => (inf.Term == 1 ? (inf.Power == 0 ? "1" : inf.Power == 1 ? "{1}" : "{1}^{2}") : inf.Power == 0 ? "({0})" : inf.Power == 1 ? "({0}) {1}" : "({0}) {1}^{2}").Fmt(inf.Term, v, inf.Power)).JoinString(" + "))
                                ).JoinString(),
                                theseTuples.NullOr(ts => ts.Select(t => @"<div>⇒ {0}{1}</div>".Fmt(t.Item2, t.Item1.NullOr(link => @" (<a href='{0}.htm'>go</a>)".Fmt(link)))).JoinString())
                            )
                        );
                    }).JoinString(),
                    equations.Select(eq => "{0} = 0".Fmt(eq.ToMaple())).JoinString(", "),
                    equations.SelectMany(eq => eq.Variables).Distinct().JoinString(", "),
                    equations.Select(eq => "{0} == 0".Fmt(eq.ToMaple())).JoinString(", ")
                ),
                filename
            );
        }

        public static IEnumerable<T> DistinctBy<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("predicate");
            var set = new HashSet<TResult>();
            foreach (var elem in source)
                if (set.Add(selector(elem)))
                    yield return elem;
        }

        public static void OutputTable(string[] columnHeaders, string[] rowHeaders, string[][] cells, string linkToNext, string filename)
        {
            var omitColumns = Enumerable.Range(0, columnHeaders.Length).Where(col => cells.All(row => row[col] == null)).ToHashSet();
            File.WriteAllText(
                filename,
                new HTML(
                    new HEAD(
                        new STYLELiteral(@"
                            table { border-collapse: collapse; }
                            td, th { white-space: nowrap; border: 1px solid #ccc; }
                        "),
                        new RawTag(@"
                            <script type='text/x-mathjax-config'>
                                MathJax.Hub.Config({
                                  tex2jax: { inlineMath: [['$','$']] }
                                });
                            </script>
                        "),
                        new SCRIPT { type = "text/javascript", src = "http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML" }
                    ),
                    new BODY(
                        new P("Number of columns: ", columnHeaders.Length - omitColumns.Count),
                        new P(new A { href = linkToNext }._("next")),
                        new TABLE(
                            new TR(new TH(), columnHeaders.Select((col, colIndex) => omitColumns.Contains(colIndex) ? null : new TH(col))),
                            rowHeaders.Select((rowHeader, rowIndex) => new TR(
                                new TH(rowHeader),
                                columnHeaders.Select((col, colIndex) => omitColumns.Contains(colIndex) ? null : new TD(cells[rowIndex][colIndex]))
                            ))
                        )
                    )
                ).ToString()
            );
        }
    }
}
