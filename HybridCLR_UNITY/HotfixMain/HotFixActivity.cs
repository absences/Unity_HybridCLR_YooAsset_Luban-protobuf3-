namespace HotfixMain
{
    public class HotFixActivity
    {

        public static void Init()
        {
            UnityEngine.Debug.Log(2222);

            GameEnter.UI.CloseUIForm<UILoadForm>();

            GameEnter.UI.OpenUIForm(nameof(TestForm), "Base");
        }
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {


        }
        public static void ShutDown()
        {

        }
    }
}
