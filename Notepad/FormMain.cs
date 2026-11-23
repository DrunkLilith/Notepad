using System.Reflection;
using System.ComponentModel;

namespace Notepad
{
    public partial class FormMain : Form
    {
        string current_filename = "";
        bool wasChanged = false;
        string sign;

        public FormMain()
        {
            InitializeComponent();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        bool SaveChanges(object sender, EventArgs e)
        {
            if (!wasChanged)
            {
                return true;
            }

            TaskDialogButton saveButton = new TaskDialogButton("Сохранить");
            TaskDialogButton discardButton = new TaskDialogButton("Не сохранять");
            TaskDialogPage page = new TaskDialogPage
            {
                Caption = "Блокнот",
                Heading = "Сохранить изменения в текущем документе?",
                Buttons = { saveButton, discardButton, TaskDialogButton.Cancel },
                DefaultButton = saveButton
            };

            TaskDialogButton result = TaskDialog.ShowDialog(this, page, TaskDialogStartupLocation.CenterOwner);
            if (result == saveButton)
            {
                сохранитьToolStripMenuItem_Click(sender, e);
                return !wasChanged;
            }

            return result == discardButton;
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SaveChanges(sender, e))
            {
                return;
            }

            textBoxField.Text = "";
            current_filename = "";
            wasChanged = false;
            UpdateTitle();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SaveChanges(sender, e))
            {
                return;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);
                textBoxField.Text = sr.ReadToEnd();
                sr.Close();
                current_filename = openFileDialog.FileName;
                wasChanged = false;
                UpdateTitle();

                /*string s="";
                while (!sr.EndOfStream)
                { 
                    s+= sr.ReadLine()+"\r\n";
                }*/
            }
        }

        void SaveToFile(string filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine(textBoxField.Text);
                sw.WriteLine(sign);
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Не удалось сохранить файл:\n" + ex.Message, "Ошибка");
                return;
            }
            current_filename = filename;
            wasChanged = false;
            UpdateTitle();
        }

        void UpdateTitle()
        {
            Text = (current_filename == "" ? "Безымянный" : Path.GetFileName(current_filename)) + " - Блокнот";
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(current_filename))
            {
                сохранитьКакToolStripMenuItem_Click(sender, e);
                return;
            }
            SaveToFile(current_filename);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveToFile(saveFileDialog.FileName);
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string settingsFile = Path.Combine(Application.StartupPath, "settings.txt");
            sign = "";
            if (File.Exists(settingsFile))
            {
                StreamReader sr = new StreamReader(settingsFile);
                textBoxField.Font = new Font(textBoxField.Font.FontFamily, int.Parse(sr.ReadLine()));
                while (!sr.EndOfStream)
                {
                    sign += "\r\n" + sr.ReadLine();
                }
                sr.Close();
            }

            fontDialog.Font = textBoxField.Font;
            colorDialog.Color = textBoxField.ForeColor;
            ActiveControl = textBoxField;
            UpdateTitle();
        }

        private void textBoxField_TextChanged(object sender, EventArgs e)
        {
            wasChanged = true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SaveChanges(sender, e))
            {
                e.Cancel = true;
            }
        }

        private void шрифтToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textBoxField.Font;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxField.Font = fontDialog.Font;
            }
        }

        private void цветToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxField.ForeColor = colorDialog.Color;
            }
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxField.SelectAll();
            textBoxField.Focus();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxField.Copy();
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxField.Cut();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxField.Paste();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings frm = new FormSettings();
            frm.ShowDialog();
        }

        void UpdateEditMenu()
        {
            bool hasSelection = textBoxField.SelectionLength > 0;
            bool canPaste = Clipboard.ContainsText();
            bool hasText = textBoxField.TextLength > 0;
            копироватьToolStripMenuItem.Enabled = hasSelection;
            вырезатьToolStripMenuItem.Enabled = hasSelection;
            вставитьToolStripMenuItem.Enabled = canPaste;
            выделитьВсеToolStripMenuItem.Enabled = hasText;
            копироватьToolStripMenuItem1.Enabled = hasSelection;
            выToolStripMenuItem.Enabled = hasSelection;
            резатьToolStripMenuItem.Enabled = canPaste;
            выделитьВсеToolStripMenuItem1.Enabled = hasText;
        }

        private void правкаToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEditMenu();
        }

        private void contextMenuStripTextBoxField_Opening(object sender, CancelEventArgs e)
        {
            UpdateEditMenu();
        }

        void ToggleStyle(FontStyle style)
        {
            textBoxField.Font = new Font(textBoxField.Font, textBoxField.Font.Style ^ style);
            textBoxField.Focus();
        }

        private void buttonBold_Click(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Bold);
        }

        private void buttonItalic_Click(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Italic);
        }

        private void buttonUnderline_Click(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Underline);
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {

        }
    }
}
