using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace CSharp_Task
{
    public partial class BooksCSV : Form
    {
        private Panel layoutPanel = new Panel();
        private Button btnBrowse = new Button();
        private Button btnLoadData = new Button();
        private Button btnUpdateFile = new Button();
        private Button btnClearDataGridView = new Button();
        private Button btnDeleteUnavailableBooks = new Button();
        private DataGridView booksDataGridView = new DataGridView();
        private Label lblDelimeter = new Label();
        private OpenFileDialog openFileDialog1 = new OpenFileDialog();
        private TextBox txtDelimeter = new TextBox();
        private TextBox txtPath = new TextBox();

        char delimiter;
        Boolean DataLoaded = false;


        public BooksCSV()
        {
            InitializeComponent();
        }

        private void BooksCSV_Load(object sender, EventArgs e)
        {
            SetupLayout();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:";
            openFileDialog1.Filter = "CSV files (*.csv)|*.CSV";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.ShowDialog();

            txtPath.Text = openFileDialog1.FileName;
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            string fileRow;
            string[] fileDataField;
            int count = 0;
            if (DataLoaded == false)
            {
                try
                {
                    delimiter = Convert.ToChar(txtDelimeter.Text);
                }
                catch (Exception exceptionObject)
                {
                    MessageBox.Show(exceptionObject.ToString());
                    this.Close();
                }

                try
                {
                    if (System.IO.File.Exists(txtPath.Text))
                    {
                        System.IO.StreamReader fileReader = new StreamReader(txtPath.Text);

                        if (fileReader.Peek() != -1)
                        {
                            fileRow = fileReader.ReadLine();
                            fileDataField = fileRow.Split(delimiter);
                            count = fileDataField.Count();
                            count = count - 1;

                            //Reading Header information
                            for (int i = 0; i <= count; i++)
                            {
                                DataGridViewTextBoxColumn columnDataGridTextBox = new DataGridViewTextBoxColumn();
                                columnDataGridTextBox.Name = fileDataField[i];
                                columnDataGridTextBox.HeaderText = fileDataField[i];
                                columnDataGridTextBox.Width = 120;
                                booksDataGridView.Columns.Add(columnDataGridTextBox);
                            }
                        }
                        else
                        {
                            MessageBox.Show("File is Empty!!");
                        }
                        //Reading Data
                        while (fileReader.Peek() != -1)
                        {
                            fileRow = fileReader.ReadLine();
                            fileDataField = fileRow.Split(delimiter);
                            booksDataGridView.Rows.Add(fileDataField);
                            booksDataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(booksDataGridView_CellFormatting);
                        }

                        fileReader.Close();
                    }
                    else
                    {
                        MessageBox.Show("No File is Selected!!");
                    }

                    DataLoaded = true;

                }
                catch (Exception exceptionObject)
                {
                    MessageBox.Show(exceptionObject.ToString());
                }
            }
            else
            {
                MessageBox.Show("Clear DataGridView First!!");
            }
        }

        private void btnClearDataGridView_Click(object sender, EventArgs e)
        {
            this.booksDataGridView.Rows.Clear();
            this.booksDataGridView.Columns.Clear();
            this.booksDataGridView.Refresh();
            DataLoaded = false;
        }

        private void booksDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            decimal number;
            if (this.booksDataGridView.Columns[e.ColumnIndex].Name == "Price")
            {
               if(e.Value != null)
                {
                    string temp = e.Value.ToString();
                    Decimal.TryParse(temp, style, culture, out number);
                    int price = Convert.ToInt32(number);
                    if (price <= 19)
                    {
                        e.CellStyle.BackColor = Color.Red;
                    }
                    if (price > 19 && price <= 20)
                    {
                        e.CellStyle.BackColor = Color.Yellow;
                    }
                    if (price > 20 && price <= 200)
                    {
                        e.CellStyle.BackColor = Color.Orange;
                    }
                }               
            }
        }

        private void btnUpdateFile_Click(object sender, EventArgs e)
        {
            try
            {
                delimiter = Convert.ToChar(txtDelimeter.Text);
            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
                this.Close();
            }

            try
            {
                System.IO.StreamWriter fileWriter = new StreamWriter(txtPath.Text, false);
                string columnHeaderText = "";

                //Writing DataGridView Header in File
                int countColumn = booksDataGridView.ColumnCount - 1;
                if (countColumn >= 0)
                {
                    columnHeaderText = booksDataGridView.Columns[0].HeaderText;
                }

                for (int i = 1; i <= countColumn; i++)
                {
                    columnHeaderText = columnHeaderText + delimiter + booksDataGridView.Columns[i].HeaderText;
                }

                fileWriter.WriteLine(columnHeaderText);

                //Writing Data in File
                foreach (DataGridViewRow dataRowObject in booksDataGridView.Rows)
                {
                    if (!dataRowObject.IsNewRow)
                    {
                        string dataFromGrid = "";
                        dataFromGrid = dataRowObject.Cells[0].Value.ToString();
                        for (int i = 1; i <= countColumn; i++)
                        {
                            dataFromGrid = dataFromGrid + delimiter + dataRowObject.Cells[i].Value.ToString();
                        }

                        fileWriter.WriteLine(dataFromGrid);
                    }
                }

                MessageBox.Show("Data is successfully saved in File");

                fileWriter.Flush();
                fileWriter.Close();
            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
            }
        }

        private void btnDeleteUnavailableBooks_Click(object sender, EventArgs e)
        {
            Boolean entriesExits = true;
            for (int i=0; i < this.booksDataGridView.Rows.Count; i++)
            {
                if (this.booksDataGridView["In Stock", i].Value != null && this.booksDataGridView["In Stock", i].Value.ToString().Trim() == "no")
                {
                    DataGridViewRow deleteRow = this.booksDataGridView.Rows[i];
                    this.booksDataGridView.Rows.Remove(deleteRow);
                }
                entriesExits = false;
            }

            this.booksDataGridView.Refresh();

            if (!entriesExits)
            {
               MessageBox.Show("All unavailable stocks entries deleted!");
            }
            
        }

        private void SetupLayout()
        {
            this.Size = new Size(851, 601);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.StartPosition = FormStartPosition.CenterScreen;

            openFileDialog1.FileName = "SelectBooksCSV";

            txtPath.Location = new Point(84, 36);
            txtPath.Name = "txtPath";
            txtPath.ReadOnly = true;
            txtPath.Size = new Size(387, 20);
            txtPath.TabIndex = 0;

            btnBrowse.Location = new Point(490, 34);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(78, 23);
            btnBrowse.TabIndex = 0;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += new EventHandler(this.btnBrowse_Click);

            btnLoadData.Location = new Point(703, 36);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(75, 23);
            btnLoadData.TabIndex = 2;
            btnLoadData.Text = "Load Data in GridView";
            btnLoadData.UseVisualStyleBackColor = true;
            btnLoadData.Click += new EventHandler(this.btnLoadData_Click);

            booksDataGridView.AllowUserToOrderColumns = true;
            booksDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            booksDataGridView.Location = new Point(84, 92);
            booksDataGridView.Name = "booksDataGridView";
            booksDataGridView.Size = new Size(694, 319);
            booksDataGridView.TabIndex = 3;

            btnUpdateFile.Location = new Point(682, 437);
            btnUpdateFile.Name = "btnUpdateFile";
            btnUpdateFile.Size = new Size(95, 23);
            btnUpdateFile.TabIndex = 4;
            btnUpdateFile.Text = "Update CSV File";
            btnUpdateFile.UseVisualStyleBackColor = true;
            btnUpdateFile.Click += new EventHandler(this.btnUpdateFile_Click);

            btnDeleteUnavailableBooks.Location = new Point(558, 437);
            btnDeleteUnavailableBooks.Name = "btnDeleteUnavailableBooks";
            btnDeleteUnavailableBooks.Size = new Size(117, 23);
            btnDeleteUnavailableBooks.TabIndex = 3;
            btnDeleteUnavailableBooks.Text = "Delete";
            btnDeleteUnavailableBooks.UseVisualStyleBackColor = true;
            btnDeleteUnavailableBooks.Click += new EventHandler(this.btnDeleteUnavailableBooks_Click);

            btnClearDataGridView.Location = new Point(84, 437);
            btnClearDataGridView.Name = "btnClearDataGridView";
            btnClearDataGridView.Size = new Size(117, 23);
            btnClearDataGridView.TabIndex = 5;
            btnClearDataGridView.Text = "Clear";
            btnClearDataGridView.UseVisualStyleBackColor = true;
            btnClearDataGridView.Click += new System.EventHandler(this.btnClearDataGridView_Click);

            lblDelimeter.AutoSize = true;
            lblDelimeter.Location = new Point(587, 41);
            lblDelimeter.Name = "lblDelimeter";
            lblDelimeter.Size = new Size(47, 13);
            lblDelimeter.TabIndex = 6;
            lblDelimeter.Text = "Delimiter";

            txtDelimeter.Location = new Point(640, 36);
            txtDelimeter.MaxLength = 1;
            txtDelimeter.Name = "txtDelimeter";
            txtDelimeter.Text = ";";
            txtDelimeter.Size = new Size(15, 20);
            txtDelimeter.TabIndex = 1;

            layoutPanel.Controls.Add(txtDelimeter);
            layoutPanel.Controls.Add(lblDelimeter);
            layoutPanel.Controls.Add(btnDeleteUnavailableBooks);
            layoutPanel.Controls.Add(btnUpdateFile);
            layoutPanel.Controls.Add(booksDataGridView);
            layoutPanel.Controls.Add(btnLoadData);
            layoutPanel.Controls.Add(btnBrowse);
            layoutPanel.Controls.Add(txtPath);
            layoutPanel.Controls.Add(btnClearDataGridView);
            layoutPanel.Width = 851;
            layoutPanel.Height = 601;

            this.Controls.Add(this.layoutPanel);
        }
       
    }
}
