using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsAppProyecto
{
    public partial class Form1 : Form
    {
        // ==========================================
        // 4. PERSISTENCIA EN MEMORIA MEDIANTE ARREGLOS
        // ==========================================
        private const int MAX_HISTORIAL = 100;
        private string[] nombresHistorial = new string[MAX_HISTORIAL];
        private int[] puntajesHistorial = new int[MAX_HISTORIAL];
        private int totalRegistros = 0;

        // Contenedores de Vistas (Paneles para gestionar estados)
        private Panel panelMenu;
        private Panel panelIndividual;
        private Panel panelDuo;
        private Panel panelRanking;

        // Variables de Estado - Modo Individual
        private string nombreInd = "";
        private int vidasInd = 3;
        private int puntajeInd = 0;
        private int num1Ind, num2Ind, respuestaCorrectaInd;
        private string operadorInd = "+";
        private Random rand = new Random();

        // Variables de Estado - Modo Dos Jugadores
        private string j1Nombre = "";
        private string j2Nombre = "";
        private int vidasJ1 = 3, vidasJ2 = 3;
        private int puntajeJ1 = 0, puntajeJ2 = 0;
        private bool turnoJ1 = true;
        private int num1Duo, num2Duo, respuestaCorrectaDuo;
        private string operadorDuo = "+";

        // Controles de Interfaz Dinámicos
        private Label lblIndInfo, lblIndPregunta, lblIndFeedback;
        private TextBox txtIndRespuesta;

        private Label lblDuoInfo, lblDuoPregunta, lblDuoFeedback;
        private TextBox txtDuoRespuesta;

        private ListBox lstRanking;

        // ==========================================
        // PALETA DE COLORES: ROJO Y ROSA
        // ==========================================
        private Color colorRojoPrincipal = Color.FromArgb(225, 29, 72);   // Rojo carmesí intenso
        private Color colorRosaAcento = Color.FromArgb(244, 63, 94);      // Rosa vibrante
        private Color colorTextoOscuro = Color.FromArgb(136, 19, 55);     // Rojo vino oscuro para títulos
        private Color colorFondoSuave = Color.FromArgb(255, 241, 242);    // Fondo rosado muy pastel

        public Form1()
        {
            InitializeComponent();
            ConfigurarVentanaPrincipal();
        }

        private void ConfigurarVentanaPrincipal()
        {
            this.Text = "Sistema Matemático Interactivo - Tema Rojo y Rosa";
            this.Size = new Size(720, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CrearPanelMenu();
            CrearPanelIndividual();
            CrearPanelDuo();
            CrearPanelRanking();

            MostrarPanel(panelMenu);
        }

        private void MostrarPanel(Panel panelActivo)
        {
            panelMenu.Visible = (panelActivo == panelMenu);
            panelIndividual.Visible = (panelActivo == panelIndividual);
            panelDuo.Visible = (panelActivo == panelDuo);
            panelRanking.Visible = (panelActivo == panelRanking);
        }

        // ==========================================
        // 1. INTERFAZ Y GESTIÓN DE ESTADOS (MENÚ)
        // ==========================================
        private void CrearPanelMenu()
        {
            panelMenu = new Panel { Dock = DockStyle.Fill, BackColor = colorFondoSuave };

            Label lblTitulo = new Label
            {
                Text = "SISTEMA MATEMÁTICO: ROJO & ROSA",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = colorTextoOscuro,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(680, 50),
                Location = new Point(20, 30)
            };
            panelMenu.Controls.Add(lblTitulo);

            Button btnInd = CrearBotonTematico("1. Modo Individual (Vidas Limitadas)", 120, colorRojoPrincipal);
            btnInd.Click += (s, e) => IniciarModoIndividualPrompt();
            panelMenu.Controls.Add(btnInd);

            Button btnDuo = CrearBotonTematico("2. Modo Acompañado (Dos Jugadores - Turnos)", 190, colorRosaAcento);
            btnDuo.Click += (s, e) => IniciarModoDuoPrompt();
            panelMenu.Controls.Add(btnDuo);

            Button btnRank = CrearBotonTematico("3. Ver Tabla de Clasificación (Ranking)", 260, colorRojoPrincipal);
            btnRank.Click += (s, e) => { ActualizarRanking(); MostrarPanel(panelRanking); };
            panelMenu.Controls.Add(btnRank);

            Button btnSalir = CrearBotonTematico("4. Salir de la Aplicación", 330, Color.FromArgb(159, 18, 57));
            btnSalir.Click += (s, e) => Application.Exit();
            panelMenu.Controls.Add(btnSalir);

            this.Controls.Add(panelMenu);
        }

        // ==========================================
        // 3. MODO INDIVIDUAL
        // ==========================================
        private void CrearPanelIndividual()
        {
            panelIndividual = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            Label lblTitulo = new Label { Text = "MODO INDIVIDUAL", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = colorTextoOscuro, Location = new Point(30, 25), AutoSize = true };
            panelIndividual.Controls.Add(lblTitulo);

            lblIndInfo = new Label { Text = "", Font = new Font("Segoe UI", 11, FontStyle.Regular), Location = new Point(30, 75), Size = new Size(630, 30) };
            panelIndividual.Controls.Add(lblIndInfo);

            lblIndPregunta = new Label { Text = "", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = colorRojoPrincipal, Location = new Point(30, 130), Size = new Size(630, 40) };
            panelIndividual.Controls.Add(lblIndPregunta);

            txtIndRespuesta = new TextBox { Font = new Font("Segoe UI", 14), Location = new Point(30, 190), Size = new Size(200, 35) };
            panelIndividual.Controls.Add(txtIndRespuesta);

            Button btnResponder = CrearBotonPequeno("Responder", 240, 190, colorRojoPrincipal);
            btnResponder.Click += (s, e) => ProcesarRespuestaIndividual();
            panelIndividual.Controls.Add(btnResponder);

            lblIndFeedback = new Label { Text = "", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(30, 250), Size = new Size(630, 60) };
            panelIndividual.Controls.Add(lblIndFeedback);

            Button btnVolver = CrearBotonPequeno("Menú Principal", 30, 390, Color.Gray);
            btnVolver.Click += (s, e) => MostrarPanel(panelMenu);
            panelIndividual.Controls.Add(btnVolver);

            this.Controls.Add(panelIndividual);
        }

        private void IniciarModoIndividualPrompt()
        {
            nombreInd = PedirTextoDialogo("Modo Individual", "Ingrese su nombre:");
            if (string.IsNullOrWhiteSpace(nombreInd)) nombreInd = "JugadorAnónimo";

            vidasInd = 3;
            puntajeInd = 0;
            MostrarPanel(panelIndividual);
            SiguientePreguntaIndividual();
        }

        private void SiguientePreguntaIndividual()
        {
            if (vidasInd <= 0)
            {
                GuardarEnHistorial(nombreInd, puntajeInd);
                MessageBox.Show($"¡Juego Terminado!\n\nJugador: {nombreInd}\nPuntaje Final: {puntajeInd}", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MostrarPanel(panelMenu);
                return;
            }

            int nivel = (puntajeInd / 30) + 1;
            int maxRango = 10 * nivel;

            num1Ind = rand.Next(1, maxRango + 1);
            num2Ind = rand.Next(1, maxRango + 1);
            int tipoOp = rand.Next(0, nivel > 2 ? 3 : 2);

            if (tipoOp == 0) { operadorInd = "+"; respuestaCorrectaInd = num1Ind + num2Ind; }
            else if (tipoOp == 1)
            {
                operadorInd = "-";
                if (num1Ind < num2Ind) { int t = num1Ind; num1Ind = num2Ind; num2Ind = t; }
                respuestaCorrectaInd = num1Ind - num2Ind;
            }
            else
            {
                operadorInd = "×";
                num1Ind = rand.Next(1, 6 + nivel);
                num2Ind = rand.Next(1, 6 + nivel);
                respuestaCorrectaInd = num1Ind * num2Ind;
            }

            lblIndInfo.Text = $"Nivel: {nivel}  |  Vidas: {vidasInd} ❤️  |  Puntaje: {puntajeInd} 🏆";
            lblIndPregunta.Text = $"¿Cuánto es {num1Ind} {operadorInd} {num2Ind}?";
            txtIndRespuesta.Text = "";
            lblIndFeedback.Text = "";
            txtIndRespuesta.Focus();
        }

        private void ProcesarRespuestaIndividual()
        {
            if (int.TryParse(txtIndRespuesta.Text, out int respUsuario))
            {
                if (respUsuario == respuestaCorrectaInd)
                {
                    lblIndFeedback.ForeColor = Color.ForestGreen;
                    lblIndFeedback.Text = "¡Correcto! +10 puntos.";
                    puntajeInd += 10;
                }
                else
                {
                    lblIndFeedback.ForeColor = colorRojoPrincipal;
                    lblIndFeedback.Text = $"Incorrecto. La respuesta era {respuestaCorrectaInd}. Pierdes 1 vida.";
                    vidasInd--;
                }
            }
            else
            {
                lblIndFeedback.ForeColor = colorRojoPrincipal;
                lblIndFeedback.Text = "Entrada inválida (no numérica o vacía). Pierdes 1 vida.";
                vidasInd--;
            }

            Timer timer = new Timer { Interval = 1500 };
            timer.Tick += (s, e) => { timer.Stop(); timer.Dispose(); SiguientePreguntaIndividual(); };
            timer.Start();
        }

        // ==========================================
        // 3. MODO DOS JUGADORES (ACOMPAÑADO)
        // ==========================================
        private void CrearPanelDuo()
        {
            panelDuo = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            Label lblTitulo = new Label { Text = "MODO DOS JUGADORES", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = colorTextoOscuro, Location = new Point(30, 25), AutoSize = true };
            panelDuo.Controls.Add(lblTitulo);

            lblDuoInfo = new Label { Text = "", Font = new Font("Segoe UI", 11, FontStyle.Regular), Location = new Point(30, 75), Size = new Size(630, 45) };
            panelDuo.Controls.Add(lblDuoInfo);

            lblDuoPregunta = new Label { Text = "", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = colorRosaAcento, Location = new Point(30, 135), Size = new Size(630, 40) };
            panelDuo.Controls.Add(lblDuoPregunta);

            txtDuoRespuesta = new TextBox { Font = new Font("Segoe UI", 14), Location = new Point(30, 195), Size = new Size(200, 35) };
            panelDuo.Controls.Add(txtDuoRespuesta);

            Button btnResponder = CrearBotonPequeno("Responder", 240, 195, colorRosaAcento);
            btnResponder.Click += (s, e) => ProcesarRespuestaDuo();
            panelDuo.Controls.Add(btnResponder);

            lblDuoFeedback = new Label { Text = "", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(30, 255), Size = new Size(630, 60) };
            panelDuo.Controls.Add(lblDuoFeedback);

            Button btnVolver = CrearBotonPequeno("Menú Principal", 30, 390, Color.Gray);
            btnVolver.Click += (s, e) => MostrarPanel(panelMenu);
            panelDuo.Controls.Add(btnVolver);

            this.Controls.Add(panelDuo);
        }

        private void IniciarModoDuoPrompt()
        {
            j1Nombre = PedirTextoDialogo("Jugador 1", "Ingrese nombre del Jugador 1:");
            if (string.IsNullOrWhiteSpace(j1Nombre)) j1Nombre = "Jugador 1";

            j2Nombre = PedirTextoDialogo("Jugador 2", "Ingrese nombre del Jugador 2:");
            if (string.IsNullOrWhiteSpace(j2Nombre)) j2Nombre = "Jugador 2";

            vidasJ1 = 3; vidasJ2 = 3;
            puntajeJ1 = 0; puntajeJ2 = 0;
            turnoJ1 = true;

            MostrarPanel(panelDuo);
            SiguientePreguntaDuo();
        }

        private void SiguientePreguntaDuo()
        {
            if (vidasJ1 <= 0 || vidasJ2 <= 0)
            {
                GuardarEnHistorial(j1Nombre, puntajeJ1);
                GuardarEnHistorial(j2Nombre, puntajeJ2);

                string ganador = puntajeJ1 > puntajeJ2 ? $"¡Ganador: {j1Nombre}!" : (puntajeJ2 > puntajeJ1 ? $"¡Ganador: {j2Nombre}!" : "¡Empate técnico!");
                MessageBox.Show($"¡Duelo Finalizado!\n\n{j1Nombre}: {puntajeJ1} pts\n{j2Nombre}: {puntajeJ2} pts\n\n{ganador}", "Fin del Duelo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MostrarPanel(panelMenu);
                return;
            }

            string jugadorActual = turnoJ1 ? j1Nombre : j2Nombre;
            int vidasActuales = turnoJ1 ? vidasJ1 : vidasJ2;
            int puntajeActual = turnoJ1 ? puntajeJ1 : puntajeJ2;
            int nivel = (puntajeActual / 20) + 1;

            num1Duo = rand.Next(1, 10 * nivel);
            num2Duo = rand.Next(1, 10 * nivel);
            int tipoOp = rand.Next(0, 2);

            if (tipoOp == 0) { operadorDuo = "+"; respuestaCorrectaDuo = num1Duo + num2Duo; }
            else
            {
                operadorDuo = "-";
                if (num1Duo < num2Duo) { int t = num1Duo; num1Duo = num2Duo; num2Duo = t; }
                respuestaCorrectaDuo = num1Duo - num2Duo;
            }

            lblDuoInfo.Text = $"Turno: {jugadorActual.ToUpper()}  |  Vidas: {vidasActuales} ❤️  |  Puntaje: {puntajeActual} 🏆\nMarcador -> {j1Nombre}: {puntajeJ1} pts | {j2Nombre}: {puntajeJ2} pts";
            lblDuoPregunta.Text = $"¿Cuánto es {num1Duo} {operadorDuo} {num2Duo}?";
            txtDuoRespuesta.Text = "";
            lblDuoFeedback.Text = "";
            txtDuoRespuesta.Focus();
        }

        private void ProcesarRespuestaDuo()
        {
            if (int.TryParse(txtDuoRespuesta.Text, out int respUsuario))
            {
                if (respUsuario == respuestaCorrectaDuo)
                {
                    lblDuoFeedback.ForeColor = Color.ForestGreen;
                    lblDuoFeedback.Text = "¡Correcto! +10 puntos.";
                    if (turnoJ1) puntajeJ1 += 10; else puntajeJ2 += 10;
                }
                else
                {
                    lblDuoFeedback.ForeColor = colorRosaAcento;
                    lblDuoFeedback.Text = $"Incorrecto ({respuestaCorrectaDuo}). Pierde 1 vida.";
                    if (turnoJ1) vidasJ1--; else vidasJ2--;
                }
            }
            else
            {
                lblDuoFeedback.ForeColor = colorRosaAcento;
                lblDuoFeedback.Text = "Entrada inválida. Pierde 1 vida.";
                if (turnoJ1) vidasJ1--; else vidasJ2--;
            }

            turnoJ1 = !turnoJ1;

            Timer timer = new Timer { Interval = 1500 };
            timer.Tick += (s, e) => { timer.Stop(); timer.Dispose(); SiguientePreguntaDuo(); };
            timer.Start();
        }

        // ==========================================
        // 4. PERSISTENCIA Y TABLA DE CLASIFICACIÓN
        // ==========================================
        private void CrearPanelRanking()
        {
            panelRanking = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            Label lblTitulo = new Label { Text = "TABLA DE CLASIFICACIÓN (RANKING)", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = colorTextoOscuro, Location = new Point(30, 25), AutoSize = true };
            panelRanking.Controls.Add(lblTitulo);

            lstRanking = new ListBox { Font = new Font("Consolas", 11), Location = new Point(30, 80), Size = new Size(640, 270) };
            panelRanking.Controls.Add(lstRanking);

            Button btnVolver = CrearBotonPequeno("Menú Principal", 30, 390, Color.Gray);
            btnVolver.Click += (s, e) => MostrarPanel(panelMenu);
            panelRanking.Controls.Add(btnVolver);

            this.Controls.Add(panelRanking);
        }

        private void GuardarEnHistorial(string nombre, int puntaje)
        {
            if (totalRegistros < MAX_HISTORIAL)
            {
                nombresHistorial[totalRegistros] = nombre;
                puntajesHistorial[totalRegistros] = puntaje;
                totalRegistros++;
            }
        }

        private void ActualizarRanking()
        {
            lstRanking.Items.Clear();
            if (totalRegistros == 0)
            {
                lstRanking.Items.Add("Aún no hay registros guardados en esta sesión.");
                return;
            }

            string[] tempNom = new string[totalRegistros];
            int[] tempPun = new int[totalRegistros];
            Array.Copy(nombresHistorial, tempNom, totalRegistros);
            Array.Copy(puntajesHistorial, tempPun, totalRegistros);

            for (int i = 0; i < totalRegistros - 1; i++)
            {
                for (int j = 0; j < totalRegistros - i - 1; j++)
                {
                    if (tempPun[j] < tempPun[j + 1])
                    {
                        int tp = tempPun[j]; tempPun[j] = tempPun[j + 1]; tempPun[j + 1] = tp;
                        string tn = tempNom[j]; tempNom[j] = tempNom[j + 1]; tempNom[j + 1] = tn;
                    }
                }
            }

            lstRanking.Items.Add($"{"POS",-5} | {"JUGADOR",-32} | {"PUNTAJE",-8}");
            lstRanking.Items.Add(new string('-', 55));
            for (int i = 0; i < totalRegistros; i++)
            {
                lstRanking.Items.Add($"{i + 1,-5} | {tempNom[i],-32} | {tempPun[i],-8}");
            }
        }

        // Métodos auxiliares de diseño (Combinación Rojo y Rosa)
        private Button CrearBotonTematico(string texto, int y, Color colorFondo)
        {
            return new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = colorFondo,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(540, 50),
                Location = new Point(80, y),
                Cursor = Cursors.Hand
            };
        }

        private Button CrearBotonPequeno(string texto, int x, int y, Color colorFondo)
        {
            return new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = colorFondo,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 40),
                Location = new Point(x, y),
                Cursor = Cursors.Hand
            };
        }

        private string PedirTextoDialogo(string titulo, string mensaje)
        {
            Form prompt = new Form()
            {
                Width = 420,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = titulo,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = mensaje, Font = new Font("Segoe UI", 10), AutoSize = true };
            TextBox textBox = new TextBox() { Left = 20, Top = 55, Width = 360, Font = new Font("Segoe UI", 12) };
            Button confirmation = new Button() { Text = "Aceptar", Left = 280, Width = 100, Top = 100, DialogResult = DialogResult.OK, Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = colorRojoPrincipal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            prompt.ShowDialog();
            return string.IsNullOrWhiteSpace(textBox.Text) ? "Jugador" : textBox.Text;
        }
    }
}