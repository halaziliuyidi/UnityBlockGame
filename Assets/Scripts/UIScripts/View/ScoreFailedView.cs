using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
    public class ScoreFailedViewContext : BaseContext
    {
        public ScoreFailedViewContext() : base(UIType.ScoreFailedView)
        {

        }
    }
    public class ScoreFailedView : BaseView
    {
        public Image progressImage;

        public TextMeshProUGUI progressText;

        public override void OnEnter(BaseContext context)
        {
            progressImage.fillAmount = (float)GameManager.Instance.nowGameScore/ (float)GameManager.Instance.nowLevel.targetScore;
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

        public void OnRetryBtnClick()
        {
            GameManager.Instance.RetryLevel();
            Singleton<ContextManager>.Instance.Push(new GameViewContext());
        }
    }
}