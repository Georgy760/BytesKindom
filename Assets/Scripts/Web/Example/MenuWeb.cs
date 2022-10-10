using TMPro;
using UnityEngine;

namespace Web.Example
{
    public class MenuWeb : MonoBehaviour
    {
        [System.Serializable]
        public class MenuLogin
        {
            public TMP_Text login, password;
        }
        [System.Serializable]
        public class MenuRegistration
        {
            public TMP_Text login, password1, password2, nickname;
        }

        public MenuLogin loginWindow;
        public MenuRegistration registrationWindow;
    
        [SerializeField] private WebManager webManager;

        public void Login()
        {
            webManager.Login(loginWindow.login.text, loginWindow.password.text);
        }
    
        public void Register()
        {
            webManager.Registration(registrationWindow.login.text, registrationWindow.password1.text, registrationWindow.password2.text, registrationWindow.nickname.text);
        }
    }
}