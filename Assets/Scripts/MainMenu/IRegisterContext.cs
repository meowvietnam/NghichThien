using System.Threading.Tasks;
using Network.Models;
using UnityEngine;

namespace MainMenu
{
    public interface IRegisterContext
    {
        RegisterModel GetDefaultRegisterModel();
        Task<RegisterResult> Register(RegisterModel model);
    }
}