using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Referencias
using Mono.Data.Sqlite;
using Mono.Data;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class SQliteScript : MonoBehaviour
{
    //LO ENRIATAMOS DESDE EL AWAKE PARA INICIAR CON EL ARCHIVO DE BASE DE DATOS YA CREADOS
    void Awake()
    {
        //ESTE CODIGO ME FUNCIONA SIN PROBLEMA EN WINDOWS Y ANDROID
        //RUTA EN EL SISTEMA : C:\Users\USUARIO\AppData\LocalLow\NOMBRE DE PAQUETE DEL PROYECTO\NOMBRE DE PROYECTO

        string RutaBD = Application.persistentDataPath + "/" + "UApp.sqlite";//(UApp.sqlite - NOMBRE_BD.sqlite) ES EL ARCHIVO DE BASE DE DATOS, PON EL NOMMBRE QUE QUIERAS SIN ESPACIOS O COSAS RARAS
        string conn = "URI=file:" + RutaBD;

        if (!File.Exists(RutaBD))
        {
            //Si no se encuentra en Android O WINDOWS creará tablas y base de datos

            Debug.LogWarning("File \"" + RutaBD + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/UApp");
            // UNITY_ANDROID
            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/UApp.sqlite");
            while (!loadDB.isDone) { }
            //luego guardar en Application.persistentDataPath
            File.WriteAllBytes(RutaBD, loadDB.bytes);
        }
        //OTROS QUE NO HE USADO
        /*// #UNITY_IOS //iOS
        var loadDb = Application.dataPath + "/Raw/" + DatabaseName;
        // RUTA StreamingAssets en iOS
        // GUARDA EN Application.persistentDataPath
        File.Copy(loadDb, RutaBD);
        // #UNITY_WP8 // Windows Phone
        var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;
        File.Copy(loadDb, RutaBD);
        // #UNITY_WINRT
        var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;
        File.Copy(loadDb, RutaBD);
        */


        ///CREAMOS TABLAS SQL - COPIA Y MODIFICA SEGUN LAS TABLAS QUE QUIERAS CREAR
        IDbConnection dbconnCuentas = new SqliteConnection(conn);
        dbconnCuentas.Open();
        string queryCuentas = "CREATE TABLE Usuarios (id INTEGER PRIMARY KEY  NOT NULL , Nombre VARCHAR, Pass VARCHAR)";
        try
        {
            IDbCommand dbcmdcuentas = dbconnCuentas.CreateCommand(); // crea un comando vacio
            dbcmdcuentas.CommandText = queryCuentas; // llena el command
            IDataReader reader = dbcmdcuentas.ExecuteReader(); // ejecuta y regresa a un lector
        }
        catch (Exception e)
        {
            //Si ya existe aqui puedes agregar alguna funcion que se necesite
            Debug.Log(e);
        }
    }

    //////CODIGOS PA QUE NO TE LA COMPLIQUES
    private void Start()
    {
        //Amos a registrar pa poner el ejemplo
        RegistarOActualizarUsuario(1, "Luis", "Barbon");//el id es automatico, aqui solo pa desmostracion
        Consultar("Usuarios");
        //LimpiarTabla("Usuarios");
        //LimpiarCampo("Usuarios", "Luis");
    }
    /////// Si necesitas consultar los datos o realizar busqueda
    public void Consultar(string NombreTabla)
    {
        string BDfile = "URI=file:" + Application.persistentDataPath + "/" + "UApp.sqlite";
        //Para evitar comportamientos extraños en algunas plataformas, recominedo que al inicio llamen la ruta de la BD

        using (IDbConnection conexionBD = new SqliteConnection(BDfile))//creamos la conexion con nuestra base de datoa
        {
            conexionBD.Open();//abrimos la conexion
            using (IDbCommand comandoBD = conexionBD.CreateCommand())//preparamos para enviar un comando que va a ser una consulta
            {
                string consultaSQlite = "";

                //Ejemplo de busqueda, es el mismo pedo
                /*if (!bBuscar)//Otras variables si quieres crear busqueda
                {
                    consultaSQlite = "SELECT * FROM Usuarios";//montamos la query
                }
                else if (bBuscar)//Son bolianos, es un ejemplo nomas
                {
                    consultaSQlite = "SELECT * FROM Usuarios WHERE " + "'" + NombreTabla + "'" + "LIKE " + "Cosa o valor a buscar";// + "OR WHERE jerarquia LIKE " + buscar;//montamos la query
                }*/

                //nomas consulta en putiza
                consultaSQlite = "SELECT * FROM " + "'" + NombreTabla + "'";//montamos la query, ejemplo usando variables
                //string consultaSQlite = "SELECT * FROM NombreTabla";//Ejemplo Directo
                //Cuando referimos una tabla con una variable String, no olvidar concatenar con "'", si usamos numero int u otro no hay pedo sin comillas

                comandoBD.CommandText = consultaSQlite; //ejecutamos el comando en funcion de la cosulta
                //ahora tenemos que leer el resultado de ejecutar este comando, y para ello necesitamos un reader o puntero
                using (IDataReader puntero = comandoBD.ExecuteReader())
                {
                    //y mientras el reader tenga algo que leer o lo que es lo mismo, pueda leer
                    while (puntero.Read())
                    {
                        Debug.Log("Existe:"+ puntero.GetInt32(0) + puntero.GetString(1) + ":" + puntero.GetString(2));//muestra los resultados de la 2 columna que sabemos es una string
                        //Enriatas la funcion que necesite el resultado
                        //funcion(puntero.GetString(1));
                    }
                    conexionBD.Close();//cerramos la conexion
                    puntero.Close();//cerramos el reader o puntero
                }
            }
        }
    }
    ///////  REGISTRAR O ACTUALIZAR DATOS - 2 EN 1 PA MAS FACIL
    public void RegistarOActualizarUsuario(int idUsuario,string nombre,string pass)//en lugar de variablas pasalo a campos.text de los objetos
    {
        string BDfile = "URI=file:" + Application.persistentDataPath + "/" + "UApp.sqlite";
        //Para evitar comportamientos extraños en algunas plataformas, recominedo que al inicio llamen la ruta de la BD
        
        string sqlQuery;//Declarar aqui o arriba si necesitas llamarlo fuero de esta duncion

        using (IDbConnection dbconn = new SqliteConnection(BDfile))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            IDbCommand checarRegistro = dbconn.CreateCommand();//Creamos otro comando para comprobar si ya existe un registro igual
            checarRegistro.CommandText = "SELECT count(*) FROM Usuarios WHERE id = " + idUsuario;//en este caso el id del usuario se compara
            //FROM usuarios - reemplazas por tu tabla, su usan variables no olvides las "'"

            int count = Convert.ToInt32(checarRegistro.ExecuteScalar());
            //Pasamos el resultado a int pa que sea mas facil condicionar
            if (count == 0)//Si devuelve 0 es que no lo encintro, entonces procedemos a registrar
            {
                sqlQuery = string.Format("insert into Usuarios (Nombre, Pass) values (\"{0}\",\"{1}\")", nombre, pass);// nombre tabla
                Debug.Log("Usuario Registrado");
            }
            else
            {//Si ya existe vamos a actualizar esos datos - en este caso donde el id sea el mismo
                sqlQuery = string.Format("UPDATE Usuarios set Nombre = @nombre , Pass = @pass WHERE id = " + idUsuario);

                SqliteParameter P_update_nombre = new SqliteParameter("@nombre", nombre);
                SqliteParameter P_update_pass = new SqliteParameter("@pass", pass);

                dbcmd.Parameters.Add(P_update_nombre);
                dbcmd.Parameters.Add(P_update_pass);
                Debug.Log("Usuario Actualizado");
            }
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }
    }
    //////// Limpiar una Tabla alv //////
    public void LimpiarTabla(string Tabla)
    {
        string BDfile = "URI=file:" + Application.persistentDataPath + "/" + "UApp.sqlite";
        //igual llamamos la ruta

        using (IDbConnection dbconn = new SqliteConnection(BDfile))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "DELETE FROM '" + Tabla + "'";//Nombre de la tabla
            //string sqlQuery = "DELETE FROM Usuarios";//O borrar directamente
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            Debug.Log("Se limpio orden Completa de la cuenta:" + Tabla);
        }
    }
    //limpiar Campo
    public void LimpiarCampo(string nomBD, string nomUsuario)
    {
        string BDfile = "URI=file:" + Application.persistentDataPath + "/" + "UApp.sqlite";
        using (IDbConnection dbconn = new SqliteConnection(BDfile))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "DELETE FROM " + nomBD + " where Nombre = '" + nomUsuario + "'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            Debug.Log("Se elimino usuario:" + nomUsuario);
        }
    }
}
