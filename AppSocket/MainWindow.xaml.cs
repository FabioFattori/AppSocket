using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using inseriti in più:
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;

namespace AppSocket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = null;
        DispatcherTimer timer = null;
        public MainWindow()
        {
            //dovrei avere un thread che ascolta continuamente se arrivano dei nuovi messaggi
            //noi però abbiamo usato un timer che ogni tot di tempo va ad ascoltare se sono arrivati nuovi mess

            InitializeComponent();

            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);//il DGram da l'imformazione sul tipo di messaggio e ProtocolType da l'informazione sul protocollo utilizzato
            //creato il socket stabilisco le due parti della conversazione:
            IPAddress local_address = IPAddress.Any;  //con questa istruzione ottengo l'ip address della mia macchina 
            IPEndPoint local_endPoint = new IPEndPoint(local_address.MapToIPv4(), 33500);//il secondo valore è la porta, la mettiamo a mano e speriamo che il firewall non blocchi la porta

            socket.Bind(local_endPoint);//istruzione per collegare il mittente al socket appena creato


            //istruzioni per creare il timer e gestirlo
            timer = new DispatcherTimer();//creo l'oggetto
            timer.Tick += new EventHandler(aggiornamentoTimer);//assegno al timer l'evento: cioè interrompo l'esecuzione del codice dopo quell'intervallo di tempo, eseguo quello che c'è dentro al metodo e poi ritorno ad eseguire le istruzioni qui di sotto
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);//ogni questo timespan avverrà il tick, quindi verrà eseguito il metodo aggiornamentoTimer
            timer.Start();//avvia il timer
        }

        private void aggiornamentoTimer(object sender, EventArgs e)
        {

            int nBytes = 0;//dichiaro una variabile per contare quanti bytes ho ricevuto

            if ((nBytes = socket.Available) > 0)//if per vedere se ci sono dei bytes da contare: se si fa se no non fa ninte
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];//array di bytes che conterrà i bytes inviati

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);//creo un remote end point -NON é QUELLO CHE MANDA IL MESS, è solo un contenitore-

                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);//mi assegna al remoteEndPoint chi è il mittente , in più mi salva il messaggio nella variabile buffer e poi mi riconta quanti sono i bytes del messaggio

                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();//recupero l'ip address in formato stringa del mittente

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);//prende il messaggio e lo decodifica il messaggio in UTF8

                Chat_lst.Items.Add(from + ": " + messaggio);
            }

        }


        private void btn_invia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress remote_Address = IPAddress.Parse(IndirizzoIP_txt.Text);//prendo l'indirizzo ip del destinatario
                IPEndPoint remote_endPoint = new IPEndPoint(remote_Address, Convert.ToInt32(porta_txt.Text));//creo il destinatario prendendo i dati dai componenti della grafica

                byte[] messaggio = Encoding.UTF8.GetBytes(Messaggio_txt.Text);//prendo il mio messaggio dalla grafica

                socket.SendTo(messaggio, remote_endPoint);//istruzione per instradare il messaggio al destinatario
            }
            catch (Exception)
            {
                MessageBox.Show("è necessario scrivere i dati per comunicare con un altro utente");
            }


        }


    }
}
