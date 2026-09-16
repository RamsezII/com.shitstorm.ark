using _UTIL_;
using System;

namespace _ARK_
{
    partial class ArkComponent2
    {
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
                rjobjs = new(),
                hjobjs = new(),
                ujobjs = new();

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

        public void LoadArkTexts(bool log = true)
        {
            NJDict
                rjobjs = new(),
                hjobjs = new(),
                ujobjs = new();

            rjobjs.LoadRTexts<RFieldAttribute>(GetType(), log);
            rjobjs.SetFields<RFieldAttribute>(this);

            LoadAndSetFields<HFieldAttribute>(hjobjs, type => NUCLEOR.GetHomeJSonPath(type));
            LoadAndSetFields<UFieldAttribute>(ujobjs, type => NUCLEOR.instance.GetCurrentUserTextPath(type));

            void LoadAndSetFields<TAttribute>(NJDict jobjs, Func<Type, string> getPath) where TAttribute : Attribute
            {
                jobjs.LoadTexts<TAttribute>(GetType(), getPath, log);
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