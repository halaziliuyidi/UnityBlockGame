using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

namespace MoleMole
{
    public class GameRestartViewContext : BaseContext
    {
        public GameRestartViewContext() : base(UIType.GameRestartView)
        {

        }
    }

    public class GameRestartView : BaseView
    {
        public TextMeshProUGUI scoreText;

        public TextMeshProUGUI maxScoreText;
        public override void OnEnter(BaseContext context)
        {
            InitUIData();
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

        private void InitUIData()
        {
            scoreText.text = Tool.GetText(GameManager.Instance.nowGameScore);
            maxScoreText.text = Tool.GetText(GameDataManager.Instance.GetMaxScore());
        }

        public void OnRestartGameBtnClick()
        {
            GameManager.Instance.GameRestart();
            Singleton<ContextManager>.Instance.Push(new GameViewContext());
            GameManager.Instance.audioController.PlayButtonClickAudio();
        }
    }
}
