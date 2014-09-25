using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zapisy
{
    public partial class Form1 : Form
    {
        private List<Group> groupsList;
        private int _groupsCurrentRowIndex;

        public Form1()
        {
            InitializeComponent();
            ReadCoursesDataFromFile("zapisy.txt");
                //do tego wzoru co podałeś to takie starcza
            //ale w tej wchili sprawdzasz tylko czy pasuje ogólnie do formatu wyszukiwania
            //to prawda, wyszukiwanie i tak trzeba by zrobić na zasadzie splita i szukania po odopwiednich komórkach w datagridzie
            //możliwe ze nie ja jeszcze pokombinuej
            checkedListBox1.CheckOnClick = true;
            label4.Text = "";
            dataGridView1.Rows[0].ContextMenuStrip = contextMenuStrip1;
            var l = comboBox1.Items.Cast<Course>().ToList();
        }

        private void ReadCoursesDataFromFile(string coursesFileLocation)
        {
            groupsList = new List<Group>();
            string s = "";
            try
            {
                s = ReadFile(coursesFileLocation);
            }
            catch (Exception)
            {
                MessageBox.Show("Nieznaloziono pliku z planem");
            }
            if (s.Length != 0)
            {
                try
                {
                    string[] coursesArray = Regex.Split(s, "\n\r\n");
                    foreach (string course in coursesArray)
                    {
                        string[] groupsArray = Regex.Split(course, "\n");
                        string[] nameArray = Regex.Split(groupsArray[0], ":");
                        Course course1 = new Course(nameArray[0], nameArray[1]);
                        dataGridView2.Rows.Add(nameArray[0], nameArray[1].Substring(0, 1));
                        comboBox1.Items.Add(course1);

                        for (int i = 1; i < groupsArray.Length; i++)
                        {
                            string[] groupDetails = groupsArray[i].Split('|');
                            string[] places = Regex.Split(groupDetails[4], Regex.Escape("\\"));
                            Group group = new Group(groupDetails[0], groupDetails[1], groupDetails[2], groupDetails[3],
                                Convert.ToInt32(places[0]), Convert.ToInt32(places[1]), groupDetails[5],
                                nameArray[0] + " - " + nameArray[1]);
                            course1.addGroup(@group);
                            groupsList.Add(@group);
                        }
                    }
                    comboBox1.SelectedItem = comboBox1.Items[0];
                }
                catch
                {
                    MessageBox.Show("Niestety Twój plik z planem jest uszkodzony");
                }
            }
        }

        public string ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            return s;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Course course = (Course) comboBox1.SelectedItem;
            List<string> teachers = checkedListBox1.CheckedItems.Cast<string>().ToList();
            if (e.NewValue == CheckState.Checked)
                teachers.Add(checkedListBox1.Items[e.Index].ToString());
            else
                teachers.Remove(checkedListBox1.Items[e.Index].ToString());
            List<Group> groups = course.Groups;
            dataGridView1.Rows.Clear();
            foreach (
                Group @group in
                    groups.Where(@group => teachers.Contains(@group.Teacher))
                        .Where(@group => !checkBox1.Checked || @group.Free != 0))
            {
                var i = dataGridView1.Rows.Add(@group.Day, @group.Time, @group.Week, @group.Teacher,
                    @group.Free + "/" + @group.Places, @group.Code);
                dataGridView1.Rows[i].ContextMenuStrip = contextMenuStrip2;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Course course = (Course) comboBox1.SelectedItem;
            checkBox2.Enabled = true;
            checkedListBox1.Enabled = true;
            textBox1.Text = "";
            checkedListBox1.Items.Clear();
            List<string> teachers = new List<string>();
            foreach (Group @group in course.Groups.Where(@group => !checkedListBox1.Items.Contains(@group.Teacher)))
            {
                checkedListBox1.Items.Add(@group.Teacher, true);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                Course course = (Course) comboBox1.SelectedItem;
                List<string> teachers = checkedListBox1.CheckedItems.Cast<string>().ToList();
                List<Group> groups = course.Groups;
                dataGridView1.Rows.Clear();
                string text = textBox1.Text;
                if (text.Length == 0)
                    foreach (
                        Group @group in
                            groups.Where(@group => teachers.Contains(@group.Teacher))
                                .Where(@group => !checkBox1.Checked || @group.Free != 0))
                    {
                        var i = dataGridView1.Rows.Add(@group.Day, @group.Time, @group.Week, @group.Teacher,
                            @group.Free + "/" + @group.Places, @group.Code);
                        dataGridView1.Rows[i].ContextMenuStrip = contextMenuStrip2;
                    }
                else
                    foreach (Group @group in from @group in groupsList
                        where
                            @group.Day.ToLower().Contains(text.ToLower()) ||
                            @group.Teacher.ToLower().Contains(text.ToLower()) ||
                            @group.Code.ToLower().Contains(text.ToLower())
                        where !checkBox1.Checked || @group.Free != 0
                        select @group)
                    {
                        var i = dataGridView1.Rows.Add(@group.Day, @group.Time, @group.Week, @group.Teacher,
                            @group.Free + "/" + @group.Places, @group.Code);
                        dataGridView1.Rows[i].ContextMenuStrip = contextMenuStrip2;
                    }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, checkBox2.Checked);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SearchInCourses(textBox1.Text);
        }

        private void SearchInCourses(string searchStr)
        {
            //if (searchStr.Length == 0)
            //{
            //    checkBox2.Enabled = true;
            //    checkedListBox1.Enabled = true;
            //    comboBox1.Enabled = true;
            //}
            //else
            //{
//                checkBox2.Enabled = false;
//                checkedListBox1.Enabled = false;
//                comboBox1.Enabled = false; //po co to? ale lagujesz xD myśle że to zbędne :P
//                //blokowało wybieranie z listy jak się korzystało z wyszukiwania, żeby ktoś nie mieszał za bardzo xD
            //najwyżej się potem poprawi jak będzie nei halo
            //}
            dataGridView1.Rows.Clear();
            if (comboBox1.Items.Count > 0)
            {
                if (searchStr.Length == 0)
                {
                    comboBox1.SelectedIndex = 0;
                    Course course = (Course) comboBox1.SelectedItem;
                    List<string> teachers = checkedListBox1.CheckedItems.Cast<string>().ToList();
                    List<Group> groups = course.Groups;
                    foreach (
                        Group @group in
                            groups.Where(@group => teachers.Contains(@group.Teacher))
                                .Where(@group => !checkBox1.Checked || @group.Free != 0))
                    {
                        var i = dataGridView1.Rows.Add(@group.Day, @group.Time, @group.Week, @group.Teacher,
                            @group.Free + "/" + @group.Places, @group.Code);
                        dataGridView1.Rows[i].ContextMenuStrip = contextMenuStrip2;
                    }
                }
                else
                {
                    if (!(searchStr.Contains("|") || searchStr.Contains(",")))
                        foreach (Group @group in from @group in groupsList
                            where
                                @group.Day.ToLower().Contains(searchStr.ToLower()) ||
                                @group.Teacher.ToLower().Contains(searchStr.ToLower()) ||
                                @group.Code.ToLower().Contains(searchStr.ToLower()) ||
                                @group.Name.ToLower().Contains(searchStr.ToLower())
                            where !checkBox1.Checked || @group.Free != 0
                            select @group)
                        {
                            var i = dataGridView1.Rows.Add(@group.Day, @group.Time, @group.Week, @group.Teacher,
                                @group.Free + "/" + @group.Places, @group.Code);
                            dataGridView1.Rows[i].ContextMenuStrip = contextMenuStrip2;
                        }
                    else
                     {
                        var courses = (from g in groupsList
                            group g by g.Name
                            into gr
                            select new
                            {
                                name = gr.Key,
                                groups = gr.ToList()
                            }).ToList();
                        if (searchStr.Contains("|") )
                        {
                            var tab = searchStr.Split('|');
                            if (tab.Count(s => !string.IsNullOrEmpty(s)) == 2)
                            {
                                List<Group> groups = null;
                                var firstOrDefault =
                                    courses.FirstOrDefault(c => c.name.ToLower().Contains(tab[0].ToLower()));
                                if (firstOrDefault != null)
                                {
                                    groups =
                                        firstOrDefault.groups.Where(g => g.Teacher.ToLower().Contains(tab[1])).ToList();
                                }
                                if (groups != null)
                                    foreach (var @group in groups)
                                    {
                                        var i = dataGridView1.Rows.Add(@group.Day, @group.Time, @group.Week,
                                            @group.Teacher,
                                            @group.Free + "/" + @group.Places, @group.Code);
                                        dataGridView1.Rows[i].ContextMenuStrip = contextMenuStrip2;
                                    }
                            }
                       
                        }
                        else if (searchStr.Contains(","))
                        {
                        }


                        var t =
                            @"Informatyczne sys. sterowania:Laboratorium-Wtorek|7:30|T|Dr inż.Magdalena Turowska|3\15|Z01-26a";
                        string pattern = "[a-zA-Z.]TEKST[a-zA-Z.]+,[a-zA-Z.]+|[a-zA-Z.]TEKST[a-zA-Z.]";
                            //"+tero+[a-zA-Z.]+,[a-zA-Z]+torium+[a-zA-Z.]+|[a-zA-Z.]+rowska+[a-zA-Z]";
                        //      MessageBox.Show("" + new Regex(pattern).IsMatch("dsadTEKSTdas,cdsad|xxTEKSTx"));//do tego wzoru co podałeś to takie starcza
                    }
                }
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var code = (string) dataGridView1.Rows[e.RowIndex].Cells[5].Value;
                var group = groupsList.First(g => g.Code.Equals(code));
                label4.Text = group.Name;
            }
            else
            {
                label4.Text = "";
            }
        }

        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            label4.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                AddAsMainGroup();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                AddAsAlternativeGroup();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                dataGridView2.SelectedCells.Cast<DataGridViewCell>().ToList().ForEach(c =>
                {
                    dataGridView2.Rows[c.RowIndex].Cells["group1"].Value = null;
                    dataGridView2.Rows[c.RowIndex].Cells["place1"].Value = null;
                });
                UpdateRegistrationInput();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                dataGridView2.SelectedCells.Cast<DataGridViewCell>().ToList().ForEach(c =>
                {
                    dataGridView2.Rows[c.RowIndex].Cells["group2"].Value = null;
                    dataGridView2.Rows[c.RowIndex].Cells["place2"].Value = null;
                });
                UpdateRegistrationInput();
            }
        }

        private void UpdateRegistrationInput()
        {
            StringBuilder s = new StringBuilder();
            dataGridView2.Rows.Cast<DataGridViewRow>().ToList().ForEach(r =>
            {
                if (r.Cells["group1"].Value != null)
                {
                    s.Append(ShorterName(r.Cells["subject"].Value.ToString(), r.Cells["type"].Value.ToString()));
                    s.Append(':');
                    s.Append(r.Cells["group1"].Value.ToString());
                }
            });
            dataGridView2.Rows.Cast<DataGridViewRow>().ToList().ForEach(r =>
            {
                if (r.Cells["group2"].Value != null)
                {
                    s.Append(ShorterName(r.Cells["subject"].Value.ToString(), r.Cells["type"].Value.ToString()));
                    s.Append("_R:");
                    s.Append(r.Cells["group2"].Value.ToString());
                }
            });
            if (s.Length > 0)
                s.Remove(s.Length - 1, 1);
            richTextBox1.Text = s.ToString();
        }

        private static string ShorterName(string name, string type)
        {
            var array = name.ToUpper().Split(' ');
            StringBuilder s = new StringBuilder();
            array.ToList().ForEach(w => s.Append(w.Substring(0, 1)));
            s.Append('_');
            s.Append(type);
            return s.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string s = "1. Wszystkie przedmioty dostępne są na liście." + Environment.NewLine
                       +
                       "2. W polu wyszukiwania można wpisywać część nazwy przedmiotów, dni tygodnia, nazwisk prowadzących i kodów kursów." +
                       Environment.NewLine
                       +
                       "3. Można zaznaczyć czy chce się przeglądać wszystkie grupy, czy tylko te, w których są jeszcze miejsca." +
                       Environment.NewLine
                       +
                       "4. Po prawej stronie dostępni są wszysy prowadzący wybranego kursu z listy, więc można określić których prowadzących kursy mają być wyświetlone." +
                       Environment.NewLine
                       +
                       "5. Jeśli w górnej tabelce najedziemy myszką na jakąś grupę to pod spodem pokaże nam jakiego kursu dotyczy wybrana grupa." +
                       Environment.NewLine
                       +
                       "6. Za pomocą przycisków dodaj grupę główną i dodaj grupę alternatywną możemy dodać do listy na dole grupę obecnie zaznaczoną w górnej tabeli." +
                       Environment.NewLine
                       +
                       "7. Aby usunąć grupy główne lub alternatywne zaznaczamy je w dolnej tabeli (można kilka wierszy od razu) i klikamy odpowiedni przycisk." +
                       Environment.NewLine
                       +
                       "8. W prawym dolnym rogu generuje się nam output, który możemy wkleić do bota z zapisami (generalnie jest aktualizowany automatycznie, w wypadku sortowania tabel trzeba ręcznie zaktualizować poprzez przyciśnięcie odpowiedniego przycisku).";
            MessageBox.Show(s);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            UpdateRegistrationInput();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length != 0)
            {
                Clipboard.SetText(richTextBox1.Text);
                MessageBox.Show("Pomyślnie skopiowano do schowka.");
            }
            else
            {
                MessageBox.Show("Nie ma czego skopiować");
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AddAsMainGroup();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                _groupsCurrentRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            }
        }

        private void AddAsMainGroup()
        {
            var code = dataGridView1.Rows[_groupsCurrentRowIndex].Cells["code"].Value.ToString();
            var group = groupsList.First(g => g.Code.Equals(code));
            var nameArray = Regex.Split(group.Name, Regex.Escape(" - "));
            var name = nameArray[0];
            var type = nameArray[1].Substring(0, 1);
            var row =
                dataGridView2.Rows.Cast<DataGridViewRow>()
                    .First(r => r.Cells["subject"].Value.Equals(name) && r.Cells["type"].Value.Equals(type));
            if (row.Cells["group1"].Value == null && row.Cells["place1"].Value == null)
            {
                if (row.Cells["group2"].Value == null)
                {
                    row.Cells["group1"].Value = code;
                    row.Cells["place1"].Value = group.Free + "/" + group.Places;
                }
                else
                {
                    if (row.Cells["group2"].Value.ToString().Equals(code))
                    {
                        if (
                            MessageBox.Show("Wybrałeś tą grupę jako alternatywną. Czy chcesz ją ustawić jako główną?",
                                "Ostrzeżenie", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            row.Cells["group1"].Value = code;
                            row.Cells["place1"].Value = group.Free + "/" + group.Places;
                            row.Cells["group2"].Value = null;
                            row.Cells["place2"].Value = null;
                        }
                    }
                    else
                    {
                        row.Cells["group1"].Value = code;
                        row.Cells["place1"].Value = group.Free + "/" + group.Places;
                    }
                }
            }
            else
            {
                if (row.Cells["group1"].Value.ToString().Equals(code))
                {
                    MessageBox.Show("Masz już ustawioną tą grupę jako główną");
                }
                else
                {
                    if (row.Cells["group2"].Value != null && row.Cells["group2"].Value.ToString().Equals(code))
                    {
                        if (
                            MessageBox.Show("Tą grupę masz ustawioną jako alternatywną. Czy zamienić je miejscami?",
                                "Ostrzeżenie", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            row.Cells["group2"].Value = row.Cells["group1"].Value;
                            row.Cells["place2"].Value = row.Cells["place1"].Value;
                            row.Cells["group1"].Value = code;
                            row.Cells["place1"].Value = group.Free + "/" + group.Places;
                        }
                    }
                    else
                    {
                        if (
                            MessageBox.Show(
                                "Wybrałeś już grupę główną dla kursu:\n" + name + " - " + nameArray[1] +
                                "\n\nCzy chcesz ją zmienić?", "Ostrzeżenie", MessageBoxButtons.YesNo) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            if (row.Cells["group2"].Value == null)
                            {
                                if (
                                    MessageBox.Show(
                                        "Czy chcesz obecną grupą główną:\n" + row.Cells["group1"].Value.ToString() +
                                        "\nustawić jako grupę awaryjną?", "Ostrzeżenie", MessageBoxButtons.YesNo) ==
                                    System.Windows.Forms.DialogResult.Yes)
                                {
                                    row.Cells["group2"].Value = row.Cells["group1"].Value;
                                    row.Cells["place2"].Value = row.Cells["place1"].Value;
                                }
                            }
                            else
                            {
                                if (
                                    MessageBox.Show(
                                        "Czy chcesz zastępić obecną grupę awaryjną:\n" +
                                        row.Cells["group2"].Value.ToString() + "\nobecną grupą główną:\n" + code,
                                        "Ostrzeżenie", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    row.Cells["group2"].Value = row.Cells["group1"].Value;
                                    row.Cells["place2"].Value = row.Cells["place1"].Value;
                                }
                            }
                            row.Cells["group1"].Value = code;
                            row.Cells["place1"].Value = group.Free + "/" + group.Places;
                        }
                    }
                }
            }
            UpdateRegistrationInput();
        }

        private void AddAsAlternativeGroup()
        {
            var code = dataGridView1.Rows[_groupsCurrentRowIndex].Cells["code"].Value.ToString();
            var group = groupsList.First(g => g.Code.Equals(code));
            var nameArray = Regex.Split(group.Name, Regex.Escape(" - "));
            var name = nameArray[0];
            var type = nameArray[1].Substring(0, 1);
            var row =
                dataGridView2.Rows.Cast<DataGridViewRow>()
                    .First(r => r.Cells["subject"].Value.Equals(name) && r.Cells["type"].Value.Equals(type));
            if (row.Cells["group2"].Value == null && row.Cells["place2"].Value == null)
            {
                if (row.Cells["group1"].Value == null)
                {
                    row.Cells["group2"].Value = code;
                    row.Cells["place2"].Value = group.Free + "/" + group.Places;
                }
                else
                {
                    if (row.Cells["group1"].Value.ToString().Equals(code))
                    {
                        if (
                            MessageBox.Show("Wybrałeś tą grupę jako głowną. Czy chcesz ją ustawić jako alternatywną?",
                                "Ostrzeżenie", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            row.Cells["group2"].Value = code;
                            row.Cells["place2"].Value = group.Free + "/" + group.Places;
                            row.Cells["group1"].Value = null;
                            row.Cells["place1"].Value = null;
                        }
                    }
                    else
                    {
                        row.Cells["group2"].Value = code;
                        row.Cells["place2"].Value = group.Free + "/" + group.Places;
                    }
                }
            }
            else
            {
                var rowVal = row.Cells["group2"].Value;
                if (rowVal != null && rowVal.ToString().Equals(code))
                {
                    MessageBox.Show("Masz już ustawioną tą grupę jako alternatywną");
                }
                else
                {
                    if (row.Cells["group1"].Value != null && row.Cells["group1"].Value.ToString().Equals(code))
                    {
                        if (
                            MessageBox.Show("Tą grupę masz ustawioną jako głowną. Czy zamienić je miejscami?",
                                "Ostrzeżenie", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            row.Cells["group1"].Value = row.Cells["group2"].Value;
                            row.Cells["place1"].Value = row.Cells["place2"].Value;
                            row.Cells["group2"].Value = code;
                            row.Cells["place2"].Value = group.Free + "/" + group.Places;
                        }
                    }
                    else
                    {
                        if (
                            MessageBox.Show(
                                "Wybrałeś już grupę alternatywną dla kursu:\n" + name + " - " + nameArray[1] +
                                "\n\nCzy chcesz ją zmienić?", "Ostrzeżenie", MessageBoxButtons.YesNo) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            row.Cells["group2"].Value = code;
                            row.Cells["place2"].Value = group.Free + "/" + group.Places;
                        }
                    }
                }
            }
            UpdateRegistrationInput();
        }

        private void selectAsMainGroup_Click(object sender, EventArgs e)
        {
            AddAsMainGroup();
        }

        private void selectAsAlternativeGroup_Click(object sender, EventArgs e)
        {
            AddAsAlternativeGroup();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string searchStr = comboBox1.Text;
            SearchInCourses(searchStr);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowSelected = e.RowIndex;
                int colIndex = e.ColumnIndex;
                if (e.RowIndex != -1)
                {
                    dataGridView1.Rows[rowSelected].Cells[colIndex].Selected = true;
                }
            }
        }
    }
}
 
