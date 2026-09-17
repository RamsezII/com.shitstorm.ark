using _UTIL_;
using System;

namespace _ARK_
{
    partial class ArkComponent2
    {
        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        protected sealed class HTextAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        protected sealed class UTextAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        protected sealed class RTextAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field)]
        protected class HFieldAttribute : NJFieldAttribute
        {
            public HFieldAttribute(bool editable = true) : base(editable)
            {
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        protected class UFieldAttribute : NJFieldAttribute
        {
            public UFieldAttribute(bool editable = true) : base(editable)
            {
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        protected class RFieldAttribute : NJFieldAttribute
        {
            public RFieldAttribute(bool editable = true) : base(editable)
            {
            }
        }

        public virtual bool CandidateForTextSave => true;

        //--------------------------------------------------------------------------------------------------------------

        public void SaveArkTexts(bool log = true, in bool reloadAllTextsAfterSave = false)
        {
            NJDict
                rjobjs = new(typeof(ArkComponent2)),
                hjobjs = new(typeof(ArkComponent2)),
                ujobjs = new(typeof(ArkComponent2));

            OnBeforeSaveFields(log);
            OnBeforeSaveFields(rjobjs, hjobjs, ujobjs, log);

#if UNITY_EDITOR
            rjobjs.SaveTexts<RFieldAttribute>(type => NUCLEOR.GetResourcesJSonPath(type), log, this);
#endif
            hjobjs.SaveTexts<HFieldAttribute>(type => NUCLEOR.GetHomeJSonPath(type), log, this);
            ujobjs.SaveTexts<UFieldAttribute>(type => NUCLEOR.instance.GetCurrentUserTextPath(type), log, this);

            if (reloadAllTextsAfterSave)
                NUCLEOR.delegates.OnApplicationFocus?.Invoke();
        }

        protected virtual void OnBeforeSaveFields(bool log)
        {
        }

        protected virtual void OnBeforeSaveFields(NJDict rjobjs, NJDict hjobjs, NJDict ujobjs, bool log)
        {
        }

        public void LoadArkTexts(bool log = true, bool rtexts = true, bool htexts = true, bool utexts = true)
        {
            NJDict
                rjobjs = new(typeof(ArkComponent2)),
                hjobjs = new(typeof(ArkComponent2)),
                ujobjs = new(typeof(ArkComponent2));

            if (rtexts)
            {
                rjobjs.LoadRTexts<RFieldAttribute, RTextAttribute>(GetType(), log);
                rjobjs.SetFields<RFieldAttribute>(this);
            }

            if (htexts)
                LoadAndSetFields<HFieldAttribute, HTextAttribute>(hjobjs, type => NUCLEOR.GetHomeJSonPath(type));

            if (utexts)
                LoadAndSetFields<UFieldAttribute, UTextAttribute>(ujobjs, type => NUCLEOR.instance.GetCurrentUserTextPath(type));

            void LoadAndSetFields<TAttribute, TTextAttribute>(NJDict jobjs, Func<Type, string> getPath) where TAttribute : Attribute where TTextAttribute : Attribute
            {
                jobjs.LoadTexts<TAttribute, TTextAttribute>(GetType(), getPath, log);
                jobjs.SetFields<TAttribute>(this);
            }

            OnAfterLoadFields(log);
            OnAfterLoadFields(rjobjs, hjobjs, ujobjs, log);
        }

        protected virtual void OnAfterLoadFields(bool log)
        {
        }

        protected virtual void OnAfterLoadFields(NJDict rjobjs, NJDict hjobjs, NJDict ujobjs, bool log)
        {
        }
    }
}
