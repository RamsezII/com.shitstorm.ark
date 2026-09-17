namespace _ARK_
{
    public abstract class OS : ArkComponent2
    {

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }
    }
}