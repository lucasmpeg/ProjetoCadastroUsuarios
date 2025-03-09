using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaCadastro
{
    public partial class Form1 : Form
    {
        string ConnectionString = "Data Source=DESKTOP-TVM2E6D;Initial Catalog=CadastroDB;Integrated Security=True;";
        private bool isEditing = false;

        public Form1()
        {
            InitializeComponent();
            CarregarDados();
        }

        private void CarregarDados()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Desabilitar a edição do DataGridView para evitar conflitos
                    dataGridView.ReadOnly = true;

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Usuarios", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar dados: " + ex.Message);
                }
                finally
                {
                    // Reabilitar a edição do DataGridView após carregar os dados
                    dataGridView.ReadOnly = false;
                }
            }
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Usuarios (Nome, Email, Telefone) VALUES (@Nome, @Email, @Telefone)", conn);
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Dados salvos com sucesso!");
                    LimparCampos();
                    CarregarDados();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar dados: " + ex.Message);
                }
            }
        }


        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                // Obtém o ID do usuário selecionado
                int idUsuario = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);

                // Confirmação antes de excluir
                DialogResult resultado = MessageBox.Show("Tem certeza que deseja excluir este usuário?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    string connectionString = "Data Source=DESKTOP-TVM2E6D;Initial Catalog=CadastroDB;Integrated Security=True;";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                            // Comando SQL para excluir o usuário
                            string query = "DELETE FROM Usuarios WHERE Id = @Id";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Id", idUsuario);

                            // Executa o comando
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Usuário excluído com sucesso!");
                                CarregarDados(); // Atualiza o DataGridView
                            }
                            else
                            {
                                MessageBox.Show("Nenhum usuário foi excluído.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao excluir usuário: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um usuário para excluir.");
            }
        }
        private void LimparCampos()
        {
            txtNome.Clear();
            txtEmail.Clear();
            txtTelefone.Clear();
        }


        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (isEditing)
                return;

            try
            {
                isEditing = true; // Impede chamadas reentrantes

                if (e.RowIndex >= 0)
                {
                    DialogResult result = MessageBox.Show("Deseja salvar as alterações?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int id = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[0].Value);
                        string nome = dataGridView.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
                        string email = dataGridView.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";
                        string telefone = dataGridView.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "";

                        using (SqlConnection conn = new SqlConnection(ConnectionString))
                        {
                            string query = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Telefone = @Telefone WHERE Id = @Id";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Nome", nome);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Telefone", telefone);
                            cmd.Parameters.AddWithValue("@Id", id);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        MessageBox.Show("Dados atualizados com sucesso!");
                        CarregarDados(); // Atualiza o DataGridView
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar dados: " + ex.Message);
            }
            finally
            {
                isEditing = false; // Permite futuras edições
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0)
            {
                // Obtém a célula selecionada
                DataGridViewCell cell = dataGridView.SelectedCells[0];

                // Permite edição da célula
                dataGridView.CurrentCell = cell;
                dataGridView.BeginEdit(true);
            }
        }

    }
}