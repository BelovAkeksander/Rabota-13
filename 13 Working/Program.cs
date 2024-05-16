using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// Класс дракона с его свойствами
public class Dragon
{
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int AttackLevel { get; set; }
    public int NavalBattlesCount { get; set; }
    public bool Visibility { get; set; }
    public Color DragonColor { get; set; }
    public string Name { get; set; }

    // Метод установки цвета по входной строке RGB
    public bool SetColor(string input)
    {
        var regex = new Regex(@"^\s*(\d{1,3})\s*,?\s*(\d{1,3})\s*,?\s*(\d{1,3})\s*$");
        if (regex.IsMatch(input))
        {
            var matches = regex.Match(input);
            int r = int.Parse(matches.Groups[1].Value);
            int g = int.Parse(matches.Groups[2].Value);
            int b = int.Parse(matches.Groups[3].Value);
            if (r < 256 && g < 256 && b < 256)
            {
                DragonColor = Color.FromArgb(r, g, b);
                return true;
            }
        }
        return false;
    }

    // Метод для установки имени с проверкой регулярного выражения
    public bool SetName(string input)
    {
        if (Regex.IsMatch(input, @"^[A-ZА-Я][a-zа-я]+(?:-[A-ZА-Я][a-zа-я]+)* [IVXLCDM]+$"))
        {
            Name = input;
            return true;
        }
        return false;
    }
}

// Основная форма приложения
public class MainForm : Form
{
    private List<Dragon> dragons = new List<Dragon>();
    private Button btnAddDragon, btnShowCriteriaForm, btnReset;
    private FlowLayoutPanel lstDragons;
    private DateTimePicker timePicker;

    public MainForm()
    {
        InitializeComponents();
        Text = "Управление драконами";
        Size = new Size(600, 450);
    }

    private void InitializeComponents()
    {
        btnAddDragon = new Button { Text = "Добавить дракона", Location = new Point(10, 10), Size = new Size(150, 30) };
        btnShowCriteriaForm = new Button { Text = "Выбор драконов на турнир", Location = new Point(170, 10), Size = new Size(200, 30) };
        btnReset = new Button { Text = "Сброс данных", Location = new Point(380, 10), Size = new Size(100, 30) };
        lstDragons = new FlowLayoutPanel { Location = new Point(10, 50), Width = 560, Height = 350, AutoScroll = true };

        timePicker = new DateTimePicker
        {
            Location = new Point(10, 410),
            Width = 200,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "HH:mm:ss",
            ShowUpDown = true
        };

        btnAddDragon.Click += (sender, e) => ShowAddDragonForm();
        btnShowCriteriaForm.Click += (sender, e) => ShowCriteriaForm();
        btnReset.Click += (sender, e) => ResetData();

        Controls.AddRange(new Control[] { btnAddDragon, btnShowCriteriaForm, btnReset, lstDragons, timePicker });
    }

    private void ShowAddDragonForm()
    {
        var form = new DragonForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            dragons.Add(form.Dragon);
            RefreshDragonList();
        }
    }

    private void ShowCriteriaForm()
    {
        var form = new CriteriaForm(dragons, RefreshFilteredDragons);
        if (form.ShowDialog() == DialogResult.OK)
        {
        }
    }

    private void RefreshFilteredDragons(List<Dragon> filteredDragons)
    {
        lstDragons.Controls.Clear();
        foreach (var dragon in filteredDragons)
        {
            var panel = new Panel { Width = 550, Height = 30, BackColor = dragon.DragonColor };
            var checkBox = new CheckBox
            {
                Text = $"{dragon.Name} - HP: {dragon.CurrentHP}/{dragon.MaxHP}, Attack: {dragon.AttackLevel}",
                AutoSize = true,
                Parent = panel,
                Location = new Point(5, 5)
            };
            lstDragons.Controls.Add(panel);
        }
    }

    private void RefreshDragonList()
    {
        lstDragons.Controls.Clear();
        foreach (var dragon in dragons)
        {
            var panel = new Panel { Width = 550, Height = 30, BackColor = dragon.DragonColor };
            var checkBox = new CheckBox
            {
                Text = $"{dragon.Name} - HP: {dragon.CurrentHP}/{dragon.MaxHP}, Attack: {dragon.AttackLevel}",
                AutoSize = true,
                Parent = panel,
                Location = new Point(5, 5)
            };
            lstDragons.Controls.Add(panel);
        }
    }

    private void ResetData()
    {
        dragons.Clear();
        lstDragons.Controls.Clear();
    }
}

// Форма для добавления нового дракона
public class DragonForm : Form
{
    private TextBox txtMaxHP, txtCurrentHP, txtAttackLevel, txtNavalBattles, txtVisibility, txtColor, txtName;
    private Button btnAdd;
    public Dragon Dragon { get; private set; }

    public DragonForm()
    {
        InitializeComponent();
        Text = "Добавление дракона";
        Size = new Size(250, 300);
    }

    private void InitializeComponent()
    {
        txtMaxHP = new TextBox { Location = new Point(10, 10), Width = 200 };
        txtCurrentHP = new TextBox { Location = new Point(10, 40), Width = 200 };
        txtAttackLevel = new TextBox { Location = new Point(10, 70), Width = 200 };
        txtNavalBattles = new TextBox { Location = new Point(10, 100), Width = 200 };
        txtVisibility = new TextBox { Location = new Point(10, 130), Width = 200 };
        txtColor = new TextBox { Location = new Point(10, 160), Width = 200 };
        txtName = new TextBox { Location = new Point(10, 190), Width = 200 };
        btnAdd = new Button { Text = "Добавить", Location = new Point(10, 220), Size = new Size(100, 30) };
        btnAdd.Click += BtnAdd_Click;

        Controls.AddRange(new Control[] { txtMaxHP, txtCurrentHP, txtAttackLevel, txtNavalBattles, txtVisibility, txtColor, txtName, btnAdd });
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        var dragon = new Dragon();

        if (!int.TryParse(txtMaxHP.Text, out int maxHp) || maxHp <= 0)
        {
            MessageBox.Show("Некорректное значение максимального HP.");
            return;
        }

        if (!int.TryParse(txtCurrentHP.Text, out int currentHp) || currentHp < 0 || currentHp > maxHp)
        {
            MessageBox.Show("Некорректное значение текущего HP.");
            return;
        }

        if (!int.TryParse(txtAttackLevel.Text, out int attackLevel) || attackLevel < 0)
        {
            MessageBox.Show("Некорректное значение уровня атаки.");
            return;
        }

        if (!int.TryParse(txtNavalBattles.Text, out int navalBattles) || navalBattles < 0)
        {
            MessageBox.Show("Некорректное значение числа морских сражений.");
            return;
        }

        dragon.MaxHP = maxHp;
        dragon.CurrentHP = currentHp;
        dragon.AttackLevel = attackLevel;
        dragon.NavalBattlesCount = navalBattles;
        dragon.Visibility = txtVisibility.Text.Equals("1") || txtVisibility.Text.ToLower().Equals("true");

        if (!dragon.SetColor(txtColor.Text))
        {
            MessageBox.Show("Некорректное значение цвета.");
            return;
        }

        if (!dragon.SetName(txtName.Text))
        {
            MessageBox.Show("Некорректное имя.");
            return;
        }

        Dragon = dragon;
        DialogResult = DialogResult.OK;
    }
}

// Форма для указания критериев выбора драконов
public class CriteriaForm : Form
{
    private List<Dragon> dragons;
    private Action<List<Dragon>> refreshDragons;
    private TextBox txtMinHP, txtMaxHP, txtMinAttack, txtMaxAttack, txtMinHPPercentage;
    private Button btnApply;

    public CriteriaForm(List<Dragon> dragons, Action<List<Dragon>> refreshDragons)
    {
        InitializeComponent();
        this.dragons = dragons;
        this.refreshDragons = refreshDragons;
        Text = "Критерии выбора драконов";
        Size = new Size(250, 250);
    }

    private void InitializeComponent()
    {
        txtMinHP = new TextBox { Location = new Point(10, 10), Width = 200 };
        txtMaxHP = new TextBox { Location = new Point(10, 40), Width = 200 };
        txtMinAttack = new TextBox { Location = new Point(10, 70), Width = 200 };
        txtMaxAttack = new TextBox { Location = new Point(10, 100), Width = 200 };
        txtMinHPPercentage = new TextBox { Location = new Point(10, 130), Width = 200 };
        btnApply = new Button { Text = "Применить", Location = new Point(10, 160), Size = new Size(100, 30) };
        btnApply.Click += BtnApply_Click;

        Controls.AddRange(new Control[] { txtMinHP, txtMaxHP, txtMinAttack, txtMaxAttack, txtMinHPPercentage, btnApply });
    }

    private void BtnApply_Click(object sender, EventArgs e)
    {
        if (int.TryParse(txtMinHP.Text, out int minHp) && int.TryParse(txtMaxHP.Text, out int maxHp) &&
            int.TryParse(txtMinAttack.Text, out int minAttack) && int.TryParse(txtMaxAttack.Text, out int maxAttack) &&
            int.TryParse(txtMinHPPercentage.Text.TrimEnd('%'), out int minHPPercentage) &&
            minHp > 0 && maxHp >= minHp && minAttack >= 0 && maxAttack >= minAttack && minHPPercentage >= 1 && minHPPercentage <= 100)
        {
            var filteredDragons = dragons.Where(d => d.MaxHP >= minHp && d.MaxHP <= maxHp &&
                                                     d.AttackLevel >= minAttack && d.AttackLevel <= maxAttack &&
                                                     (double)d.CurrentHP / d.MaxHP * 100 >= minHPPercentage).ToList();
            refreshDragons(filteredDragons);
            DialogResult = DialogResult.OK;
        }
        else
        {
            MessageBox.Show("Некорректные критерии выбора.");
        }
    }
}

public class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
