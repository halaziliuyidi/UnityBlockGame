using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
    public class StartViewContext : BaseContext
    {
        public StartViewContext() : base(UIType.StartView)
        {

        }
    }

    public class StartView : BaseView
    {

        public override void OnEnter(BaseContext context)
        {
            base.OnEnter(context);
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public override void OnPause(BaseContext context)
        {
            base.OnPause(context);
        }

        public override void OnResume(BaseContext context)
        {
            base.OnResume(context);
        }

        public void OnStartGameBtnClick()
        {
            GameManager.Instance.audioController.PlayButtonClickAudio();
            GameManager.Instance.GameStart();
            Singleton<ContextManager>.Instance.Push(new GameViewContext());


        }

        public void OnLevelBtnClick()
        {
            GameManager.Instance.audioController.PlayButtonClickAudio();
            Singleton<ContextManager>.Instance.Push(new LevelViewContext());
        }
    }
}
