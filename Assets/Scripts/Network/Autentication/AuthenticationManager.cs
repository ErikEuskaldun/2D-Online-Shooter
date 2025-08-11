using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] private Button btnLogin;
    [SerializeField] private TMP_Text txtButton;
    

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        //AuthenticationService.Instance.SignedIn += SignedIn;
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Mas tarde se puede upgradear para que pida autenticacion real (log-in con google)
        //else 
            //SignedIn();
    }
}

