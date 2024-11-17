using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asd2_lab_3
{
    public partial class Form1 : Form
    {
        private DatabaseManager dbManager;

        public Form1()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataGridView1.Rows.Clear();
            var records = dbManager.ReadIndexFile()
                                   .Select(record => dbManager.SearchRecord(record.Key))
                                   .Where(record => record != null);

            foreach ( var record in records)
            {
                dataGridView1.Rows.Add(record.Key, record.Data);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtKey.Text, out int key) && !string.IsNullOrEmpty(txtData.Text))
            {
                if (dbManager.AddRecord(key, txtData.Text))
                {
                    MessageBox.Show("Запис успішно додано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataGrid();
                }
                else
                {
                    MessageBox.Show("Ключ вже існує!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний ключ і дані!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtKey.Text, out int key))
            {
                if (dbManager.DeleteRecord(key))
                {
                    MessageBox.Show("Запис успішно видалено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataGrid();
                }
                else
                {
                    MessageBox.Show("Запис із таким ключем не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний ключ!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtKey.Text, out int key) && !string.IsNullOrEmpty(txtData.Text))
            {
                if (dbManager.EditRecord(key, txtData.Text))
                {
                    MessageBox.Show("Запис успішно оновлено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataGrid();
                }
                else
                {
                    MessageBox.Show("Запис із таким ключем не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний ключ і нові дані!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtKey.Text, out int key))
            {
                var record = dbManager.SearchRecord(key);
                if (record != null)
                {
                    MessageBox.Show($"Запис знайдено:\nКлюч: {record.Key}\nДані: {record.Data}",
                                    "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Запис із таким ключем не знайдено!", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний ключ!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtKey.Text, out _))
            {
                txtKey.BackColor = Color.LightCoral;
            }
            else
            {
                txtKey.BackColor = Color.White;
            }
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (txtData.Text.Length > 50)
            {
                txtData.BackColor = Color.LightCoral;
                MessageBox.Show("Довжина тексту не повинна перевищувати 50 символів.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtData.BackColor = Color.White;
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Ви впевнені, що хочете видалити всі записи?",
                                                "Підтвердження",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                dbManager.DeleteAllRecords();
                MessageBox.Show("Усі записи успішно видалені!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataGrid();
            }
        }
    }
}
