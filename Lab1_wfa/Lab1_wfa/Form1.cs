using System.Globalization;
using ScottPlot.WinForms;

namespace Lab1_wfa;

public partial class Form1 : Form
{
    double[,] samples = new double[0, 0];
    List<KMeansRound> rounds = new List<KMeansRound>();

    ComboBox variantBox = new ComboBox();
    ComboBox roundBox = new ComboBox();
    Button runButton = new Button();
    TextBox reportBox = new TextBox();
    FormsPlot plot = new FormsPlot();

    public Form1()
    {
        InitializeComponent();
        Text = "K-means";
        ClientSize = new Size(1200, 700);

        variantBox.DropDownStyle = ComboBoxStyle.DropDownList;
        variantBox.Items.Add("1: Euklidesowa, 3 grupy, 10 iteracji");
        variantBox.Items.Add("2: |x1|, 4 grupy, 10 iteracji");
        variantBox.SelectedIndex = 0;
        variantBox.Width = 280;

        runButton.Text = "Uruchom";
        runButton.Width = 110;
        runButton.Height = 35;
        runButton.Click += RunClicked;

        roundBox.DropDownStyle = ComboBoxStyle.DropDownList;
        roundBox.Width = 160;
        roundBox.SelectedIndexChanged += (s, e) => Draw();

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(5) };
        top.Controls.Add(variantBox);
        top.Controls.Add(runButton);
        top.Controls.Add(roundBox);

        reportBox.Multiline = true;
        reportBox.ReadOnly = true;
        reportBox.ScrollBars = ScrollBars.Vertical;
        reportBox.Dock = DockStyle.Right;
        reportBox.Width = 420;

        plot.Dock = DockStyle.Fill;

        Controls.Add(plot);
        Controls.Add(reportBox);
        Controls.Add(top);

        LoadSamples();
    }
    
    void LoadSamples()
    {
        string[] lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "spiralka.csv"));
        samples = new double[lines.Length, 2];
        for (int s = 0; s < lines.Length; s++)
        {
            string[] parts = lines[s].Split(',');
            samples[s, 0] = double.Parse(parts[0], CultureInfo.InvariantCulture);
            samples[s, 1] = double.Parse(parts[1], CultureInfo.InvariantCulture);
        }
    }

    void RunClicked(object? sender, EventArgs e)
    {
        bool second = variantBox.SelectedIndex == 1;
        rounds = Kmeans.KMeansRounds(samples, second ? 4 : 3, 10, second, new[] { 4, 10 });

        roundBox.Items.Clear();
        foreach (var r in rounds) roundBox.Items.Add($"Po {r.Iteration} iteracjach");
        roundBox.SelectedIndex = 0;
    }
    
    void Draw()
    {
        if (roundBox.SelectedIndex < 0) return;
        var round = rounds[roundBox.SelectedIndex];
        int m = round.Center.GetLength(0);
        string[] colors = { "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd" };

        plot.Plot.Clear();
        for (int j = 0; j < m; j++)
        {
            var xs = new List<double>();
            var ys = new List<double>();
            for (int s = 0; s < samples.GetLength(0); s++)
            {
                if (round.Assignments[s] != j) continue;
                xs.Add(samples[s, 0]);
                ys.Add(samples[s, 1]);
            }
            if (xs.Count == 0) continue;
            var series = plot.Plot.Add.ScatterPoints(xs.ToArray(), ys.ToArray());
            series.Color = ScottPlot.Color.FromHex(colors[j]);
            series.LegendText = $"Grupa {j + 1}";
        }

        var cx = new double[m];
        var cy = new double[m];
        for (int j = 0; j < m; j++)
        {
            cx[j] = round.Center[j, 0];
            cy[j] = round.Center[j, 1];
        }
        var centers = plot.Plot.Add.ScatterPoints(cx, cy);
        centers.Color = ScottPlot.Color.FromHex("#000000");
        centers.MarkerShape = ScottPlot.MarkerShape.Cross;
        centers.MarkerSize = 16;
        centers.LegendText = "Środki grup";

        plot.Plot.ShowLegend();
        plot.Plot.Axes.AutoScale();
        plot.Refresh();

        reportBox.Text = Kmeans.Result(samples, round);
    }
}