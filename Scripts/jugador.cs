using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class jugador : MonoBehaviourPunCallbacks
{
    
    public TMP_InputField nombre;
    public Image avatar;

    private string texto;

    public GameObject pan1;
    public GameObject pan2;
    public GameObject pan3;
    public GameObject pan4;
    public GameObject pan5;

    private TextMeshProUGUI textoGameObject;
    private Image avatarGameObject;

    private bool salasExisten;

    public TMP_InputField nombreSalaPoner;
    public TMP_InputField maxJugadores;

    private List<string> salas;
    private List<int> salasCantidad;
    private List<int> salasMaximo;

    public TMP_Dropdown listaSalasUnirse;

    public TextMeshProUGUI nombreSalaMostrar;
    public TextMeshProUGUI maxJugadoresMostrar;
    public TextMeshProUGUI nombreJugadoresMostrar;



    // Start is called before the first frame update
    void Start()
    {

        salas=new List<string>();
        salasCantidad= new List<int>();
        salasMaximo= new List<int>();

        conexionExitosa();
        salasExisten = false;

        asignarNombreAvatar(pan2);

        
    }

    //Le das el panel en el que estas para asignar el nombre y el avatar a los campos que lo necesiten
    private void asignarNombreAvatar(GameObject pan)
    {
        avatarGameObject = pan.transform.Find("avatarMostrar").gameObject.GetComponent<Image>();
        textoGameObject = pan.transform.Find("nombreMostrar").gameObject.GetComponent<TextMeshProUGUI>();
        textoGameObject.text = texto;
        avatarGameObject.sprite = avatar.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Se ha conectado");
        TypedLobby typedLobby = new TypedLobby("lobyClase", LobbyType.Default);
        PhotonNetwork.JoinLobby(typedLobby);
    }

    private void asignarNombre()
    {
        texto=nombre.text;
        if (texto == "")
        {
            System.Random rad = new System.Random();
            texto = "jugador" + rad.Next(0, 1001);
        }
        PhotonNetwork.NickName = texto;
    }

    public void recogerDatos()
    {
        asignarNombre();
        
        segundaPantalla();
    }

    private void segundaPantalla()
    {
        pan1.SetActive(false);
        pan2.SetActive(true);
        textoGameObject.text = texto;
        avatarGameObject.sprite = avatar.sprite;
    }

    //Creas una sala
    public void crearSala()
    {
        pan2.SetActive(false);
        pan3.SetActive(true);
        asignarNombreAvatar(pan3);
        
       
    }

    public void generarSala()
    {
        string textoMax = maxJugadores.text;
        string nombreSala;
        int max;
        if (string.IsNullOrEmpty(textoMax))
        {
            max = 20;
        }
        else
        {
            max = int.Parse(textoMax);

            if (max <= 0 || max > 20)
            {
                max = 20;
            }
        }
        if (string.IsNullOrEmpty(nombreSalaPoner.text))
        {
            System.Random rad = new System.Random();
            nombreSala = "sala" + rad.Next(0, 1001);
            //Debug.Log("Se crea sala con texto aleatorio " + nombreSala);
            //PhotonNetwork.JoinOrCreateRoom(nombreSala, new RoomOptions { MaxPlayers = max }, TypedLobby.Default);
            PhotonNetwork.CreateRoom(nombreSala, new RoomOptions { MaxPlayers = max });
            salaFinal(pan3);

        }
        else if (salas.Contains(nombreSalaPoner.text))
        {
            Debug.Log("Ya hay una sala con ese nombre");
        }
        else
        {
            //Debug.Log("Se crea sala con nombre incorporado " + nombreSalaPoner.text);
            //PhotonNetwork.JoinOrCreateRoom(nombreSalaPoner.text, new RoomOptions { MaxPlayers = max }, TypedLobby.Default);
            PhotonNetwork.CreateRoom(nombreSalaPoner.text, new RoomOptions { MaxPlayers = max });

            salaFinal(pan3);
        }

    }

    //Este metodo funciona
    public void pruebaCrearSala()
    {
        PhotonNetwork.CreateRoom("SalaMax2", new RoomOptions { MaxPlayers = 2 });
        //PhotonNetwork.JoinOrCreateRoom("Sala", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    private void salaFinal(GameObject pan)
    {
        pan.SetActive(false);
        pan5.SetActive(true);
        asignarNombreAvatar(pan5);
    }

    private void conexionExitosa()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conectandose");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entraste al lobby");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("La sala "+PhotonNetwork.CurrentRoom.Name+" ha sido creada");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Actualización de la lista de salas");
        if (roomList != null && roomList.Count != 0)
        {
            Debug.Log("Entra al actualizador");
            salas.Clear();
            salasCantidad.Clear();
            salasMaximo.Clear();


            foreach (RoomInfo room in roomList)
            {
                salas.Add(room.Name);
                salasCantidad.Add(room.PlayerCount);
                salasMaximo.Add(room.MaxPlayers);

                Debug.Log("Nombre de la sala: " + room.Name + " | Jugadores: " + room.PlayerCount + " | Maximo Jugadores: " + room.MaxPlayers);
            }
            salasExisten = true;
        }
        else if (roomList != null)
        {
            Debug.Log("El room list es nulo");
        }
        else
        {
            Debug.Log("No hay salas");
        }
    }

    //Te unes a una sala existente
    public void unirSala()
    {
        if (salasExisten)
        {
            pan2.SetActive(false);
            pan4.SetActive(true);
            asignarNombreAvatar(pan4);

            listarSalas();
        }
        else
        {
            Debug.Log("No hay salas existentes, por favor crea una");
        }
    }

    public void refrescarSalas()
    {
        PhotonNetwork.Disconnect();
        conexionExitosa();
        listarSalas();
    }

    private void listarSalas()
    {
        listaSalasUnirse.options.Clear();

        if (salas == null)
        {
            Debug.Log("No hay salas");
        }
        else
        {
            foreach (string sala in salas)
            {
                listaSalasUnirse.options.Add(new TMP_Dropdown.OptionData(sala));
            }
        }
    }

    public void unirseSalaSeleccionada()
    {   
        PhotonNetwork.JoinRoom(listaSalasUnirse.options[listaSalasUnirse.value].text);
        pan4.SetActive(false);
        pan5.SetActive(true);
        asignarNombreAvatar(pan5);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Conectado a la sala " + PhotonNetwork.CurrentRoom.Name + " con " + PhotonNetwork.CurrentRoom.MaxPlayers + " jugadores como máximo");
        asignarNombresSalaFinal();
    }

    private void asignarNombresSalaFinal()
    {
        nombreSalaMostrar.text = PhotonNetwork.CurrentRoom.Name;
        maxJugadoresMostrar.text=PhotonNetwork.CurrentRoom.MaxPlayers+"";
        string jugadores=null;
        Player[] player = PhotonNetwork.PlayerList;
        foreach(Player p in player)
        {
            if (string.IsNullOrEmpty(jugadores))
            {
                jugadores = p.NickName;
            }
            else
            {
                jugadores=jugadores+" - "+p.NickName;
            }
        }
        if (string.IsNullOrEmpty(jugadores))
        {
            nombreJugadoresMostrar.text = "No hay mas jugadores";
        }
        else
        {
            nombreJugadoresMostrar.text = jugadores;
        }
        
    }

    public void refrescarNombres()
    {
        asignarNombresSalaFinal();
    }

}
