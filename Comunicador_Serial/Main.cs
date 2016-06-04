using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; // necessário para ter acesso as portas

namespace Comunicador_Serial
{
    public partial class Main : Form
    {
        string RxString;
        public Main()//não modificar
        {
            InitializeComponent();
        }

        /*-------Funções de formulario--------*/
        private void Form1_Load(object sender, EventArgs e)//ao iniciar o programa
        {
            textBoxReceber.AppendText("Inicializando...");

            configurate();//le as configurações
            dicas();//inicializa as dicas

            if (timerCOM.Enabled == true)
                textBoxReceber.AppendText(Environment.NewLine+"Live scan ativado...");
            else
                textBoxReceber.AppendText(Environment.NewLine + "Live scaner desativado...");

            textBoxReceber.AppendText(Environment.NewLine +"Bem Vindo!");

            comboBox2.Items.Add("300");
            comboBox2.Items.Add("1200");
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("57600");
            comboBox2.Items.Add("74880");
            comboBox2.Items.Add("115200");
            comboBox2.Items.Add("230400");
            comboBox2.Items.Add("250000");

            comboBox2.SelectedIndex = 4;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)//ao fechar o programa
        {
            if (serialPort1.IsOpen == true)  // se porta aberta
                serialPort1.Close();         //fecha a porta
        }

        /*-------Funções de chamada---------*/
        private void atualizaListaCOMs()//atualiza as portas COM disponiveis
        {
            int i;
            bool quantDiferente; //flag para sinalizar que a quantidade de portas mudou

            i = 0;
            quantDiferente = false;

            //se a quantidade de portas mudou
            if (comboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (comboBox1.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true; 
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;                     //retorna
            }

            //limpa comboBox
            comboBox1.Items.Clear();
            int counter = 0;

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
                counter++;
            }

            //seleciona a primeira posição da lista
            if(counter != 0)//se a lista não estiver vazia
                comboBox1.SelectedIndex = 0;
            else//se a lista estiver vazia
                comboBox1.Items.Add("");
                comboBox1.SelectedIndex = 0;

        }

        private void enviarComandos()//envia os comandos pela serial
        {
            if (serialPort1.IsOpen == true)          //porta está aberta
            {
                if(textBoxEnviar.Text != "")//verifica se a textbox está vazia
                {
                    serialPort1.Write(textBoxEnviar.Text);  //envia o texto presente no textbox Enviar
                    textBoxReceber.AppendText(Environment.NewLine + "Comando ''" + textBoxEnviar.Text + "'' enviado.");
                }
                
                if(Properties.Settings.Default.clear_on_send == true)//verifica a opção clear on send
                {
                    textBoxEnviar.Text = "";
                }

            }
            else
            {
                MessageBox.Show("Conecte-se primeiro!", "Erro");
            }
        }

        private void configurate()//le as configurações
        {
            //le a configuração live scan
            timerCOM.Enabled = true;

            if (Properties.Settings.Default.live_Scan == true)
            {
                checkBox1.Checked = true;
                scanButton.Enabled = false;//desativa o botão scan
            }
            else
            {
                checkBox1.Checked = false;
                scanButton.Enabled = true;//ativa o botão scan
            }

            //le a configuração clear on sent
            if (Properties.Settings.Default.clear_on_send == true)
            {
                checkBox2.Checked = true;
                Properties.Settings.Default.clear_on_send = true;
                Properties.Settings.Default.Save();//para garantir o carregamento correto da opção
            }
            else
            {
                checkBox2.Checked = false;
            }
        }

        private void dicas()//exibe as caixas de dicas
        {
            /*-----dica live scan-----*/
            System.Windows.Forms.ToolTip dicaLiveScan = new System.Windows.Forms.ToolTip();
            dicaLiveScan.SetToolTip(this.checkBox1, "Busca automatica de Portas COM disponiveis");
            /*-----dica clean on send------*/
            System.Windows.Forms.ToolTip dicaCleanSend = new System.Windows.Forms.ToolTip();
            dicaLiveScan.SetToolTip(this.checkBox2, "Apagar comando após o envio");
            /*-----dica clear button------*/
            System.Windows.Forms.ToolTip dicaClearBox = new System.Windows.Forms.ToolTip();
            dicaLiveScan.SetToolTip(this.btClearText, "Limpar Log");
        }

        /*-------Botões---------------------*/
        private void btConectar_Click(object sender, EventArgs e)//botão de conectar
        {
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Items[comboBox2.SelectedIndex]);
                    serialPort1.Open();
                }
                catch
                {
                    return;

                }
                if (serialPort1.IsOpen)
                {
                    btConectar.Text = "Desconectar";
                    comboBox1.Enabled = false;//desativa o combobox COM port
                    comboBox2.Enabled = false;//desativa o combobox baud rate
                    scanButton.Enabled = false;//desativa o botão scan

                }
            }
            else
            {

                try
                {
                    serialPort1.Close();
                    comboBox1.Enabled = true;//ativa o combobox COM port
                    comboBox2.Enabled = true;//ativa o combobox baud rate

                    if (checkBox1.Checked == false)
                        scanButton.Enabled = true;//ativa o botão scan

                    btConectar.Text = "Conectar";
                }
                catch
                {
                    return;
                }

            }
        }

        private void btEnviar_Click(object sender, EventArgs e)//botão de enviar
        {
            enviarComandos();//envia comandos
        }

        private void scanButton_Click(object sender, EventArgs e)//botão Scan
        {
            atualizaListaCOMs();
        }

        private void btClearText_Click(object sender, EventArgs e)//limpa a caixa de recebidos
        {
            textBoxReceber.Text = "";
        }

        /*--------checkbox------------------*/
        private void checkBox1_CheckedChanged(object sender, EventArgs e)//live scan on/off
        {
            if (timerCOM.Enabled == true)
            {
                timerCOM.Enabled = false;
                Properties.Settings.Default.live_Scan = false;//altera a configuração na sessão
                Properties.Settings.Default.Save();//salva as configurações
                scanButton.Enabled = true;//ativa o botão scan
            }
            else
            {
                timerCOM.Enabled = true;
                Properties.Settings.Default.live_Scan = true;//altera a configuração do live scan
                Properties.Settings.Default.Save();//salva as configurações
                scanButton.Enabled = false;//desativa o botão scan
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)//clear on sent on/off
        {
            if(Properties.Settings.Default.clear_on_send == true)
            {
                Properties.Settings.Default.clear_on_send = false;//altera a configuração do clear on send
                Properties.Settings.Default.Save();//salva as configurações
            }
            else
            {
                Properties.Settings.Default.clear_on_send = true;//altera a configuração do clear on send
                Properties.Settings.Default.Save();//salva as configurações
            }
        }

        /*--------Serial Port, receber/enviar/tratar dados---------*/
        private void timerCOM_Tick(object sender, EventArgs e)//a cada x milisegundos executa a função
        {
            atualizaListaCOMs();
        }
                
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)//serial port
        {
            RxString = serialPort1.ReadExisting();              //le o dado disponível na serial
            this.Invoke(new EventHandler(trataDadoRecebido));   //chama outra thread para escrever o dado no text box
        }

        private void trataDadoRecebido(object sender, EventArgs e)//printa o dado recebido do dispositivo no console
        {
            textBoxReceber.AppendText(Environment.NewLine+"Recebido: "+ RxString);
        }

        private void textBoxEnviar_KeyPress(object sender, KeyPressEventArgs e)//ao Precionar ENTER na envia os comandos da textbox1
        {
            if (e.KeyChar == 13)//se precionado enter
            {
                e.Handled = true;//retira o barulho chato ao precionar enter
                enviarComandos();//envia os comandos
            }
        }
    }
}
