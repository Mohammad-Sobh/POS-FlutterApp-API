using Newtonsoft.Json;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
namespace POS_Data_API.Classes;


public class FB
{
    public readonly IFirebaseClient firebase;
    public FB()
    {
        firebase = new FirebaseClient(new FirebaseConfig
        {
            BasePath = "https://pos-data-19229-default-rtdb.europe-west1.firebasedatabase.app/"
            ,
            AuthSecret = "FYUgkQPKYb6dKG4rVj4tYADcW5qPcdrUSRywceAJ"
        });
    }
}

