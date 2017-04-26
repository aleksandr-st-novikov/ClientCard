using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using DevExpress.XtraEditors;

namespace ClientCard
{
    public partial class main : DevExpress.XtraEditors.XtraForm
    {
        private bool error = false;
        private bool convCheck = true, accountCheck = false, discCheck = false;

        public main()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void main_Load(object sender, EventArgs e)
        {
            DisableAll();
            labelControl14.Text = "Версия: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Reconnect()
        {
            try
            {
                //string Connect = "Database=ukmserver;Data Source=" + textEdit7.Text + ";User Id=root;Password=CtHDbCGK.C";
                string Connect = "Data Source=" + textEdit7.Text + ";User Id=root;Password=CtHDbCGK.C";
                //string Connect = "Data Source=" + textEdit7.Text + ";User Id=discount;Password=CtHDbCGK.C";
                MySqlConnection myConnection = new MySqlConnection(Connect);
                myConnection.Open();

                try
                {
                    this.radioGroup1.SelectedIndex = 1;

                    //запрашиваем текущие данные по картам
                    string commandIdLastCard = "SELECT MAX(ID) FROM UKMSERVER.TRM_IN_CARDS";
                    MySqlCommand myCommand = new MySqlCommand(commandIdLastCard, myConnection);

                    if (String.IsNullOrEmpty(myCommand.ExecuteScalar().ToString()))
                    {
                        textEdit3.Text = "1";
                    }
                    else
                    {
                        textEdit3.Text = (Convert.ToInt32(myCommand.ExecuteScalar()) + 1).ToString();
                    }

                    //запрашиваем текущие данные по счетам
                    string commandIdLastAccount = "SELECT MAX(ID) FROM UKMSERVER.LOCAL_AUTH_ACCOUNT";
                    myCommand = new MySqlCommand(commandIdLastAccount, myConnection);

                    if (String.IsNullOrEmpty(myCommand.ExecuteScalar().ToString()))
                    {
                        textEdit6.Text = "0";
                    }
                    else
                    {
                        textEdit6.Text = (Convert.ToInt32(myCommand.ExecuteScalar()) + 1).ToString();
                    }

                    //заполнение группами клиентов
                    string sql1 = "SELECT CC.ID, CC.NAME FROM UKMSERVER.TRM_IN_CLASSIFCLIENTS CC WHERE CC.DELETED = 0";
                    MySqlDataAdapter adr1 = new MySqlDataAdapter(sql1, myConnection);
                    adr1.SelectCommand.CommandType = CommandType.Text;
                    DataTable dt1 = new DataTable();
                    adr1.Fill(dt1);
                    gridLookUpEdit2.Properties.DataSource = dt1;
                    gridLookUpEdit2.Properties.ValueMember = "ID";
                    gridLookUpEdit2.Properties.DisplayMember = "NAME";
                    gridColumn2.FieldName = "NAME";
                    //gridColumn2.Width = 220;

                    //заполнение типами счетов
                    string sql = "SELECT AT.ID, AT.NAME FROM UKMSERVER.TRM_IN_ACCOUNT_TYPE AT WHERE AT.ID NOT IN (1,2) AND AT.DELETED = 0";
                    MySqlDataAdapter adr = new MySqlDataAdapter(sql, myConnection);
                    adr.SelectCommand.CommandType = CommandType.Text;
                    DataTable dt = new DataTable();
                    adr.Fill(dt);
                    gridLookUpEdit1.Properties.DataSource = dt;
                    gridLookUpEdit1.Properties.ValueMember = "ID";
                    gridLookUpEdit1.Properties.DisplayMember = "NAME";
                    gridColumn1.FieldName = "NAME";
                    //gridColumn1.Width = 220;

                    //заполнение видаим скидок
                    //поле EFTS 16 - На чек для зарегистрированного клиента;
                    //8 - На чек по накопительным сумматорам
                    string sql2 = "SELECT DT.ID, DT.NAME FROM UKMSERVER.TRM_IN_DISCOUNT_TYPES DT WHERE DT.DELETED = 0 AND DT.EFTS = 16";
                    MySqlDataAdapter adr2 = new MySqlDataAdapter(sql2, myConnection);
                    adr2.SelectCommand.CommandType = CommandType.Text;
                    DataTable dt2 = new DataTable();
                    adr2.Fill(dt2);
                    gridLookUpEdit3.Properties.DataSource = dt2;
                    gridLookUpEdit3.Properties.ValueMember = "ID";
                    gridLookUpEdit3.Properties.DisplayMember = "NAME";
                    gridColumn3.FieldName = "NAME";
                    //gridColumn3.Width = 220;

                    error = false;

                    CheckedElements();
                }
                catch (MySqlException msex)
                {
                    error = true;
                    XtraMessageBox.Show("Ошибка: " + msex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch (MySqlException msex)
            {
                error = true;
                XtraMessageBox.Show("Ошибка при соединении с БД: " + msex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CheckedElements()
        {
            checkEdit3.Checked = convCheck ? true : false;

            if (accountCheck)
            {
                this.checkEdit4.Checked = true;
                this.gridLookUpEdit1.Enabled = true;
                this.labelControl10.Enabled = true;
            }
            else
            {
                this.checkEdit4.Checked = false;
                this.gridLookUpEdit1.Enabled = false;
                this.labelControl10.Enabled = false;
            }

            if (discCheck)
            {
                this.checkEdit5.Checked = true;
                this.gridLookUpEdit3.Enabled = true;
                this.labelControl13.Enabled = true;
            }
            else
            {
                this.checkEdit5.Checked = false;
                this.gridLookUpEdit3.Enabled = false;
                this.labelControl13.Enabled = false;
            }
        }

        private void ReadAll()
        {
            this.comboBoxEdit1.Properties.ReadOnly = true;
            this.textEdit8.Properties.ReadOnly = true;
            this.textEdit9.Properties.ReadOnly = true;
            this.textEdit10.Properties.ReadOnly = true;
            this.spinEdit1.Properties.ReadOnly = true;
            this.gridLookUpEdit2.Properties.ReadOnly = true;
            this.radioGroup1.Properties.ReadOnly = true;
            this.gridLookUpEdit3.Properties.ReadOnly = true;
            this.gridLookUpEdit1.Properties.ReadOnly = true;
        }

        private void UnReadAll()
        {
            this.comboBoxEdit1.Properties.ReadOnly = false;
            this.textEdit8.Properties.ReadOnly = false;
            this.textEdit9.Properties.ReadOnly = false;
            this.textEdit10.Properties.ReadOnly = false;
            this.spinEdit1.Properties.ReadOnly = false;
            this.gridLookUpEdit2.Properties.ReadOnly = false;
            this.radioGroup1.Properties.ReadOnly = false;
            this.gridLookUpEdit3.Properties.ReadOnly = false;
            this.gridLookUpEdit1.Properties.ReadOnly = false;
        }

        private void DisableAll()
        {
            this.groupControl2.Enabled = false;
            this.groupControl1.Enabled = false;
            this.groupControl3.Enabled = false;
            this.simpleButton2.Enabled = false;
            this.checkEdit3.Enabled = false;
            this.checkEdit4.Enabled = false;
            this.labelControl12.Enabled = false;
            this.textEdit10.Enabled = false;
            this.groupControl4.Enabled = false;
            this.groupControl5.Enabled = false;
        }

        private void EnableAll()
        {
            this.groupControl2.Enabled = true;
            this.groupControl1.Enabled = true;
            this.groupControl3.Enabled = true;
            this.simpleButton2.Enabled = true;
            this.checkEdit3.Enabled = true;
            this.checkEdit4.Enabled = true;
            this.labelControl12.Enabled = true;
            this.textEdit10.Enabled = true;
            this.groupControl4.Enabled = true;
            this.groupControl5.Enabled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            Reconnect();
            if (error) return;
            EnableAll();
            DisableParam();
            this.checkEdit4.Checked = false;
        }

        private void DisableParam()
        {
            this.labelControl6.Enabled = false;
            this.textEdit7.Enabled = false;
            this.simpleButton3.Enabled = false;
        }

        private void textEdit8_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            textEdit4.Text = textEdit8.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0');
            textEdit2.Text = comboBoxEdit1.Text + textEdit4.Text;
            //if (String.IsNullOrEmpty(textEdit9.Text))
            //{
            //    textEdit9.Text = textEdit8.Text;
            //    textEdit5.Text = textEdit8.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - textEdit1.Text.Length, '0');
            //}
        }

        private void textEdit9_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            textEdit5.Text = textEdit9.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0');
            //if (String.IsNullOrEmpty(textEdit8.Text))
            //{
            //    textEdit8.Text = textEdit9.Text;
            //    textEdit4.Text = textEdit9.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - textEdit1.Text.Length, '0');
            //}
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Reconnect();
            Application.DoEvents();

            //проверяем диапазон номеров карт
            if (String.IsNullOrEmpty(textEdit8.Text) || String.IsNullOrEmpty(textEdit9.Text))
            {
                XtraMessageBox.Show("Неверно указан диапазон карт!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                textEdit8.Focus();
                return;
            }
            if (Int32.Parse(textEdit8.Text) > Int32.Parse(textEdit9.Text))
            {
                XtraMessageBox.Show("Неверно указан диапазон карт!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textEdit8.Focus();
                return;
            }

            if (checkEdit4.Checked)
            {
                //проверяем тип счета
                if (this.gridLookUpEdit1.EditValue == null)
                {
                    XtraMessageBox.Show("Не указан тип счета!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.gridLookUpEdit1.Focus();
                    return;
                }
            }

            if (checkEdit5.Checked)
            {
                //проверяем вид скидки
                if (this.gridLookUpEdit3.EditValue == null)
                {
                    XtraMessageBox.Show("Не указан вид скидки!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.gridLookUpEdit3.Focus();
                    return;
                }
            }

            if (checkEdit3.Checked)
            {
                if (String.IsNullOrEmpty(textEdit10.Text))
                {
                    XtraMessageBox.Show("Не указана БД конвертера!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textEdit10.Focus();
                    return;
                }
            }

            //создаем клиента (с уникальным id) -> создаем счет клиента -> создаем карту -> привязываем карту к клиенту 
            //-> привязываем клиента к скидке
            try
            {
                ReadAll();
                this.simpleButton2.Enabled = false;
                int p = this.progressBarControl1.Position = 0;
                decimal percent = 0.0m;
                this.progressBarControl1.Visible = true;

                string idClient = "";
                string Connect = "Database=ukmserver;Data Source=" + textEdit7.Text + ";User Id=root;Password=CtHDbCGK.C";
                MySqlConnection myConnection = new MySqlConnection(Connect);
                myConnection.Open();

                try
                {
                    //очищаем все необходимые таблицы (с версией = 0)
                    string[] tables = { "CLIENTS", "CARDS", "CARD_CLIENT", "SIGNAL" };
                    string sqlTextTruncate;
                    MySqlCommand myCommandTruncate;
                    foreach (string t in tables)
                    {
                        //sqlTextTruncate = "TRUNCATE TABLE " + textEdit10.Text + "." + t;
                        sqlTextTruncate = "DELETE FROM " + textEdit10.Text + "." + t + " WHERE version = 0";
                        myCommandTruncate = new MySqlCommand(sqlTextTruncate, myConnection);
                        myCommandTruncate.ExecuteNonQuery();
                    }

                    //удаление записи в таблице signal с версией 0
                    //sqlTextTruncate = "DELETE FROM " + textEdit10.Text + ".SIGNAL WHERE VERSION = 0";
                    //myCommandTruncate = new MySqlCommand(sqlTextTruncate, myConnection);
                    //myCommandTruncate.ExecuteNonQuery();


                    int idCard = Int32.Parse(textEdit3.Text);
                    int idAccount = Int32.Parse(textEdit6.Text);
                    for (int i = Int32.Parse(textEdit8.Text); i <= Int32.Parse(textEdit9.Text); i++)
                    {
                        //MySqlTransaction myTrans = myConnection.BeginTransaction();
                        try
                        {
                            //создаем уникальный id
                            string sqlText = "SELECT CONCAT('{',UUID(),'}')";
                            MySqlCommand myCommand = new MySqlCommand(sqlText, myConnection);

                            if (String.IsNullOrEmpty(myCommand.ExecuteScalar().ToString()))
                            {
                                XtraMessageBox.Show("Ошибка при создании ID клиента!", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                idClient = myCommand.ExecuteScalar().ToString();
                            }

                            //запрос для создания клиента
                            if (checkEdit3.Checked)
                            {
                                sqlText = "INSERT INTO " + textEdit10.Text + ".CLIENTS (ID,CLASSIFCLIENT,SUR_NAME,PATRONYMIC,INN,PASSPORT,ALLOW_PAYCASH,ACTIVE,VERSION,DELETED) " +
                                    "VALUES ('" + idClient + "',";
                                if (this.gridLookUpEdit2.EditValue == null)
                                {
                                    sqlText = sqlText + "0,'";
                                }
                                else
                                {
                                    sqlText = sqlText + this.gridLookUpEdit2.EditValue.ToString() + ",'";
                                }
                                sqlText = sqlText + i.ToString().PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0') + "',NULL,NULL,NULL,";
                                switch (this.radioGroup1.SelectedIndex)
                                {
                                    case 0:
                                        sqlText = sqlText + "0,";
                                        break;
                                    case 1:
                                        sqlText = sqlText + "1,";
                                        break;
                                }
                                sqlText = checkEdit2.Checked ? sqlText + "1,0,0)" : sqlText + "0,0,0)";
                            }
                            else
                            {
                                sqlText = "INSERT INTO UKMSERVER.TRM_IN_CLIENTS (GLOBAL_ID,ID,CLASSIFCLIENT,SUR_NAME,PATRONYMIC,INN,PASSPORT,PRICETYPE,ALLOW_PAYCASH,ACTIVE,VERSION,DELETED) VALUES " +
                                    "(0,'" + idClient + "',";
                                if (this.gridLookUpEdit2.EditValue == null)
                                {
                                    sqlText = sqlText + "0,'";
                                }
                                else
                                {
                                    sqlText = sqlText + this.gridLookUpEdit2.EditValue.ToString() + ",'";
                                }
                                sqlText = sqlText + i.ToString().PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0') + "',NULL,NULL,NULL,NULL,";
                                switch (this.radioGroup1.SelectedIndex)
                                {
                                    case 0:
                                        sqlText = sqlText + "0,";
                                        break;
                                    case 1:
                                        sqlText = sqlText + "1,";
                                        break;
                                }
                                sqlText = checkEdit2.Checked ? sqlText + "1,0,0)" : sqlText + "0,0,0)";
                            }
                            //XtraMessageBox.Show(sqlText);
                            myCommand = new MySqlCommand(sqlText, myConnection);
                            myCommand.ExecuteNonQuery();

                            if (checkEdit4.Checked)
                            {
                                //запрос для создания счета
                                sqlText = "INSERT INTO UKMSERVER.LOCAL_AUTH_ACCOUNT (ACCOUNT_TYPE_ID,NAME,PARAMS,CLOSED) VALUES (" +
                                    this.gridLookUpEdit1.EditValue.ToString() + "," +
                                    "'ClAcc." + idClient + "." + this.gridLookUpEdit1.EditValue.ToString() + "'," +
                                    "'" + idClient + "',NULL)";
                                myCommand = new MySqlCommand(sqlText, myConnection);
                                myCommand.ExecuteNonQuery();
                            }

                            //запрос на создание карты
                            if (checkEdit3.Checked)
                            {
                                sqlText = "INSERT INTO " + textEdit10.Text + ".CARDS (ID,START_CARD_CODE,STOP_CARD_CODE,ACTIVE,VERSION) VALUES (" +
                                    idCard.ToString() + ",'" + comboBoxEdit1.Text + i.ToString().PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0') + "',NULL,";
                            }
                            else
                            {
                                sqlText = "INSERT INTO UKMSERVER.TRM_IN_CARDS (GLOBAL_ID,ID,START_CARD_CODE,STOP_CARD_CODE,ACTIVE,VERSION) VALUES (" +
                                    "0," + idCard.ToString() + ",'" + comboBoxEdit1.Text + i.ToString().PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0') + "',NULL,";
                            }
                            sqlText = checkEdit1.Checked ? sqlText + "1,0)" : sqlText + "0,0)";
                            //XtraMessageBox.Show(sqlText);
                            myCommand = new MySqlCommand(sqlText, myConnection);
                            myCommand.ExecuteNonQuery();

                            //запрос на создание связи клиентов с картами
                            if (checkEdit3.Checked)
                            {
                                sqlText = "INSERT INTO " + textEdit10.Text + ".CARD_CLIENT (CARD,CLIENT,VERSION) VALUES (" +
                                    idCard.ToString() + ",'" + idClient + "',0)";
                            }
                            else
                            {
                                sqlText = "INSERT INTO UKMSERVER.TRM_IN_CARD_CLIENT (GLOBAL_ID,CARD,CLIENT,VERSION) VALUES (0," +
                                    idCard.ToString() + ",'" + idClient + "',0)";
                            }
                            //XtraMessageBox.Show(sqlText);
                            myCommand = new MySqlCommand(sqlText, myConnection);
                            myCommand.ExecuteNonQuery();

                            //запрос на создание связи клиентов со скидками
                            //if (checkEdit5.Checked)
                            //{
                            //    if (checkEdit3.Checked)
                            //    {
                            //        sqlText = "INSERT INTO " + textEdit10.Text + ".DISC_STD_CLIENTS (DISCOUNT_TYPE,CLIENT,MODIFICATOR," +
                            //            "VERSION) VALUES (" + this.gridLookUpEdit3.EditValue.ToString() + ",'" + idClient + "','-1%',0)";

                            //        //XtraMessageBox.Show(sqlText);
                            //        myCommand = new MySqlCommand(sqlText, myConnection);
                            //        myCommand.ExecuteNonQuery();
                            //    }
                            //}

                            idCard++;

                            //прогрессбар
                            if (Int32.Parse(textEdit9.Text) - Int32.Parse(textEdit8.Text) == 0)
                            {
                                percent = 100.0m;
                            }
                            else
                            {
                                percent = p * 100 / (Int32.Parse(textEdit9.Text) - Int32.Parse(textEdit8.Text));
                            }
                            this.progressBarControl1.Position = (int)percent;
                            p++;
                            Application.DoEvents();

                            //myTrans.Commit();
                        }
                        catch (MySqlException msex)
                        {
                            //myTrans.Rollback();
                            XtraMessageBox.Show("Ошибка при сохранении записи: " + msex.Message, "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    if (checkEdit3.Checked)
                    {
                        //даем сигнал конвертеру для загрузки
                        string sqlTxt = "INSERT INTO " + textEdit10.Text + ".SIGNAL (SIGNAL,VERSION) VALUES ('incr',0)";
                        MySqlCommand myCmd = new MySqlCommand(sqlTxt, myConnection);
                        myCmd.ExecuteNonQuery();
                    }

                    Reconnect();
                    this.progressBarControl1.Visible = false;
                    this.simpleButton2.Enabled = true;
                    UnReadAll();
                }
                finally
                {
                    myConnection.Close();
                    this.progressBarControl1.Visible = false;
                    this.simpleButton2.Enabled = true;
                    UnReadAll();
                    Reconnect();
                }
            }
            catch (MySqlException msex)
            {
                XtraMessageBox.Show("Ошибка при соединении с БД: " + msex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void textEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            textEdit2.Text = comboBoxEdit1.Text + textEdit4.Text;
        }

        private void gridLookUpEdit2_Properties_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                this.gridLookUpEdit2.EditValue = null;
                this.gridLookUpEdit2.Refresh();
            }
        }

        private void checkEdit4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit4.Checked)
            {
                this.labelControl10.Enabled = true;
                this.gridLookUpEdit1.Enabled = true;
            }
            else
            {
                this.labelControl10.Enabled = false;
                this.gridLookUpEdit1.Enabled = false;
            }
            accountCheck = checkEdit4.Checked ? true : false;
        }

        private void checkEdit5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit5.Checked)
            {
                this.labelControl13.Enabled = true;
                this.gridLookUpEdit3.Enabled = true;
            }
            else
            {
                this.labelControl13.Enabled = false;
                this.gridLookUpEdit3.Enabled = false;
            }

            discCheck = checkEdit5.Checked ? true : false;
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxEdit1.SelectedIndex)
            {
                case 0:
                    spinEdit1.Value = 15;
                    checkEdit6.Checked = false;
                    break;
                case 1:
                    spinEdit1.Value = 7;
                    checkEdit6.Checked = true;
                    break;
            }

        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            textEdit4.Text = textEdit8.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0');
            textEdit2.Text = comboBoxEdit1.Text + textEdit4.Text;
            //if (String.IsNullOrEmpty(textEdit9.Text))
            //{
            //    textEdit9.Text = textEdit8.Text;
            //    textEdit5.Text = textEdit8.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - textEdit1.Text.Length, '0');
            //}

            textEdit5.Text = textEdit9.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - comboBoxEdit1.Text.Length, '0');
            //if (String.IsNullOrEmpty(textEdit8.Text))
            //{
            //    textEdit8.Text = textEdit9.Text;
            //    textEdit4.Text = textEdit9.Text.PadLeft(Convert.ToInt32(spinEdit1.EditValue) - textEdit1.Text.Length, '0');
            //}
        }

        private void checkEdit3_CheckedChanged(object sender, EventArgs e)
        {
            convCheck = checkEdit3.Checked ? true : false;
        }

    }
}
