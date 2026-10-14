using System.Globalization;
using ScottPlot;

var ci = CultureInfo.InvariantCulture;
string dir = args.Length > 0 ? args[0] : FindSolutionDir();
string[] colors = { "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd", "#8c564b" };

string[] files = Directory.GetFiles(dir, "lab_01.*_iter*.csv", SearchOption.AllDirectories);
if (files.Length == 0)
{
    Console.WriteLine($"No lab_01.*_iter*.csv files found in {dir} (searched recursively)");
    return;
}

foreach (string file in files)
{
    string[] lines = File.ReadAllLines(file).Skip(1).ToArray();
    var groups = new SortedDictionary<int, (List<double> x, List<double> y)>();
    var cx = new List<double>();
    var cy = new List<double>();

    foreach (string line in lines)
    {
        string[] parts = line.Split(';');
        double x = double.Parse(parts[0].Replace(",", "."), ci);
        double y = double.Parse(parts[1].Replace(",", "."), ci);
        if (parts[2].StartsWith("center"))
        {
            cx.Add(x);
            cy.Add(y);
            continue;
        }
        int g = int.Parse(parts[2]);
        if (!groups.ContainsKey(g)) groups[g] = (new List<double>(), new List<double>());
        groups[g].x.Add(x);
        groups[g].y.Add(y);
    }

    Plot plt = new();
    plt.Title(Path.GetFileNameWithoutExtension(file));
    plt.XLabel("x1");
    plt.YLabel("x2");

    foreach (var kv in groups)
    {
        var series = plt.Add.ScatterPoints(kv.Value.x.ToArray(), kv.Value.y.ToArray());
        series.Color = Color.FromHex(colors[(kv.Key - 1) % colors.Length]);
        series.MarkerSize = 8;
        series.LegendText = $"Group {kv.Key}";
    }

    var centers = plt.Add.ScatterPoints(cx.ToArray(), cy.ToArray());
    centers.Color = Color.FromHex("#000000");
    centers.MarkerShape = MarkerShape.Cross;
    centers.MarkerSize = 16;
    centers.LegendText = "Group centers";

    plt.ShowLegend();
    string output = Path.ChangeExtension(file, ".png");
    plt.SavePng(output, 900, 700);
    Console.WriteLine($"Saved {output}");
}

static string FindSolutionDir()
{
    foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
    {
        DirectoryInfo? d = new DirectoryInfo(start);
        while (d != null)
        {
            if (File.Exists(Path.Combine(d.FullName, "lab_01.sln"))) return d.FullName;
            d = d.Parent;
        }
    }
    return Directory.GetCurrentDirectory();
}
