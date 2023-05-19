using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
    public class ScoreWinViewContext : BaseContext
    {
        public ScoreWinViewContext() : base(UIType.ScoreWinView)
        {

        }
    }
    public class ScoreWinView : BaseView
    {

        public Image progressImage;

        public TextMeshProUGUI progressText;
        public override void OnEnter(BaseContext context)
        {
            base.OnEnter(context);
            progressImage.fillAmount=1f;
            progressText.text = GameManager.Instance.nowGameScore + "/" + GameManager.Instance.nowLevel.targetScore;
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

        public void OnNextLevelBtnClick()
        {
            GameManager.Instance.NextLevel();
            Singleton<ContextManager>.Instance.Push(new GameViewContext());
        }
    }
}