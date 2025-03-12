// public class UserSessionManager : MonoBehaviour
// {
//     public static UserSessionManager Instance { get; private set; }
//     public string UserID { get; private set; }
//
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
//
//     public void SetUserID(string userId)
//     {
//         UserID = userId;
//     }
// }