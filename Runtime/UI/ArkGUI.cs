using _UTIL_;
using System.Text;
using UnityEngine;

namespace _ARK_
{
    public sealed partial class ArkGUI : ArkComponent1
    {
        public static ArkGUI instance;

        readonly DictListener<object, Traductions> users = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreate<ArkGUI>();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            users.AddListener1b(gameObject.SetActive);
        }

        //----------------------------------------------------------------------------------------------------------

        public void Add(in object user, in string value) => users.AddElement(user, new(value), force: true);
        public void Add(in object user, in Traductions value) => users.AddElement(user, value, force: true);

        private void OnGUI()
        {
            StringBuilder sb = new();

            foreach (var pair in users._collection)
                sb.AppendLine(pair.Value.GetAutomatic());

            GUILayout.Box(sb.TroncatedForLog());
        }
    }
}