using ScottPlot;
using ScottPlot.WinForms;
using Label = System.Windows.Forms.Label;

namespace Lab2_wfa;

record Scenario(string Name, double StartMin, double StartMax, double Spread, int[] Checkpoints);

public partial class Form1 : Form
{
    const double RangeMin = 0, RangeMax = 100, IncreaseSpeed = 1.1;
    const int Iterations = 100;

    readonly Scenario[] scenarios =
    {
        new("1: start z [0;100], rozrzut 10 (0/5/10/15)", 0, 100, 10, new[] { 0, 5, 10, 15 }),
        new("2: start z [0;100], rozrzut 10 (0..20)",     0, 100, 10, Enumerable.Range(0, 21).ToArray()),
        new("3: start z [15;35], rozrzut 5 (0..20)",      15, 35,  5, Enumerable.Range(0, 21).ToArray()),
    };

    readonly ComboBox scenarioBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
    readonly NumericUpDown seedBox = new() { Minimum = 0, Maximum = 1_000_000, Value = 1, Width = 90 };
    readonly Button randomSeedButton = new() { Text = "Losuj seed", AutoSize = true };
    readonly Button runButton = new() { Text = "Uruchom", AutoSize = true };
    readonly Button animateButton = new() { Text = "Animuj", AutoSize = true };
    readonly TrackBar iterBar = new() { Minimum = 0, Maximum = Iterations, Width = 300, TickFrequency = 5 };
    readonly Label iterLabel = new() { AutoSize = true, Text = "Iteracja: 0", Padding = new Padding(0, 6, 0, 0) };
    readonly DataGridView grid = new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };
    readonly TextBox summaryBox = new()
    {
        Dock = DockStyle.Bottom, Height = 260, Multiline = true, ReadOnly = true,
        ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 9)
    };
    readonly FormsPlot plot = new() { Dock = DockStyle.Fill };
    readonly System.Windows.Forms.Timer timer = new() { Interval = 150 };

    List<IterationResult> history = new();
    public Form1()
    {
        InitializeComponent();
        Text = "Algorytm 1+1 – sin(x/10)·sin(x/200)";
        ClientSize = new Size(1300, 750);

        foreach (var s in scenarios) scenarioBox.Items.Add(s.Name);
        scenarioBox.SelectedIndex = 0;

        runButton.Click += (_, _) => RunAlgorithm();
        randomSeedButton.Click += (_, _) => seedBox.Value = Random.Shared.Next(0, 1_000_000);
        iterBar.ValueChanged += (_, _) => ShowIteration(iterBar.Value);
        animateButton.Click += (_, _) => { iterBar.Value = 0; timer.Start(); };
        timer.Tick += (_, _) =>
        {
            if (iterBar.Value < iterBar.Maximum) iterBar.Value++;
            else timer.Stop();
        };

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5) };
        top.Controls.AddRange(new Control[]
        {
            scenarioBox, new Label { Text = "Seed:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) },
            seedBox, randomSeedButton, runButton, animateButton, iterBar, iterLabel
        });

        var right = new Panel { Dock = DockStyle.Right, Width = 430 };
        right.Controls.Add(grid);
        right.Controls.Add(summaryBox);

        Controls.Add(plot);
        Controls.Add(right);
        Controls.Add(top);

        RunAlgorithm();
    }
    
    void RunAlgorithm()
    {
        timer.Stop();
        var sc = scenarios[scenarioBox.SelectedIndex];

        history = OnePlusOne.GetIterandrationResults(sc.Spread, IncreaseSpeed, Iterations, RangeMin, RangeMax);
        grid.DataSource = history;
        foreach (var col in new[] { "X", "Y", "Spread" })
            grid.Columns[col]!.DefaultCellStyle.Format = "F5";

        summaryBox.Text = BuildSummary(sc);
        iterBar.Value = 0;
        ShowIteration(0);
    }

    string BuildSummary(Scenario sc)
    {
        var lines = new List<string>
        {
            $"Scenariusz {sc.Name}",
            $"Seed = {seedBox.Value}",
            "",
            $"{"iter",4} | {"x",10} | {"y",10} | {"rozrzut",10}",
            new string('-', 44)
        };
        foreach (int k in sc.Checkpoints)
        {
            var r = history[k];
            lines.Add($"{r.Iteration,4} | {r.X,10:F4} | {r.Y,10:F5} | {r.Spread,10:F4}");
        }
        var last = history[^1];
        lines.Add("");
        lines.Add($"Po {Iterations} iteracjach: x = {last.X:F4}, y = {last.Y:F5}");
        return string.Join(Environment.NewLine, lines);
    }
    
    void ShowIteration(int k)
    {
        if (history.Count == 0) return;
        var cur = history[k];
        iterLabel.Text = $"Iteracja: {k}   x = {cur.X:F3}   y = {cur.Y:F4}   rozrzut = {cur.Spread:F3}";

        plot.Plot.Clear();

        double[] xs = Enumerable.Range(0, 1001).Select(i => RangeMin + i * (RangeMax - RangeMin) / 1000).ToArray();
        double[] ys = xs.Select(OnePlusOne.Function).ToArray();
        var curve = plot.Plot.Add.Scatter(xs, ys);
        curve.MarkerSize = 0;
        curve.LegendText = "f(x) = sin(x/10)·sin(x/200)";

        var visited = history.Take(k + 1).ToArray();
        var path = plot.Plot.Add.Scatter(visited.Select(r => r.X).ToArray(), visited.Select(r => r.Y).ToArray());
        path.LineWidth = 0;
        path.MarkerSize = 6;
        path.LegendText = "x w kolejnych iteracjach";

        plot.Plot.Add.Marker(cur.X, cur.Y, MarkerShape.FilledCircle, 14, Colors.Red);

        plot.Plot.Title($"Iteracja {k}");
        plot.Plot.XLabel("x");
        plot.Plot.YLabel("f(x)");
        plot.Plot.ShowLegend();
        plot.Plot.Axes.SetLimits(RangeMin, RangeMax, -0.3, 0.45);
        plot.Refresh();

        if (k < grid.Rows.Count)
        {
            grid.ClearSelection();
            grid.Rows[k].Selected = true;
            grid.FirstDisplayedScrollingRowIndex = Math.Max(0, k - 5);
        }
    }
}