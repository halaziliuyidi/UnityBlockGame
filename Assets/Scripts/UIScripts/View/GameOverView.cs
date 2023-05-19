using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
    public class GameOverViewContext : BaseContext
    {
        public GameOverViewContext() : base(UIType.GameOverView)
        {

        }
    }

    public class GameOverView : BaseView
    {
        public Image countDownImage;

        public TextMeshProUGUI countDownText;

        private Coroutine countDownCor;

        public override void OnEnter(BaseContext context)
        {
            StartCountDown();
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

        public void OnReviveGameBtnClick()
        {
            StopCountDown();
            Singleton<ContextManager>.Instance.Push(new GameViewContext());
            GameManager.Instance.GameRevive();
            GameManager.Instance.audioController.PlayButtonClickAudio();
        }

        public void StartCountDown()
        {
            countDownImage.fillAmount = 1;
            StopCountDown();
            countDownCor = StartCoroutine(CountDown());
            GameManager.Instance.audioController.PlayButtonClickAudio();
        }

        public IEnumerator CountDown()
        {
            float countDownNum = GameConstManager.Instance.GameOverCountDown;

            while (countDownNum > 0)
            {
                countDownNum -= Time.deltaTime;
                countDownText.text = countDownNum.ToString("#0.0");
                countDownImage.fillAmount = countDownNum / GameConstManager.Instance.GameOverCountDown;
                yield return null;
            }
            countDownImage.fillAmount = 0;
            CountDownEnd();
        }

        private void StopCountDown()
        {
            if (countDownCor != null)
            {
                StopCoroutine(countDownCor);
            }
            countDownCor = null;
        }

        private void CountDownEnd()
        {
            Singleton<ContextManager>.Instance.Push(new GameRestartViewContext());
            GameManager.Instance.audioController.PlayCountAudio();
        }
    }
}
